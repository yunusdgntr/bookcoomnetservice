using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookCommentService.Bussiness;
using BookCommentService.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BookCommentService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BookCommnetController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<BookCommnetController> _logger;

        public BookCommnetController(ILogger<BookCommnetController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        [HttpGet("{searchText}")]
        public IEnumerable<Book> Get(string searchText)
        {
            var manager = new CommnetManager();
           return manager.GetBooks(searchText);
        }
    }
}

