using ImportFromTempoToAnotherTool.Model.Toggl;
using System.Net.Http.Headers;
using System.Text.Json;

namespace ImportFromTempoToAnotherTool.Core.API
{
    internal class Toggl
    {
        private HttpClient _client;

        public Toggl(Model.UserInformation.Toggl togglUserInformation)
        {
            _client = new HttpClient();

            _client.BaseAddress = new Uri("https://api.track.toggl.com/api/v9/");
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            AuthenticationHeaderValue? auth = null;

            if(!string.IsNullOrEmpty(togglUserInformation.Token))
            {
                auth =
                new AuthenticationHeaderValue(
                    "Basic",
                    Convert.ToBase64String(
                        System.Text.Encoding.ASCII.GetBytes(
                                   $"{togglUserInformation.Token}:api_token")));
            }
            else
            {
                auth =
                new AuthenticationHeaderValue(
                    "Basic",
                    Convert.ToBase64String(
                        System.Text.Encoding.ASCII.GetBytes(
                                   $"{togglUserInformation.Email}:{togglUserInformation.Password}")));
            }

             _client.DefaultRequestHeaders.Authorization = auth;

        }

        //since parameter needs to be on Unix Timestamp
        public async Task<List<ImportFromTempoToAnotherTool.Model.Toggl.TimeEntry>?> GetEntries(long? since)
        {
            var timeEntry = new List<ImportFromTempoToAnotherTool.Model.Toggl.TimeEntry>();

            //https://api.track.toggl.com/api/v9/me/time_entries
            var url = "me/time_entries";

            if(since.HasValue)
            {
                //https://api.track.toggl.com/api/v9/me/time_entries?since=1685588401
                url = $"{url}?since={since.Value}";
            }

            var response = _client.GetAsync(url).Result;

            if (response.IsSuccessStatusCode)
            {
                timeEntry = await response.Content.ReadAsAsync<List<ImportFromTempoToAnotherTool.Model.Toggl.TimeEntry>>();
            }
            else
            {
                return null;
            }

            return timeEntry;
        }

        public async Task CreateTimeEntry(TimeEntry timeEntry, int workspaceId)
        {
            if (timeEntry == null)
                return;

            var url = $"workspaces/{workspaceId}/time_entries";

            string jsonString = JsonSerializer.Serialize(timeEntry);

            var request = new StringContent(jsonString);

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
    }
}