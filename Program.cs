namespace ImportFromTempoToAnotherTool
{
    class Program
    {
        static void Main(string[] args)
        {
            var w = new Core.Support.Watcher();
            w.Start("starting import time entries from Tempo");
            Console.WriteLine();

            var engine = new Core.Engine();
            engine.Run();

            Console.WriteLine();
            w.Stop();
            Console.WriteLine();
        }
    }
}