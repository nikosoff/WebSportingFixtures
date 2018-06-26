using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebSportingFixtures.Core.Interfaces
{
    public interface ITextSimilarityAlgorithm
    {
        bool TryFindBestMatches(string input, IEnumerable<string> availableItems, int numberOfMatches, out IEnumerable<string> matches);
    }
}
