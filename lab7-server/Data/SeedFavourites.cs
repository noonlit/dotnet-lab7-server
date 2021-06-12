using Lab7.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lab7.Data
{
	public class SeedFavourites
	{
        private static Random random = new Random();

        public static void Seed(ApplicationDbContext context, int count)
        {
            context.Database.EnsureCreated();
            var usersCount = context.ApplicationUsers.Count();
            var moviesCount = context.Movies.Count();

            for (int i = 0; i < count; ++i)
            {
                var user = context.ApplicationUsers.Skip(random.Next(1, usersCount)).Take(1).First();
                var movies = context.Movies.Skip(random.Next(1, moviesCount)).Take(3).ToList();

                var favourites = new Models.Favourites
                {
                    Year = generateRandomInt(1950, DateTime.Now.Year),
                    User = user,
                    Movies = movies
                };


                context.Favourites.Add(favourites);
            }

            context.SaveChanges();
        }

        private static int generateRandomInt(int min, int max)
        {
            return random.Next(min, max);
        }
    }
}
