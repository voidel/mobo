/**
* Author: Christopher Cola
* Created on 30/10/2015
*/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Timers;

namespace Mobo
{
    class Projectile
    {
        Texture2D texture;
        Vector2 origin;
        Vector2 position;
        Vector2 velocity;
        float rotation;
        public Rectangle bounds;
        public AnimatedSprite death;

        Player playerOwner;
        StationNode stationNodeOwner;

        public int index;

        Timer lifeTime, deathAnim;

        Random rand = new Random(Guid.NewGuid().GetHashCode());

        public Projectile(Texture2D texture, Vector2 position, Vector2 velocity, float rotation, int index, Player playerOwner, StationNode stationNodeOwner)
        {
            bounds.X = 10000;
            bounds.Y = 10000;
            this.texture = texture;
            if (playerOwner != null)
            {
                // A player fired projectile
                origin = new Vector2(texture.Width / 2, 3);
                bounds = new Rectangle((int)position.X - bounds.Width / 2, (int)position.Y - bounds.Height / 2, 3, 3);
            }
            else
            {
                // A StationNode fired projectile
                origin = new Vector2(texture.Width / 2f, texture.Height / 2f);
                bounds = new Rectangle((int)position.X - bounds.Width / 2, (int)position.Y - bounds.Height / 2, 12, 12);
            }
            
            this.position = position;
            this.velocity = velocity;
            this.rotation = rotation;

            // Give the projectile a unique index if one not already supplied
            if (index == -1)
            {
                this.index = rand.Next();
            }
            else
            {
                this.index = index;
            }

            // Assign to owner
            if(playerOwner != null)
            {
                this.playerOwner = playerOwner;
            }
            else
            {
                this.stationNodeOwner = stationNodeOwner;
            }

            // Set up a timer to remove projectile when the lifetime is up
            lifeTime = new Timer(2000);
            lifeTime.Elapsed += OnLifeTimeOver;
            lifeTime.AutoReset = false;
            lifeTime.Enabled = true;

            // Set up a timer for the death animation
            deathAnim = new Timer(100);
            deathAnim.Elapsed += OnDeathOver;
            deathAnim.AutoReset = false;

            // Play sound
            float f = (float)rand.NextDouble();

            ContentStore.laser.Play(0.25f, f / 2, 0);
        }

        // The projectile's lifetime is over, time to remove
        private void OnLifeTimeOver(object sender, ElapsedEventArgs e)
        {
            DeathAnim();
        }

        // Remove the projectile now it's death animation is over
        private void OnDeathOver(object sender, ElapsedEventArgs e)
        {
            if(playerOwner != null)
            {
                playerOwner.projectiles.Remove(index);
            }
            else
            {
                stationNodeOwner.projectiles.Remove(index);
            }
        }

        public void Update()
        {
            // Loop world by transporting projectile to the opposite side
            if (position.X > 1800) setPositionX(-1800 + (position.X - 1800));
            else if (position.X < -1800) setPositionX(1800 + (position.X + 1800));

            if (position.Y > 1800) setPositionY(-1800 + (position.Y - 1800));
            else if (position.Y < -1800) setPositionY(1800 + (position.Y + 1800));

            position += velocity;

            // Update bounds
            bounds.X = (int)position.X - bounds.Width/2;
            bounds.Y = (int)position.Y - bounds.Height/2;

            if(stationNodeOwner != null)
            {
                rotation += 0.2f;
            }

            if (death != null)
            {
                death.Update();
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (death == null)
            {
                spriteBatch.Draw(texture, position, null, Color.White, rotation, origin, 1f, SpriteEffects.None, 0f);

                if (SettingsManager.getShowBounds())
                {
                    // Collision rectangle
                    spriteBatch.Draw(ContentStore.debug, bounds, Color.Red);
                } 
            }
            else
            {
                death.Draw(spriteBatch, position);
            }
        }

        public void setPositionX(float x)
        {
            position.X = x;
        }

        public void setPositionY(float y)
        {
            position.Y = y;
        }

        // Place the bounds far, far away while the animation plays
        public void DeathAnim()
        {
            bounds.X = 10000;
            bounds.Y = 10000;
            velocity = Vector2.Zero;
            death = new AnimatedSprite(ContentStore.projectile_explosion, 8, true);
            deathAnim.Enabled = true;
        }
    }
}
