/**
* Author: Christopher Cola
* Created on 19/04/2016
*/

using Lidgren.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Mobo
{

    // Manages the number, spawning, and drawing of stations
    class StationSpawner
    {
        public ConcurrentDictionary<int, Station> stations = new ConcurrentDictionary<int, Station>();

        Random rand;

        // Possible positions stations can be spawned (there are only 8)
        public List<Vector2> spawnPositions = new List<Vector2>();

        // Determines whether a spawn spot is free
        public bool[] availableSpot = new bool[8];

        public int max_stations;

        public StationSpawner(bool online, int max_stations)
        {
            this.max_stations = max_stations;

            //string s = new StationToXML().ToXML((stations[0]));
            //File.WriteAllText(s.GetHashCode().ToString() + ".txt",s);
            //stations.Add(new StationFromXML().FromXML(s));
            //stations[1].position = new Vector2(600, 200);

            for(int i = -720; i <= 720; i += 720)
            {
                for(int j = -720; j <= 720; j += 720)
                {
                    if (i != 0 || j != 0)
                    {
                        spawnPositions.Add(new Vector2(i, j)); 
                    }
                }
            }

            // Add to a cache for debug purposes
            foreach (Station s in stations.Values)
            {
                if (!File.Exists("stationcache"))
                {
                    Directory.CreateDirectory("stationcache");
                }
                File.WriteAllText("stationcache/" + s.GetHashCode().ToString() + ".txt", new StationToXML().ToXML(s));
            }
        }

        public void NewStation()
        {
            rand = new Random(Guid.NewGuid().GetHashCode());
            int randPos = rand.Next(0, spawnPositions.Count);
            Vector2 position = spawnPositions[randPos];
            if(!availableSpot[spawnPositions.IndexOf(position)])
            {
                Station s = new Station(spawnPositions[randPos], 15, 5, 2, this);
                stations.TryAdd(s.id, s);
                availableSpot[spawnPositions.IndexOf(position)] = true;

                if (Network.connected)
                {
                    // Send a station to the server
                    Network.out_message = Network.Client.CreateMessage();
                    Network.out_message.Write(Network.CREATE_STATION);
                    Network.out_message.Write(Network.Client.UniqueIdentifier);
                    Network.out_message.Write(s.id);
                    Network.out_message.Write(new StationToXML().ToXML(s));
                    Network.Client.SendMessage(Network.out_message, NetDeliveryMethod.ReliableOrdered);
                }
            }
            else
            {
                NewStation();
            }
        }

        public void Destroy(Station station)
        {
            availableSpot[spawnPositions.IndexOf(station.position)] = false;
            stations.Remove(station.id);
        }

        public void Update()
        {
            foreach (Station station in stations.Values)
            {
                station.Update();
            }

            if(stations.Count < max_stations)
            {
                ScreenManager.messageList.Add("A new station has appeared!", MessageType.GameHint);
                NewStation();
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (Station station in stations.Values)
            {
                station.Draw(spriteBatch);
            }
        }
    }
}
