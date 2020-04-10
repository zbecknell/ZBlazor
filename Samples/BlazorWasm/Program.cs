using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Extensions.Logging;
using System;
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
				.MinimumLevel.Verbose()
				.CreateLogger();

			Serilog.Debugging.SelfLog.Enable(message => Console.WriteLine(message));

			var builder = WebAssemblyHostBuilder.CreateDefault(args);

			builder.RootComponents.Add<App>("app");

			builder.Services.AddLogging(logBuilder =>
			{
				logBuilder.AddSerilog().SetMinimumLevel(LogLevel.Trace);

				logBuilder.Services.AddSingleton<SerilogLoggerProvider>();

				logBuilder.Services.Add(ServiceDescriptor.Singleton(typeof(Microsoft.Extensions.Logging.ILogger), typeof(SerilogLogger)));
			});

			builder.Services.AddBaseAddressHttpClient();

			await builder.Build().RunAsync();
		}
	}
}
