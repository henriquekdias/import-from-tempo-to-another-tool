using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace ImportFromTempoToAnotherTool.Core.API
{
    internal class Clockify
    {
        private HttpClient _client;

        public Clockify(string apiKey)
        {
            _client = new HttpClient();

            _client.BaseAddress = new Uri("https://api.clockify.me/api/");
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _client.DefaultRequestHeaders.Add("X-Api-Key", apiKey);
        }

        public async Task<Model.Clockify.User> GetUserInformation()
        {
            //https://api.clockify.me/api/v1/user

            var clockifyUser = new Model.Clockify.User();

            var url = "v1/user";

            var response = _client.GetAsync(url).Result;

            if (response.IsSuccessStatusCode)
            {
                clockifyUser = await response.Content.ReadAsAsync<Model.Clockify.User>();
            }
            else
            {
                return clockifyUser;
            }

            return clockifyUser;
        }

        public async Task CreateTimeEntry(Model.Clockify.TimeEntry timeEntry, string workspaceId)
        {
            if (timeEntry == null)
                return;

            //https://api.clockify.me/api/v1/workspaces/{workspaceId}/time-entries
            var url = $"v1/workspaces/{workspaceId}/time-entries";

            string jsonString = JsonSerializer.Serialize(timeEntry);
            var request = new StringContent(jsonString,
                Encoding.UTF8,
                "application/json"
            );

            HttpResponseMessage? response = null;

            try
            {
                response = _client.PostAsync(
                    url,
                    request
                ).Result;

                response.EnsureSuccessStatusCode();

                string content = await response.Content.ReadAsStringAsync();

                //Console.WriteLine(response.StatusCode);
                //Console.WriteLine(content);
            }
            catch (System.Exception e)
            {
                Console.WriteLine("=======================");
                Console.WriteLine(e.Message);

                if (response != null)
                {
                    Console.WriteLine("-----------------------");
                    Console.WriteLine(response.StatusCode);
                }

                Console.WriteLine("=======================");
                Console.WriteLine();
            }
        }

        public async Task<List<Model.Clockify.TimeEntry>> GetEntries(string workspaceId, string userId, string since)
        {
            //https://api.clockify.me/api/v1/workspaces/{workspaceId}/user/{userId}/time-entries?start=2023-09-18T00:00:01Z

            var timeEntries = new List<Model.Clockify.TimeEntry>();

            var url = $"v1/workspaces/{workspaceId}/user/{userId}/time-entries?start={since}";

            var response = _client.GetAsync(url).Result;

            if (response.IsSuccessStatusCode)
            {
                timeEntries = await response.Content.ReadAsAsync<List<Model.Clockify.TimeEntry>>();
            }

            return timeEntries;
        }
    }
}