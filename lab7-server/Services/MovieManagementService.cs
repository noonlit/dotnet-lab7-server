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
	public class MovieManagementService : IMovieManagementService
	{
		public ApplicationDbContext _context;
		public MovieManagementService(ApplicationDbContext context)
		{
			_context = context;
		}

		public async Task<ServiceResponse<PaginatedResultSet<Movie>, IEnumerable<EntityManagementError>>> GetMovies(int? page = 1, int? perPage = 10)
		{
			var movies = await _context.Movies
				.Skip((page.Value - 1) * perPage.Value)
				.Take(perPage.Value)
				.ToListAsync();

			var count = await getMoviesCount();

			var resultSet = new PaginatedResultSet<Movie>(movies, page.Value, count, perPage.Value);

			var serviceResponse = new ServiceResponse<PaginatedResultSet<Movie>, IEnumerable<EntityManagementError>>();
			serviceResponse.ResponseOk = resultSet;
			return serviceResponse;
		}

		public async Task<ServiceResponse<PaginatedResultSet<Movie>, IEnumerable<EntityManagementError>>> GetFilteredMovies(string startDate, string endDate, int? page = 1, int? perPage = 10)
		{
			var startDateDt = DateTime.Parse(startDate);
			var endDateDt = DateTime.Parse(endDate);

			var movies = await _context.Movies
				.Where(m => m.AddedAt >= startDateDt && m.AddedAt <= endDateDt)
				.OrderByDescending(m => m.ReleaseYear)
				.Skip((page.Value - 1) * perPage.Value)
				.Take(perPage.Value)
				.ToListAsync();

			var count = await getMoviesCount();

			var resultSet = new PaginatedResultSet<Movie>(movies, page.Value, count, perPage.Value);

			var serviceResponse = new ServiceResponse<PaginatedResultSet<Movie>, IEnumerable<EntityManagementError>>();
			serviceResponse.ResponseOk = resultSet;
			return serviceResponse;
		}

		public async Task<int> getMoviesCount()
		{
			return await _context.Movies.CountAsync();
		}

		public async Task<int> getCommentsCount(int movieId)
		{
			return await _context.Comments.Where(c => c.MovieId == movieId).CountAsync();
		}

		public async Task<ServiceResponse<Movie, IEnumerable<EntityManagementError>>> GetMovie(int id)
		{
			var movie = await _context.Movies.FindAsync(id);

			var serviceResponse = new ServiceResponse<Movie, IEnumerable<EntityManagementError>>();
			serviceResponse.ResponseOk = movie;
			return serviceResponse;
		}

		public async Task<ServiceResponse<PaginatedResultSet<Comment>, IEnumerable<EntityManagementError>>> GetCommentsForMovie(int id, int? page = 1, int? perPage = 10)
		{
			var comments = await _context.Comments
				.Where(c => c.MovieId == id)
				.Skip((page.Value - 1) * perPage.Value)
				.Take(perPage.Value)
				.ToListAsync();

			var count = await getCommentsCount(id);

			var resultSet = new PaginatedResultSet<Comment>(comments, page.Value, count, perPage.Value);

			var serviceResponse = new ServiceResponse<PaginatedResultSet<Comment>, IEnumerable<EntityManagementError>>();
			serviceResponse.ResponseOk = resultSet;
			return serviceResponse;
		}

		public async Task<ServiceResponse<Movie, IEnumerable<EntityManagementError>>> UpdateMovie(Movie movie)
		{
			_context.Entry(movie).State = EntityState.Modified;
			var serviceResponse = new ServiceResponse<Movie, IEnumerable<EntityManagementError>>();

			try
			{
				await _context.SaveChangesAsync();
				serviceResponse.ResponseOk = movie;
			}
			catch (DbUpdateConcurrencyException e)
			{
				var errors = new List<EntityManagementError>();
				errors.Add(new EntityManagementError { Code = e.GetType().ToString(), Description = e.Message });
			}

			return serviceResponse;
		}

		public async Task<ServiceResponse<Comment, IEnumerable<EntityManagementError>>> UpdateComment(Comment comment)
		{
			_context.Entry(comment).State = EntityState.Modified;
			var serviceResponse = new ServiceResponse<Comment, IEnumerable<EntityManagementError>>();

			try
			{
				await _context.SaveChangesAsync();

				serviceResponse.ResponseOk = comment;
			}
			catch (DbUpdateConcurrencyException e)
			{
				var errors = new List<EntityManagementError>();
				errors.Add(new EntityManagementError { Code = e.GetType().ToString(), Description = e.Message });
			}

			return serviceResponse;
		}

		public async Task<ServiceResponse<Movie, IEnumerable<EntityManagementError>>> CreateMovie(Movie movie)
		{
			_context.Movies.Add(movie);
			var serviceResponse = new ServiceResponse<Movie, IEnumerable<EntityManagementError>>();

			try
			{
				await _context.SaveChangesAsync();
				serviceResponse.ResponseOk = movie;
			}
			catch (Exception e)
			{
				var errors = new List<EntityManagementError>();
				errors.Add(new EntityManagementError { Code = e.GetType().ToString(), Description = e.Message });
			}

			return serviceResponse;
		}
		public async Task<ServiceResponse<Comment, IEnumerable<EntityManagementError>>> CreateComment(Comment comment)
		{
			_context.Comments.Add(comment);
			var serviceResponse = new ServiceResponse<Comment, IEnumerable<EntityManagementError>>();

			try
			{
				await _context.SaveChangesAsync();
				serviceResponse.ResponseOk = comment;
			}
			catch (Exception e)
			{
				var errors = new List<EntityManagementError>();
				errors.Add(new EntityManagementError { Code = e.GetType().ToString(), Description = e.Message });
			}

			return serviceResponse;
		}

		public async Task<ServiceResponse<Comment, IEnumerable<EntityManagementError>>> AddCommentToMovie(int movieId, Comment comment)
		{
			var movie = await _context.Movies
				.Where(m => m.Id == movieId)
				.Include(m => m.Comments).FirstOrDefaultAsync();

			var serviceResponse = new ServiceResponse<Comment, IEnumerable<EntityManagementError>>(); 

			if (movie == null)
			{
				var errors = new List<EntityManagementError>();
				errors.Add(new EntityManagementError { Description = "The movie doesn't exist." });
				return serviceResponse;
			}

			try
			{
				movie.Comments.Add(comment);
				_context.Entry(movie).State = EntityState.Modified;
				_context.SaveChanges();

				serviceResponse.ResponseOk = comment;
			}
			catch (Exception e)
			{
				var errors = new List<EntityManagementError>();
				errors.Add(new EntityManagementError { Code = e.GetType().ToString(), Description = e.Message });
			}

			return serviceResponse;
		}

		public async Task<ServiceResponse<bool, IEnumerable<EntityManagementError>>> DeleteComment(int commentId)
		{ 
			var serviceResponse = new ServiceResponse<bool, IEnumerable<EntityManagementError>>();

			try
			{
				var comment = await _context.Comments.FindAsync(commentId);
				_context.Comments.Remove(comment);
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

		public async Task<ServiceResponse<bool, IEnumerable<EntityManagementError>>> DeleteMovie(int movieId)
		{
			var serviceResponse = new ServiceResponse<bool, IEnumerable<EntityManagementError>>();

			try
			{
				var movie = await _context.Movies.FindAsync(movieId);
				_context.Movies.Remove(movie);
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

		public bool MovieExists(int id)
		{
			return _context.Movies.Any(e => e.Id == id);
		}

		public bool CommentExists(int id)
		{
			return _context.Comments.Any(e => e.Id == id);
		}
	}
}