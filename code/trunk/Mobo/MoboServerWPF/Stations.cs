/**
* Author: Christopher Cola
* Created on 19/04/2016
*/

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace MoboServerWPF
{
    class Stations
    {
        public static ConcurrentDictionary<int, XmlDocument> stations = new ConcurrentDictionary<int, XmlDocument>();

        public static void RemoveNode(int stationId, int nodeId)
        {
            try
            {
                XmlNodeList nodeList = stations[stationId].SelectNodes("//Node[@id='" + nodeId + "']");
                Program.window.Append("Node " + nodeId + " of Station " + stationId + " was destroyed!");
                foreach (XmlNode node in nodeList)
                {
                    node.ParentNode.RemoveChild(node);
                }
            }
            catch(Exception e)
            {
                Program.window.Append(e.Message);
            }
            
        }
    }
}
