/**
* Author: Christopher Cola
* Created on 13/11/2015
*/

using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;

namespace MoboServerWPF
{
    class Program
    {
        private static Timer timer;
        // Tick interval, 16 is roughly 60fps
        private static int tickrate = 16;

        public static MainWindow window;

        public Program(MainWindow mainWindow)
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

            window = mainWindow;
            window.Append("Server running on port " + Network.Config.Port);
            window.Append("Tick interval = " + tickrate);
            window.Append("Waiting for clients");
        }

        private static void OnTimedEvent(object sender, ElapsedEventArgs e)
        {
            Network.Update();
            Player.Update();

            // Make sure a host is connected
            NetConnection host = Network.Server.Connections.Find(item => item.RemoteUniqueIdentifier == Network.host_uid);

            // If a host isn't present but there was a host
            if (host == null)
            {
                if (Player.players.Count > 0)
                {
                    // Make the player at the front of the players list host
                    Player player = Player.players.Values.First();
                    
                    player.host = true;
                    Network.host_uid = player.uid;
                    Network.out_message = Network.Server.CreateMessage();
                    Network.out_message.Write(Network.NEW_HOST);
                    Network.out_message.Write(player.uid);
                    Network.Server.SendMessage(Network.out_message, Network.Server.Connections, NetDeliveryMethod.ReliableOrdered, 0);

                    window.Append(string.Format("New host is {0} (UID:{1})", player.name, player.uid));
                }
                else
                {
                    // Reset the host, so that the next player who joins becomes host
                    Network.host_uid = -1;
                }
            }

            window.RefreshListBox();
        }
    }
}
