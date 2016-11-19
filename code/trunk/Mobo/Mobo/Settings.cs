/**
 * Author: Christopher Cola
 * Created on 25/10/2015
 */

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Diagnostics;

namespace Mobo
{
    class Settings
    {
        HashSet<Button> menuButtons = new HashSet<Button>();
        HashSet<Field> menuFields = new HashSet<Field>();

        Process editor;
        bool editorOpen = false;

        Background background;

        public void Initialize()
        {
            // Load background
            background = new Background(ContentStore.bg1);

            // Load gui parts
            menuButtons.Add(new Button("Username", ScreenManager.screenCenter + new Vector2(-85, -192)));
            menuButtons.Add(new Button("Difficulty", ScreenManager.screenCenter + new Vector2(-85, -160)));
            menuButtons.Add(new Button("Resolution", ScreenManager.screenCenter + new Vector2(-85, -128)));
            menuButtons.Add(new Button("Option4", ScreenManager.screenCenter + new Vector2(-85, -96)));
            menuButtons.Add(new Button("Option5", ScreenManager.screenCenter + new Vector2(-85, -64)));
            menuButtons.Add(new Button("Option6", ScreenManager.screenCenter + new Vector2(-85, -32)));
            menuButtons.Add(new Button("Option7", ScreenManager.screenCenter + new Vector2(-85, 0)));
            menuButtons.Add(new Button("Option8", ScreenManager.screenCenter + new Vector2(-85, 32)));
            menuButtons.Add(new Button("Show Debug", ScreenManager.screenCenter + new Vector2(-85, 64)));
            menuButtons.Add(new Button("Show Bounds", ScreenManager.screenCenter + new Vector2(-85, 96)));
            menuButtons.Add(new Button("Show Depths", ScreenManager.screenCenter + new Vector2(-85, 128)));
            menuButtons.Add(new Button("Back", ScreenManager.screenCenter + new Vector2(0, 192)));

            menuFields.Add(new Field(0, ScreenManager.screenCenter + new Vector2(85, -192)));
            menuFields.Add(new Field(1, ScreenManager.screenCenter + new Vector2(85, -160)));
            menuFields.Add(new Field(2, ScreenManager.screenCenter + new Vector2(85, -128)));
            menuFields.Add(new Field(3, ScreenManager.screenCenter + new Vector2(85, -96)));
            menuFields.Add(new Field(4, ScreenManager.screenCenter + new Vector2(85, -64)));
            menuFields.Add(new Field(5, ScreenManager.screenCenter + new Vector2(85, -32)));
            menuFields.Add(new Field(6, ScreenManager.screenCenter + new Vector2(85, 0)));
            menuFields.Add(new Field(7, ScreenManager.screenCenter + new Vector2(85, 32)));
            menuFields.Add(new Field(8, ScreenManager.screenCenter + new Vector2(85, 64)));
            menuFields.Add(new Field(9, ScreenManager.screenCenter + new Vector2(85, 96)));
            menuFields.Add(new Field(10, ScreenManager.screenCenter + new Vector2(85, 128)));
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
                        case "Username":
                        case "Resolution":
                        case "Show Debug":
                        case "Show Bounds":
                        case "Show Depths":
                            {
                                if (!editorOpen)
                                {
                                    editorOpen = true;
                                    editor = Process.Start("notepad.exe", "MoboSettings.xml");
                                }
                                break;
                            }
                        case "Difficulty":
                            {
                                SettingsManager.toggleDifficulty();
                                break;
                            }
                        case "Back": ScreenManager.gameState = GameState.MainMenu; break;
                    }
                }
            }

            if (anyMouseOver) ScreenManager.cursorState = CursorState.Hand;
            else ScreenManager.cursorState = CursorState.Pointer;

            if (editor != null && editor.HasExited)
            {
                editorOpen = false;
                SettingsManager.Load();
            }

            KeyboardInput.HandleEscToMenu();
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            // Draw background
            background.Draw(spriteBatch);

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
                    case 0: field.Draw(spriteBatch, SettingsManager.getUsername()); break;
                    case 1: field.Draw(spriteBatch, SettingsManager.getVerboseDifficulty()); break;
                    case 2: field.Draw(spriteBatch, SettingsManager.getResolutionWidth() + "x" + SettingsManager.getResolutionHeight()); break;
                    case 8: field.Draw(spriteBatch, SettingsManager.getShowDebug().ToString()); break;
                    case 9: field.Draw(spriteBatch, SettingsManager.getShowBounds().ToString()); break;
                    case 10: field.Draw(spriteBatch, SettingsManager.getShowDepths().ToString()); break;
                    default: field.Draw(spriteBatch, "No value"); break;
                }
            }
        }
    }
}
