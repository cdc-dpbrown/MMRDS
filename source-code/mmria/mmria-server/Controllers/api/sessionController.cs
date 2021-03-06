using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using mmria.common.model.couchdb;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

//https://wiki.apache.org/couchdb/Session_API

namespace mmria.server
{

    [Route("api/[controller]")]
    public class sessionController: ControllerBase
	{

		//{"ok":true,"userCtx":{"name":null,"roles":[]},"info":{"authentication_db":"_users","authentication_handlers":["oauth","cookie","default"]}}
		//{"ok":true,"userCtx":{"name":"mmrds","roles":["_admin"]},"info":{"authentication_db":"_users","authentication_handlers":["oauth","cookie","default"],"authenticated":"cookie"}}

		// GET api/values 
		//public IEnumerable<master_record> Get() 
        [HttpGet]
		public  async System.Threading.Tasks.Task<IEnumerable<session_response>> Get() 
		{ 
			try
			{
				string request_string = Program.config_couchdb_url + "/_session";
				System.Net.WebRequest request = System.Net.WebRequest.Create(new Uri(request_string));

				request.PreAuthenticate = false;


                if (!string.IsNullOrWhiteSpace(this.Request.Cookies["AuthSession"]))
                {
                    string auth_session_value = this.Request.Cookies["AuthSession"];
                    request.Headers.Add("Cookie", "AuthSession=" + auth_session_value);
                    request.Headers.Add("X-CouchDB-WWW-Authenticate", auth_session_value);
                }


				System.Net.WebResponse response = await request.GetResponseAsync();
				System.IO.Stream dataStream = response.GetResponseStream ();
				System.IO.StreamReader reader = new System.IO.StreamReader (dataStream);
				string responseFromServer = reader.ReadToEnd ();
				session_response json_result = Newtonsoft.Json.JsonConvert.DeserializeObject<session_response>(responseFromServer);

				if(response.Headers["Set-Cookie"] != null)
				{
					this.Response.Headers.Add("Set-Cookie", response.Headers["Set-Cookie"]);
					string[] set_cookie = response.Headers["Set-Cookie"].Split(';');
					string[] auth_array = set_cookie[0].Split('=');
					if(auth_array.Length > 1)
					{
						string auth_session_token = auth_array[1];
						json_result.auth_session = auth_session_token;
					}
					else
					{
						json_result.auth_session = "";
					}
				}

				/*
		< HTTP/1.1 200 OK
		< Set-Cookie: AuthSession=YW5uYTo0QUIzOTdFQjrC4ipN-D-53hw1sJepVzcVxnriEw;
		< Version=1; Path=/; HttpOnly
		> ...
		<
		{"ok":true}*/
	
				session_response[] result =  new session_response[] 
				{ 
					json_result
				}; 

				return result;

			}
			catch(Exception ex)
			{
				Console.WriteLine (ex);

			} 

			return null;
		}


		// GET api/values 
		//public IEnumerable<master_record> Get() 
		//public System.Net.Http.HttpResponseMessage Get
		[AllowAnonymous] 
		[HttpPut]
        [HttpPost]
		public async System.Threading.Tasks.Task<IEnumerable<login_response>> Post
		(
            [FromBody] Post_Request_Struct Post_Request

		) 
		{ 

            //Post_Request_Struct Post_Request = new Post_Request_Struct();

			/*
	HOST="http://127.0.0.1:5984"
	> curl -vX POST $HOST/_session -H 'Content-Type: application/x-www-form-urlencoded' -d 'name=anna&password=secret'
*/
			try
			{
                string post_data = string.Format ("name={0}&password={1}", Post_Request.userid, Post_Request.password);
				byte[] post_byte_array = System.Text.Encoding.ASCII.GetBytes(post_data);


				//string request_string = "http://mmrds:mmrds@localhost:5984/_session";
				string request_string = Program.config_couchdb_url + "/_session";
				System.Net.WebRequest request = System.Net.WebRequest.Create(new Uri(request_string));
				//request.UseDefaultCredentials = true;

				request.PreAuthenticate = false;
				//request.Credentials = new System.Net.NetworkCredential("mmrds", "mmrds");
				request.Method = "POST";
				request.ContentType = "application/x-www-form-urlencoded";
				request.ContentLength = post_byte_array.Length;

				using (System.IO.Stream stream = request.GetRequestStream())
				{
					stream.Write(post_byte_array, 0, post_byte_array.Length);
				}/**/

				System.Net.WebResponse response = (System.Net.HttpWebResponse)request.GetResponse();

				System.IO.Stream dataStream = response.GetResponseStream ();

				// Open the stream using a StreamReader for easy access.
				System.IO.StreamReader reader = new System.IO.StreamReader (dataStream);
				// Read the content.
				string responseFromServer = reader.ReadToEnd ();

				login_response json_result = Newtonsoft.Json.JsonConvert.DeserializeObject<login_response>(responseFromServer);

				login_response[] result =  new login_response[] 
				{ 
					json_result
				}; 






				this.Response.Headers.Add("Set-Cookie", response.Headers["Set-Cookie"]);

				string[] set_cookie = response.Headers["Set-Cookie"].Split(';');
				string[] auth_array = set_cookie[0].Split('=');
				if(auth_array.Length > 1)
				{
					string auth_session_token = auth_array[1];
					result[0].auth_session = auth_session_token;
				}
				else
				{
					result[0].auth_session = "";
				}

				//{"ok":true,"userCtx":{"name":null,"roles":[]},"info":{"authentication_db":"_users","authentication_handlers":["oauth","cookie","default"]}}
				if (json_result.ok && !string.IsNullOrWhiteSpace(json_result.name)) 
				{
					const string Issuer = "https://contoso.com";

					var claims = new List<Claim>();
					claims.Add(new Claim(ClaimTypes.Name, json_result.name, ClaimValueTypes.String, Issuer));
					foreach(string role in json_result.roles)
					{
						claims.Add(new Claim(ClaimTypes.Role, role, ClaimValueTypes.String, Issuer));
					}
					
					//claims.Add(new Claim("EmployeeId", string.Empty, ClaimValueTypes.String, Issuer));
					//claims.Add(new Claim("EmployeeId", "123", ClaimValueTypes.String, Issuer));
					//claims.Add(new Claim(ClaimTypes.DateOfBirth, "1970-06-08", ClaimValueTypes.Date));

					//var userIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
					var userIdentity = new ClaimsIdentity("SuperSecureLogin");
                    userIdentity.AddClaims(claims);
					var userPrincipal = new ClaimsPrincipal(userIdentity);

					await HttpContext.SignInAsync(
						CookieAuthenticationDefaults.AuthenticationScheme,
						userPrincipal,
						new AuthenticationProperties
						{
							ExpiresUtc = DateTime.UtcNow.AddMinutes(30),
							IsPersistent = false,
							AllowRefresh = false
						});
				}

				/*
				{
					"ok":true,
					"userCtx":
					{
						"name":"mmrds",
						"roles":["_admin"]
					},
					"info":
					{
						"authentication_db":"_users",
						"authentication_handlers":
						[
							"oauth",
							"cookie",
							"default"
						],
						"authenticated":"cookie"
					}
				}
				*/


				//this.ActionContext.Response.Headers.Add("Set-Cookie", auth_session_token);

				return result;

			}
			catch(Exception ex)
			{
				Console.WriteLine (ex);

			} 

			return null;
		}

		//https://wiki.apache.org/couchdb/Session_API
		// DELETE api/_sevalues/5 
		public logout_response Delete() 
		{ 
			try
			{
				string request_string = Program.config_couchdb_url + "/_session";


				System.Net.WebRequest request = System.Net.WebRequest.Create(new Uri(request_string));
				request.Method = "DELETE";
				request.PreAuthenticate = false;

                if (!string.IsNullOrWhiteSpace(this.Request.Cookies["AuthSession"]))
                {
                    string auth_session_value = this.Request.Cookies["AuthSession"];
                    request.Headers.Add("Cookie", "AuthSession=" + auth_session_value);
                    request.Headers.Add("X-CouchDB-WWW-Authenticate", auth_session_value);
                }



				System.Net.WebResponse response = (System.Net.HttpWebResponse)request.GetResponse();
				System.IO.Stream dataStream = response.GetResponseStream ();
				System.IO.StreamReader reader = new System.IO.StreamReader (dataStream);
				string responseFromServer = reader.ReadToEnd ();
				logout_response json_result = Newtonsoft.Json.JsonConvert.DeserializeObject<logout_response>(responseFromServer);

				return json_result;
			}
			catch(Exception ex)
			{
				Console.WriteLine (ex);

			} 

			return null;
		}

	}

    public struct Post_Request_Struct
    {
        public string userid;
        public string password;
    }
}

