/**
* Author: Christopher Cola
* Created on 14/12/2015
*/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Mobo
{
    // Display a minimap in game
    class Minimap
    {
        int height = 128;
        int width = 128;

        // Max range (pixels) that the minimap will display things
        float range = 2000.0f;

        // height and width of the dots on minimap
        int dot = 2;

        Rectangle map;

        Player local;

        // Lists used to update players and stations on the map.
        ConcurrentDictionary<long, Player> players;
        ConcurrentDictionary<int, Station> stations;

        public Minimap(bool online, ConcurrentDictionary<long, Player> players, ConcurrentDictionary<int, Station> stations)
        {
            this.players = players;
            this.stations = stations;

            // Used to draw the map in the top center of the screen
            map = new Rectangle(SettingsManager.getResolutionWidth() / 2 - width / 2, 0, width, height);

            // Select the write player whether offline or online for comparing distances
            if(online)
            {
                local = Online.localPlayer;
            }
            else
            {
                local = Offline.localPlayer;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(ContentStore.radar, map, Color.White);

            // Draw Stations
            DrawStations(stations, spriteBatch);

            // Draw Players
            DrawPlayers(players, spriteBatch);
        }

        private void DrawStations(ConcurrentDictionary<int, Station> stations, SpriteBatch spriteBatch)
        {
            foreach (Station s in stations.Values) // For each station
            {
                foreach (Node node in s.flat_list.Values) // For each individual node
                {
                    float dx = node.data.position.X - local.position.X; // Get the difference in x coord from the local player to node
                    float dy = node.data.position.Y - local.position.Y; // Get the difference in y coord from the local player to node

                    if ((dx > -range && dx < range) && (dy > -range && dy < range)) // If within range
                    {
                        int gridx = (int)((dx + 2000f) / 4000f * 128f); // Convert cartesian x to computer coords, then fit to minimap width
                        int gridy = (int)((dy + 2000f) / 4000f * 128f); // Convert cartesian y to computer coords, then fit to minimap height

                        // Draw dots relative to the top left of the minimap rectangle. Red because stations are evil :(
                        spriteBatch.Draw(ContentStore.debug, new Rectangle(map.Left + gridx - 1, gridy - 1, dot, dot), Color.Red);
                    }
                }
            }
        }

        private void DrawPlayers(ConcurrentDictionary<long, Player> players, SpriteBatch spriteBatch)
        {
            foreach (Player p in players.Values) // For each player
            {
                float dx = p.position.X - local.position.X; // Get the difference in x coord from the local player to this player
                float dy = p.position.Y - local.position.Y; // Get the difference in y coord from the local player to this player

                if ((dx > -range && dx < range) && (dy > -range && dy < range)) // If within range
                {
                    int gridx = (int)((dx + 2000f) / 4000f * 128f); // Convert cartesian x to computer coords, then fit to minimap width
                    int gridy = (int)((dy + 2000f) / 4000f * 128f); // Convert cartesian y to computer coords, then fit to minimap height

                    // Draw dots relative to the top left of the minimap rectangle. Cyan because players are nice :)
                    spriteBatch.Draw(ContentStore.debug, new Rectangle(map.Left + gridx - 1, gridy - 1, dot, dot), Color.Cyan);
                }
            }
        }
    }
}
