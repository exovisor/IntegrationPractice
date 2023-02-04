using System.Text;
using System.Text.Json;
using WeatherForecast.Dto;
using WeatherForecast.Models;

var apiKey = 
    Environment.OSVersion.Platform == PlatformID.Win32NT 
    ? Environment.GetEnvironmentVariable("OPENWEATHERMAP_KEY", EnvironmentVariableTarget.User)
    : Environment.GetEnvironmentVariable("OPENWEATHERMAP_KEY");

Console.OutputEncoding = Encoding.UTF8;

Console.WriteLine("Добро пожаловать в программу прогноза погоды!");
Console.WriteLine("Укажите ваш город, чтобы продолжить:");

var city = Console.ReadLine();
GeocodeResponse? geoDto = null;

// Fetch geo data
using (var client = new HttpClient())
{
    var response = await client.GetAsync(
        "http://api.openweathermap.org/geo/1.0/direct" +
        $"?q={city}" +
        $"&appid={apiKey}"
    );

    if (response.IsSuccessStatusCode)
    {
        var stringResponse = await response.Content.ReadAsStringAsync();
        var geocodeData = JsonSerializer.Deserialize<GeocodeResponse[]>(stringResponse);
        geoDto = geocodeData?.FirstOrDefault();
    }
}

if (geoDto == null)
{
    Console.WriteLine("Не удалось получить данные по вашему запросу. Выхожу из программы...");
    return;
}

// Show current forecast
CurrentWeather? currentWeather = null;
using (var client = new HttpClient())
{
    var response = await client.GetAsync(
        "https://api.openweathermap.org/data/2.5/weather" +
        $"?lat={geoDto.Latitude}" +
        $"&lon={geoDto.Longitude}" +
        $"&appid={apiKey}" +
        "&units=metric" +
        "&lang=ru"
    );

    if (response.IsSuccessStatusCode)
    {
        var stringResponse = await response.Content.ReadAsStringAsync();
        var weatherResponse = JsonSerializer.Deserialize<CurrentWeatherResponse>(stringResponse);
        if (weatherResponse != null)
        {
            currentWeather = new CurrentWeather(weatherResponse);
        }
    }
}

if (currentWeather == null)
{
    Console.WriteLine("При получении данных о погоде произошла ошибка. Выхожу из программы...");
    return;
}

Console.WriteLine("\nТекущая погода:");
Console.WriteLine(currentWeather);

// Show podcast for 4 days
HourlyPodcast? podcast = null;
using (var client = new HttpClient())
{
    var response = await client.GetAsync(
        "https://api.openweathermap.org/data/2.5/forecast" +
        $"?lat={geoDto.Latitude}" +
        $"&lon={geoDto.Longitude}" +
        $"&appid={apiKey}" +
        "&units=metric" +
        "&lang=ru"
    );

    if (response.IsSuccessStatusCode)
    {
        var stringResponse = await response.Content.ReadAsStringAsync();
        var podcastResponse = JsonSerializer.Deserialize<HourlyPodcastResponse>(stringResponse);
        if (podcastResponse != null)
        {
            podcast = new HourlyPodcast(podcastResponse);
        }
    }
}

if (podcast == null)
{
    Console.WriteLine("При получении данных о погоде произошла ошибка. Выхожу из программы...");
    return;
}

Console.WriteLine("\nПрогноз на ближайшие 4 дня:");
var daysData = podcast.Days;
foreach (var (day, data) in daysData.Skip(1).Take(4))
{
    Console.WriteLine(day.ToString("M") + ": " + data);
}

