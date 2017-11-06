﻿using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.AspNetCore.Mvc;
using System.Dynamic;
using System.IO;
using System.Threading.Tasks;
using System.Net.Http;


namespace mmria.server
{
	[Route("api/[controller]")]
	public class zipController: ControllerBase
	{
		public zipController ()
		{
		}

		[HttpGet]
        public async System.Threading.Tasks.Task<HttpResponseMessage> Get (string id)
		{
			HttpResponseMessage result = new HttpResponseMessage (System.Net.HttpStatusCode.NoContent);

			string request_string = Program.config_couchdb_url + "/_session";
			cURL session_curl = new cURL ("GET", null, request_string, null);

			if (!string.IsNullOrWhiteSpace(this.Request.Cookies["AuthSession"]))
			{
				string auth_session_value = this.Request.Cookies["AuthSession"];
				session_curl.AddHeader("Cookie", "AuthSession=" + auth_session_value);
				session_curl.AddHeader("X-CouchDB-WWW-Authenticate", auth_session_value);
			}


			string session_curl_resonse = await session_curl.executeAsync ();

			mmria.common.model.couchdb.session_response json_result = Newtonsoft.Json.JsonConvert.DeserializeObject<mmria.common.model.couchdb.session_response> (session_curl_resonse);

			
			if (json_result.userCTX.roles.Contains ("abstractor", StringComparer.OrdinalIgnoreCase))
			{

				var get_item_curl = new cURL ("GET", null, Program.config_couchdb_url + "/export_queue/" + id, null, Program.config_timer_user_name, Program.config_timer_password);
				string responseFromServer = await get_item_curl.executeAsync ();
				export_queue_item export_queue_item = Newtonsoft.Json.JsonConvert.DeserializeObject<export_queue_item> (responseFromServer);



				var path = System.IO.Path.Combine (System.Configuration.ConfigurationManager.AppSettings ["export_directory"], export_queue_item.file_name);
				result = new HttpResponseMessage (System.Net.HttpStatusCode.OK);
				var stream = new FileStream (path, FileMode.Open, FileAccess.Read);
				result.Content = new StreamContent (stream);
				result.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue ("application/octet-stream");

				export_queue_item.status = "Downloaded";

				Newtonsoft.Json.JsonSerializerSettings settings = new Newtonsoft.Json.JsonSerializerSettings ();
				settings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
				string object_string = Newtonsoft.Json.JsonConvert.SerializeObject (export_queue_item, settings); 
				var set_item_curl = new cURL ("PUT", null, Program.config_couchdb_url + "/export_queue/" + export_queue_item._id, object_string, Program.config_timer_user_name, Program.config_timer_password);
				responseFromServer = await set_item_curl.executeAsync ();
			}

		    return result;
		}

	}

/*
	class FileResult : IHttpActionResult
	{
	    private readonly string _filePath;
	    private readonly string _contentType;
	
	    public FileResult(string filePath, string contentType = null)
	    {
	        if (filePath == null) throw new ArgumentNullException("filePath");
	
	        _filePath = filePath;
	        _contentType = contentType;
	    }
	
	    public Task<HttpResponseMessage> ExecuteAsync(System.Threading.CancellationToken cancellationToken)
	    {
			var response = new HttpResponseMessage(System.Net.HttpStatusCode.OK)
	        {
	            Content = new StreamContent(File.OpenRead(_filePath))
	        };
	
	        var contentType = _contentType ?? System.Web.Http.MimeMapping.GetMimeMapping(Path.GetExtension(_filePath));
			response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(contentType);
	
	        return Task.FromResult(response);
	    }
	}*/
}

