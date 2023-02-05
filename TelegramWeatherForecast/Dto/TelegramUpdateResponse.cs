using System.Text.Json.Serialization;

namespace TelegramWeatherForecast.Dto
{
    internal class TelegramUpdateResponse
    {
        [JsonPropertyName("ok")]
        public bool Success { get; set; }

        [JsonPropertyName("result")]
        public TelegramUpdate[] Updates { get; set; }
    }

    internal class TelegramUpdate
    {
        [JsonPropertyName("update_id")]
        public int Id { get; set; }

        [JsonPropertyName("message")]
        public Message Message { get; set; }
    }

    internal class Message
    {
        [JsonPropertyName("text")]
        public string Text { get; set; }


        [JsonPropertyName("chat")]
        public Chat Chat { get; set; }
    }

    internal class Chat
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
    }
}
