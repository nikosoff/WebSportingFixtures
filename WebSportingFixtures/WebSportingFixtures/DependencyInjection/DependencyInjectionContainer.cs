using System;
using System.IO;
using WebSportingFixtures.Core.Interfaces;
using WebSportingFixtures.Services;

namespace WebSportingFixtures.DependencyInjection
{
    public static class DependencyInjectionContainer
    {
        
        public static IStore GetStore()
        {
            string connectionString = $"Data Source={Directory.GetCurrentDirectory()}\\database.db";
            return new SQLiteStore(connectionString);
        }

        public static ITextSimilarityAlgorithm GetTextSimilarityAlgorithm()
        {
            return new TextSimilarityAlgorithm();
        }

        public static IRawEventProvider GetRawEventProvider()
        {
            string connectionString = @"http://localhost:3000/matches";
            return new HttpRawEventProvider(connectionString);
        }

    }
}
