/**
* Author: Christopher Cola
* Created on 13/11/2015
*/

using Lidgren.Network;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace MoboServerWPF
{
    class Player
    {
        public static ConcurrentDictionary<long, Player> players = new ConcurrentDictionary<long, Player>();

        public long uid;
        public string name { get; set; }
        public int x;
        public int y;
        public float rotation;
        public bool host;

        public int health = 100;
        public int score;

        public int timeout;

        public Player(string name, int x, int y, int timeout, long uid)
        {
            this.name = name;
            this.x = x;
            this.y = y;
            this.timeout = timeout;
            this.uid = uid;
        }

        public static void Update()
        {
            // Only send out positions if clients connected matches number of Player objects
            if (Network.Server.ConnectionsCount == players.Count)
            {
                foreach(Player player in players.Values)
                {
                    player.timeout++;

                    // Send positions of all players to all clients
                    Network.out_message = Network.Server.CreateMessage();

                    Network.out_message.Write(Network.MOVE);
                    Network.out_message.Write(player.uid);
                    Network.out_message.Write(player.x);
                    Network.out_message.Write(player.y);
                    Network.out_message.Write(player.rotation);

                    // Send it unreliably but quickly, since we're not too bothered if it's not recieved this time
                    Network.Server.SendMessage(Network.out_message, Network.Server.Connections, NetDeliveryMethod.Unreliable, 0);

                    // Time players out if it's been more than x ticks since a player reported their positon
                    if (player.timeout > 180)
                    {
                        // Use a lamda expression to find the player and connection with this uid
                        NetConnection toFindConnection = Network.Server.Connections.Find(item => item.RemoteUniqueIdentifier == player.uid);

                        toFindConnection.Disconnect("bye");

                        // Allow some time to disconnect
                        System.Threading.Thread.Sleep(100);

                        if (Network.Server.ConnectionsCount != 0)
                        {
                            Network.out_message = Network.Server.CreateMessage();
                            Network.out_message.Write(Network.DISCONNECT);
                            Network.out_message.Write(player.uid);

                            Network.Server.SendMessage(Network.out_message, Network.Server.Connections, NetDeliveryMethod.ReliableOrdered, 0);
                        }

                        players.Remove(player.uid);
                        Program.window.Append(string.Format("{0} (UID:{1}) timed out.", player.name, player.uid));
                    }
                }
            }
        }
    }
}
