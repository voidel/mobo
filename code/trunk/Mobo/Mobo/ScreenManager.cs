/**
 * Author: Christopher Cola
 * Created on 25/10/2015
 */

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Mobo
{
    // A class that allows access of common screen related variables anywhere in the program
    class ScreenManager
    {
        public static Vector2 screenCenter;

        public static GameState gameState;
        public static CursorState cursorState;

        public static MouseState mouse;
        public static KeyboardState keyboard;
        public static Vector2 cursorPos;

        public static MessageList messageList;
    }
}
