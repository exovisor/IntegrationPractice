using WeatherForecast.Dto;
using WeatherForecast.Extensions;

namespace WeatherForecast.Models
{
    internal class HourlyPodcast
    {
        public Dictionary<DateTime, HourlyPodcastDay> Days { get; set; }

        public HourlyPodcast(HourlyPodcastResponse dto)
        {
            var days = dto.list.Select(entry => DateTime.Parse(entry.dt_txt).Date).Distinct().ToList();
            Days = new Dictionary<DateTime, HourlyPodcastDay>();

            foreach (var day in days)
            {
                var minTemp = double.MaxValue;
                var maxTemp = double.MinValue;
                string? description = null;

                foreach (var timestamp in dto.list)
                {
                    if (DateTime.Parse(timestamp.dt_txt).Date == day)
                    {
                        if (timestamp.main.temp_max > maxTemp) maxTemp = timestamp.main.temp_max;

                        if (timestamp.main.temp_min < minTemp) minTemp = timestamp.main.temp_min;

                        if (description == null)
                        {
                            description = timestamp.weather[0].description;
                        }
                    }
                }

                Days.Add(day, new HourlyPodcastDay
                {
                    MinTemperature = minTemp,
                    MaxTemperature = maxTemp,
                    Description = description?.Capitalize() ?? "Неизвестно"
                });
            }
        }
    }

    internal class HourlyPodcastDay
    {
        public double MinTemperature { get; set; }
        public double MaxTemperature { get; set; }
        public string Description { get; set; }

        public override string ToString()
        {
            return $"{Description} (Минимальная {(int)MinTemperature}°C - Максимальная {(int)MaxTemperature}°C)";
        }
    }
}
