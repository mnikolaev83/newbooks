using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookDataProvider;
using Microsoft.AspNetCore.Mvc;
using NewBookAPI.Models;
using Microsoft.EntityFrameworkCore;
using BookDataProvider.Entities;

namespace NewBookAPI.Controllers
{
    [Route("api/")]
    public class HomeController : Controller
    {
        private bool IsInIgnoreList(Book book, List<IgnoreItem> ignoreList)
        {
            var bookSeries = String.Empty;
            if (!String.IsNullOrWhiteSpace(book.Series))
                bookSeries = book.Series;
            var bookSubcategory = String.Empty;
            if (!String.IsNullOrWhiteSpace(book.Subcategory))
                bookSubcategory = book.Subcategory;
            var bookPublisher = String.Empty;
            if (!String.IsNullOrWhiteSpace(book.Publisher))
                bookPublisher = book.Publisher;
            var bookTarget = String.Empty;
            if (!String.IsNullOrWhiteSpace(book.Target))
                bookTarget = book.Target;
            foreach (var item in ignoreList)
            {
                if (item.Category != null)
                    if (item.Category.Id != book.Category.Id)
                        continue;
                if (!String.IsNullOrWhiteSpace(item.Series))
                    if (item.Series.Trim() != bookSeries.Trim())
                        continue;
                if (!String.IsNullOrWhiteSpace(item.Subcategory))
                    if (item.Subcategory.Trim() != bookSubcategory.Trim())
                        continue;
                if (!String.IsNullOrWhiteSpace(item.Target))
                    if (item.Target.Trim() != bookTarget.Trim())
                        continue;
                if (!String.IsNullOrWhiteSpace(item.Publisher))
                    if (item.Publisher.Trim() != bookPublisher.Trim())
                        continue;
                return true;
            }
            return false;
        }
        private bool IsInFavoriteList(Book book, List<FavoriteItem> favoriteList)
        {
            var bookSeries = String.Empty;
            if (!String.IsNullOrWhiteSpace(book.Series))
                bookSeries = book.Series;
            var bookSubcategory = String.Empty;
            if (!String.IsNullOrWhiteSpace(book.Subcategory))
                bookSubcategory = book.Subcategory;
            var bookPublisher = String.Empty;
            if (!String.IsNullOrWhiteSpace(book.Publisher))
                bookPublisher = book.Publisher;
            var bookTarget = String.Empty;
            if (!String.IsNullOrWhiteSpace(book.Target))
                bookTarget = book.Target;
            foreach (var item in favoriteList)
            {
                if (item.Category != null)
                    if (item.Category.Id != book.Category.Id)
                        continue;
                if (!String.IsNullOrWhiteSpace(item.Series))
                    if (!bookSeries.ToLower().Trim().Contains(item.Series.ToLower().Trim()))
                        continue;
                if (!String.IsNullOrWhiteSpace(item.Subcategory))
                    if (!bookSubcategory.ToLower().Trim().Contains(item.Subcategory.ToLower().Trim()))
                        continue;
                if (!String.IsNullOrWhiteSpace(item.Target))
                    if (!bookTarget.ToLower().Trim().Contains(item.Target.ToLower().Trim()))
                        continue;
                if (!String.IsNullOrWhiteSpace(item.Publisher))
                    if (!bookPublisher.ToLower().Trim().Contains(item.Publisher.ToLower().Trim()))
                        continue;
                return true;
            }
            return false;
        }
        [Route("list")]
        [HttpGet]
        public IEnumerable<BookModel> List(int category_id, string date_added_from, string date_added_to)
        {

            var dateAddedFromModel = Convert.ToDateTime(date_added_from);
            var dateAddedToModel = Convert.ToDateTime(date_added_to);
            var dateAddedFrom = new DateTime(dateAddedFromModel.Year, dateAddedFromModel.Month, dateAddedFromModel.Day, 0, 0, 0);
            var dateAddedTo = new DateTime(dateAddedToModel.Year, dateAddedToModel.Month, dateAddedToModel.Day, 23, 59, 59);

            using (var db = new BookDBContext())
            {

                var category = db.Categories.Where(x => x.Id == category_id).FirstOrDefault();

                var books = db.Books.
                    Include(x => x.Category).
                    Where(x => x.Category.Id == category_id &&
                    x.AddedAt >= dateAddedFrom && x.AddedAt <= dateAddedTo).
                    ToList();

                var ignoreList = db.IgnoreList.
                    Include(x => x.Category).
                    ToList();

                var favoriteList = db.FavoriteList.
                    Include(x => x.Category).
                    ToList();

                var wishList = db.WishList.
                    Include(x => x.Book).
                    ToList();

                var wishListIds = new List<int>();
                if (wishList.Count > 0)
                    wishListIds = wishList.Select(x => x.Book.Id).ToList();

                var result = new List<BookModel>();
                foreach (var book in books.OrderByDescending(x => x.AddedAt).ToList())
                {
                    result.Add(new BookModel()
                    {
                        added_at = book.AddedAt.ToShortDateString(),
                        author_name = String.IsNullOrWhiteSpace(book.AuthorFullName) ? book.AuthorShortName : book.AuthorFullName,
                        category_id = book.Category.Id,
                        category_name = book.Category.Name,
                        description = book.Description,
                        id = book.Id,
                        image_url = book.ImageURL,
                        isbn = book.Isbn,
                        pages_amnt = book.PagesAmnt,
                        publisher = book.Publisher,
                        run_amnt = book.RunAmnt,
                        series = book.Series,
                        store_code = book.StoreCode,
                        subcategory = book.Subcategory,
                        target = book.Target,
                        title = book.Title,
                        translated = book.Translated,
                        updated_at = book.UpdatedAt.ToShortDateString(),
                        year = book.Year,
                        is_ignored = IsInIgnoreList(book, ignoreList),
                        is_favorite = IsInFavoriteList(book, favoriteList),
                        is_in_wishlist = wishListIds.Contains(book.Id)
                    });
                }
                var ignored = result.Where(x => x.is_ignored).ToList();
                var favorite = result.Where(x => x.is_favorite).ToList();

                db.QueryLog.Add(new QueryLog()
                {
                    BooksFetched = result.Where(x => !x.is_ignored).ToList().Count,
                    DateFrom = dateAddedFrom,
                    DateTo = dateAddedTo,
                    Category = category,
                    QueryAt = DateTime.Now,
                });

                db.SaveChanges();

                return result;
            }
        }
        [Route("ignored")]
        [HttpGet]
        public IEnumerable<IgnoreListItemModel> IgnoreList()
        {
            using (var db = new BookDBContext())
            {
                var ignoreList = db.IgnoreList.Include(x => x.Category).ToList();
                var result = new List<IgnoreListItemModel>();
                foreach (var item in ignoreList)
                    result.Add(new IgnoreListItemModel()
                    {
                        id = item.Id,
                        category_name = item.Category != null ? item.Category.Name : String.Empty,
                        publisher = item.Publisher,
                        series = item.Series,
                        subcategory = item.Subcategory,
                        target = item.Target
                    });
                return result;
            }
        }
        [Route("favorites")]
        [HttpGet]
        public IEnumerable<FavoriteListItemModel> FavoriteList()
        {
            using (var db = new BookDBContext())
            {
                var favoriteList = db.FavoriteList.Include(x => x.Category).ToList();
                var result = new List<FavoriteListItemModel>();
                foreach (var item in favoriteList)
                    result.Add(new FavoriteListItemModel()
                    {
                        id = item.Id,
                        category_name = item.Category != null ? item.Category.Name : String.Empty,
                        publisher = item.Publisher,
                        series = item.Series,
                        subcategory = item.Subcategory,
                        target = item.Target
                    });
                return result;
            }
        }
        [Route("wishlist")]
        [HttpGet]
        public IEnumerable<BookModel> ListWishList()
        {
            using (var db = new BookDBContext())
            {
                var wishList = db.WishList.
                    Include(x => x.Book).
                    Include(x => x.Book.Category).
                    ToList();

                var wishBooks = new List<Book>();
                if (wishList.Count > 0)
                {
                    wishBooks = wishList.Select(x => x.Book).ToList();
                    wishBooks = wishBooks.OrderBy(x => x.Category.Name).
                        ThenBy(x => x.Subcategory).
                        ThenBy(x => x.AuthorFullName).
                        ThenBy(x => x.Title).
                        ToList();
                }
                var result = new List<BookModel>();
                foreach (var book in wishBooks)
                {
                    result.Add(new BookModel()
                    {
                        added_at = book.AddedAt.ToShortDateString(),
                        author_name = String.IsNullOrWhiteSpace(book.AuthorFullName) ? book.AuthorShortName : book.AuthorFullName,
                        category_id = book.Category.Id,
                        category_name = book.Category.Name,
                        description = book.Description,
                        id = book.Id,
                        image_url = book.ImageURL,
                        isbn = book.Isbn,
                        pages_amnt = book.PagesAmnt,
                        publisher = book.Publisher,
                        run_amnt = book.RunAmnt,
                        series = book.Series,
                        store_code = book.StoreCode,
                        subcategory = book.Subcategory,
                        target = book.Target,
                        title = book.Title,
                        translated = book.Translated,
                        updated_at = book.UpdatedAt.ToShortDateString(),
                        year = book.Year
                    });
                }
                return result;
            }
        }
        [Route("categories")]
        [HttpGet]
        public IEnumerable<CategoryModel> Categories()
        {
            using (var db = new BookDBContext())
            {
                var categories = db.Categories.ToList();
                var result = new List<CategoryModel>();
                foreach (var category in categories.OrderBy(x => x.Order).ToList())
                {
                    result.Add(new CategoryModel()
                    {
                        id = category.Id,
                        category_name = category.Name
                    });
                }
                return result;
            }
        }
        [Route("wish_add")]
        [HttpGet]
        public bool AddToWishList(int id)
        {
            using (var db = new BookDBContext())
            {
                if (db.WishList.Where(x => x.Book.Id == id).FirstOrDefault() == null)
                {
                    var book = db.Books.Where(x => x.Id == id).FirstOrDefault();
                    db.WishList.Add(new WishItem() { Book = book });
                    db.SaveChanges();
                }
                return true;
            }
        }
        [Route("wish_remove")]
        [HttpGet]
        public bool RemoveFromWishList(int id)
        {
            using (var db = new BookDBContext())
            {
                var wish = db.WishList.Where(x => x.Book.Id == id).FirstOrDefault();
                if (wish != null)
                {
                    db.WishList.Remove(wish);
                    db.SaveChanges();
                }
                return true;
            }
        }
        [Route("ignore_remove")]
        [HttpGet]
        public bool RemoveFromIgnoreList(int id)
        {
            using (var db = new BookDBContext())
            {
                var item = db.IgnoreList.Where(x => x.Id == id).FirstOrDefault();
                if (item != null)
                {
                    db.IgnoreList.Remove(item);
                    db.SaveChanges();
                }
                return true;
            }
        }
        [Route("favorites_remove")]
        [HttpGet]
        public bool RemoveFromFavorites(int id)
        {
            using (var db = new BookDBContext())
            {
                var item = db.FavoriteList.Where(x => x.Id == id).FirstOrDefault();
                if (item != null)
                {
                    db.FavoriteList.Remove(item);
                    db.SaveChanges();
                }
                return true;
            }
        }
        [Route("query_log")]
        [HttpGet]
        public IEnumerable<QueryLogItemModel> QueryLog()
        {
            using (var db = new BookDBContext())
            {
                var log = db.QueryLog.
                    Include(x => x.Category).
                    OrderByDescending(x => x.QueryAt).
                    Take(1000).
                    ToList();
                var result = new List<QueryLogItemModel>();
                foreach (var item in log)
                    result.Add(new QueryLogItemModel()
                    {
                        id = item.Id,
                        category_name = item.Category != null ? item.Category.Name : String.Empty,
                        query_at = item.QueryAt.ToString("yyyy-MM-dd HH:mm:ss"),
                        period = item.DateFrom.ToString("yyyy-MM-dd") + " - " + item.DateTo.ToString("yyyy-MM-dd"),
                        books_fetched = item.BooksFetched
                    });
                return result;
            }
        }
        [Route("job_log")]
        [HttpGet]
        public IEnumerable<NewBooksLogItemModel> JobLog()
        {
            using (var db = new BookDBContext())
            {
                var log = db.JobLog.
                    OrderByDescending(x => x.StartedAt).
                    Take(1000).
                    ToList();
                var result = new List<NewBooksLogItemModel>();
                foreach (var item in log)
                    result.Add(new NewBooksLogItemModel()
                    {
                        id = item.Id,
                        books_fetched = item.BooksFetched,
                        completed_at = item.CompletedAt != null ? item.CompletedAt.Value.ToString("yyyy-MM-dd HH:mm:ss") : String.Empty,
                        error_occured = item.FailedAt == null ? false : true,
                        started_at = item.StartedAt.ToString("yyyy-MM-dd HH:mm:ss")
                    });
                return result;
            }
        }
        [Route("ignore")]
        [HttpGet]
        public bool Ignore(IgnoreModel model)
        {
            using (var db = new BookDBContext())
            {
                var publisher = String.Empty;
                if (!String.IsNullOrWhiteSpace(model.publisher))
                    publisher = model.publisher.Trim();
                var series = String.Empty;
                if (!String.IsNullOrWhiteSpace(model.series))
                    series = model.series.Trim();
                var subcategory = String.Empty;
                if (!String.IsNullOrWhiteSpace(model.subcategory))
                    subcategory = model.subcategory.Trim();
                var target = String.Empty;
                if (!String.IsNullOrWhiteSpace(model.target))
                    target = model.target.Trim();
                var categoryId = model.category_id;
                Category category = null;
                if (categoryId > 0)
                {
                    category = db.Categories.Where(x => x.Id == categoryId).FirstOrDefault();
                }
                var itemsToRemove = new List<IgnoreItem>();
                if (categoryId == 0 && 
                    String.IsNullOrWhiteSpace(target) && 
                    String.IsNullOrWhiteSpace(publisher) && 
                    String.IsNullOrWhiteSpace(series) && 
                    String.IsNullOrWhiteSpace(subcategory)) {
                    return true;
                }
                else if (categoryId > 0 &&
                    String.IsNullOrWhiteSpace(target) &&
                    String.IsNullOrWhiteSpace(publisher) &&
                    String.IsNullOrWhiteSpace(series) &&
                    String.IsNullOrWhiteSpace(subcategory))
                {
                    itemsToRemove = db.IgnoreList.
                        Where(x => x.Category.Id == categoryId).
                        ToList();
                    db.IgnoreList.Add(new IgnoreItem()
                    {
                        Category = category
                    });
                    db.SaveChanges();
                }
                else if (categoryId == 0 &&
                    !String.IsNullOrWhiteSpace(target) &&
                    String.IsNullOrWhiteSpace(publisher) &&
                    String.IsNullOrWhiteSpace(series) &&
                    String.IsNullOrWhiteSpace(subcategory))
                {
                    itemsToRemove = db.IgnoreList.
                        Where(x => x.Target == target).
                        ToList();
                    db.IgnoreList.Add(new IgnoreItem()
                    {
                        Target = target
                    });
                    db.SaveChanges();
                }
                else if (categoryId == 0 &&
                    String.IsNullOrWhiteSpace(target) &&
                    !String.IsNullOrWhiteSpace(publisher) &&
                    String.IsNullOrWhiteSpace(series) &&
                    String.IsNullOrWhiteSpace(subcategory))
                {
                    itemsToRemove = db.IgnoreList.
                        Where(x => x.Publisher == publisher).
                        ToList();
                    db.IgnoreList.Add(new IgnoreItem()
                    {
                        Publisher = publisher
                    });
                    db.SaveChanges();
                }
                else if (categoryId == 0 &&
                    String.IsNullOrWhiteSpace(target) &&
                    String.IsNullOrWhiteSpace(publisher) &&
                    !String.IsNullOrWhiteSpace(series) &&
                    String.IsNullOrWhiteSpace(subcategory))
                {
                    itemsToRemove = db.IgnoreList.
                        Where(x => x.Series == series).
                        ToList();
                    db.IgnoreList.Add(new IgnoreItem()
                    {
                        Series = series
                    });
                    db.SaveChanges();
                }
                else if (categoryId == 0 &&
                    String.IsNullOrWhiteSpace(target) &&
                    String.IsNullOrWhiteSpace(publisher) &&
                    String.IsNullOrWhiteSpace(series) &&
                    !String.IsNullOrWhiteSpace(subcategory))
                {
                    itemsToRemove = db.IgnoreList.
                        Where(x => x.Subcategory == subcategory).
                        ToList();
                    db.IgnoreList.Add(new IgnoreItem()
                    {
                        Subcategory = subcategory
                    });
                    db.SaveChanges();
                }
                else if (categoryId > 0 &&
                    !String.IsNullOrWhiteSpace(target) &&
                    String.IsNullOrWhiteSpace(publisher) &&
                    String.IsNullOrWhiteSpace(series) &&
                    String.IsNullOrWhiteSpace(subcategory))
                {
                    itemsToRemove = db.IgnoreList.
                        Where(x => x.Category.Id == categoryId && x.Target == target).
                        ToList();
                    db.IgnoreList.Add(new IgnoreItem()
                    {
                        Category = category,
                        Target = target
                    });
                    db.SaveChanges();
                }
                else if (categoryId > 0 &&
                    String.IsNullOrWhiteSpace(target) &&
                    !String.IsNullOrWhiteSpace(publisher) &&
                    String.IsNullOrWhiteSpace(series) &&
                    String.IsNullOrWhiteSpace(subcategory))
                {
                    itemsToRemove = db.IgnoreList.
                        Where(x => x.Category.Id == categoryId && x.Publisher == publisher).
                        ToList();
                    db.IgnoreList.Add(new IgnoreItem()
                    {
                        Category = category,
                        Publisher = publisher
                    });
                    db.SaveChanges();
                }
                else if (categoryId > 0 &&
                    String.IsNullOrWhiteSpace(target) &&
                    String.IsNullOrWhiteSpace(publisher) &&
                    !String.IsNullOrWhiteSpace(series) &&
                    String.IsNullOrWhiteSpace(subcategory))
                {
                    itemsToRemove = db.IgnoreList.
                        Where(x => x.Category.Id == categoryId && x.Series == series).
                        ToList();
                    db.IgnoreList.Add(new IgnoreItem()
                    {
                        Category = category,
                        Series = series
                    });
                    db.SaveChanges();
                }
                else if (categoryId > 0 &&
                    String.IsNullOrWhiteSpace(target) &&
                    String.IsNullOrWhiteSpace(publisher) &&
                    String.IsNullOrWhiteSpace(series) &&
                    !String.IsNullOrWhiteSpace(subcategory))
                {
                    itemsToRemove = db.IgnoreList.
                        Where(x => x.Category.Id == categoryId && x.Subcategory == subcategory).
                        ToList();
                    db.IgnoreList.Add(new IgnoreItem()
                    {
                        Category = category,
                        Subcategory = subcategory
                    });
                    db.SaveChanges();
                }
                else if (categoryId == 0 &&
                    !String.IsNullOrWhiteSpace(target) &&
                    !String.IsNullOrWhiteSpace(publisher) &&
                    String.IsNullOrWhiteSpace(series) &&
                    String.IsNullOrWhiteSpace(subcategory))
                {
                    itemsToRemove = db.IgnoreList.
                        Where(x => x.Target == target && x.Publisher == publisher).
                        ToList();
                    db.IgnoreList.Add(new IgnoreItem()
                    {
                        Target = target,
                        Publisher = publisher
                    });
                    db.SaveChanges();
                }
                else if (categoryId == 0 &&
                    !String.IsNullOrWhiteSpace(target) &&
                    String.IsNullOrWhiteSpace(publisher) &&
                    !String.IsNullOrWhiteSpace(series) &&
                    String.IsNullOrWhiteSpace(subcategory))
                {
                    itemsToRemove = db.IgnoreList.
                        Where(x => x.Target == target && x.Series == series).
                        ToList();
                    db.IgnoreList.Add(new IgnoreItem()
                    {
                        Target = target,
                        Series = series
                    });
                    db.SaveChanges();
                }
                else if (categoryId == 0 &&
                    !String.IsNullOrWhiteSpace(target) &&
                    String.IsNullOrWhiteSpace(publisher) &&
                    String.IsNullOrWhiteSpace(series) &&
                    !String.IsNullOrWhiteSpace(subcategory))
                {
                    itemsToRemove = db.IgnoreList.
                        Where(x => x.Target == target && x.Subcategory == subcategory).
                        ToList();
                    db.IgnoreList.Add(new IgnoreItem()
                    {
                        Target = target,
                        Subcategory = subcategory
                    });
                    db.SaveChanges();
                }
                else if (categoryId == 0 &&
                    String.IsNullOrWhiteSpace(target) &&
                    !String.IsNullOrWhiteSpace(publisher) &&
                    !String.IsNullOrWhiteSpace(series) &&
                    String.IsNullOrWhiteSpace(subcategory))
                {
                    itemsToRemove = db.IgnoreList.
                        Where(x => x.Publisher == publisher && x.Series == series).
                        ToList();
                    db.IgnoreList.Add(new IgnoreItem()
                    {
                        Publisher = publisher,
                        Series = series
                    });
                    db.SaveChanges();
                }
                else if (categoryId == 0 &&
                    String.IsNullOrWhiteSpace(target) &&
                    !String.IsNullOrWhiteSpace(publisher) &&
                    String.IsNullOrWhiteSpace(series) &&
                    !String.IsNullOrWhiteSpace(subcategory))
                {
                    itemsToRemove = db.IgnoreList.
                        Where(x => x.Publisher == publisher && x.Subcategory == subcategory).
                        ToList();
                    db.IgnoreList.Add(new IgnoreItem()
                    {
                        Publisher = publisher,
                        Subcategory = subcategory
                    });
                    db.SaveChanges();
                }
                else if (categoryId == 0 &&
                    String.IsNullOrWhiteSpace(target) &&
                    String.IsNullOrWhiteSpace(publisher) &&
                    !String.IsNullOrWhiteSpace(series) &&
                    !String.IsNullOrWhiteSpace(subcategory))
                {
                    itemsToRemove = db.IgnoreList.
                        Where(x => x.Series == series && x.Subcategory == subcategory).
                        ToList();
                    db.IgnoreList.Add(new IgnoreItem()
                    {
                        Series = series,
                        Subcategory = subcategory
                    });
                    db.SaveChanges();
                }
                else if (categoryId > 0 &&
                    !String.IsNullOrWhiteSpace(target) &&
                    !String.IsNullOrWhiteSpace(publisher) &&
                    String.IsNullOrWhiteSpace(series) &&
                    String.IsNullOrWhiteSpace(subcategory))
                {
                    itemsToRemove = db.IgnoreList.
                        Where(x => x.Category.Id == categoryId && 
                        x.Target == target &&
                        x.Publisher == publisher
                        ).
                        ToList();
                    db.IgnoreList.Add(new IgnoreItem()
                    {
                        Category = category,
                        Target = target,
                        Publisher = publisher
                    });
                    db.SaveChanges();
                }
                else if (categoryId > 0 &&
                    !String.IsNullOrWhiteSpace(target) &&
                    String.IsNullOrWhiteSpace(publisher) &&
                    !String.IsNullOrWhiteSpace(series) &&
                    String.IsNullOrWhiteSpace(subcategory))
                {
                    itemsToRemove = db.IgnoreList.
                        Where(x => x.Category.Id == categoryId &&
                        x.Target == target &&
                        x.Series == series
                        ).
                        ToList();
                    db.IgnoreList.Add(new IgnoreItem()
                    {
                        Category = category,
                        Target = target,
                        Series = series
                    });
                    db.SaveChanges();
                }
                else if (categoryId > 0 &&
                    !String.IsNullOrWhiteSpace(target) &&
                    String.IsNullOrWhiteSpace(publisher) &&
                    String.IsNullOrWhiteSpace(series) &&
                    !String.IsNullOrWhiteSpace(subcategory))
                {
                    itemsToRemove = db.IgnoreList.
                        Where(x => x.Category.Id == categoryId &&
                        x.Target == target &&
                        x.Subcategory == subcategory
                        ).
                        ToList();
                    db.IgnoreList.Add(new IgnoreItem()
                    {
                        Category = category,
                        Target = target,
                        Subcategory = subcategory
                    });
                    db.SaveChanges();
                }
                else if (categoryId > 0 &&
                    String.IsNullOrWhiteSpace(target) &&
                    !String.IsNullOrWhiteSpace(publisher) &&
                    !String.IsNullOrWhiteSpace(series) &&
                    String.IsNullOrWhiteSpace(subcategory))
                {
                    itemsToRemove = db.IgnoreList.
                        Where(x => x.Category.Id == categoryId &&
                        x.Publisher == publisher &&
                        x.Series == series
                        ).
                        ToList();
                    db.IgnoreList.Add(new IgnoreItem()
                    {
                        Category = category,
                        Publisher = publisher,
                        Series = series
                    });
                    db.SaveChanges();
                }
                else if (categoryId > 0 &&
                    String.IsNullOrWhiteSpace(target) &&
                    !String.IsNullOrWhiteSpace(publisher) &&
                    String.IsNullOrWhiteSpace(series) &&
                    !String.IsNullOrWhiteSpace(subcategory))
                {
                    itemsToRemove = db.IgnoreList.
                        Where(x => x.Category.Id == categoryId &&
                        x.Publisher == publisher &&
                        x.Subcategory == subcategory
                        ).
                        ToList();
                    db.IgnoreList.Add(new IgnoreItem()
                    {
                        Category = category,
                        Publisher = publisher,
                        Subcategory = subcategory
                    });
                    db.SaveChanges();
                }
                else if (categoryId > 0 &&
                    String.IsNullOrWhiteSpace(target) &&
                    String.IsNullOrWhiteSpace(publisher) &&
                    !String.IsNullOrWhiteSpace(series) &&
                    !String.IsNullOrWhiteSpace(subcategory))
                {
                    itemsToRemove = db.IgnoreList.
                        Where(x => x.Category.Id == categoryId &&
                        x.Series == series &&
                        x.Subcategory == subcategory
                        ).
                        ToList();
                    db.IgnoreList.Add(new IgnoreItem()
                    {
                        Category = category,
                        Series = series,
                        Subcategory = subcategory
                    });
                    db.SaveChanges();
                }
                else if (categoryId == 0 &&
                    !String.IsNullOrWhiteSpace(target) &&
                    !String.IsNullOrWhiteSpace(publisher) &&
                    !String.IsNullOrWhiteSpace(series) &&
                    String.IsNullOrWhiteSpace(subcategory))
                {
                    itemsToRemove = db.IgnoreList.
                        Where(x => 
                        x.Target == target &&
                        x.Publisher == publisher &&
                        x.Series == series
                        ).
                        ToList();
                    db.IgnoreList.Add(new IgnoreItem()
                    {
                        Target = target,
                        Publisher = publisher,
                        Series = series
                    });
                    db.SaveChanges();
                }
                else if (categoryId == 0 &&
                    !String.IsNullOrWhiteSpace(target) &&
                    !String.IsNullOrWhiteSpace(publisher) &&
                    String.IsNullOrWhiteSpace(series) &&
                    !String.IsNullOrWhiteSpace(subcategory))
                {
                    itemsToRemove = db.IgnoreList.
                        Where(x =>
                        x.Target == target &&
                        x.Publisher == publisher &&
                        x.Subcategory == subcategory
                        ).
                        ToList();
                    db.IgnoreList.Add(new IgnoreItem()
                    {
                        Target = target,
                        Publisher = publisher,
                        Subcategory = subcategory
                    });
                    db.SaveChanges();
                }
                else if (categoryId > 0 &&
                    !String.IsNullOrWhiteSpace(target) &&
                    !String.IsNullOrWhiteSpace(publisher) &&
                    !String.IsNullOrWhiteSpace(series) &&
                    String.IsNullOrWhiteSpace(subcategory))
                {
                    itemsToRemove = db.IgnoreList.
                        Where(x =>
                        x.Category.Id == categoryId &&
                        x.Target == target &&
                        x.Publisher == publisher &&
                        x.Series == series 
                        ).
                        ToList();
                    db.IgnoreList.Add(new IgnoreItem()
                    {
                        Category = category,
                        Target = target,
                        Publisher = publisher,
                        Series = series
                    });
                    db.SaveChanges();
                }
                else if (categoryId > 0 &&
                    !String.IsNullOrWhiteSpace(target) &&
                    !String.IsNullOrWhiteSpace(publisher) &&
                    String.IsNullOrWhiteSpace(series) &&
                    !String.IsNullOrWhiteSpace(subcategory))
                {
                    itemsToRemove = db.IgnoreList.
                        Where(x =>
                        x.Category.Id == categoryId &&
                        x.Target == target &&
                        x.Publisher == publisher &&
                        x.Subcategory == subcategory
                        ).
                        ToList();
                    db.IgnoreList.Add(new IgnoreItem()
                    {
                        Category = category,
                        Target = target,
                        Publisher = publisher,
                        Subcategory = subcategory
                    });
                    db.SaveChanges();
                }
                else if (categoryId > 0 &&
                    !String.IsNullOrWhiteSpace(target) &&
                    String.IsNullOrWhiteSpace(publisher) &&
                    !String.IsNullOrWhiteSpace(series) &&
                    !String.IsNullOrWhiteSpace(subcategory))
                {
                    itemsToRemove = db.IgnoreList.
                        Where(x =>
                        x.Category.Id == categoryId &&
                        x.Target == target &&
                        x.Series == series &&
                        x.Subcategory == subcategory
                        ).
                        ToList();
                    db.IgnoreList.Add(new IgnoreItem()
                    {
                        Category = category,
                        Target = target,
                        Series = series,
                        Subcategory = subcategory
                    });
                    db.SaveChanges();
                }
                else if (categoryId > 0 &&
                    String.IsNullOrWhiteSpace(target) &&
                    !String.IsNullOrWhiteSpace(publisher) &&
                    !String.IsNullOrWhiteSpace(series) &&
                    !String.IsNullOrWhiteSpace(subcategory))
                {
                    itemsToRemove = db.IgnoreList.
                        Where(x =>
                        x.Category.Id == categoryId &&
                        x.Publisher == publisher &&
                        x.Series == series &&
                        x.Subcategory == subcategory
                        ).
                        ToList();
                    db.IgnoreList.Add(new IgnoreItem()
                    {
                        Category = category,
                        Publisher = publisher,
                        Series = series,
                        Subcategory = subcategory
                    });
                    db.SaveChanges();
                }
                else if (categoryId == 0 &&
                    !String.IsNullOrWhiteSpace(target) &&
                    !String.IsNullOrWhiteSpace(publisher) &&
                    !String.IsNullOrWhiteSpace(series) &&
                    !String.IsNullOrWhiteSpace(subcategory))
                {
                    itemsToRemove = db.IgnoreList.
                        Where(x =>
                        x.Target == target &&
                        x.Publisher == publisher &&
                        x.Series == series &&
                        x.Subcategory == subcategory
                        ).
                        ToList();
                    db.IgnoreList.Add(new IgnoreItem()
                    {
                        Target = target,
                        Publisher = publisher,
                        Series = series,
                        Subcategory = subcategory
                    });
                    db.SaveChanges();
                }
                else if (categoryId > 0 &&
                    !String.IsNullOrWhiteSpace(target) &&
                    !String.IsNullOrWhiteSpace(publisher) &&
                    !String.IsNullOrWhiteSpace(series) &&
                    !String.IsNullOrWhiteSpace(subcategory))
                {
                    itemsToRemove = db.IgnoreList.
                        Where(x =>
                        x.Category.Id == categoryId &&
                        x.Target == target &&
                        x.Publisher == publisher &&
                        x.Series == series &&
                        x.Subcategory == subcategory
                        ).
                        ToList();
                    db.IgnoreList.Add(new IgnoreItem()
                    {
                        Category = category,
                        Target = target,
                        Publisher = publisher,
                        Series = series,
                        Subcategory = subcategory
                    });
                    db.SaveChanges();
                }
                foreach (var item in itemsToRemove)
                    db.Remove(item);
                db.SaveChanges();
                return true;
            }
        }
        [Route("add_to_favorite")]
        [HttpGet]
        public bool AddToFavorite(AddToFavoriteModel model)
        {
            using (var db = new BookDBContext())
            {
                var publisher = String.Empty;
                if (!String.IsNullOrWhiteSpace(model.publisher))
                    publisher = model.publisher.Trim();
                var series = String.Empty;
                if (!String.IsNullOrWhiteSpace(model.series))
                    series = model.series.Trim();
                var subcategory = String.Empty;
                if (!String.IsNullOrWhiteSpace(model.subcategory))
                    subcategory = model.subcategory.Trim();
                var target = String.Empty;
                if (!String.IsNullOrWhiteSpace(model.target))
                    target = model.target.Trim();
                var categoryId = model.category_id;
                Category category = null;
                if (categoryId > 0)
                {
                    category = db.Categories.Where(x => x.Id == categoryId).FirstOrDefault();
                }
                var itemsToRemove = new List<FavoriteItem>();
                if (categoryId == 0 &&
                    String.IsNullOrWhiteSpace(target) &&
                    String.IsNullOrWhiteSpace(publisher) &&
                    String.IsNullOrWhiteSpace(series) &&
                    String.IsNullOrWhiteSpace(subcategory))
                {
                    return true;
                }
                else if (categoryId > 0 &&
                    String.IsNullOrWhiteSpace(target) &&
                    String.IsNullOrWhiteSpace(publisher) &&
                    String.IsNullOrWhiteSpace(series) &&
                    String.IsNullOrWhiteSpace(subcategory))
                {
                    itemsToRemove = db.FavoriteList.
                        Where(x => x.Category.Id == categoryId).
                        ToList();
                    db.FavoriteList.Add(new FavoriteItem()
                    {
                        Category = category
                    });
                    db.SaveChanges();
                }
                else if (categoryId == 0 &&
                    !String.IsNullOrWhiteSpace(target) &&
                    String.IsNullOrWhiteSpace(publisher) &&
                    String.IsNullOrWhiteSpace(series) &&
                    String.IsNullOrWhiteSpace(subcategory))
                {
                    itemsToRemove = db.FavoriteList.
                        Where(x => x.Target == target).
                        ToList();
                    db.FavoriteList.Add(new FavoriteItem()
                    {
                        Target = target
                    });
                    db.SaveChanges();
                }
                else if (categoryId == 0 &&
                    String.IsNullOrWhiteSpace(target) &&
                    !String.IsNullOrWhiteSpace(publisher) &&
                    String.IsNullOrWhiteSpace(series) &&
                    String.IsNullOrWhiteSpace(subcategory))
                {
                    itemsToRemove = db.FavoriteList.
                        Where(x => x.Publisher == publisher).
                        ToList();
                    db.FavoriteList.Add(new FavoriteItem()
                    {
                        Publisher = publisher
                    });
                    db.SaveChanges();
                }
                else if (categoryId == 0 &&
                    String.IsNullOrWhiteSpace(target) &&
                    String.IsNullOrWhiteSpace(publisher) &&
                    !String.IsNullOrWhiteSpace(series) &&
                    String.IsNullOrWhiteSpace(subcategory))
                {
                    itemsToRemove = db.FavoriteList.
                        Where(x => x.Series == series).
                        ToList();
                    db.FavoriteList.Add(new FavoriteItem()
                    {
                        Series = series
                    });
                    db.SaveChanges();
                }
                else if (categoryId == 0 &&
                    String.IsNullOrWhiteSpace(target) &&
                    String.IsNullOrWhiteSpace(publisher) &&
                    String.IsNullOrWhiteSpace(series) &&
                    !String.IsNullOrWhiteSpace(subcategory))
                {
                    itemsToRemove = db.FavoriteList.
                        Where(x => x.Subcategory == subcategory).
                        ToList();
                    db.FavoriteList.Add(new FavoriteItem()
                    {
                        Subcategory = subcategory
                    });
                    db.SaveChanges();
                }
                else if (categoryId > 0 &&
                    !String.IsNullOrWhiteSpace(target) &&
                    String.IsNullOrWhiteSpace(publisher) &&
                    String.IsNullOrWhiteSpace(series) &&
                    String.IsNullOrWhiteSpace(subcategory))
                {
                    itemsToRemove = db.FavoriteList.
                        Where(x => x.Category.Id == categoryId && x.Target == target).
                        ToList();
                    db.FavoriteList.Add(new FavoriteItem()
                    {
                        Category = category,
                        Target = target
                    });
                    db.SaveChanges();
                }
                else if (categoryId > 0 &&
                    String.IsNullOrWhiteSpace(target) &&
                    !String.IsNullOrWhiteSpace(publisher) &&
                    String.IsNullOrWhiteSpace(series) &&
                    String.IsNullOrWhiteSpace(subcategory))
                {
                    itemsToRemove = db.FavoriteList.
                        Where(x => x.Category.Id == categoryId && x.Publisher == publisher).
                        ToList();
                    db.FavoriteList.Add(new FavoriteItem()
                    {
                        Category = category,
                        Publisher = publisher
                    });
                    db.SaveChanges();
                }
                else if (categoryId > 0 &&
                    String.IsNullOrWhiteSpace(target) &&
                    String.IsNullOrWhiteSpace(publisher) &&
                    !String.IsNullOrWhiteSpace(series) &&
                    String.IsNullOrWhiteSpace(subcategory))
                {
                    itemsToRemove = db.FavoriteList.
                        Where(x => x.Category.Id == categoryId && x.Series == series).
                        ToList();
                    db.FavoriteList.Add(new FavoriteItem()
                    {
                        Category = category,
                        Series = series
                    });
                    db.SaveChanges();
                }
                else if (categoryId > 0 &&
                    String.IsNullOrWhiteSpace(target) &&
                    String.IsNullOrWhiteSpace(publisher) &&
                    String.IsNullOrWhiteSpace(series) &&
                    !String.IsNullOrWhiteSpace(subcategory))
                {
                    itemsToRemove = db.FavoriteList.
                        Where(x => x.Category.Id == categoryId && x.Subcategory == subcategory).
                        ToList();
                    db.FavoriteList.Add(new FavoriteItem()
                    {
                        Category = category,
                        Subcategory = subcategory
                    });
                    db.SaveChanges();
                }
                else if (categoryId == 0 &&
                    !String.IsNullOrWhiteSpace(target) &&
                    !String.IsNullOrWhiteSpace(publisher) &&
                    String.IsNullOrWhiteSpace(series) &&
                    String.IsNullOrWhiteSpace(subcategory))
                {
                    itemsToRemove = db.FavoriteList.
                        Where(x => x.Target == target && x.Publisher == publisher).
                        ToList();
                    db.FavoriteList.Add(new FavoriteItem()
                    {
                        Target = target,
                        Publisher = publisher
                    });
                    db.SaveChanges();
                }
                else if (categoryId == 0 &&
                    !String.IsNullOrWhiteSpace(target) &&
                    String.IsNullOrWhiteSpace(publisher) &&
                    !String.IsNullOrWhiteSpace(series) &&
                    String.IsNullOrWhiteSpace(subcategory))
                {
                    itemsToRemove = db.FavoriteList.
                        Where(x => x.Target == target && x.Series == series).
                        ToList();
                    db.FavoriteList.Add(new FavoriteItem()
                    {
                        Target = target,
                        Series = series
                    });
                    db.SaveChanges();
                }
                else if (categoryId == 0 &&
                    !String.IsNullOrWhiteSpace(target) &&
                    String.IsNullOrWhiteSpace(publisher) &&
                    String.IsNullOrWhiteSpace(series) &&
                    !String.IsNullOrWhiteSpace(subcategory))
                {
                    itemsToRemove = db.FavoriteList.
                        Where(x => x.Target == target && x.Subcategory == subcategory).
                        ToList();
                    db.FavoriteList.Add(new FavoriteItem()
                    {
                        Target = target,
                        Subcategory = subcategory
                    });
                    db.SaveChanges();
                }
                else if (categoryId == 0 &&
                    String.IsNullOrWhiteSpace(target) &&
                    !String.IsNullOrWhiteSpace(publisher) &&
                    !String.IsNullOrWhiteSpace(series) &&
                    String.IsNullOrWhiteSpace(subcategory))
                {
                    itemsToRemove = db.FavoriteList.
                        Where(x => x.Publisher == publisher && x.Series == series).
                        ToList();
                    db.FavoriteList.Add(new FavoriteItem()
                    {
                        Publisher = publisher,
                        Series = series
                    });
                    db.SaveChanges();
                }
                else if (categoryId == 0 &&
                    String.IsNullOrWhiteSpace(target) &&
                    !String.IsNullOrWhiteSpace(publisher) &&
                    String.IsNullOrWhiteSpace(series) &&
                    !String.IsNullOrWhiteSpace(subcategory))
                {
                    itemsToRemove = db.FavoriteList.
                        Where(x => x.Publisher == publisher && x.Subcategory == subcategory).
                        ToList();
                    db.FavoriteList.Add(new FavoriteItem()
                    {
                        Publisher = publisher,
                        Subcategory = subcategory
                    });
                    db.SaveChanges();
                }
                else if (categoryId == 0 &&
                    String.IsNullOrWhiteSpace(target) &&
                    String.IsNullOrWhiteSpace(publisher) &&
                    !String.IsNullOrWhiteSpace(series) &&
                    !String.IsNullOrWhiteSpace(subcategory))
                {
                    itemsToRemove = db.FavoriteList.
                        Where(x => x.Series == series && x.Subcategory == subcategory).
                        ToList();
                    db.FavoriteList.Add(new FavoriteItem()
                    {
                        Series = series,
                        Subcategory = subcategory
                    });
                    db.SaveChanges();
                }
                else if (categoryId > 0 &&
                    !String.IsNullOrWhiteSpace(target) &&
                    !String.IsNullOrWhiteSpace(publisher) &&
                    String.IsNullOrWhiteSpace(series) &&
                    String.IsNullOrWhiteSpace(subcategory))
                {
                    itemsToRemove = db.FavoriteList.
                        Where(x => x.Category.Id == categoryId &&
                        x.Target == target &&
                        x.Publisher == publisher
                        ).
                        ToList();
                    db.FavoriteList.Add(new FavoriteItem()
                    {
                        Category = category,
                        Target = target,
                        Publisher = publisher
                    });
                    db.SaveChanges();
                }
                else if (categoryId > 0 &&
                    !String.IsNullOrWhiteSpace(target) &&
                    String.IsNullOrWhiteSpace(publisher) &&
                    !String.IsNullOrWhiteSpace(series) &&
                    String.IsNullOrWhiteSpace(subcategory))
                {
                    itemsToRemove = db.FavoriteList.
                        Where(x => x.Category.Id == categoryId &&
                        x.Target == target &&
                        x.Series == series
                        ).
                        ToList();
                    db.FavoriteList.Add(new FavoriteItem()
                    {
                        Category = category,
                        Target = target,
                        Series = series
                    });
                    db.SaveChanges();
                }
                else if (categoryId > 0 &&
                    !String.IsNullOrWhiteSpace(target) &&
                    String.IsNullOrWhiteSpace(publisher) &&
                    String.IsNullOrWhiteSpace(series) &&
                    !String.IsNullOrWhiteSpace(subcategory))
                {
                    itemsToRemove = db.FavoriteList.
                        Where(x => x.Category.Id == categoryId &&
                        x.Target == target &&
                        x.Subcategory == subcategory
                        ).
                        ToList();
                    db.FavoriteList.Add(new FavoriteItem()
                    {
                        Category = category,
                        Target = target,
                        Subcategory = subcategory
                    });
                    db.SaveChanges();
                }
                else if (categoryId > 0 &&
                    String.IsNullOrWhiteSpace(target) &&
                    !String.IsNullOrWhiteSpace(publisher) &&
                    !String.IsNullOrWhiteSpace(series) &&
                    String.IsNullOrWhiteSpace(subcategory))
                {
                    itemsToRemove = db.FavoriteList.
                        Where(x => x.Category.Id == categoryId &&
                        x.Publisher == publisher &&
                        x.Series == series
                        ).
                        ToList();
                    db.FavoriteList.Add(new FavoriteItem()
                    {
                        Category = category,
                        Publisher = publisher,
                        Series = series
                    });
                    db.SaveChanges();
                }
                else if (categoryId > 0 &&
                    String.IsNullOrWhiteSpace(target) &&
                    !String.IsNullOrWhiteSpace(publisher) &&
                    String.IsNullOrWhiteSpace(series) &&
                    !String.IsNullOrWhiteSpace(subcategory))
                {
                    itemsToRemove = db.FavoriteList.
                        Where(x => x.Category.Id == categoryId &&
                        x.Publisher == publisher &&
                        x.Subcategory == subcategory
                        ).
                        ToList();
                    db.FavoriteList.Add(new FavoriteItem()
                    {
                        Category = category,
                        Publisher = publisher,
                        Subcategory = subcategory
                    });
                    db.SaveChanges();
                }
                else if (categoryId > 0 &&
                    String.IsNullOrWhiteSpace(target) &&
                    String.IsNullOrWhiteSpace(publisher) &&
                    !String.IsNullOrWhiteSpace(series) &&
                    !String.IsNullOrWhiteSpace(subcategory))
                {
                    itemsToRemove = db.FavoriteList.
                        Where(x => x.Category.Id == categoryId &&
                        x.Series == series &&
                        x.Subcategory == subcategory
                        ).
                        ToList();
                    db.FavoriteList.Add(new FavoriteItem()
                    {
                        Category = category,
                        Series = series,
                        Subcategory = subcategory
                    });
                    db.SaveChanges();
                }
                else if (categoryId == 0 &&
                    !String.IsNullOrWhiteSpace(target) &&
                    !String.IsNullOrWhiteSpace(publisher) &&
                    !String.IsNullOrWhiteSpace(series) &&
                    String.IsNullOrWhiteSpace(subcategory))
                {
                    itemsToRemove = db.FavoriteList.
                        Where(x =>
                        x.Target == target &&
                        x.Publisher == publisher &&
                        x.Series == series
                        ).
                        ToList();
                    db.FavoriteList.Add(new FavoriteItem()
                    {
                        Target = target,
                        Publisher = publisher,
                        Series = series
                    });
                    db.SaveChanges();
                }
                else if (categoryId == 0 &&
                    !String.IsNullOrWhiteSpace(target) &&
                    !String.IsNullOrWhiteSpace(publisher) &&
                    String.IsNullOrWhiteSpace(series) &&
                    !String.IsNullOrWhiteSpace(subcategory))
                {
                    itemsToRemove = db.FavoriteList.
                        Where(x =>
                        x.Target == target &&
                        x.Publisher == publisher &&
                        x.Subcategory == subcategory
                        ).
                        ToList();
                    db.FavoriteList.Add(new FavoriteItem()
                    {
                        Target = target,
                        Publisher = publisher,
                        Subcategory = subcategory
                    });
                    db.SaveChanges();
                }
                else if (categoryId > 0 &&
                    !String.IsNullOrWhiteSpace(target) &&
                    !String.IsNullOrWhiteSpace(publisher) &&
                    !String.IsNullOrWhiteSpace(series) &&
                    String.IsNullOrWhiteSpace(subcategory))
                {
                    itemsToRemove = db.FavoriteList.
                        Where(x =>
                        x.Category.Id == categoryId &&
                        x.Target == target &&
                        x.Publisher == publisher &&
                        x.Series == series
                        ).
                        ToList();
                    db.FavoriteList.Add(new FavoriteItem()
                    {
                        Category = category,
                        Target = target,
                        Publisher = publisher,
                        Series = series
                    });
                    db.SaveChanges();
                }
                else if (categoryId > 0 &&
                    !String.IsNullOrWhiteSpace(target) &&
                    !String.IsNullOrWhiteSpace(publisher) &&
                    String.IsNullOrWhiteSpace(series) &&
                    !String.IsNullOrWhiteSpace(subcategory))
                {
                    itemsToRemove = db.FavoriteList.
                        Where(x =>
                        x.Category.Id == categoryId &&
                        x.Target == target &&
                        x.Publisher == publisher &&
                        x.Subcategory == subcategory
                        ).
                        ToList();
                    db.FavoriteList.Add(new FavoriteItem()
                    {
                        Category = category,
                        Target = target,
                        Publisher = publisher,
                        Subcategory = subcategory
                    });
                    db.SaveChanges();
                }
                else if (categoryId > 0 &&
                    !String.IsNullOrWhiteSpace(target) &&
                    String.IsNullOrWhiteSpace(publisher) &&
                    !String.IsNullOrWhiteSpace(series) &&
                    !String.IsNullOrWhiteSpace(subcategory))
                {
                    itemsToRemove = db.FavoriteList.
                        Where(x =>
                        x.Category.Id == categoryId &&
                        x.Target == target &&
                        x.Series == series &&
                        x.Subcategory == subcategory
                        ).
                        ToList();
                    db.FavoriteList.Add(new FavoriteItem()
                    {
                        Category = category,
                        Target = target,
                        Series = series,
                        Subcategory = subcategory
                    });
                    db.SaveChanges();
                }
                else if (categoryId > 0 &&
                    String.IsNullOrWhiteSpace(target) &&
                    !String.IsNullOrWhiteSpace(publisher) &&
                    !String.IsNullOrWhiteSpace(series) &&
                    !String.IsNullOrWhiteSpace(subcategory))
                {
                    itemsToRemove = db.FavoriteList.
                        Where(x =>
                        x.Category.Id == categoryId &&
                        x.Publisher == publisher &&
                        x.Series == series &&
                        x.Subcategory == subcategory
                        ).
                        ToList();
                    db.FavoriteList.Add(new FavoriteItem()
                    {
                        Category = category,
                        Publisher = publisher,
                        Series = series,
                        Subcategory = subcategory
                    });
                    db.SaveChanges();
                }
                else if (categoryId == 0 &&
                    !String.IsNullOrWhiteSpace(target) &&
                    !String.IsNullOrWhiteSpace(publisher) &&
                    !String.IsNullOrWhiteSpace(series) &&
                    !String.IsNullOrWhiteSpace(subcategory))
                {
                    itemsToRemove = db.FavoriteList.
                        Where(x =>
                        x.Target == target &&
                        x.Publisher == publisher &&
                        x.Series == series &&
                        x.Subcategory == subcategory
                        ).
                        ToList();
                    db.FavoriteList.Add(new FavoriteItem()
                    {
                        Target = target,
                        Publisher = publisher,
                        Series = series,
                        Subcategory = subcategory
                    });
                    db.SaveChanges();
                }
                else if (categoryId > 0 &&
                    !String.IsNullOrWhiteSpace(target) &&
                    !String.IsNullOrWhiteSpace(publisher) &&
                    !String.IsNullOrWhiteSpace(series) &&
                    !String.IsNullOrWhiteSpace(subcategory))
                {
                    itemsToRemove = db.FavoriteList.
                        Where(x =>
                        x.Category.Id == categoryId &&
                        x.Target == target &&
                        x.Publisher == publisher &&
                        x.Series == series &&
                        x.Subcategory == subcategory
                        ).
                        ToList();
                    db.FavoriteList.Add(new FavoriteItem()
                    {
                        Category = category,
                        Target = target,
                        Publisher = publisher,
                        Series = series,
                        Subcategory = subcategory
                    });
                    db.SaveChanges();
                }
                foreach (var item in itemsToRemove)
                    db.Remove(item);
                db.SaveChanges();
                return true;
            }
        }
    }
}
