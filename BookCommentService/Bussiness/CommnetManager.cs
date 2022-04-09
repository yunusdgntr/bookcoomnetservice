using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookCommentService.Models;
using HtmlAgilityPack;

namespace BookCommentService.Bussiness
{
	public class CommnetManager
	{
        ConcurrentQueue<Book> coll;
        public CommnetManager()
		{
            coll = new();
		}

        public List<Book> GetBooks(string serachtext)
        {
            var books = GerBookBySearchText(serachtext);
            var tasks = books.Select(i => DoWorkAsync(i));
            Task.WhenAll(tasks).Wait();
            var s = coll.ToList();
            return s;
        }
        private async Task DoWorkAsync(Book book)
        {
            var comments = await GetCommentsBulk("https://www.kitapyurdu.com/index.php?route=product/product/review&product_id=" + book.Id);
            book.Comments = comments;
            coll.Enqueue(book);
            Console.WriteLine(book.Id + "-->" + DateTime.Now.ToString());
            //Console.WriteLine(JsonSerializer.Serialize(book, new JsonSerializerOptions { WriteIndented = true, Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping }));
        }

        private  List<Book> GerBookBySearchText(string searchText)
        {
            try
            {
                var web = new HtmlWeb();
                var list = new List<Book>();
                var document = web.Load("https://www.kitapyurdu.com/index.php?route=product/search&filter_name=" + searchText);
                var productList = document.DocumentNode.Descendants(0).Where(n => n.Id == "product-table").FirstOrDefault();
                var productElements = productList.ChildNodes.Where(n => n.HasClass("product-cr")).ToList();
                foreach (var productElement in productElements)
                {
                    var id = productElement.Attributes["id"].Value.Split("-")[1];
                    var name = productElement.ChildNodes.Where(x => x.HasClass("name")).First().InnerText;
                    var photo = productList.ChildNodes.Descendants().Where(x => x.HasClass("pr-img-link")).First().ChildNodes.Where(x => x.Name == "img").FirstOrDefault().GetAttributeValue("src", string.Empty);
                    var author = productElement.ChildNodes.Where(x => x.HasClass("author")).Last().InnerText;
                    list.Add(new Book { Id = id, Name = name, Author = author, PhotoLink = photo });
                }

                return list;
            }
            catch (Exception)
            {

                return null;
            }
        }

        private async Task<List<Comment>> GetCommentsBulk(string link)
        {
            var allCommnets = new List<Comment>();
            var web = new HtmlWeb();
            var document = await web.LoadFromWebAsync(link);
            var links = GetLinks(document);
            for (int i = 1; i < links.Count + 1; i++)
            {
                var comments = GetCommnetsPerPage(link + "&page=" + i);
                allCommnets.AddRange(comments);
            }
            foreach (var item in links)
            {
                var comments = GetCommnetsPerPage(item);
                allCommnets.AddRange(comments);
            }
            return allCommnets;
        }

        private List<Comment> GetCommnetsPerPage(string link)
        {
            var web = new HtmlWeb();
            var document = web.Load(link);
            var reviews = document.DocumentNode.Descendants(0).Where(n => n.HasClass("review-text")).ToList();
            var user = document.DocumentNode.Descendants(0).Where(n => n.HasClass("review-user")).ToList();
            var date = document.DocumentNode.Descendants(0).Where(n => n.HasClass("review-date")).ToList();
            var list = new List<Comment>();
            for (int i = 0; i < reviews.Count; i++)
            {
                list.Add(new Comment { User = user[i].InnerText.Trim(), Commnet = reviews[i].InnerText.Trim(), Date = date[i].InnerText.Trim() });
            }
            return list;
        }

        private List<string> GetLinks(HtmlDocument document)
        {
            var linkText = new List<string>();
            try
            {
                var links = document.DocumentNode.Descendants(0).Where(n => n.HasClass("links")).First();
                foreach (HtmlNode link in links.ChildNodes.Where(c => c.Name == "a").ToList())
                {
                    // Get the value of the HREF attribute
                    string hrefValue = link.GetAttributeValue("href", string.Empty);
                    if (!linkText.Contains(hrefValue)) linkText.Add(hrefValue);

                }
            }
            catch (Exception)
            {

                // throw;
            }
            return linkText;
        }
    }
}

