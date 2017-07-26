using IFC4;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;
using IFC_server.Models;

namespace IFC_server.Controllers
{
	public class ProjectsController : Controller
	{
		private DocumentClient client;
		private readonly DbOptions dbOptions;

		public ProjectsController(IOptions<DbOptions> options)
		{
			dbOptions = options.Value;

			client = new DocumentClient(new Uri(dbOptions.EndPoint), dbOptions.AuthKey);
			CreateDatabaseIfNotExistsAsync().Wait();
			CreateCollectionIfNotExistsAsync().Wait();
			
			var testProject = new IfcProject();
			testProject.Name = new IfcLabel("Test Project");
			testProject.Description = new IfcText("A project to test IFC-dotnet-server.");

			client.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(dbOptions.Name, dbOptions.Collection), testProject).Wait();
		}

		public IActionResult Index()
		{
			ViewData["Title"] = "Projects";
			ViewData["Message"] = "Your application description page.";

			var queryOptions = new FeedOptions { MaxItemCount = -1 };

			// Query to get all the projects.
        	var projectQuery = this.client.CreateDocumentQuery<IfcProject>(
                UriFactory.CreateDocumentCollectionUri(dbOptions.Name, dbOptions.Collection), queryOptions)
                .Where(p=> p.Name.Value == "Test Project");

			foreach(var p in projectQuery)
			{
				Console.WriteLine("Found project: {0}", p.Name);
			}


			return View();
		}

		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}

		private async Task CreateDatabaseIfNotExistsAsync()
		{
			try
			{
				await client.ReadDatabaseAsync(UriFactory.CreateDatabaseUri(dbOptions.Name));
			}
			catch (DocumentClientException e)
			{
				if (e.StatusCode == System.Net.HttpStatusCode.NotFound)
				{
					await client.CreateDatabaseAsync(new Database { Id = dbOptions.Name });
				}
				else
				{
					throw;
				}
			}
		}

		private async Task CreateCollectionIfNotExistsAsync()
		{
			try
			{
				await client.ReadDocumentCollectionAsync(
					UriFactory.CreateDocumentCollectionUri(dbOptions.Name, dbOptions.Collection));
			}
			catch (DocumentClientException e)
			{
				if (e.StatusCode == System.Net.HttpStatusCode.NotFound)
				{
					await client.CreateDocumentCollectionAsync(
						UriFactory.CreateDatabaseUri(dbOptions.Name),
						new DocumentCollection { Id = dbOptions.Collection },
						new RequestOptions { OfferThroughput = 1000 });
				}
				else
				{
					throw;
				}
			}
		}
	}
}