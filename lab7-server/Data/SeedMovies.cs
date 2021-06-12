using Lab7.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace Lab7.Data
{
    public class SeedMovies
    {
        private static string Characters = "abcdefghijklmnopqrstuvwxyz";
        private static Random random = new Random();

        public static void Seed(ApplicationDbContext context, int count)
        {
            context.Database.EnsureCreated();

            var movies = new List<Movie>();
            for (int i = 0; i < count; ++i)
            {
                var movie = new Models.Movie
                {
                    Title = generateRandomString(3, 10),
                    Description = generateRandomString(5, 50),
                    Director = generateRandomString(3, 10),
                    Genre = getRandomGenre(),
                    DurationMinutes = generateRandomInt(10, 200),
                    ReleaseYear = generateRandomInt(1888, DateTime.Now.Year),
                    Watched = generateRandomBoolean(),
                };

                if (movie.Watched)
				{
                    movie.Rating = generateRandomInt(1, 10);
				}

                context.Movies.Add(movie);
                movies.Add(movie);
            }

            context.SaveChanges();
        }

        private static bool generateRandomBoolean()
		{
            return random.Next(0, 3000) < 1500;
		}

        private static int generateRandomInt(int min, int max)
		{
            return random.Next(min, max);
		}

        private static string generateRandomString(int min, int max)
        {
            string s = "";

            for (int j = 0; j < random.Next(min, max); ++j)
            {
                s += Characters[random.Next(Characters.Length)];
            }

            return s;
        }

        private static Models.Movie.GenreType getRandomGenre()
		{
            var v = Enum.GetValues(typeof(Models.Movie.GenreType));
            return (Models.Movie.GenreType) v.GetValue(random.Next(v.Length));
        }
    }
}
