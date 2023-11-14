namespace DxTestProject.Data
{
    public class WeatherForecastService
    {
        public WeatherForecast[] GetForecastAsync()
        {
            return new WeatherForecast[] {
                new WeatherForecast() { Date = new DateTime(2020, 05, 11), TemperatureC = 20,
                                        Precipitation = 5},
                new WeatherForecast() { Date = new DateTime(2020, 05, 12), TemperatureC = 22,
                                        Precipitation = 1.2},
                new WeatherForecast() { Date = new DateTime(2020, 05, 13), TemperatureC = 18,
                                        Precipitation = 0.8},
                new WeatherForecast() { Date = new DateTime(2020, 05, 14), TemperatureC = 19,
                                        Precipitation = 0.5},
                new WeatherForecast() { Date = new DateTime(2020, 05, 15), TemperatureC = 14,
                                        Precipitation = 3.3},
                new WeatherForecast() { Date = new DateTime(2020, 05, 16), TemperatureC = 15,
                                        Precipitation = 1.7},
                new WeatherForecast() { Date = new DateTime(2020, 05, 17), TemperatureC = 18,
                                        Precipitation = 1},
                new WeatherForecast() { Date = new DateTime(2020, 05, 18), TemperatureC = 24,
                                        Precipitation = 0.5},
                new WeatherForecast() { Date = new DateTime(2020, 05, 19), TemperatureC = 21,
                                        Precipitation = 4.4},
                new WeatherForecast() { Date = new DateTime(2020, 05, 20), TemperatureC = 20,
                                        Precipitation = 8.5}
            };


        }
    }
}