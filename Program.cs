using System;
using System.Diagnostics;
using System.Linq;

namespace WinTimeIt
{
    internal class Program
    {
        static bool ArrayLimitContains(string[] a,string search,int limit)
        {
            return a.Contains(search) && a.ToList().IndexOf(search) < limit;
        }
        static void Main(string[] args)
        {
            
            if (args.Length < 1)
            {
                Console.WriteLine("Please provide a command to execute. \nUsage: timeit [[-np -r -nl] <command_with_args>]|-h|-v\nExample: timeit -r msedge \"http://example.com\"");
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

            if (ArrayLimitContains(args,"-h",sstartindex) || ArrayLimitContains(args,"--help",sstartindex))
            {
                Console.WriteLine("WinTimeit\nCommand timing utility for Windows\n\nUsage: timeit [options] <command with args>\n[] means optional, <> means mandatory\n\nOptions:\n\n--help (alias -h) Print help menu and exit\n--version (" +
                    "alias -v) Print version information and exit\n" +
                    "-r (alias --return) timeit's return code will be the same as the command's return code. (This will not affect the emoticon)\n" +
                    "-np (alias --noprettyprint) print time in seconds and return code with a single space to seperate, also with no emoticon\n" +
                    "-nl Do not print newline after information\n");
                Environment.Exit(0);
            }
            if (ArrayLimitContains(args, "-v", sstartindex) || ArrayLimitContains(args, "--version", sstartindex))
            {
                Console.WriteLine("WinTimeit 1.4 (c) 2023-2024 Enderbyte Programs. Some rights reserved.");
                Environment.Exit(0);
            }
            bool prettyprint = !(ArrayLimitContains(args, "-np", sstartindex) || ArrayLimitContains(args, "--no-pretty-print", sstartindex));
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
            if (ArrayLimitContains(args, "-r", sstartindex) || ArrayLimitContains(args, "--return", sstartindex))
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
