using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace VRCTools
{
    public abstract class VRCToolsLogger
    {
        
        [DllImport("kernel32.dll")]
        internal static extern int AllocConsole();

        [DllImport("kernel32.dll")]
        public static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        public static void Init(bool showConsole)
        {
            if (showConsole)
            {
                AllocConsole();
                Console.Title = "VRChat Toolpack by Slaynash";
                Console.SetOut(new StreamWriter(Console.OpenStandardOutput())
                {
                    AutoFlush = true
                });
                Console.SetIn(new StreamReader(Console.OpenStandardInput()));
                AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
                Console.WriteLine("VRChat Toolpack by Slaynash");
            }
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Console.WriteLine(((Exception)e.ExceptionObject).Message);
        }

        public static void Info(string str)
        {
            String tmp = " [VRCTools] " + str;
            UnityEngine.Debug.Log(tmp);
            Console.WriteLine("[Info] " + tmp);
        }

        public static void Warn(string str)
        {
            String tmp = " [VRCTools] " + str;
            UnityEngine.Debug.LogWarning(tmp);
            Console.WriteLine("[Warn] " + tmp);
        }

        public static void Error(string str)
        {
            String tmp = " [VRCTools] " + str;
            UnityEngine.Debug.LogError(tmp);
            Console.WriteLine("[Error] " + tmp);
        }
    }
}
