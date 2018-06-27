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

        public bool CreateEvent(Event anEvent)
        {
            return _store.CreateEvent(anEvent);
        }

        public bool EditEvent(Event anEvent)
        {
            return _store.EditEvent(anEvent);
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
