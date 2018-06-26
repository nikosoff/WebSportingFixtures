using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using WebSportingFixtures.Core.Interfaces;

namespace WebSportingFixtures.Services
{
    public class TextSimilarityAlgorithm : ITextSimilarityAlgorithm
    {
        public bool TryFindBestMatches(string input, IEnumerable<string> availableItems, int numberOfMatches, out IEnumerable<string> matches)
        {
            Regex regex = new Regex("[\u0300-\u036f]|[\u0021-\u002f]|[0-9]"); // regex to remove accents
            matches = Enumerable.Empty<string>();

            if (availableItems == null || 
                String.IsNullOrEmpty(input) ||
                numberOfMatches == 0 || 
                numberOfMatches > availableItems.Count())
                return false;

            var candidateMatches = availableItems.OrderBy(s =>
            {
                return EditDistance(regex.Replace(input.ToLower().Normalize(NormalizationForm.FormKD), ""), regex.Replace(s.ToLower().Normalize(NormalizationForm.FormKD), ""));
            })
            .Take(numberOfMatches);

            matches = candidateMatches;
            return true;
        }

        private int EditDistance(string firstWord, string secondWord)
        {

            int N = firstWord.Length + 1;
            int M = secondWord.Length + 1;

            int[,] A = new int[N, M];

            for (int i = 0; i < N; i++)
                A[i, 0] = i;

            for (int j = 0; j < M; j++)
                A[0, j] = j;

            for (int i = 1; i < N; i++)
            {
                for (int j = 1; j < M; j++)
                {
                    A[i, j] = Math.Min(Math.Min(A[i - 1, j] + 1, A[i, j - 1] + 1), firstWord[i - 1] != secondWord[j - 1] ? A[i - 1, j - 1] + 1 : A[i - 1, j - 1]);
                }
            }

            return A[N - 1, M - 1] - CountCommonCharacters(firstWord, secondWord);
        }

        private int CountCommonCharacters(string firstWord, string secondWord)
        {
            int commonCharacters = 0;
            foreach (var term in firstWord.Split(' '))
            {
                foreach (var term2 in secondWord.Split(' '))
                {
                    Match regex = Regex.Match(term, term2);
                    commonCharacters += regex.Length;
                }
            }

            return commonCharacters;
        }
    }
}
