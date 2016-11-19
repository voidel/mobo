/**
 * Author: Christopher Cola
 * Created on 26/10/2015
 */

using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace Mobo
{
    // Settings
    class SettingsManager
    {
        private static SettingsData data;

        public static readonly int EASY = 0, NORMAL = 1, HARD = 2, EXTREME = 3;

        public static void Initialize()
        {
            data = new SettingsData();
            Load();
        }

        public static void Load()
        {
            //Read in Mobo settings XML file and deserialize
            XmlSerializer serializer = new XmlSerializer(typeof(SettingsData));
            StreamReader reader = new StreamReader("MoboSettings.xml");
            data = (SettingsData)serializer.Deserialize(reader);
            reader.Close();
        }

        // Serialise current settings as MoboSettings
        public static void Save()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(SettingsData));
            StringWriter stringWriter = new StringWriter();
            XmlWriterSettings XMLsettings = new XmlWriterSettings
            {
                Indent = true,
                OmitXmlDeclaration = true
            };
            XmlWriter XMLwriter = XmlWriter.Create(stringWriter,XMLsettings);
            serializer.Serialize(XMLwriter, data);
            File.WriteAllText("MoboSettings.xml", stringWriter.ToString());
        }

        public static string getUsername()
        {
            return data.name;
        }

        public static int getDifficulty()
        {
            return data.difficulty;
        }

        public static int getResolutionWidth()
        {
            return (int)data.resolution.X;
        }

        public static int getResolutionHeight()
        {
            return (int)data.resolution.Y;
        }

        public static bool getShowBounds()
        {
            return data.show_bounds;
        }

        public static bool getShowDebug()
        {
            return data.show_debug;
        }

        public static bool getShowDepths()
        {
            return data.show_depths;
        }

        public static void setUserName(string name)
        {
            data.name = name;
        }

        public static void toggleDifficulty()
        {
            switch (data.difficulty)
            {
                case 0: data.difficulty = NORMAL; break;
                case 1: data.difficulty = HARD; break;
                case 2: data.difficulty = EXTREME; break;
                case 3: data.difficulty = EASY; break;
                default: data.difficulty = NORMAL; break;
            }
            Save();
        }

        public static void toggleDebug()
        {
            data.show_debug = !data.show_debug;
            Save();
        }

        public static void toggleBounds()
        {
            data.show_bounds = !data.show_bounds;
            Save();
        }

        public static void toggleDepths()
        {
            data.show_depths = !data.show_depths;
            Save();
        }

        public static string getVerboseDifficulty()
        {
            switch (data.difficulty)
            {
                case 0: return "Easy";
                case 1: return "Normal";
                case 2: return "Hard";
                case 3: return "Extreme";
                default: return "INVALID";
            }
        }

        public static string getServerIP()
        {
            return data.server_ip;
        }

        public static int getPort()
        {
            return data.server_port;
        }
    }
}
