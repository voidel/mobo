/**
* Author: Christopher Cola
* Created on 13/11/2015
*/

using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Timers;

namespace MoboServer
{
    class Program
    {
        private static Timer timer;
        // Tick interval, 16 is roughly 60fps
        private static int tickrate = 16;
        // A small visualisation of the tickrate
        private static int tick = 1;
        // A log list
        public static List<string> log = new List<string>();

        static void Main(string[] args)
        {
            // Network config
            Network.Config = new NetPeerConfiguration("Mobo");
            // Port
            Network.Config.Port = 14243;
            // Start the server
            Network.Server = new NetServer(Network.Config);
            Network.Server.Start();

            // Set up a timer to perform the messaging at a certain pace
            timer = new Timer(tickrate);
            timer.Elapsed += OnTimedEvent;
            timer.Enabled = true;

            Console.ReadLine();
        }

        private static void OnTimedEvent(object sender, ElapsedEventArgs e)
        {
            Network.Update();
            Player.Update();
            DrawConsole();
            tick = tick >= 79 ? 1 : ++tick;
        }

        private static void DrawConsole()
        {
            Console.Clear();
            Console.WriteLine(string.Format("Server running on port {0} | Tick interval = {1} | Waiting for clients",Network.Config.Port,tickrate));
            Console.WriteLine(new string('*', tick));
            Console.WriteLine(string.Format("Number of connected players = {0}",Player.players.Count));
            Console.WriteLine("Press the Enter key to stop the server at any time.");
            Console.WriteLine();

            foreach (Player player in Player.players)
            {
                Console.WriteLine(string.Format("{0} - {1} - ({2},{3}) - TO: {4}", player.name, player.uid, player.x, player.y, player.timeout));
            }

            Console.WriteLine();

            WriteLog();
        }

        private static void WriteLog()
        {
            if (log.Count > 20) log.RemoveAt(0);
            foreach (string String in log)
            {
                Console.WriteLine(String);
            }
        }
    }
}
