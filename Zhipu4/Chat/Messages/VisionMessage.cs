
using System.Collections.Generic;

namespace Zhipu4
{
    public class VisionMessage
    {
        public string role { get; set; }
        public List<ChatVisionContent> content { get; set; }
    }
}