/**
* Author: Christopher Cola
* Created on 10/11/2015
*/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace Mobo
{
    // The help screen found on the main menu
    class Help
    {
        Background background;

        HashSet<Button> menuButtons = new HashSet<Button>();

        public void Initialize()
        {
            // Load background
            background = new Background(ContentStore.bg5);

            menuButtons.Add(new Button("Back", ScreenManager.screenCenter + new Vector2(0, 192)));
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
                    // Decided what to do based on the button clicked
                    switch (button.name)
                    {
                        case "Back": ScreenManager.gameState = GameState.MainMenu; break;
                    }
                }
            }

            // Change the cursor if it's hovering over a button
            if (anyMouseOver) ScreenManager.cursorState = CursorState.Hand;
            else ScreenManager.cursorState = CursorState.Pointer;

            KeyboardInput.HandleEscToMenu();
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            background.Draw(spriteBatch);

            // Draw buttons
            foreach (Button button in menuButtons)
            {
                button.Draw(spriteBatch);
            }

            spriteBatch.Draw(ContentStore.help, MoboUtils.textureOrigin(ContentStore.help), Color.White);
        }
    }
}
