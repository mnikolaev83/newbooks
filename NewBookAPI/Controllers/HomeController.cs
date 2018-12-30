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
                    x.DateAddedToStore < DateTime.Now.AddYears(-10) &&
                    x.AddedAt >= dateAddedFrom && x.AddedAt <= dateAddedTo).
                    ToList();
                books.AddRange(
                    db.Books.
                    Include(x => x.Category).
                    Where(x => x.Category.Id == category_id &&
                    x.DateAddedToStore >= DateTime.Now.AddYears(-10) &&
                    x.DateAddedToStore >= dateAddedFrom && x.DateAddedToStore <= dateAddedTo).
                    ToList());

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
                foreach (var book in books.OrderByDescending(x => x.DateAddedToStore).ToList())
                {
                    result.Add(new BookModel()
                    {
                        added_at = book.DateAddedToStore < DateTime.Now.AddYears(-20) ? book.AddedAt.ToShortDateString() : book.DateAddedToStore.ToShortDateString(),
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


                var existingItem = db.IgnoreList.
                    Include(x => x.Category).
                    Where(
                    x =>
                    ((model.category_id > 0) ? x.Category.Id == model.category_id : true) &&
                    (
                        ((!String.IsNullOrWhiteSpace(publisher) && !String.IsNullOrWhiteSpace(x.Publisher))) ? x.Publisher.Trim() == publisher.Trim() : true) &&
                    (((!String.IsNullOrWhiteSpace(series) && !String.IsNullOrWhiteSpace(x.Series))) ? x.Series.Trim() == series.Trim() : true) &&
                    (((!String.IsNullOrWhiteSpace(subcategory) && !String.IsNullOrWhiteSpace(x.Subcategory))) ? x.Subcategory.Trim() == subcategory.Trim() : true) &&
                    (((!String.IsNullOrWhiteSpace(target) && !String.IsNullOrWhiteSpace(x.Target))) ? x.Target.Trim() == target.Trim() : true)
                    ).
                    FirstOrDefault();
                if (existingItem != null)
                    return true;
                db.IgnoreList.Add(new IgnoreItem()
                {
                    Category = (model.category_id > 0) ? db.Categories.Where(x => x.Id == model.category_id).FirstOrDefault() : null,
                    Publisher = (!String.IsNullOrWhiteSpace(publisher)) ? publisher : null,
                    Series = (!String.IsNullOrWhiteSpace(series)) ? series : null,
                    Subcategory = (!String.IsNullOrWhiteSpace(subcategory.Trim())) ? subcategory : null,
                    Target = (!String.IsNullOrWhiteSpace(target.Trim())) ? target : null
                });
                db.SaveChanges();
                return true;
            }
        }
    }
}
