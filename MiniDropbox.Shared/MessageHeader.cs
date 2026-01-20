using System;
using System.Text.Json;

namespace MiniDropbox.Shared
{
    public class MessageHeader
    {
        public CommandType Command { get; set; }
        public string? PayloadJson { get; set; }

        public string ToJson()
        {
            return JsonSerializer.Serialize(this);
        }

        public static MessageHeader? FromJson(string json)
        {
            return JsonSerializer.Deserialize<MessageHeader>(json);
        }
    }
}