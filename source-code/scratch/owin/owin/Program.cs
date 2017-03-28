﻿using System;
using System.Linq;
using System.Collections.Generic;
using Owin;
using System.Net.Http;
using Swashbuckle.Application;
using System.Web.Http;
using Quartz;
using Quartz.Impl;

namespace mmria.server
{
	class Program
	{

		public static IList<string> JobInfoList;
		public static int NumberOfJobInfoList_Call_Count = 0;
		public static IList<DateTime> DateOfLastJobInfoList_Call;
		public static string Last_Seq = null;

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
				case "set_is_environment_based_true":
					System.Configuration.ConfigurationManager.AppSettings ["is_environment_based"] = "true";
					break;
				case "set_is_environment_based_false":
					System.Configuration.ConfigurationManager.AppSettings ["is_environment_based"] = "false";
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

			if (bool.Parse (System.Configuration.ConfigurationManager.AppSettings ["is_environment_based"]))
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

			if (bool.Parse (System.Configuration.ConfigurationManager.AppSettings ["is_environment_based"]))
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


			Microsoft.Owin.Hosting.WebApp.Start(url);            
			Console.WriteLine("Listening at " + url);


			//Common.Logging.ILog log = Common.Logging.LogManager.GetCurrentClassLogger();
			//log.Debug("Application_Start");
			Program.DateOfLastJobInfoList_Call = new List<DateTime>();

			mmria.server.model.check_for_changes_job icims_data_call_job = new mmria.server.model.check_for_changes_job();
			Program.JobInfoList = icims_data_call_job.GetJobInfo();
			Program.NumberOfJobInfoList_Call_Count++;
			Program.DateOfLastJobInfoList_Call.Add(DateTime.Now);

			StdSchedulerFactory sf = new StdSchedulerFactory();
			IScheduler sched = sf.GetScheduler();
			DateTimeOffset startTime = DateBuilder.NextGivenSecondDate(null, 15);

			//Quartz.Impl.Calendar.CronCalendar cronCal = new Quartz.Impl.Calendar.CronCalendar("0 * * * *");
			//sched.AddCalendar("HourlyCal", cronCal, true, true);

			// job1 will only fire once at date/time "ts"
			IJobDetail data_job = JobBuilder.Create<mmria.server.model.check_for_changes_job>()
				.WithIdentity("data_job", "group1")
				.Build();

			string cron_schedule = System.Configuration.ConfigurationManager.AppSettings["cron_schedule"];

			ITrigger trigger = (ITrigger)TriggerBuilder.Create()
														  .WithIdentity("trigger1", "group1")
														  .StartAt(startTime)
														  .WithCronSchedule(cron_schedule)
														  .Build();

			//trigger.RepeatCount = 1;
			//trigger.RepeatInterval = TimeSpan.FromMinutes(1);

			// schedule it to run!
			DateTimeOffset? ft = sched.ScheduleJob(data_job, trigger);
			//log.DebugFormat(data_job.Key + " will run at: " + ft);
			/*log.DebugFormat(data_job.Key +
                     " will run at: " + ft +
                     " and repeat: " + trigger..RepeatCount +
                     " times, every " + trigger.RepeatInterval.TotalSeconds + " seconds");*/


			//group1.data_job will run at: 1/11/2016 4:27:15 PM -05:00 and repeat: 0 times, every 0 seconds"

			sched.Start();





			if (bool.Parse (System.Configuration.ConfigurationManager.AppSettings ["is_environment_based"]))
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
				while (string.IsNullOrWhiteSpace(read_line) || read_line.ToLower () != "quit") 
				{
					read_line = Console.ReadLine();
				}
				System.Console.WriteLine ("Quit command recieved shutting down.");
			}
		}
	}



	public class Startup
	{
		public void Configuration(IAppBuilder app)
		{

			//app.Use(typeof(RequestSizeLimitingMiddleware), long.MaxValue);


			string url = null;

			if (bool.Parse (System.Configuration.ConfigurationManager.AppSettings ["is_environment_based"]))
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
					//c.DocumentFilter<mmria.server.swashbuckle.Document_Filter>();

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

			if (bool.Parse (System.Configuration.ConfigurationManager.AppSettings ["is_environment_based"]))
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

		}
			
	}
}
