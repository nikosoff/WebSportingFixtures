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
                TeamErrors teamErrors;
                bool isTeamCreated = _sportingFixturesService.TryCreateTeam(team, out teamErrors);

                if (!isTeamCreated)
                {
                    switch (teamErrors)
                    {
                        case TeamErrors.NameAlreadyExists:
                            ModelState.AddModelError("PostCreateTeamNameError",
                                $"The team \"{team.Name}\" is already exists");
                            return View();
                        case TeamErrors.KnownNameAlreadyExists:
                            ModelState.AddModelError("PostCreateTeamKnownNameError",
                                $"The team known as \"{team.KnownName}\" is already exists");
                            return View();
                        case TeamErrors.InvalidName:
                            return View();
                        case TeamErrors.InvalidKnownName:
                            return View();
                        case TeamErrors.Undefined:
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

                TeamErrors teamErrors;
                bool isTeamEdited = _sportingFixturesService.TryEditTeam(team, out teamErrors);

                if (!isTeamEdited)
                {
                    switch (teamErrors)
                    {
                        case TeamErrors.NameAlreadyExists:
                            ModelState.AddModelError("PostEditTeamNameError", $"The team \"{team.Name}\" is already exists");
                            return View(team);
                        case TeamErrors.KnownNameAlreadyExists:
                            ModelState.AddModelError("PostEditTeamKnownNameError", $"The team known as \"{team.KnownName}\" is already exists");
                            return View(team);
                        case TeamErrors.InvalidName:
                            return View(team);
                        case TeamErrors.InvalidKnownName:
                            return View(team);
                        case TeamErrors.IdDoesNotExists:
                            ModelState.AddModelError("PostEditTeamError", $"Team with id: {team.Id} does not exists");
                            return View(team);
                        case TeamErrors.Undefined:
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
            TeamErrors teamErrors;

            bool isTeamDeleted = _sportingFixturesService.TryDeleteTeam(id, out teamErrors);

            if (!isTeamDeleted)
            {
                switch (teamErrors)
                {
                    case TeamErrors.IdDoesNotExists:
                        ModelState.AddModelError("PostDeleteTeamError", $"Team with id: \"{id}\" does not exists");
                        return View();
                    case TeamErrors.Undefined:
                        ModelState.AddModelError("PostDeleteTeamError", $"Team with id: \"{id}\" could not be deleted due to database error");
                        return View();

                }
            }

            return RedirectToAction(nameof(Index));

        }

    }
}
