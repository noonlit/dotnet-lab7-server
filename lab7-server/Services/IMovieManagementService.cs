using Lab7.Errors;
using Lab7.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lab7.Services
{
	public interface IMovieManagementService
	{
		public Task<ServiceResponse<List<Movie>, IEnumerable<EntityManagementError>>> GetMovies();
		public Task<ServiceResponse<List<Movie>, IEnumerable<EntityManagementError>>> GetFilteredMovies(string startDate, string endDate);
		public Task<ServiceResponse<Movie, IEnumerable<EntityManagementError>>> GetMovie(int id);
		public Task<ServiceResponse<List<Comment>, IEnumerable<EntityManagementError>>> GetCommentsForMovie(int id);
		public Task<ServiceResponse<Movie, IEnumerable<EntityManagementError>>> UpdateMovie(Movie movie);
		public Task<ServiceResponse<Comment, IEnumerable<EntityManagementError>>> UpdateComment(Comment comment);
		public Task<ServiceResponse<Movie, IEnumerable<EntityManagementError>>> CreateMovie(Movie movie);
		public Task<ServiceResponse<Comment, IEnumerable<EntityManagementError>>> CreateComment(Comment comment);
		public Task<ServiceResponse<Comment, IEnumerable<EntityManagementError>>> AddCommentToMovie(int movieId, Comment comment);
		public Task<ServiceResponse<bool, IEnumerable<EntityManagementError>>> DeleteMovie(int movieId);
		public Task<ServiceResponse<bool, IEnumerable<EntityManagementError>>> DeleteComment(int commentId);
		public bool MovieExists(int id);
		public bool CommentExists(int id);
	}
}
