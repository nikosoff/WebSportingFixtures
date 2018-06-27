using System.Collections.Generic;
using WebSportingFixtures.Core.Models;
using WebSportingFixtures.Core.Interfaces;
using System.Linq;

namespace WebSportingFixtures.Services
{
    public class SportingFixturesService
    {
        private IStore _store;
        private ITextSimilarityAlgorithm _textSimilarityAlgorithm;

        public SportingFixturesService(IStore store, ITextSimilarityAlgorithm textSimilarityAlgorithm)
        {
            _store = store;
            _textSimilarityAlgorithm = textSimilarityAlgorithm;
        }

        public bool TryCreateTeam(Team team, out TeamErrors teamErrors)
        {
            teamErrors = new TeamErrors();

            if (string.IsNullOrEmpty(team.Name))
            {
                teamErrors = TeamErrors.InvalidName;
                return false;
            }

            if (string.IsNullOrEmpty(team.KnownName))
            {
                teamErrors = TeamErrors.InvalidKnownName;
                return false;
            }

            var foundExistingTeamByName = _store.GetAllTeams().ToList().Find(t => t.Name.ToLower() == team.Name.ToLower());
            var foundExistingTeamByKnownName = _store.GetAllTeams().ToList().Find(t => t.KnownName.ToLower() == team.KnownName.ToLower());

            if (foundExistingTeamByName != null)
            {
                teamErrors = TeamErrors.NameAlreadyExists;
                return false;
            }

            if (foundExistingTeamByKnownName != null)
            {
                teamErrors = TeamErrors.KnownNameAlreadyExists;
                return false;
            }

            Team createdTeam = _store.CreateTeam(team);

            if (createdTeam != null)
            {
                teamErrors = TeamErrors.None;
                return true;
            }
            else
            {
                teamErrors = TeamErrors.Undefined;
                return false;
            }
            
        }

        public bool TryEditTeam(Team team, out TeamErrors teamErrors)
        {
            teamErrors = new TeamErrors();

            if (team == null)
            {
                teamErrors = TeamErrors.Undefined;
                return false;
            }

            if (string.IsNullOrEmpty(team.Name))
            {
                teamErrors = TeamErrors.InvalidName;
                return false;
            }

            if (string.IsNullOrEmpty(team.KnownName))
            {
                teamErrors = TeamErrors.InvalidKnownName;
                return false;
            }

            var providedTeam = _store.GetTeam(team.Id);

            if (providedTeam == null)
            {
                teamErrors = TeamErrors.IdDoesNotExists;
                return false;
            }

            var foundExistingTeamByName = _store.GetAllTeams().ToList().Find(t => t.Name.ToLower() == team.Name.ToLower());
            var foundExistingTeamByKnownName = _store.GetAllTeams().ToList().Find(t => t.KnownName.ToLower() == team.KnownName.ToLower());

            if (foundExistingTeamByName != null && foundExistingTeamByName.Name != providedTeam.Name)
            {
                teamErrors = TeamErrors.NameAlreadyExists;
                return false;
            }

            if (foundExistingTeamByKnownName != null && foundExistingTeamByKnownName.KnownName != providedTeam.KnownName)
            {
                teamErrors = TeamErrors.KnownNameAlreadyExists;
                return false;
            }

            bool isTeamEdited = _store.EditTeam(team);

            if (isTeamEdited)
            {
                teamErrors = TeamErrors.None;
                return true;
            }
            else
            {
                teamErrors = TeamErrors.Undefined;
                return false;
            }
        }

        public bool TryDeleteTeam(int id, out TeamErrors teamErrors)
        {
            teamErrors = new TeamErrors();
            var teamToDelete = _store.GetTeam(id);

            if (teamToDelete == null)
            {
                teamErrors = TeamErrors.IdDoesNotExists;
                return false;
            }

            bool isTeamDeleted = _store.DeleteTeam(id);
            if (isTeamDeleted)
            {
                teamErrors = TeamErrors.None;
                return true;
            }
            else
            {
                teamErrors = TeamErrors.Undefined;
                return false;
            }

        }

        public Team GetTeam(int id)
        {
            return _store.GetTeam(id);
        }

        public IEnumerable<Team> GetAllTeams()
        {
            return _store.GetAllTeams();
        }

        public bool TryCreateEvent(Event anEvent, out EventErrors eventErrors)
        {
            eventErrors = new EventErrors();
            if (anEvent == null)
            {
                eventErrors = EventErrors.Undefined;
                return false;
            }

            var homeTeamName = anEvent.Home.Name;
            var awayTeamName = anEvent.Away.Name;
            var homeTeam = _store.GetAllTeams().ToList().Find(t => t.Name == homeTeamName);
            var awayTeam = _store.GetAllTeams().ToList().Find(t => t.Name == awayTeamName);

            if (string.IsNullOrEmpty(homeTeamName) )
            {
                eventErrors = EventErrors.InvalidHomeTeamName;
                return false;
            }
            if (string.IsNullOrEmpty(awayTeamName))
            {
                eventErrors = EventErrors.InvalidAwayTeamName;
                return false;
            }

            var foundExistingEvent = _store.GetAllEvents().ToList().Find(ev => ev.Home.Name == homeTeamName && ev.Away.Name == awayTeamName);

            if (foundExistingEvent != null)
            {
                eventErrors = EventErrors.EventAlreadyExists;
                return false;
            }

            if (homeTeam == null)
            {
                eventErrors = EventErrors.HomeTeamDoesNotExists;
                return false;
            }

            if (awayTeam == null)
            {
                eventErrors = EventErrors.AwayTeamDoesNotExists;
                return false;
            }

            if (homeTeamName == awayTeamName)
            {
                eventErrors = EventErrors.EventWithSameTeams;
                return false;
            }

            Event newEvent = new Event
            {
                Home = new Team { Id = homeTeam.Id, Name = homeTeam.Name, KnownName = homeTeam.KnownName },
                Away = new Team { Id = awayTeam.Id, Name = awayTeam.Name, KnownName = awayTeam.KnownName },
                Status = anEvent.Status
            };


            Event eventToCreate = _store.CreateEvent(newEvent);

            if (eventToCreate != null)
            {
                eventErrors = EventErrors.None;
                return true;
            }
            else
            {
                eventErrors = EventErrors.Undefined;
                return false;
            }

        }

        public bool TryEditEvent(Event anEvent, out EventErrors eventErrors)
        {
            eventErrors = new EventErrors();

            if (anEvent == null)
            {
                eventErrors = EventErrors.Undefined;
                return false;
            }

            var homeTeamName = anEvent.Home.Name;
            var awayTeamName = anEvent.Away.Name;
            var homeTeam = _store.GetAllTeams().ToList().Find(t => t.Name == homeTeamName);
            var awayTeam = _store.GetAllTeams().ToList().Find(t => t.Name == awayTeamName);

            if (string.IsNullOrEmpty(homeTeamName))
            {
                eventErrors = EventErrors.InvalidHomeTeamName;
                return false;
            }

            if (string.IsNullOrEmpty(awayTeamName))
            {
                eventErrors = EventErrors.InvalidAwayTeamName;
                return false;
            }

            var foundExistingEvent = _store.GetAllEvents().ToList().Find(ev => ev.Home.Name == homeTeamName && ev.Away.Name == awayTeamName);

            if (foundExistingEvent != null && foundExistingEvent.Id != anEvent.Id)
            {
                eventErrors = EventErrors.EventAlreadyExists;
                return false;
            }

            if (homeTeam == null)
            {
                eventErrors = EventErrors.HomeTeamDoesNotExists;
                return false;
            }

            if (awayTeam == null)
            {
                eventErrors = EventErrors.AwayTeamDoesNotExists;
                return false;
            }

            if (homeTeamName == awayTeamName)
            {
                eventErrors = EventErrors.EventWithSameTeams;
                return false;
            }

            var newEvent = new Event { Id = anEvent.Id, Home = homeTeam, Away = awayTeam, Status = anEvent.Status };

            bool isEventCreated = _store.EditEvent(newEvent);

            if (isEventCreated)
            {
                eventErrors = EventErrors.None;
                return true;
            }
            else
            {
                eventErrors = EventErrors.Undefined;
                return false;
            }

        }

        public bool DeleteEvent(int id)
        {
            return _store.DeleteEvent(id);
        }

        public Event GetEvent(int id)
        {
            return _store.GetEvent(id);
        }

        public IEnumerable<Event> GetAllEvents()
        {
            return _store.GetAllEvents();
        }

        public bool TryFindBestMatch(string input, IEnumerable<string> availableItems, out IEnumerable<string> matches, int numberOfMatches)
        {
           return  _textSimilarityAlgorithm.TryFindBestMatches(input, availableItems, numberOfMatches, out matches);
        }
    }
}
