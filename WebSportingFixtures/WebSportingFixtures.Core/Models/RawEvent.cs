using System.ComponentModel.DataAnnotations;

namespace WebSportingFixtures.Core.Models
{
    public class RawEvent
    {
        [Required]
        public string Home { get; set; }
        [Required]
        public string Away { get; set; }
        public Status Status { get; set; }
    }
}
