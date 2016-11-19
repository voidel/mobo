/**
* Author: Christopher Cola
* Created on 28/01/2016
*/

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System.Timers;
using Lidgren.Network;

namespace Mobo
{
    // Used to represent nodes in a Station
    class Node
    {
        // 'Grid' position of the node
        public Vector2 position;

        // Depth, used to determine StationNode type
        public int depth;

        // Associated StationNode
        public StationNode data;
        public HashSet<Node> children;
        public Node parent;
        public int id;

        // Delay when nodes get destroyed before they are removed
        Timer pipeDelay;

        // Delay when networked nodes get destroyed before removal
        Timer remoteDelay;

        Random rand = new Random(Guid.NewGuid().GetHashCode());

        public Node(int depth, Node parent)
        {
            id = rand.Next();
            this.parent = parent;
            this.depth = depth;
            children = new HashSet<Node>();

            pipeDelay = new Timer(200);
            pipeDelay.Elapsed += OnDelayOver;
            pipeDelay.AutoReset = false;

            remoteDelay = new Timer(200);
            remoteDelay.Elapsed += OnRemoteDelayOver;
            remoteDelay.AutoReset = false;
        }

        public void Remove(Node node)
        {
            children.Remove(node);
        }

        public void Add(Node node)
        {
            children.Add(node);
        }

        public int Count()
        {
            int count = 1;

            foreach (Node n in children)
            {
                count += n.Count();
            }

            return count;
        }

        public string Print(int depth)
        {
            string s = "\n";
            s = string.Concat(s, new string(' ', depth));
            s = string.Concat(s, depth.ToString());
            depth++;

            foreach (Node n in children)
            {
                s = string.Concat(s, n.Print(depth));
            }

            return s;
        }

        // Used when the local player destroys a node
        public void Destroy()
        {
            // Remove it only if it has no children and not already exploding
            if (children.Count == 0 && data.explosion == null)
            {
                if (Network.connected)
                {
                    // Send details about this health change to the server
                    Network.out_message = Network.Client.CreateMessage();
                    Network.out_message.Write(Network.SCORE);
                    Network.out_message.Write(Online.localPlayer.uid);
                    
                    switch(data.nodeType)
                    {
                        case StationNodeType.Core: Network.out_message.Write(100); break;
                        case StationNodeType.Pipe: Network.out_message.Write(5); break;
                        case StationNodeType.Turret: Network.out_message.Write(10); break;
                    }

                    Network.Client.SendMessage(Network.out_message, NetDeliveryMethod.ReliableOrdered); 
                }

                pipeDelay.Enabled = true;

                // Start exploding
                data.explosion = new AnimatedSprite(ContentStore.explosion, 12, true);
            }
        }

        // Used when a online player destroys a node
        public void RemoteDestroy()
        {
            // Remove it only if it has no children and not already exploding
            if (children.Count == 0 && data.explosion == null)
            {
                remoteDelay.Enabled = true;

                // Start exploding
                data.explosion = new AnimatedSprite(ContentStore.explosion, 12, true);
            }
        }

        private void OnRemoteDelayOver(object sender, ElapsedEventArgs e)
        {
            // Only if the flat list contains a node corresponding to this node
            if (data.station.flat_list.ContainsKey(id))
            {
                // Remove the node
                data.station.flat_list.Remove(id);
                if (data.nodeType == StationNodeType.Core)
                {
                    // If it was a core, destroy the whole station
                    data.station.spawner.Destroy(data.station);
                }
                else
                {
                    // If not the core, we remove this node from it's parent
                    parent.Remove(this);
                    if (parent != null)
                    {
                        if (parent.data != null)
                        {
                            // If the parent is a pipe, contine the line of destruction
                            if (parent.data.nodeType == StationNodeType.Pipe)
                            {
                                parent.RemoteDestroy();
                            }
                        }
                    }
                }
            }
        }

        // Networked version of the above method
        private void OnDelayOver(object sender, ElapsedEventArgs e)
        {
            // Only if the flat list contains a node corresponding to this node
            if (data.station.flat_list.ContainsKey(id))
            {
                try
                {
                    // Remove the node
                    data.station.flat_list.Remove(id);
                    if (Network.connected)
                    {
                        Network.out_message = Network.Client.CreateMessage();
                        Network.out_message.Write(Network.REMOVE_NODE);
                        Network.out_message.Write(data.station.id);
                        Network.out_message.Write(id);
                        Network.Client.SendMessage(Network.out_message, NetDeliveryMethod.ReliableOrdered);
                    }

                    if (data.nodeType == StationNodeType.Core)
                    {
                        if (Network.connected)
                        {
                            // Send details about this node removal to the server
                            Network.out_message = Network.Client.CreateMessage();
                            Network.out_message.Write(Network.REMOVE_STATION);
                            Network.out_message.Write(data.station.id);
                            Network.Client.SendMessage(Network.out_message, NetDeliveryMethod.ReliableOrdered);
                        }
                        else
                        {
                            data.station.spawner.Destroy(data.station);
                        }
                    }
                    else
                    {
                        // If not the core, we remove this node from it's parent
                        parent.Remove(this);
                        if (parent != null)
                        {
                            if (parent.data != null)
                            {
                                if (parent.data.nodeType == StationNodeType.Pipe)
                                {
                                    // If the parent is a pipe, contine the line of destruction
                                    parent.Destroy();
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    // Swallow for now.
                }
            }

        }
    }
}
