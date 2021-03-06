﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
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
        public IActionResult Create([Bind(include: " Home, Away, Status")]EventViewModel eventViewModel)
        {
            var homeTeamName = eventViewModel.Home;
            var awayTeamName = eventViewModel.Away;
            var homeTeam = _sportingFixturesService.GetAllTeams().ToList().Find(t => t.Name == homeTeamName);
            var awayTeam = _sportingFixturesService.GetAllTeams().ToList().Find(t => t.Name == awayTeamName);

            if (String.IsNullOrEmpty(homeTeamName) || String.IsNullOrEmpty(awayTeamName))
            {
                return View(eventViewModel);
            }

            var foundExistingEvent = _sportingFixturesService.GetAllEvents().ToList().Find(ev => ev.Home.Name == homeTeamName && ev.Away.Name == awayTeamName);

            if (foundExistingEvent != null)
            {
                ModelState.AddModelError("PostCreateEventError", $"The event {homeTeamName} - {awayTeamName} is already exists");
                return View(eventViewModel);
            }

            if (homeTeam == null)
            {
                ModelState.AddModelError("PostCreateEventError", $"Home team with name {homeTeamName} does not exist in our database");
                return View(eventViewModel);
            }

            if (awayTeam == null)
            {
                ModelState.AddModelError("PostCreateEventError", $"Away team with name {awayTeamName} does not exist in our database");
                return View(eventViewModel);
            }

            if (homeTeamName == awayTeamName)
            {
                ModelState.AddModelError("PostCreateEventError", $"The provided event {homeTeamName} - {awayTeamName} has two teams with the same name. This is not allowed.");
                return View(eventViewModel);
            }

            Event newEvent = new Event()
            {
                Home = new Team { Id = homeTeam.Id, Name = homeTeam.Name, KnownName=homeTeam.KnownName},
                Away = new Team { Id = awayTeam.Id, Name = awayTeam.Name, KnownName = awayTeam.KnownName },
                Status = eventViewModel.Status
            };


            bool isEventCreated = _sportingFixturesService.CreateEvent(newEvent);

            if (isEventCreated)
            {
                return RedirectToAction(nameof(Index));
            }
            else
            {
                ModelState.AddModelError("PostCreateEventError", $"Event \"{newEvent.Home.Name} - {newEvent.Away.Name}\" could not be inserted due to database error");
                return View();
            }
            
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit([Bind(include: "Id, Home, Away, Status")] EventViewModel eventViewModel)
        {
            var homeTeamName = eventViewModel.Home;
            var awayTeamName = eventViewModel.Away;
            var homeTeam = _sportingFixturesService.GetAllTeams().ToList().Find(t => t.Name == eventViewModel.Home);
            var awayTeam = _sportingFixturesService.GetAllTeams().ToList().Find(t => t.Name == eventViewModel.Away);

            if (String.IsNullOrEmpty(homeTeamName) || String.IsNullOrEmpty(awayTeamName))
            {
                return View(eventViewModel);
            }

            var foundExistingEvent = _sportingFixturesService.GetAllEvents().ToList().Find(ev => ev.Home.Name == homeTeamName && ev.Away.Name == awayTeamName);

            if (foundExistingEvent != null && foundExistingEvent.Id != eventViewModel.Id)
            {
                ModelState.AddModelError("PostEditEventError", $"The event {homeTeamName} - {awayTeamName} is already exists");
                return View(eventViewModel);
            }


            if (homeTeam == null)
            {
                ModelState.AddModelError("PostEditEventError", $"Home team with name \"{homeTeamName}\" does not exist in our database");
                return View(eventViewModel);
            }

            if (awayTeam == null)
            {
                ModelState.AddModelError("PostEditEventError", $"Away team with name \"{awayTeamName}\" does not exist in our database");
                return View(eventViewModel);
            }

            if (homeTeamName == awayTeamName)
            {
                ModelState.AddModelError("PostEditEventError", $"The provided event {homeTeamName} - {awayTeamName} has two teams with the same name. This is not allowed.");
                return View(eventViewModel);
            }

            var newEvent = new Event { Id = eventViewModel.Id, Home = homeTeam, Away = awayTeam, Status = eventViewModel.Status };

            bool isEventCreated = _sportingFixturesService.EditEvent(newEvent);

            if (isEventCreated)
            {
                return RedirectToAction(nameof(Index));
            }
            else
            {
                ModelState.AddModelError("PostEditEventError", $"Event \"{homeTeamName} - {awayTeamName}\" could not be inserted due to database error");
                return View();
            }
            
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
            bool isEventDeleted = _sportingFixturesService.DeleteEvent(id);

            if (isEventDeleted)
            {
                return RedirectToAction(nameof(Index));
            }
            else
            {
                var anEvent = _sportingFixturesService.GetEvent(id);
                ModelState.AddModelError("PostDeleteEventError", $"The event with id: {id} could not be deleted due to database error");
                return View(anEvent);
            }
        }

        [HttpGet]
        public IActionResult Fetch()
        {
            var rawEvents = _rawEventProvider.GetRawEvents();
            return View(rawEvents);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Fetch(int id)
        {
            var rawEvents = _rawEventProvider.GetRawEvents();

            var keys = Request.Form.Keys;

            var anEvent = new Event()
            {
                Home = new Team(),
                Away = new Team(),
                Status = Status.Undefined
            };

            for (int i = 0; i < keys.Count; i++)
            {
                var name = keys.ElementAt(i);
                if (name.StartsWith("home"))
                {
                    var homeTeamName = Request.Form[name];
                    var homeTeam = _sportingFixturesService.GetAllTeams().ToList().Find(t => t.Name == homeTeamName);
                    rawEvents.ToList()[(int)i / 3].Home = homeTeamName;

                    if (homeTeam == null)
                    {
                        ModelState.AddModelError("PostFetchError", $"Home team with name {Request.Form[name]} does not exist in our database");
                        return View(rawEvents);
                    }
                    else
                    {
                        anEvent.Home = homeTeam;
                    }
                    continue;
                }
                else if (name.StartsWith("away"))
                {
                    var awayTeamName = Request.Form[name];
                    var awayTeam = _sportingFixturesService.GetAllTeams().ToList().Find(t => t.Name == awayTeamName);
                    rawEvents.ToList()[(int)i / 3].Away = awayTeamName;

                    if (awayTeam == null)
                    {
                        ModelState.AddModelError("PostFetchError", $"Away team with name {Request.Form[name]} does not exist in our database");
                        return View(rawEvents);
                    }
                    else
                    {
                        anEvent.Away = awayTeam;
                    }
                    continue;
                }
                else if (name.StartsWith("eventStatus"))
                {
                    anEvent.Status = (Status)Int32.Parse(Request.Form[name].ToString());
                }
                else
                {
                    continue;
                }

                if (anEvent.Home.Name == anEvent.Away.Name)
                {
                    ModelState.AddModelError("PostFetchError", $"The provided event {anEvent.Home.Name} - {anEvent.Away.Name} has two teams with the same name. This is not allowed.");
                    return View(rawEvents);
                }

                _sportingFixturesService.CreateEvent(anEvent);

            }

            return RedirectToAction(nameof(Index));
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
