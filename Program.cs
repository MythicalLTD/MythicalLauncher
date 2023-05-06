using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace MythicalLauncher
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        public static string version = "1.0.5";
        public static string debugmdoe;
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool AttachConsole(int dwProcessId);
        private const int ATTACH_PARENT_PROCESS = -1;



        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);


            if (args.Contains("-version"))
            {
                AttachConsole(ATTACH_PARENT_PROCESS);
                Console.Title = "MythicalLauncher | CLI";
                Console.WriteLine("@echo off");
                Console.Clear();
                Console.WriteLine("MythicalLauncher version " + version + " by MythicalSystems");
                Console.WriteLine("");
                return;
            }
            if (args.Contains("-debug"))
            {
                AttachConsole(ATTACH_PARENT_PROCESS);
                Console.Title = "MythicalLauncher | CLI";
                Console.WriteLine("@echo off");
                Console.Clear();
                Console.WriteLine("MythicalLauncher version " + version + " by MythicalSystems");
                Console.WriteLine("");
                Console.WriteLine("DEBUG MODE ACTIVE");
                Console.WriteLine("Please do not use this mode if you are not a developer");
                debugmdoe = "true";
                try
                {
                    Application.Run(new FrmLoading());
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Console.WriteLine("An error occurred: " + ex.Message);
                }
                return;
            }
            if (args.Contains("-help"))
            {
                AttachConsole(ATTACH_PARENT_PROCESS);
                Console.Title = "MythicalLauncher | CLI";
                Console.WriteLine("@echo off");
                Console.Clear();
                Console.WriteLine("Welcome to MythicalLauncher version " + version);
                Console.WriteLine("");
                Console.WriteLine("Thanks for using MythicalLauncher CLI here are the commands that may help you");
                Console.WriteLine("");
                Console.WriteLine("-help | Shows this promot");
                Console.WriteLine("-version | Shows the version of the application");
                Console.WriteLine("-debug | Shows the debug console for problems");
                Console.WriteLine("-build | SOON");
                Console.WriteLine("");
                Console.WriteLine("Press any key to continue!");
                return;
            }
            debugmdoe = "true";
            Application.Run(new FrmLoading());
        }
    }
}
