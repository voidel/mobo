/**
 * Author: Christopher Cola
 * Created on 04/11/2015
 */

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Mobo
{
    // This class provides a background for any menu class that requires one
    class Background
    {
        Texture2D background;
        Rectangle rectangle = new Rectangle(0, 0, 3600, 3600);

        public Background(Texture2D background)
        {
            this.background = background;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(background, new Vector2(-1800,-1800), rectangle, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
        }
    }
}
