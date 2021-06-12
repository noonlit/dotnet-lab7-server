using Lab7.Data;
using Lab7.Errors;
using Lab7.Models;
using Lab7.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lab7.Services
{
	public class FavouritesManagementService : IFavouritesManagementService
	{
		public ApplicationDbContext _context;
		public FavouritesManagementService(ApplicationDbContext context)
		{
			_context = context;
		}

		public async Task<ServiceResponse<Favourites, IEnumerable<EntityManagementError>>> CreateFavourites(string userId, NewFavouritesForUserViewModel newFavouritesRequest)
		{
			List<Movie> movies = new List<Movie>();

			newFavouritesRequest.MovieIds.ForEach(mid =>
			{
				var movie = _context.Movies.Find(mid);
				if (movie != null)
				{
					movies.Add(movie);
				}
			});

			var favourites = new Favourites
			{
				UserId = userId,
				Movies = movies,
				Year = newFavouritesRequest.Year
			};

			_context.Favourites.Add(favourites);
			var serviceResponse = new ServiceResponse<Favourites, IEnumerable<EntityManagementError>>();

			try
			{
				await _context.SaveChangesAsync();
				serviceResponse.ResponseOk = favourites;
			}
			catch (Exception e)
			{
				var errors = new List<EntityManagementError>();
				errors.Add(new EntityManagementError { Code = e.GetType().ToString(), Description = e.Message });
			}

			return serviceResponse;
		}

		public async Task<ServiceResponse<List<Favourites>, IEnumerable<EntityManagementError>>> GetFavourites(string userId)
		{
			var result = await _context.Favourites.Where(f => f.User.Id == userId).Include(f => f.Movies).OrderByDescending(f => f.Year).ToListAsync();
			var serviceResponse = new ServiceResponse<List<Favourites>, IEnumerable<EntityManagementError>>();
			serviceResponse.ResponseOk = result;
			return serviceResponse;
		}

		public async Task<ServiceResponse<Favourites, IEnumerable<EntityManagementError>>> UpdateFavourites(Favourites favourites, UpdateFavouritesForUserViewModel updateFavouritesRequest)
		{
			favourites.Movies = await _context.Movies.Where(m => updateFavouritesRequest.MovieIds.Contains(m.Id)).ToListAsync();
			_context.Entry(favourites).State = EntityState.Modified;
			var serviceResponse = new ServiceResponse<Favourites, IEnumerable<EntityManagementError>>();

			try
			{
				await _context.SaveChangesAsync();
				serviceResponse.ResponseOk = favourites;
			}
			catch (Exception e)
			{
				var errors = new List<EntityManagementError>();
				errors.Add(new EntityManagementError { Code = e.GetType().ToString(), Description = e.Message });
			}

			return serviceResponse;
		}

		public async Task<ServiceResponse<Favourites, IEnumerable<EntityManagementError>>> GetFavouritesForYear(string userId, int year)
		{
			var result = await _context.Favourites.Where(f => f.Year == year && f.User.Id == userId).FirstOrDefaultAsync();
			var serviceResponse = new ServiceResponse<Favourites, IEnumerable<EntityManagementError>>();
			serviceResponse.ResponseOk = result;
			return serviceResponse;
		}

		public async Task<ServiceResponse<Favourites, IEnumerable<EntityManagementError>>> GetFavourite(string userId, int id)
		{
			var result = await _context.Favourites.Where(f => f.Id == id && f.User.Id == userId).FirstOrDefaultAsync();
			var serviceResponse = new ServiceResponse<Favourites, IEnumerable<EntityManagementError>>();
			serviceResponse.ResponseOk = result;
			return serviceResponse;
		}

		public async Task<ServiceResponse<bool, IEnumerable<EntityManagementError>>> DeleteFavouritesByYear(string userId, int year)
		{
			var serviceResponse = new ServiceResponse<bool, IEnumerable<EntityManagementError>>();

			try
			{
				var favourite = await _context.Favourites.Where(e => e.Year == year && e.UserId == userId).FirstOrDefaultAsync();
				_context.Favourites.Remove(favourite);
				await _context.SaveChangesAsync();
				serviceResponse.ResponseOk = true;
			}
			catch (Exception e)
			{
				var errors = new List<EntityManagementError>();
				errors.Add(new EntityManagementError { Code = e.GetType().ToString(), Description = e.Message });
			}

			return serviceResponse;
		}

		public async Task<ServiceResponse<bool, IEnumerable<EntityManagementError>>> DeleteFavourites(int id)
		{
			var serviceResponse = new ServiceResponse<bool, IEnumerable<EntityManagementError>>();

			try
			{
				var favourite = await _context.Favourites.FindAsync(id);
				_context.Favourites.Remove(favourite);
				await _context.SaveChangesAsync();
				serviceResponse.ResponseOk = true;
			}
			catch (Exception e)
			{
				var errors = new List<EntityManagementError>();
				errors.Add(new EntityManagementError { Code = e.GetType().ToString(), Description = e.Message });
			}

			return serviceResponse;
		}

		public bool FavouritesForYearExists(string userId, int year)
		{
			return _context.Favourites.Any(e => e.Year == year && e.UserId == userId);
		}

		public bool FavouritesExists(string userId, int favouritesId)
		{
			return _context.Favourites.Any(e => e.Id == favouritesId && e.UserId == userId);
		}
	}
}
