using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Bunit;
using DevExpress.Blazor.bUnit.Internal;
using DevExpress.Blazor.Internal;
using DxTestProject.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Bunit.Extensions.WaitForHelpers;

namespace DevExpressBunit.Test {
    public static class BUnitTestContextExtensions {
        public static void AddDevExpressBlazorTesting(this TestContext testContext) {
            testContext.Services.AddSingleton<WeatherForecastService>();
            testContext.Services.TryAddScoped<ScriptsProvider, ScriptsProvider>();
            testContext.Services.TryAddScoped<LicenseRenderer, LicenseRenderer>();
            testContext.Services.TryAddScoped<IDropDownSettingsProvider, DropDownSettingsProvider>();
            testContext.Services.TryAddScoped<IEnvironmentInfoFactory, MockEnvironmentInfoFactory>();
            testContext.Services.TryAddScoped<IEnvironmentInfo, MockEnvironmentInfo>();
            testContext.Services.TryAddScoped<ISvgImagesLoader, FakeSvgImagesLoader>();
            testContext.Services.TryAddScoped<IGlobalOptionsService, GlobalOptionsService>();
            testContext.Services.AddOptions();
            testContext.Services.AddLogging();
            testContext.Services.TryAddComponentRequiredServices();
            testContext.JSInterop.ConfigureJSInterop();
        }

        public static async Task WaitForAssertionAsync(this IRenderedFragmentBase renderedFragment, Action assertion, TimeSpan? timeout = null) {
            using var waiter = new WaitForAssertionHelper(renderedFragment, assertion, timeout);
            await waiter.WaitTask;
        }

        static void ConfigureJSInterop(this BunitJSInterop interop) {
            interop.Mode = JSRuntimeMode.Loose;
            var rootModule = interop.SetupModule("./_content/DevExpress.Blazor/dx-blazor.js");
            rootModule.Mode = JSRuntimeMode.Strict;
            rootModule.Setup<DeviceInfo>("getDeviceInfo", _ => true).SetResult(new DeviceInfo(false));
        }


        sealed class FakeSvgImagesLoader : ISvgImagesLoader {
            bool ISvgImagesLoader.RequirePreprocessing => false;
            static readonly RenderFragment _emptyRenderFragment = b => { };
            RenderFragment ISvgImagesLoader.GetLoaderRenderFragment(string spriteUrl, out bool useShortName) {
                useShortName = false;
                return _emptyRenderFragment;
            }
        }

        public sealed class MockEnvironmentInfoFactory : IEnvironmentInfoFactory {
            readonly IEnvironmentInfo _cached;

            public MockEnvironmentInfoFactory(bool isWasm = false) {
                _cached = new MockEnvironmentInfo(isWasm);
            }

            public IEnvironmentInfo CreateEnvironmentInfo() => _cached;
        }

        public class MockIJSRuntime : IJSRuntime, IEnvironmentInfoFactory {
            public MockIJSRuntime() {
                EnvironmentInfo = new MockEnvironmentInfo();
            }
            readonly List<IDisposable> _preventedFromGC = new List<IDisposable>();

            public IEnvironmentInfo EnvironmentInfo { get; }

            public List<string> InvokeHistory { get; } = new List<string>();

            public async ValueTask<TValue> InvokeAsync<TValue>(string identifier, object[] args) {
                InvokeHistory.Add(identifier);
                _preventedFromGC.AddRange(args.OfType<IDisposable>().Except(_preventedFromGC));
                if (typeof(TValue) == typeof(ApiScheme))
                    return (TValue)(object)await EnvironmentInfo.ApiScheme;
                if (typeof(TValue) == typeof(DeviceInfo))
                    return (TValue)(object)await EnvironmentInfo.DeviceInfo;
                return default;
            }

            public ValueTask<TValue> InvokeAsync<TValue>(string identifier, CancellationToken cancellationToken, object[] args) {
                return InvokeAsync<TValue>(identifier, args);
            }

            public IEnvironmentInfo CreateEnvironmentInfo() {
                return EnvironmentInfo;
            }
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

            Task<ApiScheme> IEnvironmentInfo.ApiScheme => Task.FromResult(new ApiScheme(true));

            public virtual Task<DeviceInfo> DeviceInfo => Task.FromResult(new DeviceInfo(false) {
                ClientMinutesOffset = DateTimeOffset.Now.Offset.TotalMinutes,
                HasAccurateInput = true
            });

            public Task InitializeRuntime() {
                return Task.CompletedTask;
            }
        }
    }
}
