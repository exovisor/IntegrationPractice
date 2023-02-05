using System.Text;
using System.Text.Json;
using TelegramWeatherForecast.Dto;
using TelegramWeatherForecast.Models;

var openWeatherMapKey =
    Environment.OSVersion.Platform == PlatformID.Win32NT
    ? Environment.GetEnvironmentVariable("OPENWEATHERMAP_KEY", EnvironmentVariableTarget.User)
    : Environment.GetEnvironmentVariable("OPENWEATHERMAP_KEY");

var botToken =
    Environment.OSVersion.Platform == PlatformID.Win32NT
    ? Environment.GetEnvironmentVariable("TELEGRAM_WEATHER_BOT_TOKEN", EnvironmentVariableTarget.User)
    : Environment.GetEnvironmentVariable("TELEGRAM_WEATHER_BOT_TOKEN");

const string GREETING_TEXT = "Привет! Я учебный бот, который может рассказать тебе о погоде в интересующем тебя городе. Чтобы начать просто напиши мне название города (например: Санкт-Петербург)";

Console.OutputEncoding = Encoding.UTF8;

Console.WriteLine("Запускаю телеграм бота...");

var telegramUrl = $"https://api.telegram.org/bot{botToken}/";
var offset = 0;

while (true)
{   
    var updates = await GetUpdatesAsync(telegramUrl, offset);
    if (updates != null)
    {
        foreach (var update in updates)
        {
            Console.WriteLine($"Получено новое сообщение: {update.Message.Text}; Чат: {update.Message.Chat.Id}");
            var chatId = update.Message.Chat.Id;
            var request = update.Message.Text;

            switch (request)
            {
                case "/start":
                    await SendMessageAsync(telegramUrl, chatId, GREETING_TEXT);
                    break;
                default:
                    var forecast = await GetWeatherForCityAsync(openWeatherMapKey, request);
                    await SendMessageAsync(telegramUrl, chatId, forecast);
                    break;
            }

            // Update offset
            var updateId = update.Id;
            if (updateId > offset)
            {
                offset = updateId;
            }
        }

        offset++;

        await Task.Delay(1000);
    }
}

static async Task<TelegramUpdate[]> GetUpdatesAsync(string url, int offset)
{
    using var httpClient = new HttpClient();
    var response = await httpClient.GetAsync($"{url}getUpdates?offset={offset}");
    if (response.IsSuccessStatusCode)
    {
        var stringResponse = await response.Content.ReadAsStringAsync();
        if (stringResponse == null) return null;

        var data = JsonSerializer.Deserialize<TelegramUpdateResponse>(stringResponse);

        if (data.Success && data.Updates.Length > 0)
        {
            return data.Updates;
        }
    }
    return null;
}

static async Task SendMessageAsync(string url, long chatId, string text)
{
    using var httpClient = new HttpClient();
    var response = await httpClient.GetAsync($"{url}sendMessage?chat_id={chatId}&text={text}");
}

static async Task<string> GetWeatherForCityAsync(string apiKey, string city)
{
    GeocodeResponse? geoDto = null;
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

    if (geoDto != null)
    {
        using var client = new HttpClient();
        Console.WriteLine($"Fetching weather forecast for {city} (lat:{geoDto.Latitude}, lon:{geoDto.Longitude})");

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
                var currentWeather = new CurrentWeather(weatherResponse);
                return $"Текущая погода в {city}:\n" +
                    currentWeather.ToString();
            }
        }
    }

    return "Во время запроса произошла ошибка. Убедитесь что не допустили ошибки в названии города или попробуйте позже.";
}