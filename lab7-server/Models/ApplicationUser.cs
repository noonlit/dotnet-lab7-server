using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Lab7.Models
{
    public class ApplicationUser : IdentityUser
    {
        [JsonIgnore]
        public List<Favourites> Favourites { get; set; }
    }
}
