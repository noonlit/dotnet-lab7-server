using FluentValidation;
using Lab7.Data;
using Lab7.ViewModels;

namespace Lab7.Validators
{
	public class CommentValidator: AbstractValidator<CommentViewModel>
	{
        private readonly ApplicationDbContext _context;

        public CommentValidator(ApplicationDbContext context)
        {
            _context = context;
            RuleFor(c => c.Text).MinimumLength(10);
            RuleFor(c => c.MovieId).NotNull();
        }
    }
}
