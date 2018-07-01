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
                            break;
                        case EventErrors.HomeTeamDoesNotExists:
                            ModelState.AddModelError("PostCreateEventError", $"Home team with name {eventViewModel.Home} does not exist in our database");
                            return View(eventViewModel);
                            break;
                        case EventErrors.AwayTeamDoesNotExists:
                            ModelState.AddModelError("PostCreateEventError", $"Away team with name {eventViewModel.Away} does not exist in our database");
                            return View(eventViewModel);
                            break;
                        case EventErrors.EventWithSameTeams:
                            ModelState.AddModelError("PostCreateEventError", $"The provided event {eventViewModel.Home} - {eventViewModel.Away} has two teams with the same name. This is not allowed.");
                            return View(eventViewModel);
                            break;
                        case EventErrors.InvalidHomeTeamName:
                            return View(eventViewModel);
                            break;
                        case EventErrors.InvalidAwayTeamName:
                            return View(eventViewModel);
                            break;
                        case EventErrors.Undefined:
                            ModelState.AddModelError("PostCreateEventError", $"Event \"{eventViewModel.Home} - {eventViewModel.Away}\" could not be inserted due to database error");
                            return View(eventViewModel);
                            break;
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
                var newEvent = new Event { Id = eventViewModel.Id, Home = new Team { Name = eventViewModel.Home }, Away = new Team { Name = eventViewModel.Away}, Status = eventViewModel.Status };
                EventErrors eventErrors;
                bool isEventEdited = _sportingFixturesService.TryEditEvent(newEvent, out eventErrors);

                if (!isEventEdited)
                {
                    switch (eventErrors)
                    {
                        case EventErrors.EventAlreadyExists:
                            ModelState.AddModelError("PostEditEventError", $"The event {eventViewModel.Home} - {eventViewModel.Away} is already exists");
                            return View(eventViewModel);
                            break;
                        case EventErrors.HomeTeamDoesNotExists:
                            ModelState.AddModelError("PostEditEventError", $"Home team with name \"{eventViewModel.Home}\" does not exist in our database");
                            return View(eventViewModel);
                            break;
                        case EventErrors.AwayTeamDoesNotExists:
                            ModelState.AddModelError("PostEditEventError", $"Away team with name \"{eventViewModel.Away}\" does not exist in our database");
                            return View(eventViewModel);
                            break;
                        case EventErrors.EventWithSameTeams:
                            ModelState.AddModelError("PostEditEventError", $"The provided event {eventViewModel.Home} - {eventViewModel.Away} has two teams with the same name. This is not allowed.");
                            return View(eventViewModel);
                            break;
                        case EventErrors.InvalidHomeTeamName:
                            return View(eventViewModel);
                            break;
                        case EventErrors.InvalidAwayTeamName:
                            return View(eventViewModel);
                            break;
                        case EventErrors.IdDoesNotExists:
                            ModelState.AddModelError("PostEditEventError", $"Event with id: {eventViewModel.Id} does not exists");
                            return View(eventViewModel);
                            break;
                        case EventErrors.Undefined:
                            ModelState.AddModelError("PostEditEventError", $"Event \"{eventViewModel.Home} - {eventViewModel.Away}\" could not be inserted due to database error");
                            return View(eventViewModel);
                            break;
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
                        break;
                    case EventErrors.Undefined:
                        ModelState.AddModelError("PostDeleteEventError", $"The event with id: {id} could not be deleted due to database error");
                        return View();
                        break;
                }
            }

            return RedirectToAction(nameof(Index));

        }

        [HttpGet]
        public IActionResult Fetch()
        {
            var rawEvents = _rawEventProvider.GetRawEvents();
            //RawsEventsViewModel rawsEventsViewModel = new RawsEventsViewModel();
            //for (int i = 0; i < rawEvents.Count(); i++)
            //{
            //    var home = rawEvents.ElementAt(i).Home;
            //    var away = rawEvents.ElementAt(i).Away;
            //    var status = rawEvents.ElementAt(i).Status;
            //    rawsEventsViewModel.map.Add("event" + i, new Tuple<string, string, Status, bool>(home, away, status, false));
            //}
            //return View(rawsEventsViewModel);
            return View(rawEvents);
        }
        
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public string Fetch(int? id)
        {
            return "";
            //var rawEvents = _rawEventProvider.GetRawEvents();
            //RawsEventsViewModel rawsEventsViewModel = new RawsEventsViewModel();
            //for (int i = 0; i < rawEvents.Count(); i++)
            //{
            //    var home = rawEvents.ElementAt(i).Home;
            //    var away = rawEvents.ElementAt(i).Away;
            //    var status = rawEvents.ElementAt(i).Status;
            //    rawsEventsViewModel.map.Add("event" + i, new Tuple<string, string, Status, bool>(home, away, status, false));
            //}

            //var keys = Request.Form.Keys;
            //var key = keys.ElementAt(1).Split('.')[0];

            //var homeTeam = Request.Form[keys.ElementAt(0)];
            //var awayTeam = Request.Form[keys.ElementAt(1)];
            //var eventStatus = (Status) Int32.Parse(Request.Form[keys.ElementAt(2)]);
            //rawsEventsViewModel.map[key] = new Tuple<string, string, Status, bool>(homeTeam, awayTeam, eventStatus, true);

            //return View(rawsEventsViewModel);
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public IActionResult Fetch(int id)
        //{
        //    var rawEvents = _rawEventProvider.GetRawEvents();

        //    var keys = Request.Form.Keys;

        //    var anEvent = new Event()
        //    {
        //        Home = new Team(),
        //        Away = new Team(),
        //        Status = Status.Undefined
        //    };

        //    for (int i = 0; i < keys.Count; i++)
        //    {
        //        var name = keys.ElementAt(i);
        //        if (name.StartsWith("home"))
        //        {
        //            var homeTeamName = Request.Form[name];
        //            var homeTeam = _sportingFixturesService.GetAllTeams().ToList().Find(t => t.Name == homeTeamName);
        //            rawEvents.ToList()[(int)i / 3].Home = homeTeamName;

        //            if (homeTeam == null)
        //            {
        //                ModelState.AddModelError("PostFetchError", $"Home team with name {Request.Form[name]} does not exist in our database");
        //                return View(rawEvents);
        //            }
        //            else
        //            {
        //                anEvent.Home = homeTeam;
        //            }
        //            continue;
        //        }
        //        else if (name.StartsWith("away"))
        //        {
        //            var awayTeamName = Request.Form[name];
        //            var awayTeam = _sportingFixturesService.GetAllTeams().ToList().Find(t => t.Name == awayTeamName);
        //            rawEvents.ToList()[(int)i / 3].Away = awayTeamName;

        //            if (awayTeam == null)
        //            {
        //                ModelState.AddModelError("PostFetchError", $"Away team with name {Request.Form[name]} does not exist in our database");
        //                return View(rawEvents);
        //            }
        //            else
        //            {
        //                anEvent.Away = awayTeam;
        //            }
        //            continue;
        //        }
        //        else if (name.StartsWith("eventStatus"))
        //        {
        //            anEvent.Status = (Status)Int32.Parse(Request.Form[name].ToString());
        //        }
        //        else
        //        {
        //            continue;
        //        }

        //        if (anEvent.Home.Name == anEvent.Away.Name)
        //        {
        //            ModelState.AddModelError("PostFetchError", $"The provided event {anEvent.Home.Name} - {anEvent.Away.Name} has two teams with the same name. This is not allowed.");
        //            return View(rawEvents);
        //        }

        //        EventErrors evetErrors;
        //        _sportingFixturesService.TryCreateEvent(anEvent, out evetErrors);

        //    }

        //    return RedirectToAction(nameof(Index));
        //}

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
