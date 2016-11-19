/**
* Author: Christopher Cola
* Created on 26/10/2015
*/

using Microsoft.Xna.Framework;
using System;

namespace Mobo
{
    // Class that is serialised to form MoboSettings.XML
    public class SettingsData
    {
        public string name;
        public int difficulty;
        public Vector2 resolution;
        public string server_ip;
        public int server_port;
        public bool show_debug;
        public bool show_bounds;
        public bool show_depths;
    }
}
