using System;
using System.Collections.Generic;
using System.Web.Http;
using System.Linq;

namespace mmria.server
{
	public class metadataController: ApiController 
	{ 
		public metadataController()
		{
		}

        public async System.Threading.Tasks.Task<System.Dynamic.ExpandoObject> Get()
		{
			System.Console.WriteLine ("Recieved message.");
			string result = null;
			System.Dynamic.ExpandoObject json_result = null;
			try
			{

				//"2016-06-12T13:49:24.759Z"
				string request_string = this.get_couch_db_url() + "/metadata/2016-06-12T13:49:24.759Z";

				System.Net.WebRequest request = System.Net.WebRequest.Create(new Uri(request_string));

				request.PreAuthenticate = false;


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
				}


				System.Net.WebResponse response = await request.GetResponseAsync();
				//System.Net.WebResponse response = request.GetResponse() as System.Net.WebResponse;
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
		/*
		// GET api/values 
		//public IEnumerable<master_record> Get() 
		public System.Dynamic.ExpandoObject Get(string value) 
		{ 
			//System.Console.WriteLine ("Recieved message.");
			string result = null;

			//"2016-06-12T13:49:24.759Z"
			string request_string = this.get_couch_db_url() + "/metadata/" + value;

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

			System.Dynamic.ExpandoObject json_result = Newtonsoft.Json.JsonConvert.DeserializeObject<System.Dynamic.ExpandoObject>(result, new  Newtonsoft.Json.Converters.ExpandoObjectConverter());

			return json_result;
		} */


		// POST api/values 
		//[Route("api/metadata")]
		[HttpPost]
        public async System.Threading.Tasks.Task<mmria.common.model.couchdb.document_put_response> Post() 
		{ 
			//bool valid_login = false;
			mmria.common.metadata.app metadata = null;
			string object_string = null;
			mmria.common.model.couchdb.document_put_response result = new mmria.common.model.couchdb.document_put_response ();

			try
			{

				System.IO.Stream dataStream0 = await this.Request.Content.ReadAsStreamAsync();
				// Open the stream using a StreamReader for easy access.
				//dataStream0.Seek(0, System.IO.SeekOrigin.Begin);
				System.IO.StreamReader reader0 = new System.IO.StreamReader (dataStream0);
				// Read the content.
				string temp = reader0.ReadToEnd ();

				metadata = Newtonsoft.Json.JsonConvert.DeserializeObject<mmria.common.metadata.app>(temp);
				//System.Dynamic.ExpandoObject json_result = Newtonsoft.Json.JsonConvert.DeserializeObject<System.Dynamic.ExpandoObject>(result, new  Newtonsoft.Json.Converters.ExpandoObjectConverter());



				//string metadata = DecodeUrlString(temp);
			}
			catch(Exception ex)
			{
				Console.WriteLine (ex);
			}
				try
				{
					Newtonsoft.Json.JsonSerializerSettings settings = new Newtonsoft.Json.JsonSerializerSettings ();
					settings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
					object_string = Newtonsoft.Json.JsonConvert.SerializeObject(metadata, settings);

					string metadata_url = this.get_couch_db_url() + "/metadata/"  + metadata._id;

					System.Net.WebRequest request = System.Net.WebRequest.Create(new System.Uri(metadata_url));
					request.Method = "PUT";
					request.ContentType = "application/json";
					request.ContentLength = object_string.Length;
					request.PreAuthenticate = false;

					if(this.Request.Headers.Contains("Cookie") && this.Request.Headers.GetValues("Cookie").Count() > 0)
					{
						string[] cookie_set = this.Request.Headers.GetValues("Cookie").First().Split(';');
						for(int i = 0; i < cookie_set.Length; i++)
						{
							string[] auth_session_token = cookie_set[i].Split('=');
							if(auth_session_token[0].Trim() == "AuthSession")
							{
								request.Headers.Add("Cookie", "AuthSession=" + auth_session_token[1]);
								request.Headers.Add("X-CouchDB-WWW-Authenticate", auth_session_token[1]);
								break;
							}
						}
					}

					using (System.IO.StreamWriter streamWriter = new System.IO.StreamWriter(request.GetRequestStream()))
					{
						try
						{
							streamWriter.Write(object_string);
							streamWriter.Flush();
							streamWriter.Close();


							System.Net.WebResponse response = await request.GetResponseAsync();
							System.IO.Stream dataStream = response.GetResponseStream ();
							System.IO.StreamReader reader = new System.IO.StreamReader (dataStream);
							string responseFromServer = reader.ReadToEnd ();

							result = Newtonsoft.Json.JsonConvert.DeserializeObject<mmria.common.model.couchdb.document_put_response>(responseFromServer);

							if(response.Headers["Set-Cookie"] != null)
							{
								string[] set_cookie = response.Headers["Set-Cookie"].Split(';');
								string[] auth_array = set_cookie[0].Split('=');
								if(auth_array.Length > 1)
								{
									string auth_session_token = auth_array[1];
									result.auth_session = auth_session_token;
								}
								else
								{
									result.auth_session = "";
								}
							}



						//System.Threading.Tasks.Task.Run( new Action(()=> { var f = new GenerateSwaggerFile(); System.IO.File.WriteAllText(Program.config_file_root_folder + "/api-docs/api.json", f.generate(metadata)); }));
							
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
				
			return result;
		} 


		private string get_couch_db_url()
		{
            string result = Program.config_couchdb_url;

			return result;
		}
	} 
}

