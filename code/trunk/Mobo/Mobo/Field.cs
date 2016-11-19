/**
 * Author: Christopher Cola
 * Created on 27/10/2015
 */

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Mobo
{
    // Field GUI element, used to display values on settings screen
    class Field
    {
        // used to identify the field
        public int id;
        Texture2D texture;
        Vector2 position;
        Rectangle rectangle;

        public Field(int id, Vector2 position)
        {
            this.id = id;
            this.texture = ContentStore.input_texture;
            this.position = position;

            // Center the drawn field about the position provided in the constructor
            rectangle = new Rectangle((int)position.X - texture.Width / 2, (int)position.Y, texture.Width, texture.Height);
        }

        public void Draw(SpriteBatch spriteBatch, string replace)
        {
            spriteBatch.Draw(texture, rectangle, Color.White);

            // Center text in the button
            Vector2 textOrigin = MoboUtils.textOrigin(replace, ContentStore.generic);
            spriteBatch.DrawString(ContentStore.generic, replace, position, Color.Black, 0.0f, textOrigin, 1f, SpriteEffects.None, 0.0f);
        }
    }
}