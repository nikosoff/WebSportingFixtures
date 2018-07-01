using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebSportingFixtures.Core.Models;

namespace WebSportingFixtures.ViewModels
{
    public class RawsEventsViewModel
    {
        public Dictionary<string, Tuple<string, string, Status, bool>> map;

        public RawsEventsViewModel()
        {
            map = new Dictionary<string, Tuple<string, string, Status, bool>>();
        }
    }
}
