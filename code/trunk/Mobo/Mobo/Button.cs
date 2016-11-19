/**
 * Author: Christopher Cola
 * Created on 17/10/2015
 */

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Mobo
{
    // Button GUI element
    class Button
    {
        // Name of button, used to check for press and provides display text
        public string name;
        Texture2D texture;
        Vector2 position;
        Rectangle rectangle;
        Color color = new Color(255, 255, 255, 255);
        bool hidetext;

        // Buttons with text (like those found on menu)
        public Button(string name, Vector2 position)
        {
            this.name = name;
            this.texture = ContentStore.button_texture;
            this.position = position;

            // Center the button about the position it was specified
            rectangle = new Rectangle((int)position.X - texture.Width / 2, (int)position.Y, texture.Width, texture.Height);
        }

        // Buttons without text (like the + and - buttons in station generator)
        public Button(string name, Vector2 position, Texture2D texture, bool hidetext)
        {
            this.name = name;
            this.texture = texture;
            this.position = position;
            rectangle = new Rectangle((int)position.X - texture.Width / 2, (int)position.Y, texture.Width, texture.Height);
            this.hidetext = hidetext;
        }

        // Used to determine if the button was clicked and prevent repeated clicks
        public bool justClicked;
        public bool wasClicked;
        public bool isClicked;

        // Used to contextually change mouse cursor and apply blue effect
        public bool isMouseOver;

        // Used to prevent hover sound repeating
        public bool soundPlayed;

        public void Update(MouseState mouse)
        {
            Rectangle mouseRectangle = new Rectangle(mouse.X, mouse.Y, 1, 1);

            // If mouse is over the button
            if (mouseRectangle.Intersects(rectangle))
            {
                isMouseOver = true;
                if (!soundPlayed)
                {
                    ContentStore.boop.Play();
                    soundPlayed = true;
                }

                // Reduce red component for blue visual effect
                if (color.R > 20) color.R -= 20;

                if (mouse.LeftButton == ButtonState.Pressed) justClicked = true;
                else justClicked = false;

                if (justClicked && !wasClicked)
                {
                    // the player just pressed down
                    isClicked = true;
                }
                else if (justClicked && wasClicked)
                {
                    // the player is holding the key down
                    isClicked = false;
                }
                else isClicked = false;
                wasClicked = justClicked;

            }
            else
            {
                // If there is no mouse intersection, none of the following can be true
                isMouseOver = false;
                isClicked = false;
                justClicked = false;
                wasClicked = false;
                soundPlayed = false;

                // Slowly recover the red component until it is back to normal
                if (color.R < 255) color.R += 20;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, rectangle, color);
            if (!hidetext)
            {
                // Calculate the center of the text to center the text in the button
                Vector2 textOrigin = MoboUtils.textOrigin(name, ContentStore.generic);
                spriteBatch.DrawString(ContentStore.generic, name, position, Color.Black, 0.0f, textOrigin, 1f, SpriteEffects.None, 0.0f); 
            }
        }
    }
}
