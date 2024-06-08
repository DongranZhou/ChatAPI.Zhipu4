
namespace Zhipu4
{
    public class ChatResponse
    {
        public string id { get; set; }
        public long created { get; set; }
        public string model { get; set; } = "glm-4";
        public List<ChatChoice> choices { get; set; } = new List<ChatChoice>();
        public ChatUsage usage { get; set; }
    }
}