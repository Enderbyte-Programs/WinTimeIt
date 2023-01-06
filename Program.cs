using System;
using System.Diagnostics;
using System.Linq;

namespace WinTimeIt
{
    internal class Program
    {
        static void Main(string[] args)
        {
            bool prettyprint = !(args.Contains("-np") || args.Contains("--no-pretty-print"));
            if (args.Length < 1)
            {
                Console.WriteLine("Please provide a command to execute. Example: timeit [-np -r] <command_with_args>");
                Environment.Exit(1);
            }
            int sstartindex = 0;
            foreach (string s in args)
            {
                if (!s.ToCharArray()[0].Equals('-'))//Trimming options from actual command
                {
                    break;//Found start of actual command
                } else
                {
                    sstartindex++;
                }
            }
            if (args.Contains("-h") || args.Contains("--help"))
            {
                Console.WriteLine("WinTimeit\nThe GNU time command for Windows\n\nUsage: timeit [options] <command with args>\n[] means optional, <> means mandatory\n\nOptions:\n\n--help (alias -h) Print help menu and exit\n--version (" +
                    "alias -v) Print version information and exit\n" +
                    "-r (alias --return) timeit's return code will be the same as the command's return code\n" +
                    "-np (alias --noprettyprint) print time in seconds and return code with a single space to seperate, also with no emoticon\n" +
                    "-nl Do not print newline after information");
                Environment.Exit(0);
            }
            if (args.Contains("-v") || args.Contains("--version"))
            {
                Console.WriteLine("WinTimeit 2.0");
                Environment.Exit(0);
            }
            string[] outargs = new string[args.Length - sstartindex];
            Array.Copy(args, sstartindex,outargs, 0,args.Length - sstartindex);
            TimedProcess timed = new TimedProcess(outargs);
            double[] results = timed.Execute();
            if (prettyprint)
            {
                Console.Write($"\nTime: {Math.Round(results[0], 3)} seconds");
                if (results[1] != 0)
                {
                    Console.Write("       ");
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write($":(  {results[1]}");
                    Console.ResetColor();
                }
                else
                {
                    Console.Write("       ");
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write(":)");
                    Console.ResetColor();
                }
            } else
            {
                Console.WriteLine("");
                Console.Write(Math.Round(results[0],3));
                Console.Write(" ");
                Console.Write(results[1]);
            }
            if (!args.Contains("-nl"))
            { Console.Write("\n"); }//Making sure that newline is available
            if (args.Contains("-r") || args.Contains("--return"))
            {
                //Return process exit value
                Environment.Exit(Convert.ToInt16(results[1]));
            }
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
