using System.Collections.Generic;
using WebSportingFixtures.Core.Models;

namespace WebSportingFixtures.Core.Interfaces
{
    public interface IStore
    {
        Team CreateTeam(Team team);
        bool EditTeam(Team team);
        bool DeleteTeam(int id);
        Team GetTeam(int id);
        IEnumerable<Team> GetAllTeams();
        bool CreateEvent(Event anEvent);
        bool EditEvent(Event anEvent);
        bool DeleteEvent(int id);
        Event GetEvent(int id);
        IEnumerable<Event> GetAllEvents();
    }
}
