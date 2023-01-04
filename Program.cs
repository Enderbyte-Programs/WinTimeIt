using System;
using System.Diagnostics;

namespace WinTimeIt
{
    internal class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("Please provide a command to execute. Example: timeit <command_with_args>");
                Environment.Exit(1);
            }
            TimedProcess timed = new TimedProcess(args);
            double[] results = timed.Execute();
            Console.Write($"\nTime: {Math.Round(results[0],3)} seconds");
            if (results[1] != 0)
            {
                Console.Write("       ");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write($":(  {results[1]}");
                Console.ResetColor();
            } else
            {
                Console.Write("       ");
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write(":)");
                Console.ResetColor();
            }
            Console.Write("\n");//Making sure that newline is available
            Environment.Exit(0);
        }
    }
    public class TimedProcess
    {
        private string args;
        public TimedProcess(string[] args)
        {
 
            this.args = String.Join(" ",args);

        }
        public double[] Execute ()
        {
            //Returns [time_in_seconds,return code]
            Process p = new Process();
            p.StartInfo.FileName = "cmd.exe";
            p.StartInfo.Arguments = "/C " + this.args;
            p.StartInfo.UseShellExecute = false;
            DateTime start = DateTime.Now;
            p.Start();
            p.WaitForExit();
            DateTime end = DateTime.Now;
            double total_seconds = (end - start).TotalMilliseconds / 1000;
            return new double[] { total_seconds, p.ExitCode };
        }
    }
}
