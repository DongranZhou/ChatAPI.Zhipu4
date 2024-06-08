namespace Zhipu4
{
    public class ChatJwtPayload
    {
        public string api_key;
        public long exp;
        public long timestamp;
        public ChatJwtPayload() { }
        public ChatJwtPayload(string api_key, long exp, long timestamp)
        {
            this.api_key = api_key;
            this.exp = exp;
            this.timestamp = timestamp;
        }
    }
}