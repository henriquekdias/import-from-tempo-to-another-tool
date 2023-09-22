using System.Diagnostics;

namespace ImportFromTempoToAnotherTool.Core.Support
{
    internal class Watcher
    {
        Stopwatch _sw;

        internal Watcher()
        {
            _sw = new Stopwatch();
        }

        internal void Start(string description)
        {
            _sw.Start();

            Console.WriteLine();
            Console.Write($"{description}... ");
        }

        internal void Stop()
        {
            _sw.Stop();

            Console.Write($"Done ({_sw.Elapsed.ToString(@"hh\:mm\:ss\:ff")})");
        }
    }
}