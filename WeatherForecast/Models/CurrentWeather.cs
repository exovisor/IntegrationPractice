using WeatherForecast.Dto;
using WeatherForecast.Extensions;

namespace WeatherForecast.Models
{
    internal class CurrentWeather
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public double Temperature { get; set; }

        public double FeelsLike { get; set; }

        public int Pressure { get; set; }

        public int Humidity { get; set; }

        public double WindSpeed { get; set; }

        public int WindDegree { get; set; }

        public CurrentWeather() {
            Name = "Неизвестно";
            Description = string.Empty;
        }

        public CurrentWeather(CurrentWeatherResponse dto)
        {
            var weather = dto.weather[0];

            Name        = weather.main;
            Description = weather.description;

            Temperature = dto.main.temp;
            FeelsLike   = dto.main.feels_like;
            Pressure    = dto.main.pressure;
            Humidity    = dto.main.humidity;

            WindSpeed   = dto.wind.speed;
            WindDegree  = dto.wind.deg;
        }

        public string WindDirection
        {
            get
            {
                if (WindDegree >= 337 || WindDegree < 23)
                {
                    return "Северный";
                }
                else if (WindDegree >= 23 && WindDegree < 68)
                {
                    return "Северо-восточный";
                }
                else if (WindDegree >= 68 && WindDegree < 113)
                {
                    return "Восточный";
                }
                else if (WindDegree >= 113 && WindDegree < 158)
                {
                    return "Юго-восточный";
                }
                else if (WindDegree >= 158 && WindDegree < 203)
                {
                    return "Южный";
                }
                else if (WindDegree >= 203 && WindDegree < 248)
                {
                    return "Юго-западный";
                }
                else if (WindDegree >= 248 && WindDegree < 293)
                {
                    return "Западный";
                }
                else
                {
                    return "Северо-западный";
                }
            }
        }

        public override string ToString()
        {
            return $"{Description.Capitalize()} {(int)Temperature}°C (Ощущается как {(int)FeelsLike}°C)" +
                $"\nВлажность {Humidity}% | Давление {Pressure} мм рт.с. | Ветер {WindSpeed} м/c {WindDirection}";
        }
    }
}
