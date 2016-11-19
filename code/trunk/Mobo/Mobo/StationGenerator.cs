/**
* Author: Christopher Cola
* Created on 07/02/2016
*/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace Mobo
{
    // The StationGenerator tool found in the menu
    class StationGenerator
    {
        Background background;

        HashSet<Button> menuButtons = new HashSet<Button>();
        HashSet<Field> menuFields = new HashSet<Field>();

        Station station;

        int size = 10, difficulty = 5, branches = 2;

        public void Initialize()
        {
            // Load background
            background = new Background(ContentStore.bg5);

            menuButtons.Add(new Button("Back", ScreenManager.screenCenter + new Vector2(0, 192)));
            menuButtons.Add(new Button("Generate!", ScreenManager.screenCenter + new Vector2(240, 192)));

            menuFields.Add(new Field(3, ScreenManager.screenCenter + new Vector2(-304, 96)));
            menuButtons.Add(new Button("debug", ScreenManager.screenCenter + new Vector2(-200, 96), ContentStore.button_plus, true));

            menuFields.Add(new Field(0, ScreenManager.screenCenter + new Vector2(-304, 128)));
            menuButtons.Add(new Button("+size", ScreenManager.screenCenter + new Vector2(-200, 128), ContentStore.button_plus, true));
            menuButtons.Add(new Button("-size", ScreenManager.screenCenter + new Vector2(-170, 128), ContentStore.button_minus, true));

            menuFields.Add(new Field(1, ScreenManager.screenCenter + new Vector2(-304, 160)));
            menuButtons.Add(new Button("+diff", ScreenManager.screenCenter + new Vector2(-200, 160), ContentStore.button_plus, true));
            menuButtons.Add(new Button("-diff", ScreenManager.screenCenter + new Vector2(-170, 160), ContentStore.button_minus, true));

            menuFields.Add(new Field(2, ScreenManager.screenCenter + new Vector2(-304, 192)));
            menuButtons.Add(new Button("+branch", ScreenManager.screenCenter + new Vector2(-200, 192), ContentStore.button_plus, true));
            menuButtons.Add(new Button("-branch", ScreenManager.screenCenter + new Vector2(-170, 192), ContentStore.button_minus, true));

        }

        // Spawn a new station
        public void Generate()
        {
            // size, difficulty, max_branches
            station = new Station(ScreenManager.screenCenter, size, difficulty, branches, null);
        }

        public void Update()
        {
            bool anyMouseOver = false;

            foreach (Button button in menuButtons)
            {
                button.Update(ScreenManager.mouse);
                if (button.isMouseOver) anyMouseOver = true;
                if (button.isClicked)
                {
                    switch (button.name)
                    {
                        case "Back": ScreenManager.gameState = GameState.MainMenu; break;
                        case "Generate!": Generate(); break;
                        case "+size": size++; break;
                        case "-size": size--; break;
                        case "+diff": difficulty++; break;
                        case "-diff": difficulty--; break;
                        case "+branch": branches++; break;
                        case "-branch": branches--; break;
                        case "debug": SettingsManager.toggleDepths(); break;
                    }
                }
            }

            if (anyMouseOver) ScreenManager.cursorState = CursorState.Hand;
            else ScreenManager.cursorState = CursorState.Pointer;

            if(station != null)
                station.Update();

            KeyboardInput.HandleEscToMenu();
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            background.Draw(spriteBatch);

            if (station != null)
            {
                station.Draw(spriteBatch);

                if(SettingsManager.getShowDepths())
                {
                    spriteBatch.DrawString(ContentStore.generic, station.debug_size.ToString(), Vector2.Zero, Color.White);
                    spriteBatch.DrawString(ContentStore.generic, station.debug_string, Vector2.Zero, Color.White);
                }
            }

            // Draw buttons
            foreach (Button button in menuButtons)
            {
                button.Draw(spriteBatch);
            }

            // Draw fields
            foreach (Field field in menuFields)
            {
                switch (field.id)
                {
                    case 0: field.Draw(spriteBatch, "Size: " + size); break;
                    case 1: field.Draw(spriteBatch, "Difficulty: " + difficulty); break;
                    case 2: field.Draw(spriteBatch, "Max Branches: " + branches); break;
                    case 3: field.Draw(spriteBatch, "Show Depths: " + SettingsManager.getShowDepths().ToString()); break;
                }
            }
        }
    }
}
