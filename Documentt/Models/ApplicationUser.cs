
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema; 

namespace Documentt.Models
{

    public class ApplicationUser : IdentityUser
    {

        public decimal Balance { get; set; } = 0.00m; 
    }
}