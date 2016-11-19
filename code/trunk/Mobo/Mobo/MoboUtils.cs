/**
 * Author: Christopher Cola
 * Created on 28/10/2015
 */

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Mobo
{
    // Class that contains commonly used functions that can be accessed anywhere
    class MoboUtils
    {
        // Obtain the origin for any length of text for drawing centered text
        public static Vector2 textOrigin(string str, SpriteFont font)
        {
            return new Vector2((int)font.MeasureString(str).X / 2, -(int)font.MeasureString(str).Y / 2);
        }

        // Obtain the origin for any texture for drawing graphics centered
        public static Vector2 textureOrigin(Texture2D texture)
        {
            return ScreenManager.screenCenter - new Vector2(texture.Width / 2, texture.Height / 2);
        }
    }
}
