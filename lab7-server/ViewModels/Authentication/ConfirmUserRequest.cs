using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lab7.ViewModels.Authentication
{
	public class ConfirmUserRequest
	{
		public string Email { get; set; }
		public string ConfirmationToken { get; set; }
	}
}
