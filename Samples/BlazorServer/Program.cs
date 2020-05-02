using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace BlazorServer
{
	public class Program
	{
		public static void Main(string[] args)
		{
			Log.Logger = new LoggerConfiguration()
				.WriteTo.Console(Serilog.Events.LogEventLevel.Debug)
				.MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning)
				.MinimumLevel.Debug()
				.CreateLogger();

			CreateHostBuilder(args).Build().Run();
		}

		public static IHostBuilder CreateHostBuilder(string[] args) =>
			Host.CreateDefaultBuilder(args)
				.ConfigureWebHostDefaults(webBuilder =>
				{
					webBuilder
						.UseStartup<Startup>();
				});
	}
}
