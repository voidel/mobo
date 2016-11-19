/**
* Author: Christopher Cola
* Created on 24/10/2015
*/

using Lidgren.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Timers;

namespace Mobo
{
    class Player
    {
        public static int bulletSpeed = 10;

        Texture2D texture;
        Texture2D shield;
        Vector2 origin;
        public Vector2 position;
        public Vector2 velocity;
        public Rectangle bounds;
        Rectangle healthBar;
        Rectangle fullBar;

        int sheildRotation;
        public float playerRotation;
        public float mouseRotation;

        public int health;
        public int score;
        public string name;
        public long uid;
        
        public bool playerControlled;

        public bool m_up;
        public bool m_down;
        public bool m_left;
        public bool m_right;
        public bool m_shoot;

        bool weaponsEnabled = true;
        public bool invulnerable = false;
        Timer cooldown;
        public Timer invulerability;

        public bool collisionEnabled = true;
        public Timer collisionCooldown;

        public ConcurrentDictionary<int, Projectile> projectiles = new ConcurrentDictionary<int, Projectile>();

        public Player(Vector2 position, string name, int health, bool playerControlled, long uid)
        {
            texture = ContentStore.player_sprite; 
            shield = ContentStore.player_shield;
            this.position = position;
            origin = new Vector2((float)texture.Width / 2f, (float)texture.Height / 2f);
            velocity = Vector2.Zero;
            this.health = health;
            this.name = name;
            this.uid = uid;
            healthBar = new Rectangle((int)position.X,(int)position.Y,(int)(health*2.52f),16);
            fullBar = new Rectangle((int)position.X, (int)position.Y, (int)(100 * 2.52f), 16);
            this.playerControlled = playerControlled;

            // Set up a timer to simulate weapons cooldown between shots
            cooldown = new Timer(200);
            cooldown.Elapsed += OnCooldown;
            cooldown.AutoReset = false;

            // Set up a timer to simulate weapons cooldown between physical collisions
            collisionCooldown = new Timer(50);
            collisionCooldown.Elapsed += OnCollisionCooldown;
            collisionCooldown.AutoReset = false;

            // Initialse bounds for the local player only
            if (playerControlled)
            {
                bounds = new Rectangle((int)position.X - bounds.Width / 2, (int)position.Y - bounds.Height / 2, 28, 28);

                // Set up a timer to simulate invulnerability when hit
                invulerability = new Timer(500);
                invulerability.Elapsed += OnHit;
                invulerability.AutoReset = false;
            }
        }

        public void Update()
        {
            // Only handle keyboard input and follow player if PC
            if (playerControlled)
            {

                HandleInput();

                // Loop world by transporting player to the opposite side
                if (position.X > 1800) setPositionX(-1800 + (position.X - 1800));
                else if (position.X < -1800) setPositionX(1800 + (position.X + 1800));

                if (position.Y > 1800) setPositionY(-1800 + (position.Y - 1800));
                else if (position.Y < -1800) setPositionY(1800 + (position.Y + 1800));

                // If velocity on either axis is less than zero, make it zero;
                if (-0.2 < velocity.X && velocity.X < 0.2) velocity.X = 0;
                if (-0.2 < velocity.Y && velocity.Y < 0.2) velocity.Y = 0;

                // Increase position by velocity divided by a constant (to make it a slower acceleration)
                position += velocity;

                // Simulate friction to slow the player down if not moving
                velocity *= 0.96f;

                // Calculate the direction the ship is pointing based upon velocity
                playerRotation = calculateDirection(velocity);

                // Calculate the angle to the mouse from the screen center
                mouseRotation = angleToMouse() + MathHelper.PiOver2;

                // Update bounds
                bounds.X = (int)position.X - bounds.Width/2;
                bounds.Y = (int)position.Y - bounds.Height/2;

                // Update the camera positon with the player coordinates
                Camera2D.Pos = position;

                // Only if this is an online player do we then broadcast our location to server
                if (Network.connected)
                {
                    Network.out_message = Network.Client.CreateMessage();
                    Network.out_message.Write(Network.MOVE);
                    Network.out_message.Write(uid);
                    Network.out_message.Write((int)position.X);
                    Network.out_message.Write((int)position.Y);
                    Network.out_message.Write(playerRotation);
                    Network.Client.SendMessage(Network.out_message, NetDeliveryMethod.Unreliable);

                    // Check for collisions to other players
                    foreach (Player p in Online.players.Values)
                        if (!p.playerControlled)
                            // Reverse for loop so we can safely remove projectiles
                            foreach(Projectile proj in p.projectiles.Values)
                                if (proj != null)
                                {
                                    if (bounds.Intersects(proj.bounds))
                                    {
                                        if (!invulnerable)
                                        {
                                            invulnerable = true;
                                            invulerability.Enabled = true;

                                            // Send details about this health change to the server
                                            Network.out_message = Network.Client.CreateMessage();
                                            Network.out_message.Write(Network.HEALTH);
                                            Network.out_message.Write(uid);
                                            Network.out_message.Write(-10);
                                            Network.Client.SendMessage(Network.out_message, NetDeliveryMethod.ReliableOrdered);
                                        }

                                        //Send details about the removal of the projectile
                                        Network.out_message = Network.Client.CreateMessage();
                                        Network.out_message.Write(Network.REMOVE_PROJECTILE);
                                        Network.out_message.Write(p.uid);
                                        Network.out_message.Write(proj.index);
                                        Network.Client.SendMessage(Network.out_message, NetDeliveryMethod.ReliableOrdered);
                                    } 
                                }
                }
            }

            foreach (Projectile projectile in projectiles.Values)
            {
                if (projectile != null)
                {
                    // Update projectile positions
                    projectile.Update();
                }
            }

            // Update health bar
            healthBar.Width = (int)(health * 2.52f);
        }

        public void Draw(SpriteBatch staticSpriteBatch, SpriteBatch cameraSpriteBatch)
        {
            //Draw projectiles
            foreach (Projectile projectile in projectiles.Values)
            {
                if (projectile != null)
                {
                    // Draw projectiles
                    projectile.Draw(cameraSpriteBatch);
                }
            }

            // Ship
            if (!invulnerable)
                cameraSpriteBatch.Draw(texture, position, null, Color.White, playerRotation, origin, 1f, SpriteEffects.None, 0f);
            else
                cameraSpriteBatch.Draw(texture, position, null, Color.Red, playerRotation, origin, 1f, SpriteEffects.None, 0f);

            // Shield
            cameraSpriteBatch.Draw(shield, position, null, Color.White, sheildRotation++, origin + new Vector2(3,3), 1f, SpriteEffects.None, 0f);

            Color healthColor = Color.White;

            // Health
            if (health > 67)
                healthColor = Color.LightGreen;
            else if (health > 34)
                healthColor = Color.Yellow;
            else
                healthColor = Color.PaleVioletRed;

            if (playerControlled)
            {
                staticSpriteBatch.Draw(ContentStore.debug, new Vector2(6, 6), healthBar, healthColor, 0.0f, Vector2.Zero, 1f, SpriteEffects.None, 0.0f);
                staticSpriteBatch.Draw(ContentStore.health_bar, new Vector2(4, 4), Color.White);

                // Name
                staticSpriteBatch.DrawString(ContentStore.generic, name, new Vector2(13, 7), Color.LightGray, 0.0f, Vector2.Zero, 1f, SpriteEffects.None, 0.0f);

                // Debug
                if (SettingsManager.getShowDebug())
                {
                    staticSpriteBatch.DrawString(ContentStore.generic, "Health: " + health, Vector2.Zero, Color.White);
                    staticSpriteBatch.DrawString(ContentStore.generic, "Position: " + position.ToString(), new Vector2(0, 16), Color.White);
                    staticSpriteBatch.DrawString(ContentStore.generic, "Velocity: " + velocity.ToString(), new Vector2(0, 32), Color.White);
                    staticSpriteBatch.DrawString(ContentStore.generic, "Direction: " + playerRotation, new Vector2(0, 48), Color.White); 
                }

                if(SettingsManager.getShowBounds())
                {
                    // Collision rectangle
                    cameraSpriteBatch.Draw(ContentStore.debug, bounds, Color.White);
                }
            }

            // Else draw a small health bar above player
            else
            {
                // Name
                Vector2 textOrigin = MoboUtils.textOrigin(name, ContentStore.generic);
                cameraSpriteBatch.DrawString(ContentStore.generic, name, position + new Vector2(0, -56), Color.White, 0.0f, textOrigin, 1f, SpriteEffects.None, 0.0f);

                // Health Bar
                cameraSpriteBatch.Draw(ContentStore.debug, position, new Rectangle(0,0,fullBar.Width+8,fullBar.Height+8), Color.White, 0.0f, new Vector2(fullBar.Width / 2 + 4, 132), 0.25f, SpriteEffects.None, 0.0f);
                cameraSpriteBatch.Draw(ContentStore.debug, position, fullBar, Color.Black, 0.0f, new Vector2(fullBar.Width / 2, 128), 0.25f, SpriteEffects.None, 0.0f);
                cameraSpriteBatch.Draw(ContentStore.debug, position, healthBar, healthColor, 0.0f, new Vector2(healthBar.Width/2, 128), 0.25f, SpriteEffects.None, 0.0f);
            }
        }

        public float angleToMouse()
        {
            Vector2 mouseVector = ScreenManager.cursorPos - ScreenManager.screenCenter + new Vector2(16, 16);
            return (float)Math.Atan2(mouseVector.Y, mouseVector.X);
        }

        // Calculates the angle of the local player ship based upon current x and y velocity
        public float calculateDirection(Vector2 velocity)
        {
            if (KeyboardInput.keyPressed)
                return (float)Math.Atan2(velocity.Y, velocity.X) + MathHelper.PiOver2;
            else
                return mouseRotation;
        }

        // Calculate the starting velocity of a projectile based on an angle
        public Vector2 projectileVelocity(float angle)
        {
            float velocity_x = -(float)Math.Cos(angle + MathHelper.PiOver2) * bulletSpeed;
            float velocity_y = -(float)Math.Sin(angle + MathHelper.PiOver2) * bulletSpeed;

            Vector2 totalVelocity = new Vector2(velocity_x, velocity_y);

            return totalVelocity + velocity;
        }

        private void shoot()
        {
            if (weaponsEnabled)
            {
                // Start the cooldown timer and disable weapons until it's over
                cooldown.Enabled = true;
                weaponsEnabled = false;

                // Calculate the resultant velocity based upon the position of the mouse
                Vector2 bullet_velocity = projectileVelocity(mouseRotation);

                // Add the projectile to the list in player
                Projectile p = new Projectile(ContentStore.laser_blue, position, bullet_velocity, mouseRotation, -1, this, null);

                // Remember the unique index of the projectile
                int index = p.index;

                projectiles.TryAdd(p.index, p);

                // If we are online we send the position of player, velocity of proj. and uid of player to server
                if (Network.connected)
                {
                    Network.out_message = Network.Client.CreateMessage();
                    Network.out_message.Write(Network.CREATE_PROJECTILE);
                    Network.out_message.Write(uid);
                    Network.out_message.Write(index);
                    Network.out_message.Write((int)position.X);
                    Network.out_message.Write((int)position.Y);
                    Network.out_message.Write(mouseRotation);
                    Network.out_message.Write(bullet_velocity.X);
                    Network.out_message.Write(bullet_velocity.Y);
                    Network.Client.SendMessage(Network.out_message, NetDeliveryMethod.ReliableOrdered);
                }
            }
        }

        // Sets the position of the player (mostly for syncing online players)
        public void setPosition(int x, int y)
        {
            position.X = x;
            position.Y = y;
        }

        public void setPositionX(float x)
        {
            position.X = x;
        }

        public void setPositionY(float y)
        {
            position.Y = y;
        }

        private void OnCollisionCooldown(object sender, ElapsedEventArgs e)
        {
            collisionEnabled = true;
        }

        // Enables firing of the bullets at intervals
        private void OnCooldown(object sender, ElapsedEventArgs e)
        {
            weaponsEnabled = true;
        }

        // Enables an invulnerable period when hit
        private void OnHit(object sender, ElapsedEventArgs e)
        {
            invulnerable = false;
        }

        private void HandleInput()
        {
            KeyboardInput.HandleMovementInput(this);

            if(m_up)
            {
                velocity -= Vector2.UnitY/5;
                m_up = false;
            }

            if(m_down)
            {
                velocity += Vector2.UnitY/5;
                m_down = false;
            }

            if(m_left)
            {
                velocity -= Vector2.UnitX/5;
                m_left = false;
            }

            if(m_right)
            {
                velocity += Vector2.UnitX/5;
                m_right = false;
            }

            if(m_shoot)
            {
                shoot();
                m_shoot = false;
            }
        }
    }
}
