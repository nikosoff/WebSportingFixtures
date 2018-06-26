using System.ComponentModel.DataAnnotations;

namespace WebSportingFixtures.Core.Models
{
    public class Event
    {
        public int Id { get; set; }
        [Required]
        public Team Home { get; set; }
        [Required]
        public Team Away { get; set; }
        [Required]
        public Status Status { get; set; }
    }
}
