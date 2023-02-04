using System.Text.Json.Serialization;

namespace WeatherForecast.Dto
{
    internal class GeocodeResponse
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("lat")]
        public double Latitude { get; set; }

        [JsonPropertyName("lon")]
        public double Longitude { get; set; }

        [JsonPropertyName("country")]
        public string CountryCode { get; set; }

        public override string ToString()
        {
            return $"[{CountryCode}] {Name} ({Latitude}, {Longitude})";
        }
    }
}
