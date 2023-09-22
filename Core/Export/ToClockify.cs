using ImportFromTempoToAnotherTool.Core.Extensions;
using ImportFromTempoToAnotherTool.Core.Support;

namespace ImportFromTempoToAnotherTool.Core.Export
{
    internal class ToClockify
    {
        internal void Export(List<Model.Tempo.TimeEntry> tempoTimeEntries, Model.UserInformation.Clockify clockifyUserInformation)
        {
            if (tempoTimeEntries == null)
                return;

            if (clockifyUserInformation == null)
                return;

            var clockifyTimeEntries = ConvertTempoTimeEntriesToClockifyTimeEntries(tempoTimeEntries, clockifyUserInformation.ProjectId);
            if (clockifyTimeEntries.Count == 0)
            {
                Console.WriteLine("Unable to convert Tempo time entries to Clockify time entries");
                return;
            }

            if (!CheckIfTimeEntrisOnlyFromCurrentWeek(clockifyTimeEntries))
            {
                return;
            }

            CreateTogglTimeEntries(clockifyTimeEntries, clockifyUserInformation.ApiKey);
        }

        private List<Model.Clockify.TimeEntry> ConvertTempoTimeEntriesToClockifyTimeEntries(List<Model.Tempo.TimeEntry> tempoTimeEntries, string? projectId)
        {
            var w = new Watcher();
            w.Start("converting tempo time entries to toggl time entries");

            var clockifyTimeEntries = new List<Model.Clockify.TimeEntry>();

            if (tempoTimeEntries == null)
                return clockifyTimeEntries;

            if (string.IsNullOrEmpty(projectId))
            {
                //Unpaid leave project id 64b6cf4afe52712557256bd7
                projectId = "64b6cf4afe52712557256bd7";
            }

            foreach (var tempoTimeEntry in tempoTimeEntries)
            {
                var clockifyTimeEntry = new Model.Clockify.TimeEntry();

                clockifyTimeEntry.ProjectId = projectId;

                clockifyTimeEntry.Description = $"{tempoTimeEntry.Key} - {tempoTimeEntry.Summary}";

                clockifyTimeEntry.Start = tempoTimeEntry.Date.ConvertTempoDateStringToUTCStringFormat();

                clockifyTimeEntry.DurationInHours = tempoTimeEntry.Duration.ConvertTempoDurationStringToTimeSpan();

                clockifyTimeEntry.End = clockifyTimeEntry.Start.GetClockifyEndDateTime(tempoTimeEntry.Duration);

                clockifyTimeEntries.Add(clockifyTimeEntry);
            }

            w.Stop();

            return clockifyTimeEntries;
        }

        private bool CheckIfTimeEntrisOnlyFromCurrentWeek(List<Model.Clockify.TimeEntry> clockifyTimeEntries)
        {
            var currentDateTime = DateTime.Now;
            var firstDayOfWeek = currentDateTime.GetDayStartOfWeek(DayOfWeek.Sunday);//Sunday
            var lastDayOfWeek = firstDayOfWeek.AddDays(6);//Saturday

            string? userCommand = string.Empty;

            foreach (var clockifyTimeEntry in clockifyTimeEntries)
            {
                if (!AreTempoTimeEntriesOnlyFromCurrentWeek(clockifyTimeEntry, firstDayOfWeek, lastDayOfWeek))
                {
                    while (string.IsNullOrEmpty(userCommand))
                    {
                        Console.WriteLine();

                        Console.WriteLine("  Looks like there is more times entries on Tempo Raw CSV than current week.");
                        Console.Write("  Do you wish do continue? [Y/n]");

                        userCommand = Console.ReadLine();
                        if (!string.IsNullOrEmpty(userCommand))
                        {
                            if (string.IsNullOrEmpty(userCommand) || userCommand.Contains('Y') || userCommand.Contains('y'))
                            {
                                return true;
                            }
                            else if (userCommand.Contains('N') || userCommand.Contains('n'))
                            {
                                Console.WriteLine();
                                Console.WriteLine("  Conversion stopped by user!!!");

                                return false;
                            }
                            else
                            {
                                Console.WriteLine("  Invalid input");

                                userCommand = string.Empty;
                            }
                        }
                    }
                }
            }

            return true;
        }

        private bool AreTempoTimeEntriesOnlyFromCurrentWeek(Model.Clockify.TimeEntry clockifyTimeEntry, DateTime firstDayOfWeek, DateTime lastDayOfWeek)
        {
            if (clockifyTimeEntry.StartTime.HasValue)
            {
                if (clockifyTimeEntry.StartTime.Value.IsOlderThan(firstDayOfWeek))
                {
                    return false;
                }

                if (clockifyTimeEntry.StartTime.Value.IsNewerThan(lastDayOfWeek))
                {
                    return false;
                }
            }

            return true;
        }

        private void CreateTogglTimeEntries(List<Model.Clockify.TimeEntry> newClockifyTimeEntries, string? apiKey)
        {
            if (string.IsNullOrEmpty(apiKey))
                return;

            var w = new Watcher();
            w.Start($"creating time entries on Clockify using {apiKey}");

            var clockifyApi = new API.Clockify(apiKey);

            var user = clockifyApi.GetUserInformation().Result;
            if (user == null)
                return;

            if (string.IsNullOrEmpty(user.Id))
                return;

            if (string.IsNullOrEmpty(user.DefaultWorkspace))
                return;

            var oldClockifyTimeEntries = GetClockifyTimeEntries(clockifyApi, user);

            if (oldClockifyTimeEntries == null)
                return;

            foreach (var newClockifyTimeEntry in newClockifyTimeEntries)
            {
                if (ShouldCreateTimeEntryAlreadyOnClockify(newClockifyTimeEntry, oldClockifyTimeEntries))
                {
                    clockifyApi.CreateTimeEntry(newClockifyTimeEntry, user.DefaultWorkspace).Wait();
                }
            }

            w.Stop();
        }

        private List<Model.Clockify.TimeEntry>? GetClockifyTimeEntries(API.Clockify clockifyApi, Model.Clockify.User clockifyUser)
        {
            if (clockifyApi == null)
                return null;

            if (clockifyUser == null)
                return null;

            if (string.IsNullOrEmpty(clockifyUser.DefaultWorkspace))
                return null;

            if (string.IsNullOrEmpty(clockifyUser.Id))
                return null;

            var currentDateTime = DateTime.Now;
            DateTime? firstDayOfLastMonth = new DateTime(
                currentDateTime.Year,
                currentDateTime.AddMonths(-1).Month,
                1, //day
                0, //hour
                0, //minute
                1 //second
            );

            var since = firstDayOfLastMonth.ConvertDateTimeToUTCStringFormat();

            var oldClockifyTimeEntries = clockifyApi
                .GetEntries(
                    clockifyUser.DefaultWorkspace,
                    clockifyUser.Id,
                    since
                )
            .Result;

            return oldClockifyTimeEntries;
        }

        private bool ShouldCreateTimeEntryAlreadyOnClockify(Model.Clockify.TimeEntry newClockifyTimeEntry, List<Model.Clockify.TimeEntry> oldClockifyTimeEntries)
        {
            if (IsThereAlreadySameTimeEntryOnToggl(newClockifyTimeEntry, oldClockifyTimeEntries))
            {
                string? userCommand = string.Empty;

                while (string.IsNullOrEmpty(userCommand))
                {
                    Console.WriteLine();
                    Console.WriteLine();

                    Console.WriteLine("  Looks like there is a time entry on your Clockify that match Start Time, End Time and Description");
                    Console.Write("  Do you wish do continue anyway? [Y/n]");

                    userCommand = Console.ReadLine();
                    if (string.IsNullOrEmpty(userCommand) || userCommand.Contains('Y') || userCommand.Contains('y'))
                    {
                        return true;
                    }
                    else if (userCommand.Contains('N') || userCommand.Contains('n'))
                    {
                        Console.WriteLine("  Time entry will not be add");

                        return false;
                    }
                    else
                    {
                        Console.WriteLine("  Invalid input");

                        userCommand = string.Empty;
                    }
                }
            }

            return true;
        }

        private bool IsThereAlreadySameTimeEntryOnToggl(Model.Clockify.TimeEntry newClockifyTimeEntry, List<Model.Clockify.TimeEntry> oldClockifyTimeEntries)
        {
            if (newClockifyTimeEntry == null)
                return false;

            if (oldClockifyTimeEntries == null)
                return false;

            foreach (var oldClockifyTimeEntry in oldClockifyTimeEntries)
            {
                if (oldClockifyTimeEntry.TimeInterval == null)
                    continue;

                if (!oldClockifyTimeEntry.TimeInterval.StartTime.HasValue)
                    continue;

                if (!newClockifyTimeEntry.StartTime.HasValue)
                    continue;

                if (!oldClockifyTimeEntry.TimeInterval.StartTime.Value.IsSameThan(newClockifyTimeEntry.StartTime.Value))
                    continue;

                if (!oldClockifyTimeEntry.TimeInterval.EndTime.HasValue)
                    continue;

                if (!newClockifyTimeEntry.EndTime.HasValue)
                    continue;

                if (!oldClockifyTimeEntry.TimeInterval.EndTime.Value.IsSameThan(newClockifyTimeEntry.EndTime.Value))
                    continue;

                if (string.IsNullOrEmpty(oldClockifyTimeEntry.Description))
                    continue;

                if (string.IsNullOrEmpty(newClockifyTimeEntry.Description))
                    continue;

                if (!oldClockifyTimeEntry.Description.IsEqual(newClockifyTimeEntry.Description))
                    continue;

                return true;
            }

            return false;
        }
    }
}
