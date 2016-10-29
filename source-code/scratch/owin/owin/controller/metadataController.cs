﻿using System;
using System.Collections.Generic;
using System.Web.Http;
using System.Linq;

namespace owin
{
	public class metadataController: ApiController 
	{ 
		private static string couchdb_url = null;

		static metadataController()
		{
			if (bool.Parse (System.Configuration.ConfigurationManager.AppSettings ["is_container_based"])) 
			{
				couchdb_url = System.Environment.GetEnvironmentVariable ("couchdb_url");
			} 
			else
			{
				couchdb_url = System.Configuration.ConfigurationManager.AppSettings ["couchdb_url"];
			}

		}


		public string Get() 
		{ 
			System.Console.WriteLine ("Recieved message.");
			string result = null;

			//"2016-06-12T13:49:24.759Z"
			string request_string = couchdb_url + "/metadata/2016-06-12T13:49:24.759Z";

			System.Net.WebRequest request = System.Net.WebRequest.Create(new Uri(request_string));

			request.PreAuthenticate = false;


			if(this.Request.Headers.Contains("Cookie") && this.Request.Headers.GetValues("Cookie").Count() > 0)
			{
				string[] auth_session_token = this.Request.Headers.GetValues("Cookie").First().Split('=');
				request.Headers.Add("Cookie", "AuthSession=" + auth_session_token[1]);
				//request.Headers.Add(this.Request.Headers.GetValues("Cookie").First(), "");
				request.Headers.Add("X-CouchDB-WWW-Authenticate", auth_session_token[1]);

			}


			System.Net.WebResponse response = (System.Net.HttpWebResponse)request.GetResponse();
			System.IO.Stream dataStream = response.GetResponseStream ();
			System.IO.StreamReader reader = new System.IO.StreamReader (dataStream);
			result = reader.ReadToEnd ();
			//owin.metadata.app json_result = Newtonsoft.Json.JsonConvert.DeserializeObject<owin.metadata.app >(result);



			return result;
			//return json_result;
		} 
		// GET api/values 
		//public IEnumerable<master_record> Get() 
		public string Get(string value) 
		{ 
			System.Console.WriteLine ("Recieved message\n { value }");

			return "done";
		} 

		private document_put_response PutDocument(string postUrl, string document)
		{
			document_put_response result = new document_put_response ();

			byte[] data = new System.Text.ASCIIEncoding().GetBytes(document);

			System.Net.WebRequest request = System.Net.WebRequest.Create("request_string");
			request.UseDefaultCredentials = true;
			request.Credentials = new System.Net.NetworkCredential("_username", "_password");
			request.Method = "PUT";
			request.ContentType = "text/json";
			request.ContentLength = data.Length;

			using (System.IO.StreamWriter streamWriter = new System.IO.StreamWriter(request.GetRequestStream()))
			{
				try
				{
					streamWriter.Write(document);
					streamWriter.Flush();
					streamWriter.Close();

					System.Net.HttpWebResponse httpResponse = (System.Net.HttpWebResponse)request.GetResponse();
					using (System.IO.StreamReader streamReader = new System.IO.StreamReader(httpResponse.GetResponseStream()))
					{
						string json_result = streamReader.ReadToEnd();
						streamReader.Close();

						result = Newtonsoft.Json.JsonConvert.DeserializeObject<document_put_response>(json_result);
					}
				}
				catch (System.Exception e)
				{
					//_logger.Error("Exception thrown when contacting service.", e);
					//_logger.ErrorFormat("Error posting document to {0}", postUrl);
				}
			}

			return result;
		}


		// GET api/values/5 
		public home_record Get(int id) 
		{ 
			return default(home_record); 
		} 

		// POST api/values 
		//[Route("api/metadata")]
		[HttpPost]
		public document_put_response Post() 
		{ 
			bool valid_login = false;
			owin.metadata.app metadata = null;

			document_put_response result = new document_put_response ();

			try
			{

				System.IO.Stream dataStream0 = this.Request.Content.ReadAsStreamAsync().Result;
				// Open the stream using a StreamReader for easy access.
				//dataStream0.Seek(0, System.IO.SeekOrigin.Begin);
				System.IO.StreamReader reader0 = new System.IO.StreamReader (dataStream0);
				// Read the content.
				string temp = reader0.ReadToEnd ();

				metadata = Newtonsoft.Json.JsonConvert.DeserializeObject<owin.metadata.app>(temp);

				//string metadata = DecodeUrlString(temp);
			}
			catch(Exception ex)
			{
				Console.WriteLine (ex);
			}

			/*
			try
			{
				string request_string = couchdb_url + "/_session";
				System.Net.WebRequest request = System.Net.WebRequest.Create(new System.Uri(request_string));

				request.PreAuthenticate = false;

				if(this.Request.Headers.Contains("Cookie") && this.Request.Headers.GetValues("Cookie").Count() > 0)
				{
					string[] auth_session_token = this.Request.Headers.GetValues("Cookie").First().Split('=');
					request.Headers.Add("Cookie", "AuthSession=" + auth_session_token[1]);
					//request.Headers.Add(this.Request.Headers.GetValues("Cookie").First(), "");
					request.Headers.Add("X-CouchDB-WWW-Authenticate", auth_session_token[1]);
				}

				System.Net.WebResponse response = (System.Net.HttpWebResponse)request.GetResponse();
				System.IO.Stream dataStream = response.GetResponseStream ();
				System.IO.StreamReader reader = new System.IO.StreamReader (dataStream);
				string responseFromServer = reader.ReadToEnd ();
				session_response json_result = Newtonsoft.Json.JsonConvert.DeserializeObject<session_response>(responseFromServer);

				valid_login = json_result.userCTX.name != null;
			}
			catch(Exception ex)
			{
				Console.WriteLine (ex);
			} 

			if (valid_login) 
			{*/

				try
				{
					Newtonsoft.Json.JsonSerializerSettings settings = new Newtonsoft.Json.JsonSerializerSettings ();
					settings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
					string object_string = Newtonsoft.Json.JsonConvert.SerializeObject(metadata, settings);

					string metadata_url = couchdb_url + "/metadata/"  + metadata._id;

					System.Net.WebRequest request = System.Net.WebRequest.Create(new System.Uri(metadata_url));
					request.Method = "PUT";
					request.ContentType = "application/json";
					request.ContentLength = object_string.Length;
					request.PreAuthenticate = false;

					if(this.Request.Headers.Contains("Cookie") && this.Request.Headers.GetValues("Cookie").Count() > 0)
					{
						string[] auth_session_token = this.Request.Headers.GetValues("Cookie").First().Split('=');
						request.Headers.Add("Cookie", "AuthSession=" + auth_session_token[1]);
						//request.Headers.Add(this.Request.Headers.GetValues("Cookie").First(), "");
						request.Headers.Add("X-CouchDB-WWW-Authenticate", auth_session_token[1]);
					}

					using (System.IO.StreamWriter streamWriter = new System.IO.StreamWriter(request.GetRequestStream()))
					{
						try
						{
							streamWriter.Write(object_string);
							streamWriter.Flush();
							streamWriter.Close();


							System.Net.WebResponse response = (System.Net.HttpWebResponse)request.GetResponse();
							System.IO.Stream dataStream = response.GetResponseStream ();
							System.IO.StreamReader reader = new System.IO.StreamReader (dataStream);
							string responseFromServer = reader.ReadToEnd ();

							result = Newtonsoft.Json.JsonConvert.DeserializeObject<document_put_response>(responseFromServer);
						}
						catch(Exception ex)
						{
							Console.WriteLine (ex);
						}
					}

					if (!result.ok) 
					{

					}

				}
				catch(Exception ex) 
				{
					Console.WriteLine (ex);
				}


			//}

			return result;
		} 


		//private void async process(System.Net.Http.)
		// PUT api/values/5 
		public void Put(int id, [FromBody]home_record value) 
		{ 
		} 

		// DELETE api/values/5 
		public void Delete(System.Guid  id) 
		{ 
		} 

		private static string DecodeUrlString(string url) {
			string newUrl;
			while ((newUrl = System.Uri.UnescapeDataString(url)) != url)
				url = newUrl;
			return newUrl;
		}
	} 
}

