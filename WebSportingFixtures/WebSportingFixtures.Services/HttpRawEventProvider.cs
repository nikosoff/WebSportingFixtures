using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using WebSportingFixtures.Core.Interfaces;
using WebSportingFixtures.Core.Models;

namespace WebSportingFixtures.Services
{
    public class HttpRawEventProvider : IRawEventProvider
    {
        private readonly string _connectionString;
        private List<RawEvent> _rawEvents;

        public HttpRawEventProvider(string connectionString)
        {
            _rawEvents = new List<RawEvent>();
            _connectionString = connectionString;
        }

        public IEnumerable<RawEvent> GetRawEvents()
        {
            return GetRequest(_connectionString).Result;
        }

        public async Task<IEnumerable<RawEvent>> GetRequest(string url)
        {
            try
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    using (HttpResponseMessage httpResponseMessage = await httpClient.GetAsync(url))
                    {
                        using (HttpContent httpContent = httpResponseMessage.Content)
                        {
                            string content = await httpContent.ReadAsStringAsync();
                            return await JsonParser(content);
                        }
                    }
                }
            }
            catch(HttpRequestException)
            {
                return Enumerable.Empty<RawEvent>();
            }
        }

        public Task<IEnumerable<RawEvent>> JsonParser(string jsonFile)
        {
            JsonSerializer jsonSerializer = new JsonSerializer();
            dynamic jsonObject = JsonConvert.DeserializeObject(jsonFile);
            var matches = jsonObject;

            return Task.Factory.StartNew(() =>
            {
                foreach (var match in matches)
                {
                    _rawEvents.Add(new RawEvent()
                    {
                        Home = match.team1.name,
                        Away = match.team2.name,
                        Status = (Status)Status.Finished
                    });
                }
                return (IEnumerable<RawEvent>)_rawEvents;
            });

        }

    }
}
