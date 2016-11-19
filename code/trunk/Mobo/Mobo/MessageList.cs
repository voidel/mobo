/**
 * Author: Christopher Cola
 * Created on 06/11/2015
 */

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Timers;

namespace Mobo
{
    // Used to display messages in the bottom left of the screen
    class MessageList
    {
        // Tuple of the message and the intended colour
        List<Tuple<string, Color>> messageList = new List<Tuple<string, Color>>();

        Timer timeout;

        public MessageList()
        {
            Add("Mobo - Christopher J. Cola 2016.", MessageType.System);

            // Set up a timer to remove messages after a certain amount of time
            timeout = new Timer(5000);
            timeout.Elapsed += OnTimeout;
            timeout.AutoReset = true;
            timeout.Enabled = true;
        }

        // After the timer is up remove a message if there are remaining messages in the list
        private void OnTimeout(object sender, ElapsedEventArgs e)
        {
            if(messageList.Count > 0)
            {
                messageList.RemoveAt(0);
            }
        }

        public void Add(string message, MessageType type)
        {
            string toAdd = "";
            Color color = Color.White;

            // Append a prefix and tint text depending on the message type
            switch (type)
            {
                case MessageType.System: toAdd =   "[SYSTEM] " + message; color = Color.Yellow; break;
                case MessageType.Network: toAdd =  "[NETWORK] " + message; color = Color.Aqua; break;
                case MessageType.GameHint: toAdd = "[HINT] " + message; color = Color.Lime; break;
                case MessageType.ChatMsg: toAdd =  "[CHAT] " + message; break;
            }

            // If there are more than 10 messages start deleting the oldest
            if (messageList.Count > 10) messageList.RemoveAt(0);

            messageList.Add(new Tuple<string, Color>(toAdd, color));
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            // Place messages in the bottom left of the screen, raising the start of the draw for each message in the list
            Vector2 firstPosition = new Vector2(4, SettingsManager.getResolutionHeight() - messageList.Count*16);

            for(int i=0; i<messageList.Count; i++)
            {
                // Draw messages while dropping the position of teh draw 16 pixels each time, resulting in a list of messages if there are more than one
                spriteBatch.DrawString(ContentStore.generic, messageList[i].Item1, firstPosition + (new Vector2(0,16)*i), messageList[i].Item2);
            }
        }
    }
}
