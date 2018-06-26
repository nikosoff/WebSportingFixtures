using System.ComponentModel.DataAnnotations;

namespace WebSportingFixtures.Core.Models
{
    public class Team
    {
        public int Id { get; set; }
        [Required, MinLength(3)]
        public string Name { get; set; }
        [Required, MinLength(3)]
        public string KnownName { get; set; }
    }
}
