/**
 * Author: Christopher Cola
 * Created on 25/01/2016
 */

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mobo
{
    // This class is used for the explosions and when projectiles die
    class AnimatedSprite
    {
        // The sprite sheet
        public Texture2D texture;

        // Number of frames in the spritesheet
        int frames;

        // Current frame of the animation
        int currentFrame = 0;

        // Whether the animation should repeat or not
        bool repeat;

        // Whether the animation has finished
        bool finished;

        public AnimatedSprite(Texture2D texture, int frames, bool repeat)
        {
            this.texture = texture;
            this.frames = frames;
            this.repeat = repeat;
        }

        public void Update()
        {
            currentFrame++;

            // If we intend to repeat this anim, set the current frame to 0 and continue animation
            if (currentFrame == frames && repeat)
            {
                currentFrame = 0;
            }
                
            // Otherwise set the texture to null and stop
            else if (currentFrame == frames && !repeat)
            {
                texture = null;
                finished = true;
            }
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 location)
        {
            // Only draw if the animation hasn't finished
            if (!finished)
            {
                // Width of a single frame
                int width = texture.Width / frames;

                // Height of a single frame
                int height = texture.Height;

                // Select the frame in the spritesheet that corresponds to the current frame
                Rectangle sourceRectangle = new Rectangle(width * currentFrame, 0, width, height);

                // The rectangle the frame will be drawn in
                Rectangle destinationRectangle = new Rectangle((int)location.X - width / 2, (int)location.Y - height / 2, width, height);

                spriteBatch.Draw(texture, destinationRectangle, sourceRectangle, Color.White);
            }
        }
    }
}
