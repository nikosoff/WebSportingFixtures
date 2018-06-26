using System.ComponentModel.DataAnnotations;

namespace WebSportingFixtures.Core.Models
{
    public class Team
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "The name of the team is required")]
        [MinLength(3, ErrorMessage = "The name of the team should be a name with a minimum length of 3")]
        [Display(Name = "Name of the team")]
        public string Name { get; set; }
        [Required(ErrorMessage = "The known name of the team is required")]
        [MinLength(3, ErrorMessage = "The known name of the team should be a name with a minimum length of 3")]
        [Display(Name = "Known name of the team")]
        public string KnownName { get; set; }
    }
}
