using GPT_Assistant;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Speech.Synthesis;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using static GPT_Assistant.ResponseModels;
using Message = GPT_Assistant.Message;


namespace OpenAI
{
    class OpenAI_API
    {
        string apikey = "";

        public OpenAI_API(string apikey)
        {
            this.apikey = apikey;
        }

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

        public async Task<string> TTS(string filename, CancellationToken token = default)
        {
            string uri = @"https://api.openai.com/v1/audio/transcriptions";
            HttpResponseMessage response;

            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", apikey);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                TTSModelRequest request = new TTSModelRequest()
                {
                    model = "whisper-1",
                    language = "en",
                    file = Path.GetFullPath(filename),
                    Audio = File.OpenRead(filename)
                };

                await Task.Delay(1000);
                using var content = new MultipartFormDataContent();
                using var audioData = new MemoryStream();
                await request.Audio.CopyToAsync(audioData, token).ConfigureAwait(false);
                content.Add(new ByteArrayContent(audioData.ToArray()), "file", request.file);
                content.Add(new StringContent(request.model), "model");
                content.Add(new StringContent(request.language), "language");
                response = await client.PostAsync(uri, content);
                Console.WriteLine(response.StatusCode);

                request.Audio.Flush();
                request.Audio.Dispose();
                audioData.Close();
                audioData.Dispose();
            };

            TTSModelResponse model = (TTSModelResponse)await JsonSerializer.DeserializeAsync(response.Content.ReadAsStream(), typeof(TTSModelResponse));
            return model.text;
         }

    }
}
