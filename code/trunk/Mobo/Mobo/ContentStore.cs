using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
/**
 * Author: Christopher Cola
 * Created on 25/10/2015
 */

using Microsoft.Xna.Framework.Graphics;

namespace Mobo
{
    // This class loads all the content and stores them to be accessed anywhere
    class ContentStore
    {
        // TEXTURES

        // Main Menu
        public static Texture2D logo;
        public static Texture2D bg1, bg5, bg7;
        public static Texture2D help;

        // UI
        public static Texture2D button_texture;
        public static Texture2D input_texture;
        public static Texture2D radar;
        public static Texture2D button_plus;
        public static Texture2D button_minus;
        public static Texture2D health_bar;

        // Cursors
        public static Texture2D cursor_pointer;
        public static Texture2D cursor_hand;
        public static Texture2D cursor_crosshair;

        // Fonts
        public static SpriteFont generic;

        // Entities
        public static Texture2D player_sprite;
        public static Texture2D player_shield;
        public static Texture2D station_shield;
        public static Texture2D laser_blue;
        public static Texture2D laser_yellow;
        public static Texture2D station_core;
        public static Texture2D orb_red;
        public static Texture2D orb_blue;
        public static Texture2D orb_green;
        public static Texture2D orb_yellow;
        public static Texture2D explosion;
        public static Texture2D projectile_explosion;

        // Pipes
        public static Texture2D pipe_nw;
        public static Texture2D pipe_ne;
        public static Texture2D pipe_sw;
        public static Texture2D pipe_se;
        public static Texture2D pipe_h;
        public static Texture2D pipe_v;
        public static Texture2D pipe_n;
        public static Texture2D pipe_e;
        public static Texture2D pipe_s;
        public static Texture2D pipe_w;
        public static Texture2D pipe_c;

        // Sound
        public static SoundEffect boop;
        public static SoundEffect laser;

        // Debug
        public static Texture2D debug;

        public ContentStore(ContentManager Content)
        {
            logo = Content.Load<Texture2D>("UI/splash_logo_alt");

            // Load Backgrounds
            bg1 = Content.Load<Texture2D>("Backgrounds/1");
            bg5 = Content.Load<Texture2D>("Backgrounds/5");
            bg7 = Content.Load<Texture2D>("Backgrounds/7");
            help = Content.Load<Texture2D>("Backgrounds/help");

            // Load UI
            button_texture = Content.Load<Texture2D>("UI/buttonBlue");
            input_texture = Content.Load<Texture2D>("UI/buttonGray");
            radar = Content.Load<Texture2D>("UI/radar");
            button_plus = Content.Load<Texture2D>("UI/buttonPlus");
            button_minus = Content.Load<Texture2D>("UI/buttonMinus");
            health_bar = Content.Load<Texture2D>("UI/EmptyBar");

            // Load cursors
            cursor_pointer = Content.Load<Texture2D>("Cursors/pointer");
            cursor_hand = Content.Load<Texture2D>("Cursors/hand");
            cursor_crosshair = Content.Load<Texture2D>("Cursors/crosshair_red");

            // Load fonts
            generic = Content.Load<SpriteFont>("Fonts/generic3");

            // Load entities
            player_sprite = Content.Load<Texture2D>("Entities/player");
            player_shield = Content.Load<Texture2D>("Entities/playerShield");
            station_shield = Content.Load<Texture2D>("Entities/shield");
            laser_blue = Content.Load<Texture2D>("Entities/laserBlue");
            laser_yellow = Content.Load<Texture2D>("Entities/laserYellow");
            station_core = Content.Load<Texture2D>("Entities/core");
            orb_red = Content.Load<Texture2D>("Entities/orbRed");
            orb_blue = Content.Load<Texture2D>("Entities/orbBlue");
            orb_green = Content.Load<Texture2D>("Entities/orbGreen");
            orb_yellow = Content.Load<Texture2D>("Entities/orbYellow");
            explosion = Content.Load<Texture2D>("Entities/explode");
            projectile_explosion = Content.Load<Texture2D>("Entities/projectile_explode");


            // Load pipes
            pipe_nw = Content.Load<Texture2D>("Entities/pipe_nw");
            pipe_ne = Content.Load<Texture2D>("Entities/pipe_ne");
            pipe_sw = Content.Load<Texture2D>("Entities/pipe_sw");
            pipe_se = Content.Load<Texture2D>("Entities/pipe_se");
            pipe_h = Content.Load<Texture2D>("Entities/pipe_h");
            pipe_v = Content.Load<Texture2D>("Entities/pipe_v");
            pipe_n = Content.Load<Texture2D>("Entities/pipe_n");
            pipe_e = Content.Load<Texture2D>("Entities/pipe_e");
            pipe_s = Content.Load<Texture2D>("Entities/pipe_s");
            pipe_w = Content.Load<Texture2D>("Entities/pipe_w");
            pipe_c = Content.Load<Texture2D>("Entities/pipe_c");

            // Load sounds
            boop = Content.Load<SoundEffect>("Sounds/boop");
            laser = Content.Load<SoundEffect>("Sounds/laser");
        }
    }
}
