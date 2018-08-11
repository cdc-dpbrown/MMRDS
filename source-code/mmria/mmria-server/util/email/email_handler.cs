﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Mail;
using System.Net;
using Microsoft.Extensions.Configuration;

namespace mmria.server.util.email
{

	public class Email_Handler
	{ 

		/// <summary>
		/// the following method takes email and responseUrl as argument and email redirection url to the user 
		/// </summary>
		/// <param name="emailAddress">email address for sending message (email is NOT saved)</param>
		/// <param name="redirectUrl">url for resuming the saved survey</param>
		/// <param name="surveyName">Name of the survey</param>
		/// <param name="passCode"> Code for accessing an unfinished survey </param>
		/// <returns></returns>

        private IConfiguration Configuration;
        public Email_Handler(IConfiguration configuration)
        {
            Configuration = configuration;
        }


		public bool SendMessage( Email Email)
		{
			try
			{
				bool isAuthenticated = false;
				bool isUsingSSL = false;
				int SMTPPort = 25;

				// App Config Settings:
				// EMAIL_USE_AUTHENTICATION [ True | False ] default is False
				// EMAIL_USE_SSL [ True | False] default is False
				// SMTP_HOST [ url or ip address of smtp server ]
				// SMTP_PORT [ port number to use ] default is 25
				// EMAIL_FROM [ email address of sender and authenticator ]
				// EMAIL_PASSWORD [ password of sender and authenticator ]


				string s = this.Configuration["mmria_settings:EMAIL_USE_AUTHENTICATION"];
				if (!String.IsNullOrEmpty(s))
				{
					if (s.ToUpper() == "TRUE")
					{
						isAuthenticated = true;
					}
				}

				s = this.Configuration["mmria_settings:EMAIL_USE_SSL"];
				if (!String.IsNullOrEmpty(s))
				{
					if (s.ToUpper() == "TRUE")
					{
						isUsingSSL = true;
					}
				}

				s = this.Configuration["mmria_settings:SMTP_PORT"];
				if (!int.TryParse(s, out SMTPPort))
				{
					SMTPPort = 25;
				}

				System.Net.Mail.MailMessage message = new System.Net.Mail.MailMessage();
				foreach (string item in Email.To)
				{
					message.To.Add(item);
				}

				message.Subject = Email.Subject;
				message.From =  new System.Net.Mail.MailAddress(Email.From.ToString());
				message.Body = Email.Body;  
				System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient(this.Configuration["mmria_settings:SMTP_HOST"].ToString());
				smtp.Port = SMTPPort;

				if (isAuthenticated)
				{
					smtp.Credentials = new System.Net.NetworkCredential(this.Configuration["mmria_settings:EMAIL_FROM"].ToString(), this.Configuration["mmria_settings:EMAIL_PASSWORD"].ToString());
				}


				smtp.EnableSsl = isUsingSSL;


				smtp.Send(message);

				return true;

			}
			catch (System.Exception ex)
			{
				return false;
			}
		}

	}



}

