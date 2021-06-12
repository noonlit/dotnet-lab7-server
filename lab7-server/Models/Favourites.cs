﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lab7.Models
{
	public class Favourites
	{
		public int Id { get; set; }
		public ApplicationUser User { get; set; }
		public string UserId { get; set; }
		public List<Movie> Movies { get; set; }
		public int Year { get; set; }
	}
}
