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
    // Convert Station XML representation to Station object
    class StationFromXML
    {
        XmlDocument doc;

        public Station FromXML(string s)
        {
            // Create a document from the XML
            doc = new XmlDocument();
            doc.LoadXml(s);

            // Get the overall Station element
            XmlNode xmlNode = doc.CloneNode(true).FirstChild;

            // Station id
            int id = int.Parse(xmlNode.Attributes.GetNamedItem("id").Value);

            // Position
            int stationX = int.Parse(xmlNode.Attributes.GetNamedItem("X").Value);
            int stationY = int.Parse(xmlNode.Attributes.GetNamedItem("Y").Value);

            // Get first node (core)
            xmlNode = doc.CloneNode(true).FirstChild.FirstChild;
            int nid = int.Parse(xmlNode.Attributes.GetNamedItem("id").Value);
            int health = int.Parse(xmlNode.Attributes.GetNamedItem("health").Value);

            // Set up core
            Node node = new Node(0, null);
            node.position = new Vector2(0, 0);
            node.data = new StationNode(node, StationNodeType.Core, health);
            node.id = nid;

            foreach (XmlNode child in xmlNode.ChildNodes)
            {
                RecursiveBuild(child, node);
            }

            // Create the station from preset tree
            Station fromXML = new Station(node, new Vector2(stationX, stationY));
            fromXML.id = id;

            SetStationRef(fromXML.station_tree, fromXML);

            return fromXML;
        }

        // For each node, create a new node with attributes
        public void RecursiveBuild(XmlNode xmlNode, Node parent)
        {
            int id = int.Parse(xmlNode.Attributes.GetNamedItem("id").Value);
            int depth = int.Parse(xmlNode.Attributes.GetNamedItem("depth").Value);
            int health = int.Parse(xmlNode.Attributes.GetNamedItem("health").Value);
            int X = int.Parse(xmlNode.Attributes.GetNamedItem("X").Value);
            int Y = int.Parse(xmlNode.Attributes.GetNamedItem("Y").Value);
            string type = xmlNode.Attributes.GetNamedItem("type").Value;
            StationNodeType typeValue = 0;

            switch (type)
            {
                case "Core": typeValue = StationNodeType.Core; break;
                case "Pipe": typeValue = StationNodeType.Pipe; break;
                case "Turret": typeValue = StationNodeType.Turret; break;
            }

            Node node = new Node(depth, parent);
            node.position = new Vector2(X, Y);
            node.data = new StationNode(node, typeValue, health);
            node.id = id;

            parent.Add(node);

            foreach (XmlNode child in xmlNode.ChildNodes)
            {
                RecursiveBuild(child, node);
            }
        }

        // Update all station refs since they will all be null by default
        public void SetStationRef(Node node, Station station)
        {
            node.data.station = station;

            foreach (Node child in node.children.ToArray())
            {
                SetStationRef(child, station);
            }
        }
    }
}
