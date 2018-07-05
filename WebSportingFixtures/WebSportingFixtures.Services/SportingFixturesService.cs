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

        public bool TryCreateTeam(Team team, out TeamError teamErrors)
        {
            teamErrors = new TeamError();

            if (string.IsNullOrEmpty(team.Name))
            {
                teamErrors = TeamError.InvalidName;
                return false;
            }

            if (string.IsNullOrEmpty(team.KnownName))
            {
                teamErrors = TeamError.InvalidKnownName;
                return false;
            }

            var foundExistingTeamByName = _store.GetAllTeams().ToList().Find(t => t.Name.ToLower() == team.Name.ToLower());
            var foundExistingTeamByKnownName = _store.GetAllTeams().ToList().Find(t => t.KnownName.ToLower() == team.KnownName.ToLower());

            if (foundExistingTeamByName != null)
            {
                teamErrors = TeamError.NameAlreadyExists;
                return false;
            }

            if (foundExistingTeamByKnownName != null)
            {
                teamErrors = TeamError.KnownNameAlreadyExists;
                return false;
            }

            Team createdTeam = _store.CreateTeam(team);

            if (createdTeam != null)
            {
                teamErrors = TeamError.None;
                return true;
            }
            else
            {
                teamErrors = TeamError.Undefined;
                return false;
            }
            
        }

        public bool TryEditTeam(Team team, out TeamError teamErrors)
        {
            teamErrors = new TeamError();

            if (team == null)
            {
                teamErrors = TeamError.Undefined;
                return false;
            }

            if (string.IsNullOrEmpty(team.Name))
            {
                teamErrors = TeamError.InvalidName;
                return false;
            }

            if (string.IsNullOrEmpty(team.KnownName))
            {
                teamErrors = TeamError.InvalidKnownName;
                return false;
            }

            var providedTeam = _store.GetTeam(team.Id);

            if (providedTeam == null)
            {
                teamErrors = TeamError.IdDoesNotExists;
                return false;
            }

            var foundExistingTeamByName = _store.GetAllTeams().ToList().Find(t => t.Name.ToLower() == team.Name.ToLower());
            var foundExistingTeamByKnownName = _store.GetAllTeams().ToList().Find(t => t.KnownName.ToLower() == team.KnownName.ToLower());

            if (foundExistingTeamByName != null && foundExistingTeamByName.Name != providedTeam.Name)
            {
                teamErrors = TeamError.NameAlreadyExists;
                return false;
            }

            if (foundExistingTeamByKnownName != null && foundExistingTeamByKnownName.KnownName != providedTeam.KnownName)
            {
                teamErrors = TeamError.KnownNameAlreadyExists;
                return false;
            }

            bool isTeamEdited = _store.EditTeam(team);

            if (isTeamEdited)
            {
                teamErrors = TeamError.None;
                return true;
            }
            else
            {
                teamErrors = TeamError.Undefined;
                return false;
            }
        }

        public bool TryDeleteTeam(int id, out TeamError teamErrors)
        {
            teamErrors = new TeamError();
            var teamToDelete = _store.GetTeam(id);

            if (teamToDelete == null)
            {
                teamErrors = TeamError.IdDoesNotExists;
                return false;
            }

            bool isTeamDeleted = _store.DeleteTeam(id);
            if (isTeamDeleted)
            {
                teamErrors = TeamError.None;
                return true;
            }
            else
            {
                teamErrors = TeamError.Undefined;
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

        public bool TryCreateEvent(Event anEvent, out EventError eventErrors)
        {
            eventErrors = new EventError();
            if (anEvent == null)
            {
                eventErrors = EventError.Undefined;
                return false;
            }

            var homeTeamName = anEvent.Home.Name;
            var awayTeamName = anEvent.Away.Name;
            var homeTeam = _store.GetAllTeams().ToList().Find(t => t.Name == homeTeamName);
            var awayTeam = _store.GetAllTeams().ToList().Find(t => t.Name == awayTeamName);

            if (string.IsNullOrEmpty(homeTeamName) )
            {
                eventErrors = EventError.InvalidHomeTeamName;
                return false;
            }
            if (string.IsNullOrEmpty(awayTeamName))
            {
                eventErrors = EventError.InvalidAwayTeamName;
                return false;
            }

            var foundExistingEvent = _store.GetAllEvents().ToList().Find(ev => ev.Home.Name == homeTeamName && ev.Away.Name == awayTeamName);

            if (foundExistingEvent != null)
            {
                eventErrors = EventError.EventAlreadyExists;
                return false;
            }

            if (homeTeam == null)
            {
                eventErrors = EventError.HomeTeamDoesNotExists;
                return false;
            }

            if (awayTeam == null)
            {
                eventErrors = EventError.AwayTeamDoesNotExists;
                return false;
            }

            if (homeTeamName == awayTeamName)
            {
                eventErrors = EventError.EventWithSameTeams;
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
                eventErrors = EventError.None;
                return true;
            }
            else
            {
                eventErrors = EventError.Undefined;
                return false;
            }

        }

        public bool TryEditEvent(Event anEvent, out EventError eventErrors)
        {
            eventErrors = new EventError();

            if (anEvent == null)
            {
                eventErrors = EventError.Undefined;
                return false;
            }

            var homeTeamName = anEvent.Home.Name;
            var awayTeamName = anEvent.Away.Name;
            var homeTeam = _store.GetAllTeams().ToList().Find(t => t.Name == homeTeamName);
            var awayTeam = _store.GetAllTeams().ToList().Find(t => t.Name == awayTeamName);

            if (string.IsNullOrEmpty(homeTeamName))
            {
                eventErrors = EventError.InvalidHomeTeamName;
                return false;
            }

            if (string.IsNullOrEmpty(awayTeamName))
            {
                eventErrors = EventError.InvalidAwayTeamName;
                return false;
            }

            var foundExistingEvent = _store.GetAllEvents().ToList().Find(ev => ev.Home.Name == homeTeamName && ev.Away.Name == awayTeamName);

            if (foundExistingEvent != null && foundExistingEvent.Id != anEvent.Id)
            {
                eventErrors = EventError.EventAlreadyExists;
                return false;
            }

            if (homeTeam == null)
            {
                eventErrors = EventError.HomeTeamDoesNotExists;
                return false;
            }

            if (awayTeam == null)
            {
                eventErrors = EventError.AwayTeamDoesNotExists;
                return false;
            }

            if (homeTeamName == awayTeamName)
            {
                eventErrors = EventError.EventWithSameTeams;
                return false;
            }

            var newEvent = new Event { Id = anEvent.Id, Home = homeTeam, Away = awayTeam, Status = anEvent.Status };

            bool isEventCreated = _store.EditEvent(newEvent);

            if (isEventCreated)
            {
                eventErrors = EventError.None;
                return true;
            }
            else
            {
                eventErrors = EventError.Undefined;
                return false;
            }

        }

        public bool TryDeleteEvent(int id, out EventError eventErrors)
        {
            eventErrors = new EventError();
            var anEvent = _store.GetEvent(id);

            if (anEvent == null)
            {
                eventErrors = EventError.IdDoesNotExists;
                return false;
            } 

            bool isEventDeleted = _store.DeleteEvent(id);

            if (isEventDeleted)
            {
                eventErrors = EventError.None;
                return true;
            }
            else
            {
                eventErrors = EventError.Undefined;
                return false;
            }
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
