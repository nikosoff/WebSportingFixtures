using System.ComponentModel.DataAnnotations;
using WebSportingFixtures.Core.Models;

namespace WebSportingFixtures.ViewModels
{
    public class EventViewModel
    {
        public int Id { get; set; }
        [Required]
        public string Home { get; set; }
        [Required]
        public string Away { get; set; }
        [Required]
        public Status Status { get; set; }
    }
}
