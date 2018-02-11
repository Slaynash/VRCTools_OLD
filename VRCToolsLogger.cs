using System;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;

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

        private static bool errorOccured = false;

        public static void Init(bool showConsole)
        {
            if (showConsole)
            {
                AllocConsole();
                Console.Title = "VRCTools by Slaynash";
                Console.SetOut(new StreamWriter(Console.OpenStandardOutput())
                {
                    AutoFlush = true
                });
                Console.SetIn(new StreamReader(Console.OpenStandardInput()));
                AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
                Console.WriteLine("VRCTools by Slaynash");
            }
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Error(((Exception)e.ExceptionObject).Message);
        }

        public static void Info(string str)
        {
            String tmp = " [VRCTools] " + str;
            Debug.Log(tmp);
            Console.WriteLine("[Info] " + tmp);
        }

        public static void Warn(string str)
        {
            String tmp = " [VRCTools] " + str;
            Debug.LogWarning(tmp);
            Console.WriteLine("[Warn] " + tmp);
        }

        public static void Update()
        {
            if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.K))
            {
                errorOccured = false;
            }
        }

        public static int OnGUI(int padding)
        {
            if (errorOccured)
            {
                GUI.color = Color.red;
                GUI.Label(new Rect(0, Screen.height - 20 - padding, Screen.width, 20), "VRCTools: An error has occured (Press CTRL+K to hide/show)");
                return 20;
            }
            return 0;
        }

        public static void Error(string str)
        {
            String tmp = " [VRCTools] " + str;
            Debug.LogError(tmp);
            Console.WriteLine("[Error] " + tmp);
            errorOccured = true;
        }
    }
}
