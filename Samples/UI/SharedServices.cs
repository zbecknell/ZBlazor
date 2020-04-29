using BlazorStrap.Util;
using Microsoft.Extensions.DependencyInjection;

namespace UI
{
	public static class SharedServices
	{
		public static IServiceCollection AddSharedServices(this IServiceCollection services)
		{
			services.AddScoped<BlazorStrapInterop>();

			return services;
		}
	}
}
