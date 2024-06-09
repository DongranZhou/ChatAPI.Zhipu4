using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace Zhipu4
{
    public class ChatAPI
    {
        public static T DeserializeObject<T>(string json)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            };
            return JsonConvert.DeserializeObject<T>(json, settings);
        }
        public static string SerializeObject(object obj)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            };
            return JsonConvert.SerializeObject(obj, settings);
        }

        public string BaseUrl { get; set; } = "https://open.bigmodel.cn/api/paas/v4/";
        public string APIKey { get; set; } = "";
        public ChatAPI() { }
        public ChatAPI(string baseUrl, string apiKey)
        {
            BaseUrl = baseUrl;
            APIKey = apiKey;
        }
        public async Task<EmbeddingResponse> GetEmbeddings(EmbeddingRequest req, CancellationToken cancellationToken = new CancellationToken())
        {
            string url = BaseUrl.TrimEnd('/') + "/embeddings";
            using (HttpClient client = new HttpClient())
            {
                string json = SerializeObject(req);
                HttpContent content = new StringContent(json);

                string token = GenerateToken();
                client.DefaultRequestHeaders.Add("Authorization", token);
                content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                HttpResponseMessage httpResponse = await client.PostAsync(url, content, cancellationToken);
                if (!cancellationToken.IsCancellationRequested)
                {
                    if (httpResponse.StatusCode == HttpStatusCode.OK)
                    {
                        var response = await httpResponse.Content.ReadAsStringAsync(cancellationToken);
                        return DeserializeObject<EmbeddingResponse>(response);
                    }
                }
                return new EmbeddingResponse();
            }
        }
        public async Task<ChatResponse> Chat<T>(ChatRequest<T> req, CancellationToken cancellationToken = new CancellationToken())
        {
            using (HttpClient client = new HttpClient())
            {
                string url = BaseUrl.TrimEnd('/') + "/chat/completions";
                req.stream = false;
                string json = SerializeObject(req);
                HttpContent content = new StringContent(json);
                content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                string[] sp = APIKey.Split('.');
                TimeSpan ts = DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0, 0);
                long timestamp = Convert.ToInt64(ts.TotalMilliseconds) * 1000;
                string token = Encode(new ChatJwtHeader(), new ChatJwtPayload { api_key = sp[0], exp = timestamp + 60 * 60 * 100, timestamp = timestamp }, Encoding.UTF8.GetBytes(sp[1]));
                client.DefaultRequestHeaders.Add("Authorization", token);
                HttpResponseMessage httpResponse = await client.PostAsync(url, content, cancellationToken);
                if (httpResponse.StatusCode == HttpStatusCode.OK)
                {
                    string str = await httpResponse.Content.ReadAsStringAsync(cancellationToken);
                    return DeserializeObject<ChatResponse>(str);
                }
                throw new Exception(httpResponse.ToString());
            }
        }
        public async IAsyncEnumerable<ChatResponse> ChatStream<T>(ChatRequest<T> req, CancellationToken cancellationToken = new CancellationToken())
        {
            using (HttpClient client = new HttpClient())
            {
                string url = BaseUrl.TrimEnd('/') + "/chat/completions";
                req.stream = true;
                string json = SerializeObject(req);
                HttpContent content = new StringContent(json);
                content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

                string token = GenerateToken();
                //client.DefaultRequestHeaders.Add("Accept", "text/event-stream");
                client.DefaultRequestHeaders.Add("Authorization", token);

                HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post, url) { Content = content };
                HttpResponseMessage httpResponse = await client.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
                if (!cancellationToken.IsCancellationRequested)
                {
                    if (httpResponse.IsSuccessStatusCode)
                    {
                        using (var responseStream = await httpResponse.Content.ReadAsStreamAsync(cancellationToken))
                        {
                            if (responseStream.CanRead)
                            {
                                using (StreamReader reader = new StreamReader(responseStream))
                                {
                                    while (!reader.EndOfStream && !cancellationToken.IsCancellationRequested)
                                    {
                                        string line = await reader.ReadLineAsync();
                                        string head = "data:";
                                        if (line.IndexOf(head) == 0)
                                        {
                                            string data = line.Substring(head.Length);
                                            if (Regex.IsMatch(data, "\\s?\\[DONE\\]"))
                                            {
                                                yield break;
                                            }
                                            else
                                            {
                                                ChatResponse rep = DeserializeObject<ChatResponse>(data);
                                                yield return rep;
                                            }
                                        }
                                    }

                                }
                            }
                            throw new Exception(httpResponse.ToString());
                        }
                    }
                    throw new Exception(httpResponse.ToString());
                }
            }
        }
        public async Task<ImageResponse> ImageGen(ImageRequest req, CancellationToken cancellationToken = new CancellationToken())
        {
            using (HttpClient client = new HttpClient())
            {
                string url = BaseUrl.TrimEnd('/') + "/images/generations";
                string json = SerializeObject(req);
                HttpContent content = new StringContent(json);
                content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                string[] sp = APIKey.Split('.');
                TimeSpan ts = DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0, 0);
                long timestamp = Convert.ToInt64(ts.TotalMilliseconds) * 1000;
                string token = Encode(new ChatJwtHeader(), new ChatJwtPayload { api_key = sp[0], exp = timestamp + 60 * 60 * 100, timestamp = timestamp }, Encoding.UTF8.GetBytes(sp[1]));
                client.DefaultRequestHeaders.Add("Authorization", token);
                HttpResponseMessage httpResponse = await client.PostAsync(url, content, cancellationToken);
                if (httpResponse.StatusCode == HttpStatusCode.OK)
                {
                    string str = await httpResponse.Content.ReadAsStringAsync(cancellationToken);
                    return DeserializeObject<ImageResponse>(str);
                }
                throw new Exception(httpResponse.ToString());
            }
        }
        public string GenerateToken()
        {
            string[] sp = APIKey.Split('.');
            TimeSpan ts = DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            long timestamp = Convert.ToInt64(ts.TotalMilliseconds) * 1000;
            return Encode(new ChatJwtHeader(), new ChatJwtPayload { api_key = sp[0], exp = timestamp + 60 * 60 * 100, timestamp = timestamp }, Encoding.UTF8.GetBytes(sp[1]));
        }
        public string Encode(ChatJwtHeader header, ChatJwtPayload payload, byte[] key)
        {
            var segments = new List<string>();

            byte[] headerBytes = Encoding.UTF8.GetBytes(SerializeObject(header));
            byte[] payloadBytes = Encoding.UTF8.GetBytes(SerializeObject(payload));

            segments.Add(Base64UrlEncode(headerBytes));
            segments.Add(Base64UrlEncode(payloadBytes));

            var stringToSign = string.Join(".", segments.ToArray());

            var bytesToSign = Encoding.UTF8.GetBytes(stringToSign);

            using (var sha = new System.Security.Cryptography.HMACSHA256(key))
            {
                byte[] signature = sha.ComputeHash(bytesToSign);
                segments.Add(Base64UrlEncode(signature));
            }
            return string.Join(".", segments.ToArray());
        }
        string Base64UrlEncode(byte[] input)
        {
            var output = Convert.ToBase64String(input);
            output = output.Split('=')[0]; // Remove any trailing '='s
            output = output.Replace('+', '-'); // 62nd char of encoding
            output = output.Replace('/', '_'); // 63rd char of encoding
            return output;
        }
    }
}
