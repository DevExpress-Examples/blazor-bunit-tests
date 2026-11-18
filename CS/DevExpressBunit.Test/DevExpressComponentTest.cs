using Bunit;
using DxTestProject.Data;
using Microsoft.Extensions.DependencyInjection;

namespace DevExpressBunit.Test {
    public static class BUnitTestContextExtensions {
        public static void AddDevExpressBlazorTesting(this BunitContext testContext) {
            testContext.Services.AddSingleton<WeatherForecastService>();
            testContext.Services.AddDevExpressBlazor();
            testContext.JSInterop.Mode = JSRuntimeMode.Loose;
        }
    }
}
