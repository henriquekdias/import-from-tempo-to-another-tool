using Microsoft.VisualBasic.FileIO;
using ImportFromTempoToAnotherTool.Core.Support;
using System.Text.Json;

namespace ImportFromTempoToAnotherTool.Core
{
    internal class Engine
    {
        internal readonly string _applicationPath = System.AppDomain.CurrentDomain.BaseDirectory;

        public void Run()
        {
            var tempoTimeEntries = GetTempoTimeEntriesFromCsv();
            if (tempoTimeEntries.Count == 0)
            {
                Console.WriteLine("Not able to get Tempo time entries from csv file");
                return;
            }

            var trackingUserInformation = GetTrackingToolUserInformation();
            if (trackingUserInformation == null)
            {
                Console.WriteLine("Not able to get Tracking User information from json");
                return;
            }

            if (trackingUserInformation.Toggl != null)
            {
                var exportToToggl = new Export.ToToggl();
                exportToToggl.Export(tempoTimeEntries, trackingUserInformation.Toggl);
            }
            else if(trackingUserInformation.Clockify != null)
            {
                var exportToClockify = new Export.ToClockify();
                exportToClockify.Export(tempoTimeEntries, trackingUserInformation.Clockify);
            }
        }

        private List<Model.Tempo.TimeEntry> GetTempoTimeEntriesFromCsv()
        {
            var w = new Watcher();
            w.Start("getting timeEntries from tempo.csv");

            var tempoTimeEntries = new List<Model.Tempo.TimeEntry>();

            try
            {
                var filePath = $"{_applicationPath}rawtempo.csv";

                if (!File.Exists(filePath))
                    return tempoTimeEntries;

                using (TextFieldParser csvParser = new TextFieldParser(filePath))
                {
                    csvParser.CommentTokens = new string[] { "#" };
                    csvParser.SetDelimiters(new string[] { "," });
                    csvParser.HasFieldsEnclosedInQuotes = true;

                    // Read headers row
                    string[]? header = csvParser.ReadFields();

                    while (!csvParser.EndOfData)
                    {
                        // Read current line fields, pointer moves to the next line.
                        string[]? fields = csvParser.ReadFields();

                        var tempoTimeEntry = new Model.Tempo.TimeEntry();
                        tempoTimeEntry.Key = fields[0];
                        tempoTimeEntry.Summary = fields[1];
                        tempoTimeEntry.Duration = fields[2];
                        tempoTimeEntry.Date = fields[3];

                        tempoTimeEntries.Add(tempoTimeEntry);
                    }
                }
            }
            catch
            {
                throw;
            }

            w.Stop();

            return tempoTimeEntries;
        }

        private Model.UserInformation.TrackingTools? GetTrackingToolUserInformation()
        {
            var trackingUserInformation = ReadTrackingUserInformation();

            if (trackingUserInformation == null)
                return null;

            if (trackingUserInformation.Toggl != null)
            {
                if (string.IsNullOrEmpty(trackingUserInformation.Toggl.Token)
                    && string.IsNullOrEmpty(trackingUserInformation.Toggl.Email)
                    && string.IsNullOrEmpty(trackingUserInformation.Toggl.Password)
                )
                {
                    trackingUserInformation.Toggl = null;
                }
            }

            if (trackingUserInformation.Clockify != null)
            {
                if(string.IsNullOrEmpty(trackingUserInformation.Clockify.ApiKey))
                {
                    trackingUserInformation.Clockify = null;
                }
            }

            if (
                trackingUserInformation.Toggl == null
                && trackingUserInformation.Clockify == null
            )
            {
                return null;
            }

            return trackingUserInformation;

        }

        private Model.UserInformation.TrackingTools? ReadTrackingUserInformation()
        {
            var w = new Watcher();
            w.Start("getting tracking tool user information");

            var userInformation = new Model.UserInformation.TrackingTools();

            try
            {
                var fileName = $"{_applicationPath}trackingtoolsuserinformation.json";

                if (File.Exists(fileName))
                {
                    string jsonString = File.ReadAllText(fileName);
                    userInformation = JsonSerializer.Deserialize<Model.UserInformation.TrackingTools>(jsonString);
                }
                else
                {
                    userInformation = null;
                }
            }
            catch
            {
                userInformation = null;
            }

            w.Stop();

            return userInformation;
        }

    }
}