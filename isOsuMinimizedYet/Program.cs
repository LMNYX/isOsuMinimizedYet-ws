using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace isOsuMinimizedYet
{

    public class Minimism : WebSocketBehavior
    {
    }

    internal class Program
    {
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetWindowPlacement(IntPtr hWnd, ref WINDOWPLACEMENT lpwndpl);

        private struct WINDOWPLACEMENT
        {
            public int length;
            public int flags;
            public int showCmd;
        }


        static void Main(string[] args)
        {
            Process[] osuproc = Process.GetProcessesByName("osu!");
            while (osuproc.Length < 1)
                Thread.Sleep(100);

            var wssv = new WebSocketServer("ws://localhost:6969");
            wssv.AddWebSocketService<Minimism>("/ws");
            wssv.Start();

            while (true)
            {
                osuproc = Process.GetProcessesByName("osu!");
                if (osuproc.Length < 1)
                {
                    wssv.WebSocketServices.Broadcast("2");
                    continue;
                }
                var handle = osuproc[0].MainWindowHandle;

                if (handle == IntPtr.Zero)
                    continue;

                WINDOWPLACEMENT placement = new WINDOWPLACEMENT();
                GetWindowPlacement(handle, ref placement);
                wssv.WebSocketServices.Broadcast(placement.showCmd.ToString());
                Thread.Sleep(100);
            }
            Console.WriteLine("we die");
            Console.ReadKey();
        }
    }
}
