/**
 * Author: Christopher Cola
 * Created on 25/10/2015
 */

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Mobo
{
    class MainMenu
    {

        Background background;
        HashSet<Button> menuButtons = new HashSet<Button>();

        public void Initialize()
        {
            // Load background
            background = new Background(ContentStore.bg1);

            // Load gui parts
            menuButtons.Add(new Button("New Game", ScreenManager.screenCenter + new Vector2(0, 32)));
            menuButtons.Add(new Button("Join Server", ScreenManager.screenCenter + new Vector2(0, 64)));
            menuButtons.Add(new Button("Settings", ScreenManager.screenCenter + new Vector2(0, 96)));
            menuButtons.Add(new Button("Help", ScreenManager.screenCenter + new Vector2(0, 128)));
            menuButtons.Add(new Button("Exit", ScreenManager.screenCenter + new Vector2(0, 160)));
            menuButtons.Add(new Button("Station Generator", ScreenManager.screenCenter + new Vector2(240, 160)));
        }

        public void Update()
        {
            bool anyMouseOver = false;

            foreach (Button button in menuButtons)
            {
                button.Update(ScreenManager.mouse);
                if (button.isMouseOver) anyMouseOver = true;
                if (button.justClicked)
                {
                    // Decided what to do based on the button clicked
                    switch (button.name)
                    {
                        case "New Game": ScreenManager.gameState = GameState.Offline; break;
                        case "Join Server": ScreenManager.gameState = GameState.Online; break;
                        case "Settings": ScreenManager.gameState = GameState.Settings; break;
                        case "Help": ScreenManager.gameState = GameState.Help; break;
                        case "Exit": ScreenManager.gameState = GameState.Exiting; break;
                        case "Station Generator": ScreenManager.gameState = GameState.StationGenerator; break;
                    }
                }
            }

            // Change the cursor if it's hovering over a button
            if (anyMouseOver) ScreenManager.cursorState = CursorState.Hand;
            else ScreenManager.cursorState = CursorState.Pointer;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            // Draw background
            background.Draw(spriteBatch);

            // Draw logo in middle of screen
            spriteBatch.Draw(ContentStore.logo, MoboUtils.textureOrigin(ContentStore.logo) - new Vector2(0,64), Color.White);

            // Draw buttons
            foreach (Button button in menuButtons)
            {
                button.Draw(spriteBatch);
            }
        }
    }
}
