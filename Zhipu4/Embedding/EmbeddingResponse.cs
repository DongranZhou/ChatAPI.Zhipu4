namespace Zhipu4
{
  public class EmbeddingResponse
  {
    public string model {get;set;}
    public string @object {get;set;}
    public bool success {get;set;}
    public ChatUsage usage {get;set;}
  }
}