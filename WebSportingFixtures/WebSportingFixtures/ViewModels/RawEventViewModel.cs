using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebSportingFixtures.Core.Models;

namespace WebSportingFixtures.ViewModels
{
    public class RawEventViewModel
    {
        private IEnumerable<RawEvent> _rawEvents;

        public void SetRawEvents(IEnumerable<RawEvent> rawEvents)
        {
            _rawEvents = rawEvents;
        }

        public IEnumerable<RawEvent> GetRawEvents()
        {
            return _rawEvents;
        }

    }
}
