using Salaros.Configuration;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace MythicalLauncher
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        public static string version = "1.0.7";
        public static string debugmode;
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
                debugmode = "true";
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
            if (args.Contains("-update"))
            {
                AttachConsole(ATTACH_PARENT_PROCESS);
                Console.Title = "MythicalLauncher | CLI";
                Console.WriteLine("@echo off");
                Console.Clear();
                Console.WriteLine("MythicalLauncher version " + version + " by MythicalSystems");
                Console.WriteLine("");
                askUpdate();
                Boolean checkUpdate()
                {
                    Console.WriteLine("[{0:HH:mm:ss}] [UPDATER] Please wait while we check for updates", DateTime.Now);
                    Boolean versionStatu;
                    try
                    {
                        WebClient client = new WebClient();
                        var appcfg = new ConfigParser(Application.StartupPath + @"\settings.ini");
                        var r_key = appcfg.GetValue("RemoteLauncher", "key");
                        Stream str = client.OpenRead(r_key + "/api/mythicallauncher/settings/update.php?version=" + Program.version);
                        StreamReader reader = new StreamReader(str);
                        String content = reader.ReadToEnd();
                        versionStatu = (content == "GETUPDATE") ? versionStatu = true : versionStatu = false;
                    }
                    catch { versionStatu = false; }
                    return versionStatu;

                }
                void askUpdate()
                {

                    if (checkUpdate())
                    {
                        Console.WriteLine("[{0:HH:mm:ss}] [UPDATER] Update found!", DateTime.Now);
                        DialogResult dr = MessageBox.Show("New update is available. \n\r Would you like to install it now?", "Update found", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                        if (dr == DialogResult.Yes)
                        {
                            getUpdate();
                        }
                        if (dr == DialogResult.No)
                        {
                            Application.Exit();
                        }
                    }
                    else
                    {
                        Console.WriteLine("[{0:HH:mm:ss}] [UPDATER] No updates found", DateTime.Now);
                    }
                }

                void getUpdate()
                {
                    Application.Run(new FrmUpdate());
                }
               
                Console.WriteLine("Press any key to continue!");
                return;
            }
            if (args.Contains("-fix-missing"))
            {
                AttachConsole(ATTACH_PARENT_PROCESS);
                Console.Title = "MythicalLauncher | CLI";
                Console.WriteLine("@echo off");
                Console.Clear();
                Console.WriteLine("Welcome to MythicalLauncher version " + version);
                Console.WriteLine("");
                if (File.Exists("version"))
                {
                    Console.WriteLine("[{0:HH:mm:ss}] [REGEN] Nothing to do file called 'verion' exist", DateTime.Now);                   
                }
                else
                {
                    try
                    {
                        using (StreamWriter sw = File.CreateText("version"))
                        {
                            sw.WriteLine(version);
                            Console.WriteLine("[{0:HH:mm:ss}] [REGEN] We created the file called 'version'!", DateTime.Now);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("[{0:HH:mm:ss}] [REGEN] " + ex.Message, DateTime.Now);
                    }
                }
                if (File.Exists("buildnumber"))
                {
                    Console.WriteLine("[{0:HH:mm:ss}] [REGEN] Nothing to do file called 'buildnumber' exist", DateTime.Now);                                       
                }
                else
                {
                    try
                    {
                        using (StreamWriter sw = File.CreateText("buildnumber"))
                        {
                            sw.WriteLine("Unknown");
                            Console.WriteLine("[{0:HH:mm:ss}] [REGEN] We created the file called 'buildnumber'!", DateTime.Now);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("[{0:HH:mm:ss}] [REGEN] " + ex.Message, DateTime.Now);
                    }
                }
                Console.WriteLine("Press any key to continue!");
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
                Console.WriteLine("-fix-missing | This is a fix if the files are missing");
                Console.WriteLine("-update | This updates to the last version from github");
                Console.WriteLine("");
                Console.WriteLine("Press any key to continue!");
                return;
            }
            debugmode = "true";
            Application.Run(new FrmLoading());
        }
    }
}
