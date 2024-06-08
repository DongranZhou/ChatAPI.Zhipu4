namespace Zhipu4
{
  public class ChatTool
  {
    public string type { get; set; }
    public ChatToolFunction function { get; set; }
    public ChatToolRetrieval retrieval { get; set; }
    public ChatToolWebSearch web_search { get; set; }
  }
}