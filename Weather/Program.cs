using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;
using System.Security.Cryptography.X509Certificates;

class Programm
{
    private const string API_KEY = "X-Yandex-API-Key: 95f6e6a4-9025-4169-b772-538f1c069859";
    static async Task Main(string[] args)
    {  
        string weather = Menu();

        try
        {
            string result = await GetWeather(weather);
            Console.WriteLine($"Погода: {result}");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Ошибка: " + ex.Message);
        }
        
    }

    public static async Task<string> GetWeather(string weather)
    {
        using (var client = new HttpClient())
        {
            client.DefaultRequestHeaders.Add("X-Yandex-API-Key", API_KEY);

            var response = await client.GetAsync(weather);
            response.EnsureSuccessStatusCode(); // Проверяется статус ответа

            var data = JsonSerializer.Deserialize<WeatherResponse>(await response.Content.ReadAsStringAsync());

            if (data == null || data.fact == null)
                throw new Exception("Погода не найдена");
            
            return FormatWeatherInfo(data);
        }
    }

    public static string FormatWeatherInfo(WeatherResponse weatherData)
    {
        // Текущая погода
        string currentWeather = $"Текущая температура: {weatherData.fact.temp}°C\n" +
                                $"Ощущается как: {weatherData.fact.feels_like}°C\n" +
                                $"Состояние: {weatherData.fact.condition}\n" +
                                $"Влажность: {weatherData.fact.humidity}%\n" +
                                $"Давление: {weatherData.fact.pressure_mm} мм рт. ст.\n" +
                                $"Скорость ветра: {weatherData.fact.wind_speed} м/с ({weatherData.fact.wind_dir})\n";

        // Прогнозы
        string forecasts = "Прогнозы:\n";
        foreach (var forecast in weatherData.forecasts)
        {
            forecasts += $"Дата: {forecast.date}\n" +
                         $"Максимальная температура: {forecast.parts.day.temp_max}°C\n" +
                         $"Минимальная температура: {forecast.parts.day.temp_min}°C\n" +
                         $"Состояние: {forecast.parts.day.condition}\n" +
                         $"Вероятность осадков: {forecast.parts.day.prec_prob}%\n" +
                         new string('-', 30) + "\n";
        }

        return currentWeather + "\n" + forecasts;
    }

    public class WeatherResponse
    {
        public Fact fact { get; set; }
        public List<Forecast> forecasts { get; set; }
    }

    public class Fact
    {
        public int temp { get; set; } // Температура
        public int feels_like { get; set; } // Ощущаемая температура
        public string condition { get; set; } // Состояние
        public int humidity { get; set; } // Влажность
        public int pressure_mm { get; set; } // Давление
        public double wind_speed { get; set; } // Скорость ветра
        public string wind_dir { get; set; } // Направление ветра
    }

    public class Forecast
    {
        public string date { get; set; } // Дата
        public Parts parts { get; set; } // Части дня
    }

    public class Parts
    {
        public Day day { get; set; } // Данные на день
    }

    public class Day
    {
        public int temp_max { get; set; } // Максимальная температура
        public int temp_min { get; set; } // Минимальная температура
        public string condition { get; set; } // Состояние
        public int prec_prob { get; set; } // Вероятность осадков
    }

    public static string Menu()
    {
        Console.WriteLine("Добро пожаловать в прогноз погоды. Выберите город в котором хотите просмотреть погоду: ");
        Console.WriteLine("1 - Париж");
        Console.WriteLine("2 - Дубай");
        Console.WriteLine("3 - Амстердам");
        Console.WriteLine("4 - Мадрид");
        Console.WriteLine("5 - Рим");
        Console.WriteLine("6 - Лондон");
        Console.WriteLine("7 - Мюнхен");
        Console.WriteLine("8 - Берлин");
        Console.WriteLine("9 - Барселона");
        Console.WriteLine("10 - Нью-Йорк");
        Console.WriteLine("11 - Токио");
        Console.WriteLine("12 - Стамбул");
        Console.WriteLine("13 - Бангкок");
        Console.WriteLine("14 - Доха");
        Console.WriteLine("15 - Шарджа");
        Console.WriteLine("16 - Тель-Авив");
        Console.WriteLine("17 - Анталья");
        Console.WriteLine("18 - Дели");
        Console.WriteLine("19 - Каир");
        Console.WriteLine("20 - Иерусалим");

        int key = Convert.ToInt32(Console.ReadLine());

        var cities = new Dictionary<int, (string lat, string lon)>
        {
            { 1, ("48.8566", "2.3522") },   // Париж
            { 2, ("25.276987", "55.296249") }, // Дубай
            { 3, ("52.3676", "4.9041") },   // Амстердам
            { 4, ("40.4168", "-3.7038") },  // Мадрид
            { 5, ("41.9028", "12.4964") },  // Рим
            { 6, ("51.5074", "0.1278") },  // Лондон
            { 7, ("48.1351", "11.5820") },  // Мюнхен
            { 8, ("52.5200", "13.4050") },  // Берлин
            { 9, ("41.3851", "2.1734") },   // Барселона
            { 10, ("40.7128", "-74.0060") }, // Нью-Йорк
            { 11, ("35.6895", "139.6917") }, // Токио
            { 12, ("41.0082", "28.9784") }, // Стамбул
            { 13, ("13.7563", "100.5018") }, // Бангкок
            { 14, ("25.276987", "51.520008") }, // Доха
            { 15, ("25.3463", "55.4209") }, // Шарджа
            { 16, ("32.0853", "34.7818") }, // Тель-Авив
            { 17, ("36.8969", "30.7133") }, // Анталья
            { 18, ("28.6139", "77.2090") }, // Дели
            { 19, ("30.0444", "31.2357") }, // Каир
            { 20, ("31.7683", "35.2137") }  // Иерусалим
        };

        if (cities.TryGetValue(key, out var coordinates))
            return $"https://api.weather.yandex.ru/v2/forecast?lat={coordinates.lat}&lon={coordinates.lon}";
        else
            return " ";

    }

}