using System;
using System.Collections.Generic;
using System.Text;

namespace UrGuide.Model.Messages
{
    public class ChatMessage
    {
        public string To { get; set; }
        public string Content { get; set; }
        public string ReferenceLink { get; set; }
    }
}
