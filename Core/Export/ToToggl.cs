using ImportFromTempoToAnotherTool.Core.Extensions;
using ImportFromTempoToAnotherTool.Core.Support;

namespace ImportFromTempoToAnotherTool.Core.Export
{
    internal class ToToggl
    {
        internal void Export(List<Model.Tempo.TimeEntry> tempoTimeEntries, Model.UserInformation.Toggl togglUserInformation)
        {
            if (tempoTimeEntries == null)
                return;

            if (togglUserInformation == null)
                return;

            var togglTimeEntries = ConvertTempoTimeEntriesToTogglTimeEntries(tempoTimeEntries, togglUserInformation.WorkSpaceId);
            if (togglTimeEntries.Count == 0)
            {
                Console.WriteLine("Unable to convert Tempo time entries to Toggl time entries");
                return;
            }

            if (!CheckIfTimeEntrisOnlyFromCurrentWeek(togglTimeEntries))
            {
                return;
            }

            CreateTogglTimeEntries(togglTimeEntries, togglUserInformation);
        }

        private List<Model.Toggl.TimeEntry> ConvertTempoTimeEntriesToTogglTimeEntries(List<Model.Tempo.TimeEntry> tempoTimeEntries, int? togglWorkSpaceId)
        {
            var w = new Watcher();
            w.Start("converting tempo time entries to toggl time entries");

            var togglTimeEntries = new List<Model.Toggl.TimeEntry>();

            if (tempoTimeEntries == null)
                return togglTimeEntries;

            if (!togglWorkSpaceId.HasValue)
                return togglTimeEntries;

            foreach (var tempoTimeEntry in tempoTimeEntries)
            {
                var togglTimeEntry = new Model.Toggl.TimeEntry();

                togglTimeEntry.WorkspaceId = togglWorkSpaceId.Value;

                togglTimeEntry.Description = $"{tempoTimeEntry.Key} - {tempoTimeEntry.Summary}";

                togglTimeEntry.Duration = tempoTimeEntry.Duration.ConvertTempoDurationStringToSecondsInt();

                togglTimeEntry.Start = tempoTimeEntry.Date.ConvertTempoDateStringToUTCStringFormat();

                togglTimeEntries.Add(togglTimeEntry);
            }

            w.Stop();

            return togglTimeEntries;
        }

        private bool CheckIfTimeEntrisOnlyFromCurrentWeek(List<Model.Toggl.TimeEntry> togglTimeEntries)
        {
            var currentDateTime = DateTime.Now;
            var firstDayOfWeek = currentDateTime.GetDayStartOfWeek(DayOfWeek.Sunday);//Sunday
            var lastDayOfWeek = firstDayOfWeek.AddDays(6);//Saturday

            string? userCommand = string.Empty;

            foreach (var togglTimeEntry in togglTimeEntries)
            {
                if (!AreTempoTimeEntriesOnlyFromCurrentWeek(togglTimeEntry, firstDayOfWeek, lastDayOfWeek))
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

        private bool AreTempoTimeEntriesOnlyFromCurrentWeek(Model.Toggl.TimeEntry tooglTimeEntry, DateTime firstDayOfWeek, DateTime lastDayOfWeek)
        {
            if (tooglTimeEntry.StartTime.HasValue)
            {
                if (tooglTimeEntry.StartTime.Value.IsOlderThan(firstDayOfWeek))
                {
                    return false;
                }

                if (tooglTimeEntry.StartTime.Value.IsNewerThan(lastDayOfWeek))
                {
                    return false;
                }
            }

            return true;
        }

        private void CreateTogglTimeEntries(List<Model.Toggl.TimeEntry> newTogglTimeEntries, Model.UserInformation.Toggl togglUserInformation)
        {
            var user = togglUserInformation.Email;
            if (string.IsNullOrEmpty(user))
            {
                user = togglUserInformation.Token;
            }

            var w = new Watcher();
            w.Start($"creating time entries on Toggl using {user}");

            if (togglUserInformation == null)
                return;

            if (string.IsNullOrEmpty(togglUserInformation.Token))
            {
                if (string.IsNullOrEmpty(togglUserInformation.Email))
                    return;

                if (string.IsNullOrEmpty(togglUserInformation.Password))
                    return;
            }

            if (!togglUserInformation.WorkSpaceId.HasValue)
                return;

            var togglApi = new API.Toggl(togglUserInformation);

            var oldTimeEntriesOnToggl = GetTogglTimeEntries(togglApi);

            if (oldTimeEntriesOnToggl == null)
                return;

            foreach (var newTogglTimeEntry in newTogglTimeEntries)
            {
                if (ShouldCreateTimeEntryAlreadyOnToggl(newTogglTimeEntry, oldTimeEntriesOnToggl))
                {
                    togglApi.CreateTimeEntry(newTogglTimeEntry, togglUserInformation.WorkSpaceId.Value).Wait();
                }
            }

            w.Stop();
        }

        private List<Model.Toggl.TimeEntry>? GetTogglTimeEntries(API.Toggl togglApi)
        {
            var currentDateTime = DateTime.Now;
            var firstDayOfLastMonth = new DateTime(currentDateTime.Year, currentDateTime.AddMonths(-1).Month, 1);

            DateTimeOffset utcTime = DateTime.SpecifyKind(firstDayOfLastMonth, DateTimeKind.Utc);

            var unixTimestamp = utcTime.ToUnixTimeSeconds();

            var oldTimeEntriesOnToggl = togglApi.GetEntries(unixTimestamp).Result;

            return oldTimeEntriesOnToggl;
        }

        private bool ShouldCreateTimeEntryAlreadyOnToggl(Model.Toggl.TimeEntry togglTimeEntry, List<Model.Toggl.TimeEntry> oldTimeEntriesOnToggl)
        {
            if (IsThereAlreadySameTimeEntryOnToggl(togglTimeEntry, oldTimeEntriesOnToggl))
            {
                string? userCommand = string.Empty;

                while (string.IsNullOrEmpty(userCommand))
                {
                    Console.WriteLine();
                    Console.WriteLine();

                    Console.WriteLine("  Looks like there is a time entry on your Toggl that match Start Time, Duration and Description");
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

        private bool IsThereAlreadySameTimeEntryOnToggl(Model.Toggl.TimeEntry newTogglTimeEntry, List<Model.Toggl.TimeEntry> oldTimeEntriesOnToggl)
        {
            if (newTogglTimeEntry == null)
            {
                return false;
            }

            if (oldTimeEntriesOnToggl == null)
            {
                return false;
            }

            foreach (var oldTimeEntryOnToggl in oldTimeEntriesOnToggl)
            {
                if (!string.IsNullOrEmpty(oldTimeEntryOnToggl.ServerDeletedAt))
                    continue;

                if (!oldTimeEntryOnToggl.StartTime.HasValue)
                    continue;

                if (!newTogglTimeEntry.StartTime.HasValue)
                    continue;

                if (!oldTimeEntryOnToggl.StartTime.Value.IsSameThan(newTogglTimeEntry.StartTime.Value))
                    continue;

                if (oldTimeEntryOnToggl.Duration != newTogglTimeEntry.Duration)
                    continue;

                if (string.IsNullOrEmpty(oldTimeEntryOnToggl.Description))
                    continue;

                if (string.IsNullOrEmpty(newTogglTimeEntry.Description))
                    continue;

                if (!oldTimeEntryOnToggl.Description.IsEqual(newTogglTimeEntry.Description))
                    continue;

                return true;
            }

            return false;
        }
    }
}