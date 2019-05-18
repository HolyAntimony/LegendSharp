using System;
using System.Collections.Generic;
using System.Text;

namespace LegendSharp
{
    public class ChatMessage
    {
        public string author;
        public string message;
        public string authorId;
        public Guid uuid;

        public ChatMessage(string author, string message, string authorId, Guid uuid)
        {
            this.author = author;
            this.message = message;
            this.authorId = authorId;
            this.uuid = uuid;
        }

        public ChatMessage(string author, string message, string authorId) : this(author, message, authorId, Guid.NewGuid())
        {
            
        }
    }
}
