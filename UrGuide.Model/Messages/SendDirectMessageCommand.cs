using System;
using System.Collections.Generic;
using System.Text;

namespace UrGuide.Model.Messages
{
    public class SendDirectMessageCommand
    {
        public string To { get; set; }
        public string ToName { get; set; }
        public string Subject { get; set; }
        public string Content { get; set; }
        public string Link { get; set; }
        public string LinkText { get; set; }
    }
}
