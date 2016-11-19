/**
* Author: Christopher Cola
* Created on 10/11/2015
*/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Mobo
{
    class Online
    {
        // Tracks whether the onFirstRun method has already been run
        bool firstRun;

        public static ConcurrentDictionary<long, Player> players = new ConcurrentDictionary<long, Player>();

        Background background;

        Minimap minimap;

        public static StationSpawner spawner;

        public static Player localPlayer;

        public void Initialize()
        {
            firstRun = true;

            // Load background
            background = new Background(ContentStore.bg7);

            spawner = new StationSpawner(true, 0);
        }

        private void onFirstRun()
        {
            Network.Initialize();
            firstRun = false;

            // Create a new controllable player and add it to the list
            localPlayer = new Player(Vector2.Zero, SettingsManager.getUsername(), 100, true, Network.Client.UniqueIdentifier);
            players.TryAdd(localPlayer.uid, localPlayer);

            // Create a minimap
            minimap = new Minimap(true, players, spawner.stations);
        }

        public void Update()
        {
            if (firstRun) onFirstRun();

            if (Network.connected)
            {
                Network.Update();

                foreach (Player player in players.Values)
                {
                    // Update player positions
                    player.Update();
                }

                spawner.Update();
            }
            else
            {
                ScreenManager.gameState = GameState.MainMenu;
                firstRun = true;
            }

            KeyboardInput.HandleEscToMenu();
            ScreenManager.cursorState = CursorState.Crosshair;
        }

        public void Draw(SpriteBatch staticSpriteBatch, SpriteBatch cameraSpriteBatch)
        {
            // If the Network is starting display some basic connecting text
            if (firstRun)
            {
                Vector2 textOrigin = MoboUtils.textOrigin("Connecting...", ContentStore.generic);
                staticSpriteBatch.DrawString(ContentStore.generic, "Connecting...", ScreenManager.screenCenter, Color.White, 0.0f, textOrigin, 1f, SpriteEffects.None, 0.0f);
            }
            else
            {
                //Draw background
                background.Draw(cameraSpriteBatch);

                int i = 0;

                foreach (Player player in players.Values)
                {
                    foreach (Projectile projectile in player.projectiles.Values)
                    {
                        if(projectile != null)
                        {
                            projectile.Draw(cameraSpriteBatch);
                        }
                    }

                    player.Draw(staticSpriteBatch, cameraSpriteBatch);

                    // Print players for HUD
                    string name = player.name + " (" + player.score + ")";
                    int rightAlign2 = (int)ContentStore.generic.MeasureString(name).X;
                    staticSpriteBatch.DrawString(ContentStore.generic, name, new Vector2(SettingsManager.getResolutionWidth() - rightAlign2 - 4,(i+1)*16), Color.White, 0.0f, Vector2.Zero, 1f, SpriteEffects.None, 0.0f);
                    i++;
                }

                // Draw station
                spawner.Draw(cameraSpriteBatch);

                // Draw all players in the top right
                string str = "Players";
                int rightAlign = (int)ContentStore.generic.MeasureString(str).X;
                staticSpriteBatch.DrawString(ContentStore.generic, str, new Vector2(SettingsManager.getResolutionWidth() - rightAlign - 4, 0), Color.White, 0.0f, Vector2.Zero, 1f, SpriteEffects.None, 0.0f);

                // Draw minimap
                minimap.Draw(staticSpriteBatch);
            }
        }
    }
}
