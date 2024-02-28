using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using System.Text;

namespace QuizGenweb.OpenAI
{
    public class OpenAiClient
    {
        private const string uri = "https://api.openai.com/v1/chat/completions";
        private static readonly string token = Environment.GetEnvironmentVariable("OPENAI_KEY") ?? string.Empty;

        public static string GetopenAiDescription(string subject)
        {
            if(string.IsNullOrEmpty(token))
            {
                throw new ArgumentNullException("OpenAI token is not set");
            }

            string result = "";

            using HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var response = client.PostAsync(uri, GetContent(subject)).GetAwaiter().GetResult();

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine($"OpenAI response for {subject}:");
                result = GetResult(response);
            }

            return result;
        }

        private static StringContent GetContent(string subject)
        {
            string query = $"Who was {subject}?";

            var message = new
            {
                role = "user",
                content = query
            };

            var searchParams = new
            {
                model = "gpt-3.5-turbo",
                messages = new[] { message },
                max_tokens = 1500,
                temperature = 0.7
            };

            string json = JsonConvert.SerializeObject(searchParams);
            return new StringContent(json, Encoding.UTF8, "application/json");
        }

        private static string GetResult(HttpResponseMessage httpResponseMessage)
        {
            var result = "";
            var httpResult = httpResponseMessage.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            var rsp = JsonConvert.DeserializeObject(httpResult) as JToken;

            if (null != rsp)
            {
                var choices = rsp["choices"];
                result = choices[0]["message"]["content"].ToString();     
                Console.WriteLine($"{result}:");
                Console.WriteLine();
            }

            return result;
        }
    }
}
