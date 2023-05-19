using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPT_Assistant
{
    public class ResponseModels
    {

        public class GPTResponseModel
        {
            public string id { get; set; }
            public string _object { get; set; }
            public int created { get; set; }
            public Choice[] choices { get; set; }
            public Usage usage { get; set; }
        }

        public class Usage
        {
            public int prompt_tokens { get; set; }
            public int completion_tokens { get; set; }
            public int total_tokens { get; set; }
        }

        public class Choice
        {
            public int index { get; set; }
            public Message message { get; set; }
            public string finish_reason { get; set; }
        }

        public class Message
        {
            public string role { get; set; }
            public string content { get; set; }
        }

    }



    public class RequestModel
    {
        public string model { get; set; }
        public Message[] messages { get; set; }
    }

    public class Message
    {
        public string role { get; set; }
        public string content { get; set; }
    }



    public class TTSModelRequest
    {
        public Stream Audio { get; set; }
        public string file { get; set; }
        public string model { get; set; }
        public string language { get; set; }
    }


    public class TTSModelResponse
    {
        public string text { get; set; }
    }


}
