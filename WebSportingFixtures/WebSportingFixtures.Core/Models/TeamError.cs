using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebSportingFixtures.Core.Models
{
    public enum TeamError
    {
        None = 0,
        Undefined,
        InvalidName, 
        InvalidKnownName,
        NameAlreadyExists,
        KnownNameAlreadyExists,
        IdDoesNotExists
    }
}
