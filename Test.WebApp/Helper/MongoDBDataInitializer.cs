using System.Collections.Generic;
using MongoDB.Driver;
using Test.WebApp.Models;

namespace Test.WebApp.Helper
{
    public class MongoDBDataInitializer
    {
        public static void Initialize(IMongoDatabase database)
        {
            IMongoCollection<Movie> movieCollection = database.GetCollection<Movie>("movies");
            
            if (!movieCollection.AsQueryable().Any())
            {
                var movieList = new List<Movie>()
                {
                    new Movie() { Id = 1, Name = "WALL-E", Genre = "Animation", LeadStudio = "Disney", Year = 2008 },
                    new Movie() { Id = 2, Name = "Twilight", Genre = "Romance", LeadStudio = "Summit", Year = 2008 },
                    new Movie() { Id = 3, Name = "Remember Me", Genre = "Drama", LeadStudio = "Drama", Year = 2010 },
                    new Movie() { Id = 4, Name = "One Day", Genre = "Romance", LeadStudio = "Independent", Year = 2011 },
                    new Movie() { Id = 5, Name = "(500) Days of Summer", Genre = "Comedy", LeadStudio = "Fox", Year = 2008 },
                    new Movie() { Id = 6, Name = "P.S. I Love You", Genre = "Romance", LeadStudio = "Independent", Year = 2007 },
                    new Movie() { Id = 7, Name = "Midnight in Paris", Genre = "Romence", LeadStudio = "Sony", Year = 2011 },
                };

                movieCollection.InsertMany(movieList);
            }
        }
    }
}