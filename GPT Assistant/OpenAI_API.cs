using GPT_Assistant;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static GPT_Assistant.ResponseModels;
using Message = GPT_Assistant.Message;

namespace OpenAI
{
    class OpenAI_API
    {
        string apikey = "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";

        public async Task<string> Ask(string prompt)
        {
            string uri = "https://api.openai.com/v1/chat/completions";
            HttpResponseMessage response;

            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", apikey);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                Message mes = new Message() {role = "user", content = prompt };
                RequestModel request = new RequestModel()
                {
                    model = "gpt-3.5-turbo",
                    messages = new Message[] {mes}
                };
                HttpContent content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");
                response = await client.PostAsync(uri, content);
            };

            GPTResponseModel model = (GPTResponseModel)await JsonSerializer.DeserializeAsync(response.Content.ReadAsStream(), typeof(GPTResponseModel));
            return model.choices[0].message.content; 
        }

    }
}
