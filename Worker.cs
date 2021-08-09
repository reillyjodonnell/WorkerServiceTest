using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.ServiceProcess;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.InteropServices;

namespace WorkerServiceTest
{
    public class Worker : BackgroundService
    {

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", SetLastError = true)]
        static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);


        private readonly ILogger<Worker> _logger;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }

        public static Process getForegroundProcess()
        {
            uint processID = 0;
            IntPtr hWnd = GetForegroundWindow(); // Get foreground window handle
            uint threadID = GetWindowThreadProcessId(hWnd, out processID); // Get PID from window handle
            Process fgProc = Process.GetProcessById(Convert.ToInt32(processID)); // Get it as a C# obj.
                                                                                 // NOTE: In some rare cases ProcessID will be NULL. Handle this how you want. 
            return fgProc;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {

                getForegroundProcess();




                DateTime localDate = DateTime.Now;

                Console.WriteLine(localDate);

                string userID = Environment.UserName;

                Console.WriteLine(userID);


                //This runs once then exits.

                Process[] processlist = Process.GetProcesses();
                
                List<string> runningProcesses = new List<string>();

                int numberOfProcesses = processlist.Length;

                for(int i = 0; i < numberOfProcesses; i++)
                {
                    if(processlist[i].MainWindowTitle.Length > 1)
                    {
                        string title = processlist[i].MainWindowTitle;

                        runningProcesses.Add(title);
                        
                    }
                }

                Filter filter = new Filter();
                filter.readData(runningProcesses, numberOfProcesses);
                

               

                //Gives a list of every single process running on the taskbar

                foreach (Process theprocess in processlist)
                {
                    if (theprocess.MainWindowTitle.Length > 1)
                    {
                        string currentProcess = theprocess.ToString();
                        string running = runningProcesses.ToString();

                        //Check if theprocess is in the runningProcesses
                        if(running.Contains(currentProcess))
                        {
                            Console.WriteLine("Found a match");
                        } else if(running.Contains(currentProcess) == false)
                        {
                            Console.WriteLine("Adding now!");
                            runningProcesses.Add(theprocess.MainWindowTitle);
                        }




                        //Save the titles into strings and if the strings are different/any new ones append it.
                        //This prevents the same name from repeating over and over

                        File.AppendAllText("Test.txt" , theprocess.MainWindowTitle + localDate);
                        File.AppendAllText("Test.txt", "\n");
                    }

                }
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                Console.WriteLine("Test");
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}
