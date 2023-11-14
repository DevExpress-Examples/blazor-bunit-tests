using System;
using System.Globalization;
using System.Threading.Tasks;
using Bunit;
using DevExpress.Blazor.Internal;
using DxTestProject.Data;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DevExpressBunit.Test
{
    public static class BUnitTestContextExtensions {
        public static TestContext AddDevExpressBlazorTesting(this TestContext testContext) {
            testContext.Services.TryAddScoped<IEnvironmentInfoFactory, MockEnvironmentInfoFactory>();
            testContext.Services.TryAddScoped<IEnvironmentInfo, MockEnvironmentInfo>();
            testContext.Services.TryAddScoped<ISvgImagesLoader, FakeSvgImagesLoader>();
            testContext.Services.AddSingleton<WeatherForecastService>();
            testContext.Services.AddOptions();
            testContext.Services.AddLogging();
            testContext.Services.TryAddComponentRequiredServices();
            testContext.JSInterop.ConfigureJSInterop();
            return testContext;
        }

        public static BunitJSInterop ConfigureJSInterop(this BunitJSInterop interop) {
            interop.Mode = JSRuntimeMode.Loose;
            var rootModule = interop.SetupModule("./_content/DevExpress.Blazor/dx-blazor.js");
            rootModule.Mode = JSRuntimeMode.Strict;
            rootModule.Setup<DeviceInfo>("getDeviceInfo", _ => true).SetResult(new DeviceInfo(false));
            return interop;
        }
    }
    sealed class FakeSvgImagesLoader : ISvgImagesLoader {
        static readonly RenderFragment _emptyRenderFragment = b => { };
        RenderFragment ISvgImagesLoader.GetLoaderRenderFragment(string spriteUrl, out bool useShortName) {
            useShortName = false;
            return _emptyRenderFragment;
        }
    }
    sealed class MockEnvironmentInfoFactory : IEnvironmentInfoFactory {
        readonly IEnvironmentInfo _cached;

        public MockEnvironmentInfoFactory(bool isWasm = false) {
            _cached = new MockEnvironmentInfo(isWasm);
        }

        public IEnvironmentInfo CreateEnvironmentInfo() => _cached;
    }

    public class MockEnvironmentInfo : IEnvironmentInfo {
        public bool IsWasm { get; }
        public CultureInfo CurrentCulture { get; }
        public DateTime Now { get; }
        public static readonly DateTime DateTimeNow = DateTime.Now.Date;

        public MockEnvironmentInfo(bool isWasm = false, CultureInfo currentCulture = default, DateTime? dateTime = default) {
            IsWasm = isWasm;
            CurrentCulture = currentCulture ?? CultureInfo.CurrentCulture;
            Now = dateTime ?? DateTimeNow;
        }

        public DateTime GetDateTimeNow() {
            return Now;
        }
        public DateTime GetDateTimeUtcNow() {
            return DateTime.UtcNow;
        }
        public Task InitializeRuntime() {
            return Task.CompletedTask;
        }

        Task<ApiScheme> IEnvironmentInfo.ApiScheme => Task.FromResult(new ApiScheme(true));
        Task<DeviceInfo> IEnvironmentInfo.DeviceInfo => Task.FromResult(new DeviceInfo(false) {
            ClientMinutesOffset = DateTimeOffset.Now.Offset.TotalMinutes
        });
    }
}