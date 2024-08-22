
using System.Collections.Generic;

namespace Zhipu4
{
    public class ChatRequest<T>
    {
        public string model { get; set; } = "glm-4";
        public List<T> messages { get; set; } = new List<T>();
        public string request_id { get; set; }
        public bool? do_sample { get; set; }
        public bool? stream { get; set; }
        public float? temperature { get; set; } = 0.7f;
        public float? top_p { get; set; } = 0.7f;
        public int? max_tokens { get; set; } = 1024;
        public List<string> stop { get; set; }
        public List<ChatTool> tools { get; set; }
        public string tool_choice { get; set; }
        public ChatMeta meta { get; set; }
    }
}
