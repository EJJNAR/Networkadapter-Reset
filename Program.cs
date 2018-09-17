using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NetworkFixer
{
    public class Program
    {
        /// <summary>
        /// This is a simple program to run when your network adapter is giving you troubles for no reason at all. 
        /// I made this when my laptop's network adapter refused to work as soon you connected to a new wireless network.
        /// It simply runs a few commands in cmd to get it to working again, and therefore is a solution to manually resetting the adapter through cmd, or relying on windows to solve the issue.
        /// 
        /// </summary>

        #region Global Variables

        private static string welcomeString = "Network adapter Fixer | By Thomas Nordqvist\n";
        private static string checkingInternet = "Checking for internet connectivity...";
        private static bool internetstatus;
        #endregion

        #region Main Entry Point

        static void Main(string[] args)
        {
            Console.WriteLine(welcomeString);
            AdapterHandler();
        }

        #endregion

        #region Method for handling the resetting of the network adapter(s)
        public static void AdapterHandler()
        {
            string input;
            while(true)
            {
                Console.Clear();
                Console.WriteLine(welcomeString);
                var spinner = new Spinner(37, 2);  //Loading animation
                Console.WriteLine(checkingInternet);               
                spinner.Start();
                Thread.Sleep(3000); //Give the adapters time to restart if they have been reset before checking the internetconnection
                internetstatus = CheckForInternetConnection();

                if (!internetstatus)  //No internet connection found
                {
                    spinner.Dispose();
                    Console.Clear();
                    Console.WriteLine(welcomeString);
                    Console.WriteLine("No internet connection found. Do you want to reset your network adapter(s)? 'Y/N'");
                    input = Console.ReadLine();
                    if (input.ToUpper() == "Y") //Reset the adapters
                    {
                        ResetAdapter();
                    } 
                    else if (input.ToUpper() == "N") { Environment.Exit(1); }  //Exit the program
                    else if (input.ToUpper() != "N" && input.ToUpper() != "N")
                    {
                        Console.WriteLine("invalid input");
                    }
                }

                if (internetstatus) //Internet connection found
                {
                    spinner.Dispose();
                    Console.Clear();
                    Console.WriteLine(welcomeString);
                    Console.WriteLine("Your connection seems to be working. Do you really want to reset your network adapter(s)? 'Y/N' \n *This may cause your network adapter to stop working normally ");
                    input = Console.ReadLine();
                    if (input.ToUpper() == "Y") //Reset the adapters
                    {
                        ResetAdapter();
                    }
                    else if (input.ToUpper() == "N") { Environment.Exit(1); }  //Exit the program
                    else if (input.ToUpper() != "N" && input.ToUpper() != "N")
                    {
                        Console.WriteLine("invalid input");
                    }
                }
            }
        }

        #endregion

        #region Logic for resetting the network adapter(s)

        //Simply a bunch of lines that run in cmd to reset to network adapter
        private static void ResetAdapter()
        {        

            System.Diagnostics.Process.Start("netsh", "winsock reset");
            System.Diagnostics.Process.Start("netsh", "int ip reset reset");
            System.Diagnostics.Process.Start("ipconfig", "/release");
            System.Diagnostics.Process.Start("ipconfig", "/reset");
            System.Diagnostics.Process.Start("ipconfig", "/flushdns");

            string netAdapter = "300Mbps Wireless High Power USB Adapter";  //Replace this with the adapter you wish to enable/disable
            Disable(netAdapter);
            Enable(netAdapter);
        }

        static void Enable(string interfaceName)
        {
            System.Diagnostics.ProcessStartInfo psi =
                   new System.Diagnostics.ProcessStartInfo("netsh", "interface set interface \"" + interfaceName + "\" enable");
            System.Diagnostics.Process p = new System.Diagnostics.Process();
            p.StartInfo = psi;
            p.Start();
        }

        static void Disable(string interfaceName)
        {
            System.Diagnostics.ProcessStartInfo psi =
                new System.Diagnostics.ProcessStartInfo("netsh", "interface set interface \"" + interfaceName + "\" disable");
            System.Diagnostics.Process p = new System.Diagnostics.Process();
            p.StartInfo = psi;
            p.Start();
        }

        #endregion        

        #region Check for internet connectivity
        //Method for checking if user is connected to internet, returns true if true
        private static bool CheckForInternetConnection()
        {
            try
            {
                using (var client = new WebClient())
                using (client.OpenRead("http://clients3.google.com/generate_204"))
                {
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }
        #endregion

        #region Class for the spinning loading animation

        //Class for handling the loading animations
        public class Spinner : IDisposable
        {
            private const string Sequence = @"/-\|";
            private int counter = 0;
            private readonly int left;
            private readonly int top;
            private readonly int delay;
            private bool active;
            private readonly Thread thread;

            public Spinner(int left, int top, int delay = 100)
            {
                this.left = left;
                this.top = top;
                this.delay = delay;
                thread = new Thread(Spin);
            }

            public void Start()
            {
                active = true;
                if (!thread.IsAlive)
                    thread.Start();
            }

            public void Stop()
            {
                active = false;
                Draw(' ');
            }

            private void Spin()
            {
                while (active)
                {
                    Turn();
                    Thread.Sleep(delay);
                }
            }

            private void Draw(char c)
            {
                Console.SetCursorPosition(left, top);             
                Console.Write(c);
            }

            private void Turn()
            {
                Draw(Sequence[++counter % Sequence.Length]);
            }

            public void Dispose()
            {
                Stop();
            }
        }
        #endregion

    }
}
