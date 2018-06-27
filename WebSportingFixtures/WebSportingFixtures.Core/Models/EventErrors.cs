using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebSportingFixtures.Core.Models
{
    public enum EventErrors
    {
        None = 0,
        Undefined,
        EventAlreadyExists,
        HomeTeamDoesNotExists,
        AwayTeamDoesNotExists,
        EventWithSameTeams,
        InvalidHomeTeamName,
        InvalidAwayTeamName
    }
}
