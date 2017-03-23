﻿using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace mmria
{

	//http://stackoverflow.com/questions/21255725/webrequest-equivalent-to-curl-command
	public class cURL
	{
		public cURL (string method, string headers, string url, string pay_load)
		{
		}


		public string execute ()
		{
			//curl http://IP:PORT/my/path/to/endpoint -H 'Content-type:application/json' -d '[{...json data...}]'
			var url = "http://IP:PORT/my/path/to/endpoint";
			var jsonData = "[{...json data...}]";
			string response = null;
			using (var client = new WebClient())
			{
				client.Headers.Add("content-type", "application/json");
				response = client.UploadString(url, jsonData);
			}

			return response;
		}

		public static void PostJsonDataToApi(string jsonData)
		{
			string thePostBody = "";

			var httpWebRequest = (HttpWebRequest)WebRequest.Create("https://api.somewhere.com/v2/cases");
			httpWebRequest.ReadWriteTimeout = 100000; //this can cause issues which is why we are manually setting this
			httpWebRequest.ContentType = "application/json";
			httpWebRequest.Accept = "*/*";
			httpWebRequest.Method = "POST";
			httpWebRequest.Headers.Add("Authorization", "Basic ThisShouldbeBase64String"); // "Basic 4dfsdfsfs4sf5ssfsdfs=="
			using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
			{
				// we want to remove new line characters otherwise it will return an error
				jsonData= thePostBody.Replace("\n", ""); 
				jsonData= thePostBody.Replace("\r", "");
				streamWriter.Write(jsonData);
				streamWriter.Flush();
				streamWriter.Close();
			}

			try
			{
				HttpWebResponse resp = (HttpWebResponse)httpWebRequest.GetResponse();
				string respStr = new StreamReader(resp.GetResponseStream()).ReadToEnd();
				Console.WriteLine("Response : " + respStr); // if you want see the output
			}
			catch(Exception ex)
			{
				//process exception here   
			}

		}

		public mmria.common.metadata.app get_metadata(string URL)
		{
			mmria.common.metadata.app result = null;

			//string URL = this.mmria_url + "/api/metadata";
			//string urlParameters = "?api_key=123";
			string urlParameters = "";

			HttpClient client = new HttpClient();
			client.BaseAddress = new Uri(URL);

			// Add an Accept header for JSON format.
			client.DefaultRequestHeaders.Accept.Add(
				new MediaTypeWithQualityHeaderValue("application/json"));

			// List data response.
			HttpResponseMessage response = client.GetAsync(urlParameters).Result;  // Blocking call!
			if (response.IsSuccessStatusCode)
			{
				// Parse the response body. Blocking!
				result = response.Content.ReadAsAsync<mmria.common.metadata.app>().Result;
			}
			else
			{
				Console.WriteLine("{0} ({1})", (int)response.StatusCode, response.ReasonPhrase);
			}

			return result;
		}


		public System.Dynamic.ExpandoObject Get()
		{
			System.Console.WriteLine ("Recieved message.");
			string result = null;
			System.Dynamic.ExpandoObject json_result = null;
			try
			{

				//"2016-06-12T13:49:24.759Z"
				string request_string = "this.get_couch_db_url()" + "/metadata/2016-06-12T13:49:24.759Z";

				System.Net.WebRequest request = System.Net.WebRequest.Create(new Uri(request_string));

				request.PreAuthenticate = false;

				/*
				if(this.Request.Headers.Contains("Cookie") && this.Request.Headers.GetValues("Cookie").Count() > 0)
				{
					string[] cookie_set = this.Request.Headers.GetValues("Cookie").First().Split(';');
					for(int i = 0; i < cookie_set.Length; i++)
					{
						string[] auth_session_token = cookie_set[i].Split('=');
						if(auth_session_token[0].Trim() == "AuthSession" && auth_session_token[1] != "null")
						{
							request.Headers.Add("Cookie", "AuthSession=" + auth_session_token[1]);
							request.Headers.Add("X-CouchDB-WWW-Authenticate", auth_session_token[1]);
							break;
						}
					}
				}*/


				System.Net.WebResponse response = (System.Net.HttpWebResponse)request.GetResponse();
				System.IO.Stream dataStream = response.GetResponseStream ();
				System.IO.StreamReader reader = new System.IO.StreamReader (dataStream);
				result = reader.ReadToEnd ();

				json_result = Newtonsoft.Json.JsonConvert.DeserializeObject<System.Dynamic.ExpandoObject>(result, new  Newtonsoft.Json.Converters.ExpandoObjectConverter());

			}
			catch(Exception ex) 
			{
				Console.WriteLine (ex);
			}


			//return result;
			return json_result;
		}

		public mmria.common.model.couchdb.document_put_response set_case(string URL, string case_json)
		{
			mmria.common.model.couchdb.document_put_response result = null;

			//string URL = this.mmria_url + "/api/case";
			dynamic json_response = null;


			System.Net.CookieContainer cookieContainer = new System.Net.CookieContainer();
			//cookieContainer.Add(new System.Uri("http://localhost:12345"), new System.Net.Cookie("AuthSession", this.auth_token));

			var handler = new HttpClientHandler() { CookieContainer = cookieContainer };

			HttpClient client = new HttpClient(handler);
			client.BaseAddress = new Uri(URL);

			// Add an Accept header for JSON format.
			client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

			//client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Cookie", "AuthSession=" + this.auth_token);
			var content = new StringContent(case_json, System.Text.Encoding.UTF8, "application/json");

			// List data response.
			HttpResponseMessage response = client.PostAsync(URL, content).Result;  // Blocking call!
			if (response.IsSuccessStatusCode)
			{
				// Parse the response body. Blocking!
				json_response = response.Content.ReadAsAsync<mmria.common.model.couchdb.document_put_response>().Result;
			}
			else
			{
				Console.WriteLine("{0} ({1})", (int)response.StatusCode, response.ReasonPhrase);
			}

			if (json_response.ok == true)
			{
				if (json_response.auth_session != null && !string.IsNullOrWhiteSpace(json_response.auth_session))
				{
					//this.auth_token = json_response.auth_session;
				}
			}
			else
			{

			}

			return json_response;
		}

		//http://stackoverflow.com/questions/7929013/making-a-curl-call-in-c-sharp
		public async Task<string> nubian()
		{
			var client = new HttpClient();

			// Create the HttpContent for the form to be posted.
			var requestContent = new FormUrlEncodedContent(new [] {
				new System.Collections.Generic.KeyValuePair<string, string>("text", "This is a block of text"),
			});

			// Get the response.
			HttpResponseMessage response = await client.PostAsync(
				"http://api.repustate.com/v2/demokey/score.json",
				requestContent);

			// Get the response content.
			HttpContent responseContent = response.Content;

			// Get the stream of the content.
			using (var reader = new System.IO.StreamReader(await responseContent.ReadAsStreamAsync()))
			{
				// Write the output.
				return await reader.ReadToEndAsync();
			}
		}
	}
}
