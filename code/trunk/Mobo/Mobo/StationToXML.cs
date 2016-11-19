/**
* Author: Christopher Cola
* Created on 07/03/2016
*/

using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Mobo
{
    // Convert Station object to XML
    class StationToXML
    {
        XmlWriter writer;

        StringBuilder sb = new StringBuilder();

        public string ToXML(Station s)
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.OmitXmlDeclaration = true;

            XmlDocument doc = new XmlDocument();
            writer = XmlWriter.Create(sb, settings);

            // Start Document
            writer.WriteStartDocument();
            writer.WriteStartElement("Station");

            writer.WriteAttributeString("id", s.id.ToString());

            writer.WriteAttributeString("X", s.position.X.ToString());
            writer.WriteAttributeString("Y", s.position.Y.ToString());

            RecursiveWrite(s.station_tree);

            // End Document
            writer.WriteEndElement();
            writer.WriteEndDocument();

            writer.Close();

            return sb.ToString();
        }

        public void RecursiveWrite(Node node)
        {
            // Start Node
            writer.WriteStartElement("Node");
            writer.WriteAttributeString("id", node.id.ToString());
            writer.WriteAttributeString("depth", node.depth.ToString());
            writer.WriteAttributeString("health", node.data.health.ToString());
            writer.WriteAttributeString("X", node.position.X.ToString());
            writer.WriteAttributeString("Y", node.position.Y.ToString());
            writer.WriteAttributeString("type", node.data.nodeType.ToString());

            foreach (Node child in node.children.ToArray())
            {
                RecursiveWrite(child);
            }
            writer.WriteEndElement();
        }
    }
}
