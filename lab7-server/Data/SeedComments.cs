using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lab7.Data
{
	public class SeedComments
	{
        private static string Characters = "abcdefghijklmnopqrstuvwxyz123456890";
        private static Random random = new Random();

        public static void Seed(ApplicationDbContext context, int count)
        {
            context.Database.EnsureCreated();
            var moviesCount = context.Movies.Count();

            for (int i = 0; i < count; ++i)
            {
                var movie = context.Movies.Skip(random.Next(1, moviesCount)).Take(1).First();

                var comment = new Models.Comment
                {
                    Text = generateRandomString(3, 10),
                    Important = generateRandomBoolean(),
                    Movie = movie
                };


                context.Comments.Add(comment);
            }

            context.SaveChanges();
        }

        private static bool generateRandomBoolean()
        {
            return random.Next() < 1500;
        }

        private static string generateRandomString(int min, int max)
        {
            string title = "";

            for (int j = 0; j < random.Next(min, max); ++j)
            {
                title += Characters[random.Next(Characters.Length)];
            }

            return title;
        }
    }
}
