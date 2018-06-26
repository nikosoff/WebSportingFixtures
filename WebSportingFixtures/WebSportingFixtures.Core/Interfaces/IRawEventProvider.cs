using System.Collections.Generic;
using WebSportingFixtures.Core.Models;

namespace WebSportingFixtures.Core.Interfaces
{
    public interface IRawEventProvider
    {
        IEnumerable<RawEvent> GetRawEvents(); // I should change this declaration
    }
}
