﻿using System;
namespace owin.metadata
{
	public class app
	{
		public string _id { get; set; }
		public string _rev { get; set; }
		public string name { get; set; }
		public string type { get; set; }
		public string date_created { get; set; } 
		public string created_by { get; set; } 
		public string date_last_updated { get; set; } 
		public string last_updated_by { get; set; } 
		public node[] children { get; set; } 

		public app()
		{


		}
	}
}
