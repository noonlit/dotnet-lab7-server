using Lab7.Data;
using NUnit.Framework;
using System;
using Microsoft.EntityFrameworkCore;
using static tests.OperationalStoreForTests;
using System.Threading.Tasks;
using Lab7.Models;
using System.Collections.Generic;
using Lab7.ViewModels;

namespace tests
{
	class FavouritesManagementService
	{
        private ApplicationDbContext _context;
        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDB")
                .Options;

            _context = new ApplicationDbContext(options, new OperationalStoreOptionsForTests());

            _context.Favourites.Add(new Favourites { Id = 3, Movies = new List<Movie>(), User = new ApplicationUser { Id = "1a" }, Year = 2000 });
            _context.Favourites.Add(new Favourites { Id = 4, Movies = new List<Movie>(), User = new ApplicationUser { Id = "1b" }, Year = 2002 });
            _context.SaveChanges();
        }

        [TearDown]
        public void Teardown()
        {
            foreach (var favourite in _context.Favourites)
            {
                _context.Remove(favourite);
            }

            foreach (var user in _context.ApplicationUsers)
			{
                _context.Remove(user);
			}

            _context.SaveChanges();
        }

        [Test]
        public async Task TestGetFavourites()
        {
            var service = new Lab7.Services.FavouritesManagementService(_context);
            var favouritesResponse = await service.GetFavourites("1a");
            var favouritesCount = favouritesResponse.ResponseOk.Count;
            Assert.AreEqual(1, favouritesCount);
        }

        [Test]
        public async Task TestCreateFavourites()
        {
            var service = new Lab7.Services.FavouritesManagementService(_context);

            var favourites = new NewFavouritesForUserViewModel { MovieIds = new List<int>(), Year = 2001 };
            var favouritesResponse = await service.CreateFavourites("1a", favourites);
            Assert.True(favouritesResponse.ResponseOk is Favourites);
        }
    }
}
