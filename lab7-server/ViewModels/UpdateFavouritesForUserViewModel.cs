using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lab7.ViewModels
{
	public class UpdateFavouritesForUserViewModel
	{
		public int Id { get; set; }
		public List<int> MovieIds { get; set; }
	}
}
