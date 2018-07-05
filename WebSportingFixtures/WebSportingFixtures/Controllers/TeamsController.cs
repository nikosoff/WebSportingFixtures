using Microsoft.AspNetCore.Mvc;
using WebSportingFixtures.Core.Models;
using WebSportingFixtures.Services;
using System.Linq;

namespace WebSportingFixtures.Controllers
{
    public class TeamsController : Controller
    {
        private SportingFixturesService _sportingFixturesService;

        public TeamsController(SportingFixturesService sportingFixturesService)
        {
            _sportingFixturesService = sportingFixturesService;
        }

        public IActionResult Index()
        {
            var model = _sportingFixturesService.GetAllTeams();
            return View(model);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind(include: "Name, KnownName")] Team team)
        {

            if (ModelState.IsValid)
            {
                TeamError teamErrors;
                bool isTeamCreated = _sportingFixturesService.TryCreateTeam(team, out teamErrors);

                if (!isTeamCreated)
                {
                    switch (teamErrors)
                    {
                        case TeamError.NameAlreadyExists:
                            ModelState.AddModelError("PostCreateTeamNameError",
                                $"The team \"{team.Name}\" is already exists");
                            return View();
                        case TeamError.KnownNameAlreadyExists:
                            ModelState.AddModelError("PostCreateTeamKnownNameError",
                                $"The team known as \"{team.KnownName}\" is already exists");
                            return View();
                        case TeamError.InvalidName:
                            return View();
                        case TeamError.InvalidKnownName:
                            return View();
                        case TeamError.Undefined:
                            ModelState.AddModelError("PostCreateTeamError",
                                $"Team \"{team.Name}\" could not be inserted due to database error");
                            return View();
                    }
                }
                else
                {
                    return RedirectToAction(nameof(Index));
                }

            }

            return View();

        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var team = _sportingFixturesService.GetTeam(id);
            return View(team);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit([Bind(include:"Id, Name, KnownName")] Team team)
        {
            if (ModelState.IsValid)
            {

                TeamError teamErrors;
                bool isTeamEdited = _sportingFixturesService.TryEditTeam(team, out teamErrors);

                if (!isTeamEdited)
                {
                    switch (teamErrors)
                    {
                        case TeamError.NameAlreadyExists:
                            ModelState.AddModelError("PostEditTeamNameError", $"The team \"{team.Name}\" is already exists");
                            return View(team);
                        case TeamError.KnownNameAlreadyExists:
                            ModelState.AddModelError("PostEditTeamKnownNameError", $"The team known as \"{team.KnownName}\" is already exists");
                            return View(team);
                        case TeamError.InvalidName:
                            return View(team);
                        case TeamError.InvalidKnownName:
                            return View(team);
                        case TeamError.IdDoesNotExists:
                            ModelState.AddModelError("PostEditTeamError", $"Team with id: {team.Id} does not exists");
                            return View(team);
                        case TeamError.Undefined:
                            ModelState.AddModelError("PostEditTeamError", $"Team \"{team.Name}\" could not be edited due to database error");
                            return View(team);
                    }
                }

                return RedirectToAction(nameof(Index));
            }

            return View(team);
           
        }

        public IActionResult Delete(int id)
        {
            var team = _sportingFixturesService.GetTeam(id);
            return View(team);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            TeamError teamErrors;

            bool isTeamDeleted = _sportingFixturesService.TryDeleteTeam(id, out teamErrors);

            if (!isTeamDeleted)
            {
                switch (teamErrors)
                {
                    case TeamError.IdDoesNotExists:
                        ModelState.AddModelError("PostDeleteTeamError", $"Team with id: \"{id}\" does not exists");
                        return View();
                    case TeamError.Undefined:
                        ModelState.AddModelError("PostDeleteTeamError", $"Team with id: \"{id}\" could not be deleted due to database error");
                        return View();

                }
            }

            return RedirectToAction(nameof(Index));

        }

    }
}
