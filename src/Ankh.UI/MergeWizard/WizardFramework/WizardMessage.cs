using System;
using System.Collections.Generic;
using System.Text;

/* 
 * WizardMessage.cs
 * 
 * Copyright (c) 2008 CollabNet, Inc. ("CollabNet"), http://www.collab.net,
 * Licensed under the Apache License, Version 2.0 (the "License"); 
 * you may not use this file except in compliance with the License. 
 * You may obtain a copy of the License at 
 * 
 * http://www.apache.org/licenses/LICENSE-2.0 
 * 
 * Unless required by applicable law or agreed to in writing, software 
 * distributed under the License is distributed on an "AS IS" BASIS, 
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. 
 * See the License for the specific language governing permissions and 
 * limitations under the License.
 * 
 **/
namespace WizardFramework
{
    /// <summary>
    /// Simple class to create a message with message type.
    /// </summary>
    public class WizardMessage
    {
        /// <summary>
        /// Constructor creating a regular message.
        /// </summary>
        /// <param name="message"></param>
        public WizardMessage(string message)
            : this(message, 0) { }

        /// <summary>
        /// Constructor creating a custom message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="type">The message type.</param>
        public WizardMessage(string message, MessageType type)
        {
            message_prop = message;
            messagetype_prop = type;
        }

        /// <summary>
        /// Return the message.
        /// </summary>
        public string Message { get { return message_prop; } }

        /// <summary>
        /// Return the message type.
        /// </summary>
        public MessageType Type { get { return messagetype_prop; } }

        /// <summary>
        /// Enumeration of message types.
        /// </summary>
        public enum MessageType
        {
            None,
            Information,
            Warning,
            Error
        }

        private MessageType messagetype_prop = MessageType.None;
        private string message_prop = null;
    }
}