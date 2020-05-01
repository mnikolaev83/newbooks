using BookDataProvider;
using BookDataProvider.Entities;
using HtmlAgilityPack;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BookFetcher
{
	class Program
	{
		public const string ROOT_URL = "http://mdk-arbat.ru";
		delegate Task<bool> WebJobOperation();
		public static int logJobId = 0;
		public static int booksFetched = 0;

		private class JobTask
		{
			private WebJobOperation Task { get; set; }
			public JobTask(WebJobOperation task)
			{
				Task = task;
			}
			public Task<bool> Run()
			{
				return Task();
			}
		}

		private static List<HtmlNode> ListNodes(HtmlNode node)
		{
			var list = new List<HtmlNode>();
			foreach (var child in node.ChildNodes)
			{
				list.Add(child);
				if (child.ChildNodes.Count > 0)
				{
					list.AddRange(ListNodes(child));
				}
			}
			return list;
		}
		private class BookModel
		{
			public string StoreCode { get; set; }
			public string AuthorShortName { get; set; }
			public string Title { get; set; }
		}
		private class BookDetails
		{
			public string AuthorFullName { get; set; }
			public DateTime? DateAddedToStore { get; set; }
			public string ImageURL { get; set; }
			public string Publisher { get; set; }
			public string Subcategory { get; set; }
			public int Year { get; set; }
			public int PagesAmnt { get; set; }
			public int RunAmnt { get; set; }
			public string Target { get; set; }
			public string Description { get; set; }
			public string Translated { get; set; }
			public string Series { get; set; }
			public string ISBN { get; set; }
		}
		private const string DIGITS = "0123456789";

		private static string TrimDigitValue(string value)
		{
			return Regex.Replace(value, @"[^\d]", String.Empty);
		}
		private static bool NodeHasClass(HtmlNode node, string className)
		{
			if (node.Attributes.Count > 0)
				if (node.Attributes.Where(x => x.Name == "class").Count() > 0)
				{
					var aClass = node.Attributes.Where(x => x.Name == "class").FirstOrDefault().Value;
					var classes = aClass.Split(" ");
					if (classes.Contains(className))
						return true;
				}
			return false;
		}
		private static string GetIdFromNode(HtmlNode node)
		{
			if (node.InnerHtml.Trim().StartsWith("Код товара:"))
				return TrimDigitValue(node.InnerHtml);
			return String.Empty;
		}
		private static string GetIdFromNodeHref(HtmlNode node)
		{
			var childNodes = ListNodes(node);
			foreach (var childNode in childNodes)
			{
				if (childNode.Attributes.Where(x => x.Name == "href").Count() > 0)
				{
					var href = childNode.Attributes.Where(x => x.Name == "href").FirstOrDefault().Value;
					return TrimDigitValue(href);
				}
			}
			return null;
		}
		private static string GetTitleFromNode(HtmlNode node)
		{
			var h3node = node.ChildNodes.Where(x => x.Name == "h3").FirstOrDefault();
			if (h3node == null)
				return null;
			var h3anode = h3node.ChildNodes.Where(x => x.Name == "a").FirstOrDefault();
			if (h3anode == null)
				return null;
			return h3anode.InnerHtml.Trim();
		}
		private static string GetShortAuthorFromNode(HtmlNode node)
		{
			var anode = node.ChildNodes.Where(x => x.Name == "a").FirstOrDefault();
			if (anode == null)
				return null;
			return anode.InnerHtml.Trim();
		}
		private static List<BookModel> ParseHtmlForBookList(HtmlDocument pageDocument)
		{
			var nodes = new List<HtmlNode>();
			var books = new List<BookModel>();
			foreach (var node in pageDocument.DocumentNode.ChildNodes)
			{
				var childList = ListNodes(node);
				nodes.AddRange(childList);
			}
			var goodDescriptions = new List<HtmlNode>();
			foreach (var item in nodes)
			{
				if (NodeHasClass(item, "tg-postbook"))
					goodDescriptions.Add(item);
			}
			foreach (var description in goodDescriptions)
			{
				var book = new BookModel();
				var descriptionNodes = ListNodes(description);
				foreach (var item in descriptionNodes)
				{
					if (NodeHasClass(item, "tg-booktitle"))
					{
						var id = GetIdFromNodeHref(item);
						if (!String.IsNullOrWhiteSpace(id))
							book.StoreCode = id;
						var title = GetTitleFromNode(item);
						if (!String.IsNullOrWhiteSpace(title))
							book.Title = title;
					}
					if (NodeHasClass(item, "tg-bookwriter"))
					{
						var author = GetShortAuthorFromNode(item);
						if (!String.IsNullOrWhiteSpace(author))
							book.AuthorShortName = author;
					}
				}
				books.Add(book);
			}
			return books;
		}
		private static BookDetails ParseHtmlForBookDetails(HtmlDocument pageDocument)
		{
			var nodes = new List<HtmlNode>();
			var bookDetails = new BookDetails();
			foreach (var node in pageDocument.DocumentNode.ChildNodes)
			{
				var childList = ListNodes(node);
				nodes.AddRange(childList);
			}
			var imageNodes = nodes.Where(x => x.Name == "figure").ToList();
			foreach (var imageNode in imageNodes)
			{
				if (NodeHasClass(imageNode, "tg-featureimg"))
				{
					var imgSrcNode = imageNode.ChildNodes.Where(x => x.Name == "img").FirstOrDefault();
					if (imgSrcNode != null)
					{
						bookDetails.ImageURL = imgSrcNode.
							Attributes.Where(x => x.Name == "src").
							FirstOrDefault()?.Value;
					}
				}
			}
			var tgBookWriterNode = nodes.Where(x => x.Attributes.
				Where(y => y.Name == "itemprop" && y.Value == "author").Any()).
				FirstOrDefault();
			if (tgBookWriterNode != null)
				bookDetails.AuthorFullName = tgBookWriterNode.InnerText;
			var descriptionNode = nodes.Where(x => x.Attributes.
				Where(y => y.Name == "itemprop" && y.Value == "description").Any()).
				FirstOrDefault();
			var currentDescription = String.Empty;
			if (descriptionNode != null)
				currentDescription = descriptionNode.InnerText;
			bookDetails.Description = currentDescription.Trim();
			bookDetails.DateAddedToStore = DateTime.Now;
			var ulNodes = nodes.Where(x => x.Name == "ul").ToList();
			HtmlNode mainDataNode = null;
			foreach (var ulNode in ulNodes)
				if (NodeHasClass(ulNode, "tg-productinfo"))
					mainDataNode = ulNode;
			if (mainDataNode != null)
			{
				var mainDataNodes = ListNodes(mainDataNode);
				foreach (var node in mainDataNodes.Where(x => x.Name == "li").ToList())
				{
					if (node.InnerText.StartsWith("ISBN:"))
						try
						{
							bookDetails.ISBN = TrimDigitValue(node.InnerText.Replace("ISBN:", String.Empty));
						}
						catch (Exception)
						{
							bookDetails.ISBN = String.Empty;
						}
					if (node.InnerText.StartsWith("Издательство:"))
						bookDetails.Publisher = node.InnerText.Replace("Издательство:", String.Empty).Trim();
					if (node.InnerText.StartsWith("Серия:"))
						bookDetails.Series = node.InnerText.Replace("Серия:", String.Empty).Trim();
					if (node.InnerText.StartsWith("Рубрика:"))
						bookDetails.Subcategory = node.InnerText.Replace("Рубрика:", String.Empty).Trim();
					if (node.InnerText.StartsWith("Целевое назначение:"))
						bookDetails.Target = node.InnerText.Replace("Целевое назначение:", String.Empty).Trim();
					if (node.InnerText.StartsWith("Год издания:"))
						try
						{
							bookDetails.Year = Convert.ToInt32(TrimDigitValue(node.InnerText.Replace("Год издания:", String.Empty)));
						}
						catch (Exception)
						{
							bookDetails.Year = 0;
						}
					if (node.InnerText.StartsWith("Количество страниц:"))
						try
						{
							bookDetails.PagesAmnt = Convert.ToInt32(TrimDigitValue(node.InnerText.Replace("Количество страниц:", String.Empty)));
						}
						catch (Exception)
						{
							bookDetails.PagesAmnt = 0;
						}
					if (node.InnerText.StartsWith("Тираж:"))
						try
						{
							bookDetails.RunAmnt = Convert.ToInt32(TrimDigitValue(node.InnerText.Replace("Тираж:", String.Empty)));
						}
						catch (Exception)
						{
							bookDetails.RunAmnt = 0;
						}
				}
			}
			return bookDetails;
		}
		private static int GetAmountOfPagesInDocument(HtmlDocument pageDocument)
		{
			var nodes = new List<HtmlNode>();
			var amount = 1;
			foreach (var node in pageDocument.DocumentNode.ChildNodes)
			{
				var childList = ListNodes(node);
				nodes.AddRange(childList);
			}
			foreach (var item in nodes)
			{
				if (NodeHasClass(item, "tg-pagination"))
				{
					var ulNode = item.ChildNodes.Where(x => x.Name.ToLower().Equals("ul")).FirstOrDefault();
					if (ulNode == null)
						continue;
					foreach (var childItem in ulNode.ChildNodes)
					{
						var pageNo = TrimDigitValue(childItem.InnerText);
						var pageNoAsInt = 0;
						try
						{
							pageNoAsInt = Convert.ToInt32(pageNo);
						}
						catch (Exception)
						{
						}
						if (pageNoAsInt > 0)
							if (pageNoAsInt > amount)
								amount = pageNoAsInt;
					}
				}
			}
			return amount;
		}
		private static async Task<string> ReadPage(string url, int categoryId)
		{
			var pageDocument = await ReadStorePage(url, Method.POST);
			var books = ParseHtmlForBookList(pageDocument);
			using (var db = new BookDBContext())
			{
				var category = await db.Categories.
					Where(x => x.Id == categoryId).
					FirstOrDefaultAsync();

				foreach (var bookModel in books)
				{
					var code = bookModel.StoreCode;
					var existingBook = db.Books.
						Include(x => x.Category).
						Where(x => (!String.IsNullOrWhiteSpace(code) ? x.StoreCode == code : false)
						).
						FirstOrDefault();
					Book book;
					if (existingBook != null)
					{
						if ((existingBook.PagesAmnt > 0) &&
							(!existingBook.ImageURL.Contains("no_foto") &&
							(!String.IsNullOrWhiteSpace(existingBook.AuthorFullName)) &&
							(!String.IsNullOrWhiteSpace(existingBook.ImageURL)) &&
							(!String.IsNullOrWhiteSpace(existingBook.Publisher)) &&
							(!String.IsNullOrWhiteSpace(existingBook.Subcategory)) &&
							(existingBook.Description.Length > 10))
							)
							continue;
						book = existingBook;
					}
					else
					{
						book = new Book();
						db.Books.Add(book);
						booksFetched++;
					}
					var details = await ReadBookDetails(bookModel.StoreCode);
					book.UpdatedAt = DateTime.Now;
					book.AuthorShortName = bookModel.AuthorShortName;
					book.Series = details.Series;
					book.StoreCode = bookModel.StoreCode;
					book.Title = bookModel.Title;
					book.UpdatedAt = DateTime.Now;
					book.Category = category;
					book.ImageURL = details.ImageURL;
					book.Publisher = details.Publisher;
					book.Year = details.Year;
					book.Isbn = details.ISBN;
					book.AuthorFullName = details.AuthorFullName;
					book.PagesAmnt = details.PagesAmnt;
					book.RunAmnt = details.RunAmnt;
					book.Subcategory = details.Subcategory;
					book.Target = details.Target;
					book.Translated = details.Translated;
					book.Description = details.Description;
				}
				db.SaveChanges();
			}
			return String.Empty;
		}
		private static async Task<bool> ReadBookLists()
		{
			try
			{
				using (var db = new BookDBContext())
				{

					var logRecord = new JobLog()
					{
						BooksFetched = 0,
						StartedAt = DateTime.Now,
					};
					db.JobLog.Add(logRecord);
					await db.SaveChangesAsync();
					logJobId = logRecord.Id;

					var categories = await db.Categories.ToListAsync();
					foreach (var category in categories)
					{
						var url = category.Url;
						var amountOfPages = await GetAmountOfPages(url);
						for (int i = 1; i <= amountOfPages; i++)
						{
							var urlWithPage = url + "&pid=" + i.ToString();
							await ReadPage(urlWithPage, category.Id);
						}
					}

					logRecord.BooksFetched = booksFetched;
					logRecord.CompletedAt = DateTime.Now;
					await db.SaveChangesAsync();

				}
				return true;
			}
			catch (Exception e)
			{
				using (var db = new BookDBContext())
				{
					var logRecord = await db.JobLog.
						Where(x => x.Id == logJobId).
						FirstOrDefaultAsync();
					logRecord.ExceptonMessage = e.Message;
					logRecord.ExceptonStacktrace = e.StackTrace;
					logRecord.FailedAt = DateTime.Now;
					logRecord.BooksFetched = booksFetched;
					await db.SaveChangesAsync();
				}
				return false;
			}
		}
		private static async Task<BookDetails> ReadBookDetails(string id)
		{
			var url = ROOT_URL + "/book/" + id;
			var pageDocument = await ReadStorePage(url, Method.GET);
			var details = ParseHtmlForBookDetails(pageDocument);
			return details;
		}
		public enum Method
		{
			POST = 1,
			GET = 2
		}
		private static async Task<HtmlDocument> ReadHTTPData(string url, Method method)
		{
			var client = new HttpClient();
			HttpResponseMessage response;
			Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
			Encoding encoding = Encoding.GetEncoding("utf-8");
			if (method == Method.GET)
			{
				response = await client.GetAsync(url);

			}
			else
			{
				var data = new
				{
					fD = 1,
					fI = 2,
					fO = 0,
					fQ = 1,
					fR = 2,
				};
				var content = new FormUrlEncodedContent(new Dictionary<string, string>() {
					{ "fD", "1" },
					{ "fI", "2" },
					{ "fO", "0" },
					{ "fQ", "1" },
					{ "fR", "2" },
				});
				response = await client.PostAsync(url, content);
			}

			var contentAsBytes = await response.Content.ReadAsByteArrayAsync();
			string responseString = encoding.GetString(contentAsBytes, 0, contentAsBytes.Length);
			var pageDocument = new HtmlDocument();
			pageDocument.LoadHtml(responseString);
			return pageDocument;
		}
		private static async Task<HtmlDocument> ReadStorePage(string url, Method method)
		{
			HtmlDocument document;
			try
			{
				document = await ReadHTTPData(url, method);
			}
			catch (Exception)
			{
				await Task.Delay(60 * 1000);
				try
				{
					document = await ReadHTTPData(url, method);
				}
				catch (Exception)
				{
					await Task.Delay(60 * 1000 * 10);
					try
					{
						document = await ReadHTTPData(url, method);
					}
					catch (Exception)
					{
						await Task.Delay(60 * 1000 * 60);
						try
						{
							document = await ReadHTTPData(url, method);
						}
						catch (Exception e)
						{
							throw e;
						}
					}
				}
			}
			return document;
		}
		private static async Task<int> GetAmountOfPages(string url)
		{
			var pageDocument = await ReadStorePage(url, Method.POST);
			var amountOfPages = GetAmountOfPagesInDocument(pageDocument);
			return amountOfPages;
		}
		private static async Task<bool> RunJob()
		{
			var tasksToRun = new List<JobTask>()
				{
					new JobTask(ReadBookLists),
				};
			foreach (var task in tasksToRun)
			{
				var result = await task.Run();
			}
			return true;
		}
		static void Main(string[] args)
		{
            var builder = new ConfigurationBuilder()
                   .SetBasePath(Directory.GetCurrentDirectory())
                   .AddJsonFile("appsettings.json");
            var configuration = builder.Build();
            var connectionString = configuration["connectionString"];
            BookDBContext.SetConnectionString(connectionString);
            var task = RunJob();
			task.Wait();
		}
	}
}
