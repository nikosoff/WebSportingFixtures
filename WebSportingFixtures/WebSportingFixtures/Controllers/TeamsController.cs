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
            if (string.IsNullOrEmpty(team.Name) || string.IsNullOrEmpty(team.KnownName))
            {
                return View();
            }

            var foundExistingTeamByName = _sportingFixturesService.GetAllTeams().ToList().Find(t => t.Name.ToLower() == team.Name.ToLower());
            var foundExistingTeamByKnownName = _sportingFixturesService.GetAllTeams().ToList().Find(t => t.KnownName.ToLower() == team.KnownName.ToLower());

            if (foundExistingTeamByName != null)
            {
                ModelState.AddModelError("PostCreateTeamNameError", $"The team \"{foundExistingTeamByName.Name}\" is already exists");
                return View();
            }
            else if (foundExistingTeamByKnownName != null)
            {
                ModelState.AddModelError("PostCreateTeamKnownNameError", $"The team known as \"{foundExistingTeamByKnownName.KnownName}\" is already exists");
                return View();
            }
            
            if (ModelState.IsValid)
            {
                var newTeam = new Team { Name = team.Name, KnownName = team.KnownName };

                bool isTeamCreated = _sportingFixturesService.CreateTeam(newTeam);
                if (isTeamCreated)
                {
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ModelState.AddModelError("PostCreateTeamError", $"Team \"{foundExistingTeamByName.Name}\" could not be inserted due to database error");
                    return View();
                }
            }
            else
            {
                return View();
            }
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
                var providedTeam = _sportingFixturesService.GetTeam(team.Id);

                if (providedTeam == null)
                {
                    return View();
                }

                var foundExistingTeamByName = _sportingFixturesService.GetAllTeams().ToList().Find(t => t.Name.ToLower() == team.Name.ToLower());
                var foundExistingTeamByKnownName = _sportingFixturesService.GetAllTeams().ToList().Find(t => t.KnownName.ToLower() == team.KnownName.ToLower());

                if (foundExistingTeamByName != null && foundExistingTeamByName.Name != providedTeam.Name)
                {
                    ModelState.AddModelError("PostEditTeamNameError", $"The team \"{foundExistingTeamByName.Name}\" is already exists");
                    return View(team);
                }
                else if (foundExistingTeamByKnownName != null && foundExistingTeamByKnownName.KnownName != providedTeam.KnownName)
                {
                    ModelState.AddModelError("PostEditTeamKnownNameError", $"The team known as \"{foundExistingTeamByKnownName.KnownName}\" is already exists");
                    return View(team);
                }

                bool isTeamEdited = _sportingFixturesService.EditTeam(team);

                if (isTeamEdited)
                {
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ModelState.AddModelError("PostEditTeamError", $"Team \"{foundExistingTeamByName.Name}\" could not be edited due to database error");
                    return View(team);
                }
            }
            else
            {
                return View(team);
            }
           
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
            bool isTeamDeleted = _sportingFixturesService.DeleteTeam(id);

            if (!isTeamDeleted)
            {
                return View();
            }
            else
            {
                return RedirectToAction(nameof(Index));
            }

        }

    }
}
