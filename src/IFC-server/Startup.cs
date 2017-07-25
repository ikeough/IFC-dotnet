using IFC4;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IFC_server
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddMvc();
			services.AddDbContext<IfcDbContext>(opt => opt.UseInMemoryDatabase("IFC"));
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env)
		{
			var context = app.ApplicationServices.GetService<IfcDbContext>();
			AddTestData(context);

			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}
			else
			{
				app.UseExceptionHandler("/Home/Error");
			}

			app.UseStaticFiles();

			app.UseMvc(routes =>
			{
				routes.MapRoute(
					name: "default",
					template: "{controller=Home}/{action=Index}/{id?}");
			});
		}

		private static void AddTestData(IfcDbContext context)
		{
			var testProject = new IfcProject();
			testProject.Name = new IfcLabel("Test Project");
			testProject.Description = new IfcText("A project to test IFC-dotnet-server.");

			context.Projects.Add(testProject);

			context.SaveChanges();
		}
	}
}
