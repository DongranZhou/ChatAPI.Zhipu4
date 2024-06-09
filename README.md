# ChatAPI.Zhipu4

智谱AI 调用

## 使用

### 对话

```csharp
  Zhipu4.ChatAPI chat = new Zhipu4.ChatAPI
  {
    APIKey = ""
  };

  Zhipu4.ChatRequest req = new Zhipu4.ChatRequest
  {
    messages = new List<Zhipu4.ChatMessage>{
      new Zhipu4.ChatMessage{
        role = "user",
        content = "你好"
      }
    }
  };
  Zhipu4.ChatResponse rep = await chat.Chat(req);
  Console.WriteLine(rep.choices[0].message.content);
```

### 对话（流式）

```csharp
  Zhipu4.ChatAPI chat = new Zhipu4.ChatAPI
  {
    APIKey = ""
  };

  Zhipu4.ChatRequest req = new Zhipu4.ChatRequest
  {
    messages = new List<Zhipu4.ChatMessage>{
      new Zhipu4.ChatMessage{
        role = "user",
        content = "你好"
      }
    }
  };
  await foreach (Zhipu4.ChatResponse rep in chat.ChatStream(req))
  {
    Console.WriteLine(rep.choices[0].delta?.content);
  }
```

### 图像生成

```csharp
  Zhipu4.ChatAPI chat = new Zhipu4.ChatAPI
  {
    APIKey = ""
  };

  Zhipu4.ImageRequest req = new Zhipu4.ImageRequest
  {
    prompt = "一只可爱的小猫咪"
  };
  Zhipu4.ImageResponse rep = await chat.ImageGen(req);

  Console.WriteLine(rep.data[0].url);
```

### 拟人角色对话

```csharp
  Zhipu4.ChatAPI chat = new Zhipu4.ChatAPI
  {
    APIKey = ""
  };

  Zhipu4.ChatRequest req = new Zhipu4.ChatRequest
  {
    model = "charglm-3",
    messages = new List<Zhipu4.ChatMessage>{
      new Zhipu4.ChatMessage{
        role = "user",
        content = "你好"
      }
    },
    meta = new Zhipu4.ChatMeta
    {
      user_info = "我是陆星辰，是一个男性，是一位知名导演，也是苏梦远的合作导演。我擅长拍摄音乐题材的电影。苏梦远对我的态度是尊敬的，并视我为良师益友。",
      bot_info = "苏梦远，本名苏远心，是一位当红的国内女歌手及演员。在参加选秀节目后，凭借独特的嗓音及出众的舞台魅力迅速成名，进入娱乐圈。她外表美丽动人，但真正的魅力在于她的才华和勤奋。苏梦远是音乐学院毕业的优秀生，善于创作，拥有多首热门原创歌曲。除了音乐方面的成就，她还热衷于慈善事业，积极参加公益活动，用实际行动传递正能量。在工作中，她对待工作非常敬业，拍戏时总是全身心投入角色，赢得了业内人士的赞誉和粉丝的喜爱。虽然在娱乐圈，但她始终保持低调、谦逊的态度，深得同行尊重。在表达时，苏梦远喜欢使用“我们”和“一起”，强调团队精神。",
      bot_name = "苏梦远",
      user_name = "陆星辰"
    }
  };
  await foreach (Zhipu4.ChatResponse rep in chat.ChatStream(req))
    Console.WriteLine(rep.choices[0].delta?.content);
```

### 图像对话

```csharp
  Zhipu4.ChatAPI chat = new Zhipu4.ChatAPI
  {
    APIKey = APIKey
  };

  Zhipu4.ChatRequest<Zhipu4.VisionMessage> req = new Zhipu4.ChatRequest<Zhipu4.VisionMessage>
  {
    model = "glm-4v",
    messages = new List<Zhipu4.VisionMessage>{
      new Zhipu4.VisionMessage{
        role = "user",
        content = new List<Zhipu4.ChatVisionContent>{
          new Zhipu4.ChatVisionContent{
            type = "text",
            text = "描述一下图片内容"
          },
          new Zhipu4.ChatVisionContent{
            type = "image_url",
            image_url = new Zhipu4.ImageInfo{
              url = "http://***/***.jpg"
            }
          }
        }
      }
    }
  };
  await foreach (Zhipu4.ChatResponse rep in chat.ChatStream(req))
  {
    Console.WriteLine(rep.choices[0].delta?.content);
  }
```
