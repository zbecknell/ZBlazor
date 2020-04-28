using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System.Diagnostics;
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
				.MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning)
				.MinimumLevel.Verbose()
				.CreateLogger();

			Serilog.Debugging.SelfLog.Enable(message => Debug.WriteLine(message));

			var builder = WebAssemblyHostBuilder.CreateDefault(args);

			builder.RootComponents.Add<App>("app");

			builder.Services.AddLogging(options =>
			{
				options.AddSerilog();
			});

			await builder.Build().RunAsync();
		}
	}
}
