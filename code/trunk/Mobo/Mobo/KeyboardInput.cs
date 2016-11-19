/**
* Author: Christopher Cola
* Created on 11/11/2015
*/

using Microsoft.Xna.Framework.Input;

namespace Mobo
{
    // Used to handle keyboard and mouse clicks
    class KeyboardInput
    {
        static KeyboardState keyState;
        static MouseState mouseState;

        // Only used to switch the player back to mouseRotation when keys are released
        public static bool keyPressed;

        public static void HandleEscToMenu()
        {
            // Obtain a recent state of mouse and keyboard
            keyState = ScreenManager.keyboard;
            mouseState = ScreenManager.mouse;

            // Exit to main in Esc pressed
            if (keyState.IsKeyDown(Keys.Escape))
            {
                ScreenManager.gameState = GameState.MainMenu;
            }
        }

        // Set variables in local player that relate to keyboard/mouse inputs
        public static void HandleMovementInput(Player p)
        {
            // Obtain a recent state of mouse and keyboard
            keyState = ScreenManager.keyboard;
            mouseState = ScreenManager.mouse;

            keyPressed = false;

            if (keyState.IsKeyDown(Keys.Up) || keyState.IsKeyDown(Keys.W))
            {
                p.m_up = true;
                keyPressed = true;
            }
            if (keyState.IsKeyDown(Keys.Down) || keyState.IsKeyDown(Keys.S))
            {
                p.m_down = true;
                keyPressed = true;
            }
            if (keyState.IsKeyDown(Keys.Left) || keyState.IsKeyDown(Keys.A))
            {
                p.m_left = true;
                keyPressed = true;
            }
            if (keyState.IsKeyDown(Keys.Right) || keyState.IsKeyDown(Keys.D))
            {
                p.m_right = true;
                keyPressed = true;
            }
            if (mouseState.LeftButton == ButtonState.Pressed)
            {
                p.m_shoot = true;
            }
        }
    }
}
