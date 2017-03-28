using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Breeze.Api
{
	public class Program
	{
		public static void Main(string[] args)
		{
			Initialize();
		}

		public static void Initialize(IEnumerable<ServiceDescriptor> services = null)
		{
			var host = new WebHostBuilder()
				.UseKestrel()
				.UseContentRoot(Directory.GetCurrentDirectory())
				.UseIISIntegration()
				.ConfigureServices(collection =>
				{
					if (services == null)
					{
						return;
					}

					foreach (var service in services)
					{
						collection.Add(service);
					}
				})
				.UseStartup<Startup>()
				.Build();

			host.Run();
		}
	}
}
