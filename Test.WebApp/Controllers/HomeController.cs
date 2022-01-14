using DevExtreme.AspNet.Data.MongoDB;
using DevExtreme.AspNet.Data.MongoDB.Mvc;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Newtonsoft.Json;
using Test.WebApp.Models;

namespace Test.WebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly IMongoCollection<Movie> _movieCollection;

        public HomeController(IMongoDatabase database)
        {
            _movieCollection = database.GetCollection<Movie>("movies");
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Movies(DataSourceLoadOptions loadOptions)
        {
            var movieList = _movieCollection.Find(FilterDefinition<Movie>.Empty);
            var loadResult = MongoDataSourceLoader.Load(movieList, loadOptions);
            var jsonResult = JsonConvert.SerializeObject(loadResult);
            
            return Content(jsonResult, "application/json");
        }
    }
}