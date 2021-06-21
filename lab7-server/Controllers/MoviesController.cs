using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Lab7.Data;
using Lab7.Models;
using Microsoft.AspNetCore.Authorization;
using AutoMapper;
using Lab7.ViewModels;
using Lab7.Services;

namespace Lab7.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	[Produces("application/json")]
	public class MoviesController : ControllerBase
	{
		private readonly IMapper _mapper;
		private readonly IMovieManagementService _movieService;

		public MoviesController(IMapper mapper, IMovieManagementService movieService)
		{
			_mapper = mapper;
			_movieService = movieService;
		}

		/// <summary>
		/// Retrieves a list of movies filtered by the interval when they were added, ordered descendingly by release year.
		/// </summary>
		/// <remarks>
		/// Sample request:
		/// GET /api/Movies/filter/2011-02-10T12:10:00_2022-01-01
		/// </remarks>
		/// <param name="startDate"></param>
		/// <param name="endDate"></param>
		/// <response code="200">The filtered movies.</response>
		[HttpGet]
		[Route("filter/{startDate}_{endDate}")]
		public async Task<ActionResult<PaginatedResultSet<Movie>>> GetFilteredMovies(string startDate, string endDate, int? page = 1, int? perPage = 20)
		{
			var result = await _movieService.GetFilteredMovies(startDate, endDate, page, perPage);
			return result.ResponseOk;
		}

		/// <summary>
		/// Retrieves a list of movies.
		/// </summary>
		/// <remarks>
		/// Sample request:
		/// GET /api/Movies
		/// </remarks>
		/// <response code="200">The movies.</response>
		[HttpGet]
		public async Task<ActionResult<PaginatedResultSet<Movie>>> GetMovies(int? page = 1, int? perPage = 20)
		{
			var result = await _movieService.GetMovies(page, perPage);
			return result.ResponseOk;
		}

		/// <summary>
		/// Retrieves a movie by ID, including its comments.
		/// </summary>
		/// <remarks>
		/// Sample request:
		/// GET api/Movies/5/Comments
		/// </remarks>
		/// <param name="id">The movie ID</param>
		/// <response code="200">The movie.</response>
		/// <response code="404">If the movie is not found.</response>
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[HttpGet("{id}/Comments")]
		public async Task<ActionResult<PaginatedResultSet<Comment>>> GetCommentsForMovieAsync(int id, int? page = 1, int? perPage = 20)
		{
			if (!_movieService.MovieExists(id))
			{
				return NotFound();
			}

			var movieResponse = await _movieService.GetMovie(id);
			var movie = movieResponse.ResponseOk;

			if (movie == null)
			{
				return NotFound();
			}

			var commentsResponse = await _movieService.GetCommentsForMovie(id);
			return commentsResponse.ResponseOk;
		}

		/// <summary>
		/// Retrieves a movie by ID.
		/// </summary>
		/// <remarks>
		/// Sample request:
		/// GET api/Movies/5
		/// </remarks>
		/// <param name="id">The movie ID</param>
		/// <response code="200">The movie.</response>
		/// <response code="404">If the movie is not found.</response>
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[HttpGet("{id}")]
		public async Task<ActionResult<MovieViewModel>> GetMovie(int id)
		{
			var movieResponse = await _movieService.GetMovie(id);
			var movie = movieResponse.ResponseOk;

			if (movie == null)
			{
				return NotFound();
			}

			return _mapper.Map<MovieViewModel>(movie);
		}

		/// <summary>
		/// Updates a movie.
		/// </summary>
		/// <remarks>
		/// Sample request:
		///
		/// PUT /api/Movies/5
		/// {
		///		"id": 5
		///    "title": "Title",
		///    "description": "Description!",
		///    "genre": "Comedy",
		///    "durationMinutes": 20,
		///    "releaseYear": 2021,
		///    "director": "Some Director",
		///    "addedAt": "2021-08-10",
		///    "rating": 2,
		///    "watched": true
		/// }
		///
		/// </remarks>
		/// <param name="id">The movie ID</param>
		/// <param name="movie">The movie body.</param>
		/// <response code="204">If the item was successfully added.</response>
		/// <response code="400">If the ID in the URL doesn't match the one in the body.</response>
		/// <response code="404">If the item is not found.</response>
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[HttpPut("{id}")]
		[Authorize(AuthenticationSchemes = "Identity.Application,Bearer")]
		public async Task<IActionResult> PutMovie(int id, MovieViewModel movie)
		{
			if (id != movie.Id)
			{
				return BadRequest();
			}

			var movieResponse = await _movieService.UpdateMovie(_mapper.Map<Movie>(movie));

			if (movieResponse.ResponseError == null)
			{
				return NoContent();
			}

			if (!_movieService.MovieExists(id))
			{
				return NotFound();
			}

			return StatusCode(500);
		}

		/// <summary>
		/// Updates a movie comment.
		/// </summary>
		/// <remarks>
		/// Sample request:
		///
		/// PUT: api/Movies/1/Comments/2
		/// {
		///    "text": "some comment",
		///    "important": false,
		///    "movieId": 3,
		/// }
		///
		/// </remarks>
		/// <param name="commentId">The comment ID</param>
		/// <param name="comment">The comment body</param>
		/// <response code="204">If the item was successfully added.</response>
		/// <response code="400">If the ID in the URL doesn't match the one in the body.</response>
		/// <response code="404">If the item is not found.</response>
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[HttpPut("{id}/Comments/{commentId}")]
		public async Task<IActionResult> PutComment(int commentId, CommentViewModel comment)
		{
			if (commentId != comment.Id)
			{
				return BadRequest();
			}

			if (!_movieService.MovieExists(comment.MovieId))
			{
				return NotFound();
			}

			var commentResponse = await _movieService.UpdateComment(_mapper.Map<Comment>(comment));

			if (commentResponse.ResponseError == null)
			{
				return NoContent();
			}

			if (!_movieService.CommentExists(commentId))
			{
				return NotFound();
			}

			return StatusCode(500);
		}

		// POST: api/Movies
		/// <summary>
		/// Creates a movie.
		/// </summary>
		/// <remarks>
		/// Sample request:
		///
		/// POST /api/Movies
		/// {
		///    "title": "Title",
		///    "description": "Description!",
		///    "genre": "Comedy",
		///    "durationMinutes": 20,
		///    "releaseYear": 2021,
		///    "director": "Some Director",
		///    "addedAt": "2021-08-10",
		///    "rating": 2,
		///    "watched": true
		/// }
		///
		/// </remarks>
		/// <param name="movie"></param>
		/// <response code="201">Returns the newly created item</response>
		/// <response code="400">If the item is null or the rating is not a value between 1 and 10.</response>
		[ProducesResponseType(StatusCodes.Status201Created)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[HttpPost]
		[Authorize(AuthenticationSchemes = "Identity.Application,Bearer")]
		public async Task<ActionResult<Movie>> PostMovie(MovieViewModel movie)
		{
			var movieResponse = await _movieService.CreateMovie(_mapper.Map<Movie>(movie));

			if (movieResponse.ResponseError == null)
			{
				return CreatedAtAction("GetMovie", new { id = movie.Id }, movie);
			}

			return StatusCode(500);
		}

		/// <summary>
		/// Creates a movie comment.
		/// </summary>
		/// <remarks>
		/// Sample request:
		///
		/// POST /api/Movies/3/Comments
		/// {
		///    "text": "some comment",
		///    "important": false,
		///    "movieId": 3,
		/// }
		///
		/// </remarks>
		/// <param name="id">The movie ID</param>
		/// <param name="comment">The comment body</param>
		/// <response code="200">If the item was successfully added.</response>
		/// <response code="404">If movie is not found.</response>  
		[ProducesResponseType(StatusCodes.Status201Created)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[HttpPost("{id}/Comments")]
		public async Task<IActionResult> PostCommentForMovie(int id, CommentViewModel comment)
		{
			var commentResponse = await _movieService.AddCommentToMovie(id, _mapper.Map<Comment>(comment));

			if (commentResponse.ResponseError == null)
			{
				return Ok();
			}

			return StatusCode(500);
		}

		// DELETE: api/Movies/5
		/// <summary>
		/// Deletes a movie.
		/// </summary>
		/// <remarks>
		/// Sample request:
		///
		/// DELETE api/Movies/1
		///
		/// </remarks>
		/// <param name="id"></param>
		/// <response code="204">No content if successful.</response>
		/// <response code="404">If the movie doesn't exist.</response>  
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[HttpDelete("{id}")]
		[Authorize(AuthenticationSchemes = "Identity.Application,Bearer")]
		public async Task<IActionResult> DeleteMovie(int id)
		{
			if (!_movieService.MovieExists(id))
			{
				return NotFound();
			}

			var result = await _movieService.DeleteMovie(id);

			if (result.ResponseError == null)
			{
				return NoContent();
			}


			return StatusCode(500);
		}


		// DELETE: api/Movies/1/Comments/5
		/// <summary>
		/// Deletes a movie comment.
		/// </summary>
		/// <remarks>
		/// Sample request:
		///
		/// DELETE api/Movies/1/Comments/5
		///
		/// </remarks>
		/// <param name="commentId"></param>
		/// <response code="204">No content if successful.</response>
		/// <response code="404">If the comment doesn't exist.</response>  
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[HttpDelete("{id}/Comments/{commentId}")]
		[Authorize(AuthenticationSchemes = "Identity.Application,Bearer")]
		public async Task<IActionResult> DeleteComment(int commentId)
		{
			if (!_movieService.CommentExists(commentId))
			{
				return NotFound();
			}

			var result = await _movieService.DeleteComment(commentId);

			if (result.ResponseError == null)
			{
				return NoContent();
			}


			return StatusCode(500);
		}
	}
}
