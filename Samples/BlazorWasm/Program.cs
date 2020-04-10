using Blazor.Extensions.Logging;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using System.Threading.Tasks;
using UI;

namespace BlazorWasm
{
	public class Program
	{
		public static async Task Main(string[] args)
		{
            Log.Logger = new LoggerConfiguration()
                .WriteTo.BrowserConsole()
				.MinimumLevel.Debug()
                .CreateLogger();

            var builder = WebAssemblyHostBuilder.CreateDefault(args);
			builder.RootComponents.Add<App>("app");

            builder.Services.AddLogging(builder =>
            {
                builder.AddBrowserConsole().SetMinimumLevel(LogLevel.Trace);
                builder.AddSerilog().SetMinimumLevel(LogLevel.Trace);
            });

            builder.Services.AddBaseAddressHttpClient();

			await builder.Build().RunAsync();
		}
	}
}
