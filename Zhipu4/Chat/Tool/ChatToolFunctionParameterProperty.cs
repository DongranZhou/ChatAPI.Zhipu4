using System.Collections.Generic;

namespace Zhipu4
{
  public class ChatToolFunctionParameterProperty
  {
    public string type { get; set; } = "string";
    public List<string> @enum { get; set; }
    public string description { get; set; }
  }
}