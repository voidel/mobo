

using Lidgren.Network;
/**
* Author: Christopher Cola
* Created on 30/01/2016
*/
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;

namespace Mobo
{
    class StationNode
    {
        public Node node;
        public Station station;
        public Texture2D texture;
        public Color color;
        public Rectangle bounds;
        public Vector2 origin;
        public StationNodeType nodeType;
        private float rotation;
        public Vector2 position;
        public int health;
        public AnimatedSprite explosion;

        Random rand = new Random(Guid.NewGuid().GetHashCode());

        bool weaponsEnabled = true;
        Timer weaponCooldown;

        public ConcurrentDictionary<int, Projectile> projectiles = new ConcurrentDictionary<int, Projectile>();

        private static readonly object syncLock = new object();

        public StationNode(Node node, Station station)
        {
            this.node = node;
            this.station = station;
            this.texture = ContentStore.station_core;
            this.color = Color.White;
            bounds = new Rectangle(100000, 100000, 28, 28);

            if (node.depth == 0)
            {
                nodeType = StationNodeType.Core;
            }
            else if (node.depth == station.max_depth || node.depth % station.split_depth == 0)
            {
                nodeType = StationNodeType.Turret;
            }
            else
            {
                nodeType = StationNodeType.Pipe;
            }

            health = 100;

            Initialise();
        }

        public StationNode(Node node, StationNodeType type, int health)
        {
            this.node = node;
            this.texture = ContentStore.station_core;
            this.color = Color.White;
            this.nodeType = type;
            this.health = health;

            Initialise();
        }

        private void Initialise()
        {
            bounds = new Rectangle(100000, 100000, 28, 28);

            // Set up a timer to simulate weapons cooldown between shots
            int timerVariance = rand.Next(-1000, 1001);
            weaponCooldown = new Timer(2000 + timerVariance);
            weaponCooldown.Elapsed += OnWeaponCooldown;
            weaponCooldown.AutoReset = false;
        }

        private void OnWeaponCooldown(object sender, ElapsedEventArgs e)
        {
            weaponsEnabled = true;
        }

        public void Update()
        {
            if (explosion == null)
            {
                float posx = station.position.X - 32 * (node.position.X);
                float posy = station.position.Y - 32 * (node.position.Y);

                position = new Vector2(posx, posy);

                // Update bounds
                bounds.X = (int)position.X - bounds.Width / 2;
                bounds.Y = (int)position.Y - bounds.Height / 2;

                if (nodeType == StationNodeType.Turret && node.children.Count == 0)
                {
                    texture = ContentStore.orb_red;
                }

                Player player;

                if(Network.connected)
                {
                    player = Online.localPlayer;
                }
                else
                {
                    player = Offline.localPlayer;
                }

                // Check for player bullet collisions with the node
                if (nodeType == StationNodeType.Turret || nodeType == StationNodeType.Core)
                {
                    foreach (Projectile p in player.projectiles.Values)
                    {
                        if (p != null)
                        {
                            if (bounds.Intersects(p.bounds))
                            {
                                if (p.death == null)
                                {
                                    if (Network.connected)
                                    {
                                        //Send details about the removal of the projectile
                                        Network.out_message = Network.Client.CreateMessage();
                                        Network.out_message.Write(Network.REMOVE_PROJECTILE);
                                        Network.out_message.Write(player.uid);
                                        Network.out_message.Write(p.index);
                                        Network.Client.SendMessage(Network.out_message, NetDeliveryMethod.ReliableOrdered);
                                    }

                                    p.DeathAnim();
                                    node.Destroy();
                                }
                            }
                        }
                    }
                }

                // Check for physical collisions with players
                if (player.collisionEnabled)
                {
                    if (bounds.Intersects(player.bounds))
                    {
                        player.velocity = -player.velocity * 1.5f;
                        player.collisionEnabled = false;
                        player.collisionCooldown.Enabled = true;
                    }
                }

                // Shoot if the player is near
                if (weaponsEnabled && texture == ContentStore.orb_red)
                {
                    if (Vector2.Distance(position, player.position) < 540)
                    {
                        float angle = calculateAngle(position, player.position);
                        Vector2 bullet_velocity = projectileVelocity(angle);
                        Projectile p = new Projectile(ContentStore.laser_yellow, position, bullet_velocity, angle, -1, null, this);
                        projectiles.TryAdd(p.index, p);
                        weaponsEnabled = false;
                        weaponCooldown.Enabled = true;

                        // If we are online we send the position of the node, velocity of proj. to server
                        if (Network.connected)
                        {
                            Network.out_message = Network.Client.CreateMessage();
                            Network.out_message.Write(Network.CREATE_STATION_PROJECTILE);
                            Network.out_message.Write(Network.Client.UniqueIdentifier);
                            Network.out_message.Write(station.id);
                            Network.out_message.Write(node.id);
                            Network.out_message.Write((int)position.X);
                            Network.out_message.Write((int)position.Y);
                            Network.out_message.Write(bullet_velocity.X);
                            Network.out_message.Write(bullet_velocity.Y);
                            Network.Client.SendMessage(Network.out_message, NetDeliveryMethod.ReliableOrdered);
                        }
                    }
                }

                //Check for own projectile collisions with players
                foreach (Projectile p in projectiles.Values)
                {
                    if (p.bounds.Intersects(player.bounds))
                    {
                        if (!player.invulnerable)
                        {
                            player.invulnerable = true;
                            player.invulerability.Enabled = true;

                            if (Network.connected)
                            {
                                // Send details about this health change to the server
                                Network.out_message = Network.Client.CreateMessage();
                                Network.out_message.Write(Network.HEALTH);
                                Network.out_message.Write(player.uid);
                                Network.out_message.Write(-10);
                                Network.Client.SendMessage(Network.out_message, NetDeliveryMethod.ReliableOrdered);
                            }
                            else
                            {
                                // Enable death and respawn for offline play
                                player.health -= 10;
                                if (player.health <= 0)
                                {
                                    ScreenManager.messageList.Add("You have died!", MessageType.GameHint);
                                    player.position = new Vector2(0, 0);
                                    player.health = 100;
                                }
                            }
                        }
                    }
                }

                //Final check to destroy node if a destroy attempt was made previously
                if(nodeType == StationNodeType.Pipe && node.children.Count == 0)
                {
                    node.Destroy();
                }
            }

            else
            {
                explosion.Update();
            }

            foreach (Projectile projectile in projectiles.Values)
            {
                if (projectile != null)
                {
                    // Update projectile positions
                    projectile.Update();
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (texture != null)
            {
                spriteBatch.Draw(texture, position, null, color, 0f, origin, 1f, SpriteEffects.None, 0f);

                if (SettingsManager.getShowDepths())
                {
                    spriteBatch.DrawString(ContentStore.generic, node.depth.ToString(), position, Color.White);
                }

                if (SettingsManager.getShowBounds())
                {
                    if(nodeType == StationNodeType.Turret)
                    {
                        spriteBatch.Draw(ContentStore.debug, bounds, Color.Red);
                    }
                    else
                    // Collision rectangle
                    spriteBatch.Draw(ContentStore.debug, bounds, Color.White);
                }

                if (explosion != null)
                {
                    explosion.Draw(spriteBatch, position);
                    color = new Color(120, 120, 120);
                }

                if (nodeType == StationNodeType.Turret)
                {
                    spriteBatch.Draw(ContentStore.station_shield, position, null, Color.White, rotation++, origin, 0.15f, SpriteEffects.None, 0f);
                }
            }

            //Draw projectiles
            foreach (Projectile projectile in projectiles.Values)
            {
                if (projectile != null)
                {
                    // Draw projectiles
                    projectile.Draw(spriteBatch);
                }
            }
        }

        // Calculates the angle of the players to the stationNode
        public float calculateAngle(Vector2 vector1, Vector2 vector2)
        {
            Vector2 vector = vector2 - vector1;
            return (float)Math.Atan2(vector.Y, vector.X) + MathHelper.PiOver2;
        }

        // Calculate the starting velocity of a projectile based on above angle
        public Vector2 projectileVelocity(float angle)
        {
            int bullet_speed = 3;

            float velocity_x = -(float)Math.Cos(angle + MathHelper.PiOver2) * bullet_speed;
            float velocity_y = -(float)Math.Sin(angle + MathHelper.PiOver2) * bullet_speed;

            Vector2 totalVelocity = new Vector2(velocity_x, velocity_y);

            return totalVelocity;
        }
    }
}
