using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using WebSportingFixtures.Core.Interfaces;
using WebSportingFixtures.Core.Models;
using WebSportingFixtures.Services;
using WebSportingFixtures.ViewModels;

namespace WebSportingFixtures.Controllers
{
    public class EventsController : Controller
    {
        private SportingFixturesService _sportingFixturesService;
        private IRawEventProvider _rawEventProvider;

        public EventsController(SportingFixturesService sportingFixturesService, IRawEventProvider rawEventProvider)
        {
            _sportingFixturesService = sportingFixturesService;
            _rawEventProvider = rawEventProvider;
        }

        public IActionResult Index()
        {
            var events = _sportingFixturesService.GetAllEvents();
            return View(events);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind(include: "Home, Away, Status")]EventViewModel eventViewModel)
        {

            if (ModelState.IsValid)
            {
                var newEvent = new Event { Home = new Team { Name = eventViewModel.Home }, Away = new Team { Name = eventViewModel.Away }, Status = eventViewModel.Status };

                EventErrors eventErrors;
                bool isEventCreated = _sportingFixturesService.TryCreateEvent(newEvent, out eventErrors);

                if (!isEventCreated)
                {
                    switch (eventErrors)
                    {
                        case EventErrors.EventAlreadyExists:
                            ModelState.AddModelError("PostCreateEventError", $"The event {eventViewModel.Home} - {eventViewModel.Away} is already exists");
                            return View(eventViewModel);
                        case EventErrors.HomeTeamDoesNotExists:
                            ModelState.AddModelError("PostCreateEventError", $"Home team with name {eventViewModel.Home} does not exist in our database");
                            return View(eventViewModel);
                        case EventErrors.AwayTeamDoesNotExists:
                            ModelState.AddModelError("PostCreateEventError", $"Away team with name {eventViewModel.Away} does not exist in our database");
                            return View(eventViewModel);
                        case EventErrors.EventWithSameTeams:
                            ModelState.AddModelError("PostCreateEventError", $"The provided event {eventViewModel.Home} - {eventViewModel.Away} has two teams with the same name. This is not allowed.");
                            return View(eventViewModel);
                        case EventErrors.InvalidHomeTeamName:
                            return View(eventViewModel);
                        case EventErrors.InvalidAwayTeamName:
                            return View(eventViewModel);
                        case EventErrors.Undefined:
                            ModelState.AddModelError("PostCreateEventError", $"Event \"{eventViewModel.Home} - {eventViewModel.Away}\" could not be inserted due to database error");
                            return View(eventViewModel);
                    }
                }

                return RedirectToAction(nameof(Index));
            }

            return View();

        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            EventViewModel eventViewModel = null;
            var anEvent = _sportingFixturesService.GetEvent(id);

            if (anEvent != null)
            {
                eventViewModel = new EventViewModel() { Home = anEvent.Home.Name, Away = anEvent.Away.Name, Status = anEvent.Status };
            }
            return View(eventViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit([Bind(include: "Id, Home, Away, Status")] EventViewModel eventViewModel)
        {

            if (ModelState.IsValid)
            {
                var newEvent = new Event { Id = eventViewModel.Id, Home = new Team { Name = eventViewModel.Home }, Away = new Team { Name = eventViewModel.Away }, Status = eventViewModel.Status };
                EventErrors eventErrors;
                bool isEventEdited = _sportingFixturesService.TryEditEvent(newEvent, out eventErrors);

                if (!isEventEdited)
                {
                    switch (eventErrors)
                    {
                        case EventErrors.EventAlreadyExists:
                            ModelState.AddModelError("PostEditEventError", $"The event {eventViewModel.Home} - {eventViewModel.Away} is already exists");
                            return View(eventViewModel);
                        case EventErrors.HomeTeamDoesNotExists:
                            ModelState.AddModelError("PostEditEventError", $"Home team with name \"{eventViewModel.Home}\" does not exist in our database");
                            return View(eventViewModel);
                        case EventErrors.AwayTeamDoesNotExists:
                            ModelState.AddModelError("PostEditEventError", $"Away team with name \"{eventViewModel.Away}\" does not exist in our database");
                            return View(eventViewModel);
                        case EventErrors.EventWithSameTeams:
                            ModelState.AddModelError("PostEditEventError", $"The provided event {eventViewModel.Home} - {eventViewModel.Away} has two teams with the same name. This is not allowed.");
                            return View(eventViewModel);
                        case EventErrors.InvalidHomeTeamName:
                            return View(eventViewModel);
                        case EventErrors.InvalidAwayTeamName:
                            return View(eventViewModel);
                        case EventErrors.IdDoesNotExists:
                            ModelState.AddModelError("PostEditEventError", $"Event with id: {eventViewModel.Id} does not exists");
                            return View(eventViewModel);
                        case EventErrors.Undefined:
                            ModelState.AddModelError("PostEditEventError", $"Event \"{eventViewModel.Home} - {eventViewModel.Away}\" could not be inserted due to database error");
                            return View(eventViewModel);
                    }
                }

                return RedirectToAction(nameof(Index));

            }

            return View(eventViewModel);

        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            var anEvent = _sportingFixturesService.GetEvent(id);
            return View(anEvent);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmation(int id)
        {

            EventErrors eventErrors;
            bool isEventDeleted = _sportingFixturesService.TryDeleteEvent(id, out eventErrors);

            if (!isEventDeleted)
            {
                switch (eventErrors)
                {
                    case EventErrors.IdDoesNotExists:
                        ModelState.AddModelError("PostDeleteEventError", $"The event with id: {id} does not exists");
                        return View();
                    case EventErrors.Undefined:
                        ModelState.AddModelError("PostDeleteEventError", $"The event with id: {id} could not be deleted due to database error");
                        return View();
                }
            }

            return RedirectToAction(nameof(Index));

        }

        [HttpGet]
        public IActionResult Fetch()
        {
            var rawEvents = _rawEventProvider.GetRawEvents();
            var availableTeams = _sportingFixturesService.GetAllTeams().Select(t => t.Name);
            var events = _sportingFixturesService.GetAllEvents();
            var rawEventsToReturned = new List<RawEvent>();
            foreach (var rawEvent in rawEvents)
            {
                var homeMatches = Enumerable.Empty<string>();
                var awayMatches = Enumerable.Empty<string>();
                if (_sportingFixturesService.TryFindBestMatch(rawEvent.Home, availableTeams, out homeMatches, 1))
                {
                    rawEvent.Home = homeMatches.ElementAt(0);
                }

                if (_sportingFixturesService.TryFindBestMatch(rawEvent.Away, availableTeams, out awayMatches, 1))
                {
                    rawEvent.Away = awayMatches.ElementAt(0);
                }

                var foundExistingEvent = events.ToList().Find(ev => ev.Home.Name == rawEvent.Home && ev.Away.Name == rawEvent.Away);
                if (foundExistingEvent == null)
                {
                    rawEventsToReturned.Add(rawEvent);
                }

            }
            return View(rawEventsToReturned);
        }

        public class EventError
        {
            public string Status { get; set; }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public object Fetch([Bind(include: "Home, Away, Status")]RawEvent rawEvent)
        {

            if (ModelState.IsValid)
            {
                var newEvent = new Event
                {
                    Home = new Team { Name = rawEvent.Home },
                    Away = new Team { Name = rawEvent.Away },
                    Status = rawEvent.Status
                };
                var eventErrors = new EventErrors();
                if (!_sportingFixturesService.TryCreateEvent(newEvent, out eventErrors))
                {
                    switch (eventErrors)
                    {
                        case EventErrors.HomeTeamDoesNotExists:
                            return new EventError { Status = "HomeTeamDoesNotExists" };
                        case EventErrors.AwayTeamDoesNotExists:
                            return new EventError { Status = "AwayTeamDoesNotExists" };
                        case EventErrors.EventAlreadyExists:
                            return new EventError { Status = "EventAlreadyExists" };
                        case EventErrors.EventWithSameTeams:
                            return new EventError { Status = "EventWithSameTeams" };
                        case EventErrors.Undefined:
                            return new EventError { Status = "Undefined" };
                    }
                }
                return new EventError { Status = "Success" };
            }

            if (string.IsNullOrEmpty(rawEvent.Home))
            {
                return new EventError { Status = "InvalidHomeTeamName" };
            }

            if (string.IsNullOrEmpty(rawEvent.Away))
            {
                return new EventError { Status = "InvalidAwayTeamName" };
            }

            return new EventError { Status = "Undefined" };
        }

        //[ResponseCache(Duration = 60, Location = ResponseCacheLocation.Any)]
        public string SuggestedTeams(string givenTeamName)
        {
            var availableTeamNames = _sportingFixturesService.GetAllTeams().Select(t => t.Name);
            var matches = Enumerable.Empty<string>();
            _sportingFixturesService.TryFindBestMatch(givenTeamName, availableTeamNames, out matches, 5);
            return "[" + String.Join(",", matches.Select(teamName => "\"" + teamName + "\"")) + "]";
        }
    }
}
