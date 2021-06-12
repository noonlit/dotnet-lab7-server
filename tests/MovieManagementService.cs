using Lab7.Data;
using NUnit.Framework;
using System;
using Microsoft.EntityFrameworkCore;
using static tests.OperationalStoreForTests;
using Lab7.Services;
using System.Threading.Tasks;

namespace tests
{
	class MovieManagementService
    {
        private ApplicationDbContext _context;
        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDB")
                .Options;

            _context = new ApplicationDbContext(options, new OperationalStoreOptionsForTests());

            _context.Movies.Add(new Lab7.Models.Movie { });
            _context.Movies.Add(new Lab7.Models.Movie { });
            _context.SaveChanges();
        }

        [TearDown]
        public void Teardown()
        {
            foreach (var movie in _context.Movies)
            {
                _context.Remove(movie);
            }
            _context.SaveChanges();
        }

        [Test]
        public async Task TestGetMovies()
        {
            var service = new Lab7.Services.MovieManagementService(_context);
            var moviesResponse = await service.GetMovies();
            var moviesCount = moviesResponse.ResponseOk.Count;
            Assert.AreEqual(2, moviesCount);
        }
    }
}
