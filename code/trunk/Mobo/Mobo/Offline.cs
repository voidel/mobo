/**
* Author: Christopher Cola
* Created on 25/10/2015
*/

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System;
using System.IO;
using Lidgren.Network;
using System.Linq;
using System.Collections.Concurrent;

namespace Mobo
{
    class Offline
    {
        Background background;

        Minimap minimap;

        StationSpawner spawner;

        public static ConcurrentDictionary<long, Player> players = new ConcurrentDictionary<long, Player>();

        public static Player localPlayer;

        public void Initialize()
        {
            // Disconnect if we are connected online
            if(Network.connected)
            {
                Network.connected = false;
            }

            // Create Station Spawner
            spawner = new StationSpawner(false, 3);

            // Load background
            background = new Background(ContentStore.bg7);

            // Create a new controllable player
            localPlayer = new Player(Vector2.Zero, SettingsManager.getUsername(), 100, true, 0);
            players.TryAdd(localPlayer.uid, localPlayer);

            // Create a minimap
            minimap = new Minimap(false, players, spawner.stations);
        }

        public void Update()
        {
            // Update player position
            foreach (Player player in players.Values)
            {
                // Update player positions
                player.Update();
            }

            spawner.Update();

            KeyboardInput.HandleEscToMenu();
            ScreenManager.cursorState = CursorState.Crosshair;
        }

        public void Draw(SpriteBatch staticSpriteBatch, SpriteBatch cameraSpriteBatch)
        {
            //Draw background
            background.Draw(cameraSpriteBatch);

            foreach (Player player in players.Values)
            {
                // Draw players
                player.Draw(staticSpriteBatch, cameraSpriteBatch);
            }

            // Draw stations
            spawner.Draw(cameraSpriteBatch);

            // Draw minimap
            minimap.Draw(staticSpriteBatch);
        }
    }
}