﻿using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;
using Owin.WebSocket.Extensions;
using System.Net.Http;
using Swashbuckle.Application;
using vtortola.WebSockets;



using System.Web.Http;

namespace owin
{

	// http://owin.org/extensions/owin-WebSocket-Extension-v0.4.0.htm
	using WebSocketAccept = Action<System.Collections.Generic.IDictionary<string, object>, // options
	Func<System.Collections.Generic.IDictionary<string, object>, Task>>; // callback
	using WebSocketCloseAsync =
		Func<int /* closeStatus */,
	string /* closeDescription */,
	CancellationToken /* cancel */,
	Task>;
	using WebSocketReceiveAsync =
		Func<ArraySegment<byte> /* data */,
	CancellationToken /* cancel */,
	Task<Tuple<int /* messageType */,
	bool /* endOfMessage */,
	int /* count */>>>;
	using WebSocketSendAsync =
		Func<ArraySegment<byte> /* data */,
	int /* messageType */,
	bool /* endOfMessage */,
	CancellationToken /* cancel */,
	Task>;
	using WebSocketReceiveResult = Tuple<int, // type
	bool, // end of message?
	int>; // count


	class MainClass
	{
		


		// http://www.asp.net/aspnet/samples/owin-katana

		//http://localhost:12345
		//http://localhost:12345/api/values
		//http://localhost:12345/api/geocode?street_address=123 main street&city=los angeles&state=ca&zip=90007
		//http://localhost:12345/api/session?userid=mmrds&password=mmrds
		//http://localhost:12345/api/session?userid=user1&password=password
		//http://localhost:12345/api/session
		//http://localhost:12345/swagger/docs/v1
		//http://localhost:12345/sandbox/index

		static void Main(string[] args)
		{
			for(int i = 0; i < args.Length; i++)
			{
				switch (args [i].ToLower()) 
				{
				case "set_is_container_based_true":
					System.Configuration.ConfigurationManager.AppSettings ["is_container_based"] = "true";
					break;
				case "set_is_container_based_false":
					System.Configuration.ConfigurationManager.AppSettings ["is_container_based"] = "false";
					break;

				default:
					Console.WriteLine ("unsued command line argument: Arg[{0}] = [{1}]", i, args [i]);
					break;
				}

			}

			#if (FILE_WATCHED)
			Console.WriteLine ("starting file watch.");
			WatchFiles.StartWatch();
			#endif

			//data_access da = new data_access ();
			//da.login ("mmrds","mmrds");
			#if (DEBUG)

			if (bool.Parse (System.Configuration.ConfigurationManager.AppSettings ["is_container_based"]))
			{
				/*
				System.Environment.SetEnvironmentVariable("geocode_api_key","7c39ae93786d4aa3adb806cb66de51b8");
				System.Environment.SetEnvironmentVariable("couchdb_url", "http://localhost:5984");
				System.Environment.SetEnvironmentVariable("web_site_url", "http://localhost:12345");
				System.Environment.SetEnvironmentVariable("file_root_folder", "/vagrant/source-code/scratch/owin/owin/psk/app");
				*/
			}
			#endif


			string url = null;

			if (bool.Parse (System.Configuration.ConfigurationManager.AppSettings ["is_container_based"]))
			{
				System.Console.WriteLine ("using Environment");
				System.Console.WriteLine ("geocode_api_key: {0}", System.Environment.GetEnvironmentVariable ("geocode_api_key"));
				System.Console.WriteLine ("couchdb_url: {0}", System.Environment.GetEnvironmentVariable ("couchdb_url"));
				System.Console.WriteLine ("web_site_url: {0}", System.Environment.GetEnvironmentVariable ("web_site_url"));
				System.Console.WriteLine ("file_root_folder: {0}", System.Environment.GetEnvironmentVariable ("file_root_folder"));


				url = System.Environment.GetEnvironmentVariable ("web_site_url");
			}
			else
			{
				System.Console.WriteLine("using AppSettings");
				System.Console.WriteLine("geocode_api_key: {0}", System.Configuration.ConfigurationManager.AppSettings["geocode_api_key"]);
				System.Console.WriteLine("couchdb_url: {0}", System.Configuration.ConfigurationManager.AppSettings["couchdb_url"]);
				System.Console.WriteLine("web_site_url: {0}", System.Configuration.ConfigurationManager.AppSettings["web_site_url"]);
				System.Console.WriteLine("file_root_folder: {0}", System.Configuration.ConfigurationManager.AppSettings["file_root_folder"]);

				url = System.Configuration.ConfigurationManager.AppSettings["web_site_url"];
			}

			CancellationTokenSource cancellation = new CancellationTokenSource();

			var endpoint = new System.Net.IPEndPoint(System.Net.IPAddress.Any, 8005);
			vtortola.WebSockets.WebSocketListener server = new vtortola.WebSockets.WebSocketListener(endpoint);
			var rfc6455 = new vtortola.WebSockets.Rfc6455.WebSocketFactoryRfc6455(server);
			server.Standards.RegisterStandard(rfc6455);
			server.Start();

			Console.WriteLine("Web Socket Echo Server started at " + endpoint.ToString());

			var task = Task.Run(() => AcceptWebSocketClientsAsync(server, cancellation.Token));
			/**/




			Microsoft.Owin.Hosting.WebApp.Start(url);            
			Console.WriteLine("Listening at " + url);


			if (bool.Parse (System.Configuration.ConfigurationManager.AppSettings ["is_container_based"]))
			{
				bool stay_on_till_power_fail = true;

				while(stay_on_till_power_fail)
				{

				}
			}
			else
			{
				//http://odetocode.com/blogs/scott/archive/2014/02/10/building-a-simple-file-server-with-owin-and-katana.aspx
				string read_line = Console.ReadLine();
				while (read_line.ToLower () != "quit") 
				{
					read_line = Console.ReadLine();
				}
				System.Console.WriteLine ("Quit command recieved shutting down.");
			}


		}

		static async Task AcceptWebSocketClientsAsync(WebSocketListener server, CancellationToken token)
		{
			while (!token.IsCancellationRequested)
			{
				try
				{
					var ws = await server.AcceptWebSocketAsync(token);
					System.Console.WriteLine ("Connected " + ws??"Null");
					if (ws != null)
						Task.Run(()=>HandleConnectionAsync(ws, token));
				}
				catch(Exception aex)
				{
					System.Console.WriteLine ("Error Accepting clients: " + aex.GetBaseException().Message);
				}
			}
			System.Console.WriteLine ("Server Stop accepting clients");
		}

		static async Task HandleConnectionAsync(WebSocket ws, CancellationToken cancellation)
		{
			try
			{
				while (ws.IsConnected && !cancellation.IsCancellationRequested)
				{
					String msg = await ws.ReadStringAsync(cancellation).ConfigureAwait(false);
					System.Console.WriteLine ("Message: " + msg);
					ws.WriteString(msg);
				}
			}
			catch (Exception aex)
			{
				System.Console.WriteLine ("Error Handling connection: " + aex.GetBaseException().Message);
				try { ws.Close(); }
				catch { }
			}
			finally
			{
				ws.Dispose();
			}
		}

	}



	public class Startup
	{
		public void Configuration(IAppBuilder app)
		{

			string url = null;

			if (bool.Parse (System.Configuration.ConfigurationManager.AppSettings ["is_container_based"]))
			{
				url = System.Environment.GetEnvironmentVariable ("web_site_url");
			}
			else
			{
				url = System.Configuration.ConfigurationManager.AppSettings["web_site_url"];
			}


			#if DEBUG
			app.UseErrorPage();
			#endif
			//app.UseWelcomePage("/");
			// Configure Web API for self-host. 
			HttpConfiguration config = new HttpConfiguration(); 

			config.Routes.MapHttpRoute( 
				name: "DefaultApi", 
				routeTemplate: "api/{controller}/{id}", 
				defaults: new { id = RouteParameter.Optional } 
			); 

			/*
			config.Routes.MapHttpRoute( 
				name: "DynamicApi", 
				routeTemplate: "api-docs/{controller}/{id}", 
				defaults: new { id = RouteParameter.Optional } 
			); */

			config.Formatters.Clear();
			config.Formatters.Add(new  System.Net.Http.Formatting.JsonMediaTypeFormatter());
			config.Formatters.JsonFormatter.SerializerSettings =
				new Newtonsoft.Json.JsonSerializerSettings
			{
				ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver()//,
				//NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore
			};

			//https://github.com/NSwag/NSwag/wiki/Middlewares
			//app.UseSwaggerUi(typeof(Startup).Assembly, new SwaggerUiOwinSettings());
			//app.UseWebApi(config);


			config
				.EnableSwagger("docs/{apiVersion}/swagger", c => { 
					
					c.SingleApiVersion("v1", "MMRIA data API");

					c.RootUrl(req =>
						req.RequestUri.GetLeftPart(UriPartial.Authority) +
						req.GetRequestContext().VirtualPathRoot.TrimEnd('/'));

					
					c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First()); 
					//c.DocumentFilter<owin.swashbuckle.Document_Filter>();

				})
				.EnableSwaggerUi("sandbox/{*assetPath}");
			/*
			config
				.EnableSwagger(c =>
					{
						//c.RootUrl(req => url);
						c.RootUrl(req =>
							req.RequestUri.GetLeftPart(UriPartial.Authority) +
							req.GetRequestContext().VirtualPathRoot.TrimEnd('/'));

						c.Schemes(new[] { "http", "https" });

						c.SingleApiVersion("v1", "Swashbuckle.Dummy")
							.Description("A sample API for testing and prototyping Swashbuckle features")
							.TermsOfService("Some terms")
							.Contact(cc => cc
								.Name("Some contact")
								.Url("http://tempuri.org/contact")
								.Email("some.contact@tempuri.org"))
							.License(lc => lc
								.Name("Some License")
								.Url("http://tempuri.org/license"));
					})
				.EnableSwaggerUi("sandbox/{*assetPath}");*/


			app.UseWebApi(config); 
			//

			//app.UseSwagger();

			string root = null;

			if (bool.Parse (System.Configuration.ConfigurationManager.AppSettings ["is_container_based"]))
			{
				root = System.Environment.GetEnvironmentVariable("file_root_folder");
			}
			else
			{
				root = System.Configuration.ConfigurationManager.AppSettings["file_root_folder"];
			}

			var fileSystem = new Microsoft.Owin.FileSystems.PhysicalFileSystem(root);
			var options = new Microsoft.Owin.StaticFiles.FileServerOptions()
			{
				EnableDirectoryBrowsing = true,
				EnableDefaultFiles = true,
				DefaultFilesOptions = { DefaultFileNames = {"index.html"}},
				FileSystem = fileSystem,
				StaticFileOptions = { ContentTypeProvider = new CustomContentTypeProvider() 
				}
			};
			app.UseFileServer (options);
			/*
			*/

			// websocket - start
			//For static routes http://foo.com/ws use MapWebSocketRoute and attribute the WebSocketConnection with [WebSocketRoute('/ws')]
			//app.MapWebSocketRoute<owin.websocket.MyWebSocket>();

					//For static routes http://foo.com/ws use MapWebSocketRoute

			//app.Use(UpgradeToWebSockets, "/echo");
			/*
			app.Use(async (context, next) =>
				{
					if (context.WebSockets.IsWebSocketRequest)
					{
						WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync();
						await EchoWebSocket(webSocket);
					}
					else
					{
						await next();
					}
				});*/

			//app.MapWebSocketRoute<owin.websocket.MyWebSocket>("/echo");

			//For dynamic routes where you may want to capture the URI arguments use a Regex route
			//app.MapWebSocketPattern<MyWebSocket>("/captures/(?<capture1>.+)/(?<capture2>.+)");

			// websocket - end
			/**/
		}
			
	}
}
