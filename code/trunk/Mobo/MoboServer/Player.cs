/**
* Author: Christopher Cola
* Created on 13/11/2015
*/

using Lidgren.Network;
using System;
using System.Collections.Generic;

namespace MoboServer
{
    class Player
    {
        public static List<Player> players = new List<Player>();

        public long uid;
        public string name;
        public int x;
        public int y;
        public float rotation;

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
                for(int i = 0; i < players.Count; i++)
                {
                    players[i].timeout++;

                    // Send positions of all players to all clients
                    Network.out_message = Network.Server.CreateMessage();

                    Network.out_message.Write(Network.MOVE);
                    Network.out_message.Write(players[i].uid);
                    Network.out_message.Write(players[i].x);
                    Network.out_message.Write(players[i].y);
                    Network.out_message.Write(players[i].rotation);

                    // Send it unreliably but quickly, since we're not too bothered if it's not recieved this time
                    Network.Server.SendMessage(Network.out_message, Network.Server.Connections, NetDeliveryMethod.Unreliable, 0);

                    // Time players out if it's been more than x ticks since a player reported their positon
                    if (players[i].timeout > 180)
                    {

                        if(Network.Server.ConnectionsCount != 0)
                        {
                            Network.out_message = Network.Server.CreateMessage();

                            Network.out_message.Write(Network.DISCONNECT);
                            Network.out_message.Write(players[i].uid);

                            Network.Server.SendMessage(Network.out_message, Network.Server.Connections, NetDeliveryMethod.ReliableOrdered, 0);
                        }

                        // Disconnect the player
                        Network.Server.Connections[i].Disconnect("bye");

                        Program.log.Add(players[i].name + " left. (timed out)");

                        // Allow some time to disconnect
                        System.Threading.Thread.Sleep(100);

                        players.RemoveAt(i);
                        i--;
                        break;
                    }
                }
            }
        }
    }
}
