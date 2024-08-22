using System.Collections.Generic;

namespace Zhipu4
{
  public class ChatChoice
  {
    public int? index { get; set; }
    public string finish_reason { get; set; }
    public AssisantMessage message { get; set; }
    public List<ChatToolCall> tool_calls { get; set; } = new List<ChatToolCall>();
    public ChatUsage usage{ get; set; }

    public AssisantMessage delta { get; set; }
  }
}