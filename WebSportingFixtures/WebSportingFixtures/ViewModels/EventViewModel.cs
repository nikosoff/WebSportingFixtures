using System.ComponentModel.DataAnnotations;
using Microsoft.Azure.KeyVault.Models;
using WebSportingFixtures.Core.Models;

namespace WebSportingFixtures.ViewModels
{
    public class EventViewModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "The name of home team is required")]
        public string Home { get; set; }
        [Required(ErrorMessage = "Tha name of away team is required")]
        public string Away { get; set; }
        [Required]
        public Status Status { get; set; }
    }
}
