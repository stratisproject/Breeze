﻿using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Swagger;

namespace Breeze.Api
{
	public class Startup
	{
		public Startup(IHostingEnvironment env)
		{
			var builder = new ConfigurationBuilder()
				.SetBasePath(env.ContentRootPath)
				.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
				.AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
				.AddEnvironmentVariables();
			Configuration = builder.Build();			
		}

		public IConfigurationRoot Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			// Add framework services.
			services.AddMvc()
				// add serializers for NBitcoin objects
				.AddJsonOptions(options => NBitcoin.JsonConverters.Serializer.RegisterFrontConverters(options.SerializerSettings))
				.AddControllers(services);

			services.AddApiVersioning(options =>
			{				
				options.DefaultApiVersion = new ApiVersion(1, 0);
			});

			// Register the Swagger generator, defining one or more Swagger documents
			services.AddSwaggerGen(setup =>
			{
				setup.SwaggerDoc("v1", new Info { Title = "Breeze.Api", Version = "v1" });

				// FIXME: prepopulates the version in the URL of the Swagger UI found at http://localhost:5000/swagger
				// temporary needed until Swashbuckle supports it out-of-the-box  
				setup.DocInclusionPredicate((version, apiDescription) =>
				{
					apiDescription.RelativePath = apiDescription.RelativePath.Replace("v{version}", version);					
					var versionParameter = apiDescription.ParameterDescriptions.SingleOrDefault(p => p.Name == "version");
					if (versionParameter != null)
					{
						apiDescription.ParameterDescriptions.Remove(versionParameter);
					}

					return true;
				});
			});
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
		{
			loggerFactory.AddConsole(this.Configuration.GetSection("Logging"));
			loggerFactory.AddDebug();

			app.UseMvc();

			// Enable middleware to serve generated Swagger as a JSON endpoint.
			app.UseSwagger();

			// Enable middleware to serve swagger-ui (HTML, JS, CSS etc.), specifying the Swagger JSON endpoint.
			app.UseSwaggerUI(c =>
			{
				c.SwaggerEndpoint("/swagger/v1/swagger.json", "Breeze.Api V1");
			});
		}
	}
}
