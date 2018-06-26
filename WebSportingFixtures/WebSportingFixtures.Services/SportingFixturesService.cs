using System.Collections.Generic;
using WebSportingFixtures.Core.Models;
using WebSportingFixtures.Core.Interfaces;

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

        public bool CreateTeam(Team team)
        {
            return _store.CreateTeam(team);
        }

        public bool EditTeam(Team team)
        {
           return _store.EditTeam(team);
        }

        public bool DeleteTeam(int id)
        {
            return _store.DeleteTeam(id);
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
