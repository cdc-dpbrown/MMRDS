using System;
using System.Collections.Generic;
using System.Linq;
using Akka.Actor;

namespace mmria.server.model.actor.quartz
{
    public class Process_Export_Queue : UntypedActor
    {
        protected override void PreStart() => Console.WriteLine("Process_Export_Queue started");
        protected override void PostStop() => Console.WriteLine("Process_Export_Queue stopped");

        protected override void OnReceive(object message)
        {
            switch(message)
            {
                case ScheduleInfoMessage scheduleInfoMessage:
            
                Console.WriteLine($"Process_Export_Queue {System.DateTime.Now}");

                //System.Console.WriteLine ("{0} Beginning Export Queue Item Processing", System.DateTime.Now);
                try
                {
                    Process_Export_Queue_Item (scheduleInfoMessage);
                }
                catch(Exception ex)
                {
                    // to nothing for now
                    System.Console.WriteLine ("{0} check_for_changes_job.Process_Export_Queue_Item: error\n{1}", System.DateTime.Now, ex);

                }

                try
                {
                    Process_Export_Queue_Delete (scheduleInfoMessage);
                }
                catch(Exception ex)
                {
                    // to nothing for now
                    System.Console.WriteLine ("{0} check_for_changes_job.Process_Export_Queue_Delete: error\n{1}", System.DateTime.Now, ex);

                }
                break;
            }
        }


        public void Process_Export_Queue_Item (ScheduleInfoMessage scheduleInfoMessage)
        {
			//System.Console.WriteLine ("{0} check_for_changes_job.Process_Export_Queue_Item: started", System.DateTime.Now);

			List<export_queue_item> result = new List<export_queue_item> ();
			
			var get_curl = new cURL ("GET", null, Program.config_couchdb_url + "/export_queue/_all_docs?include_docs=true", null, scheduleInfoMessage.user_name, scheduleInfoMessage.password);

			string responseFromServer = get_curl.execute ();

			IDictionary<string,object> response_result = Newtonsoft.Json.JsonConvert.DeserializeObject<System.Dynamic.ExpandoObject> (responseFromServer) as IDictionary<string,object>; 
			IList<object> enumerable_rows = response_result ["rows"] as IList<object>;

			foreach (IDictionary<string,object> enumerable_item in enumerable_rows)
			{
				IDictionary<string,object> doc_item = enumerable_item ["doc"] as IDictionary<string,object>;
		
				if (
					doc_item ["status"] != null &&
					doc_item ["status"].ToString ().StartsWith("In Queue...", StringComparison.OrdinalIgnoreCase))
				{
					export_queue_item item = new export_queue_item ();
	
					item._id = doc_item ["_id"].ToString ();
					item._rev = doc_item ["_rev"].ToString ();
					item._deleted = doc_item .ContainsKey("_deleted") ? doc_item["_deleted"] as bool?: null;
					item.date_created = doc_item ["date_created"] as DateTime?;
					item.created_by = doc_item.ContainsKey("created_by") && doc_item ["created_by"] != null ? doc_item ["created_by"].ToString () : null;
					item.date_last_updated = doc_item ["date_last_updated"] as DateTime?;
					item.last_updated_by = doc_item.ContainsKey("last_updated_by") && doc_item ["last_updated_by"] != null ? doc_item ["last_updated_by"].ToString () : null;
					item.file_name = doc_item ["file_name"] != null ? doc_item ["file_name"].ToString () : null;
					item.export_type = doc_item ["export_type"] != null ? doc_item ["export_type"].ToString () : null;
					item.status = doc_item ["status"] != null ? doc_item ["status"].ToString () : null;
	
					result.Add (item);
				}
			}

		
			if (result.Count > 0)
			{
				if (result.Count > 1)
				{
					var comparer = Comparer<export_queue_item>.Create
					(
						               (x, y) => x.date_created.Value.CompareTo (y.date_created.Value) 
					               );
	
					result.Sort (comparer);
				}

				export_queue_item item_to_process = result [0];

				item_to_process.date_last_updated = new DateTime?();
				//item_to_process.last_updated_by = g_uid;


				List<string> args = new List<string>();
                args.Add("exporter:exporter");
				args.Add("user_name:" + scheduleInfoMessage.user_name);
				args.Add("password:" + scheduleInfoMessage.password);
				args.Add("database_url:" + scheduleInfoMessage.couch_db_url);
				args.Add ("item_file_name:" + item_to_process.file_name);
				args.Add ("item_id:" + item_to_process._id);


				if (item_to_process.export_type.StartsWith ("core csv", StringComparison.OrdinalIgnoreCase))
				{

					item_to_process.status = "Creating Export...";
					item_to_process.last_updated_by = "mmria-server";
					item_to_process.date_last_updated = DateTime.Now;

					Newtonsoft.Json.JsonSerializerSettings settings = new Newtonsoft.Json.JsonSerializerSettings ();
					settings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
					string object_string = Newtonsoft.Json.JsonConvert.SerializeObject (item_to_process, settings);
					var set_curl = new cURL ("PUT", null, Program.config_couchdb_url + "/export_queue/" + item_to_process._id, object_string, scheduleInfoMessage.user_name, scheduleInfoMessage.password);

					responseFromServer = set_curl.execute ();

                    try
                    {
					
    					mmria.server.util.core_element_exporter core_element_exporter = new mmria.server.util.core_element_exporter(scheduleInfoMessage);
    					core_element_exporter.Execute(args.ToArray());
                    }
                    catch(Exception ex)
                    {
                        System.Console.WriteLine (ex);
                    }

				
				}
				else if(item_to_process.export_type.StartsWith ("all csv", StringComparison.OrdinalIgnoreCase))
				{
					item_to_process.status = "Creating Export...";
					item_to_process.last_updated_by = "mmria-server";
					item_to_process.date_last_updated = DateTime.Now;

					Newtonsoft.Json.JsonSerializerSettings settings = new Newtonsoft.Json.JsonSerializerSettings ();
					settings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
					string object_string = Newtonsoft.Json.JsonConvert.SerializeObject (item_to_process, settings);
					var set_curl = new cURL ("PUT", null, Program.config_couchdb_url + "/export_queue/" + item_to_process._id, object_string, scheduleInfoMessage.user_name, scheduleInfoMessage.password);

					responseFromServer = set_curl.execute ();


                    try
                    {
    					mmria.server.util.mmrds_exporter mmrds_exporter = new mmria.server.util.mmrds_exporter(scheduleInfoMessage);
    					mmrds_exporter.Execute(args.ToArray());
                    }
                    catch(Exception ex)
                    {
                        System.Console.WriteLine (ex);
                    }

				}
				else if (item_to_process.export_type.StartsWith ("cdc csv", StringComparison.OrdinalIgnoreCase)) 
				{


					item_to_process.status = "Creating Export...";
					item_to_process.last_updated_by = "mmria-server";
					item_to_process.date_last_updated = DateTime.Now;

					Newtonsoft.Json.JsonSerializerSettings settings = new Newtonsoft.Json.JsonSerializerSettings ();
					settings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
					string object_string = Newtonsoft.Json.JsonConvert.SerializeObject (item_to_process, settings);
					var set_curl = new cURL ("PUT", null, Program.config_couchdb_url + "/export_queue/" + item_to_process._id, object_string, scheduleInfoMessage.user_name, scheduleInfoMessage.password);

					responseFromServer = set_curl.execute ();
					args.Add ("is_cdc_de_identified:true");

					try
					{
						mmria.server.util.mmrds_exporter mmrds_exporter = new mmria.server.util.mmrds_exporter (scheduleInfoMessage);
						mmrds_exporter.Execute (args.ToArray ());
					}
					catch(Exception ex)
					{
						System.Console.WriteLine (ex);
					}


				}

			}

        }


		public void Process_Export_Queue_Delete(ScheduleInfoMessage scheduleInfoMessage)
		{
			//System.Console.WriteLine ("{0} check_for_changes_job.Process_Export_Queue_Delete: started", System.DateTime.Now);

			List<export_queue_item> result = new List<export_queue_item> ();

			var get_curl = new cURL ("GET", null, Program.config_couchdb_url + "/export_queue/_all_docs?include_docs=true", null, scheduleInfoMessage.user_name, scheduleInfoMessage.password);

			string responseFromServer = get_curl.execute ();

			IDictionary<string,object> response_result = Newtonsoft.Json.JsonConvert.DeserializeObject<System.Dynamic.ExpandoObject> (responseFromServer) as IDictionary<string,object>; 
			IList<object> enumerable_rows = response_result ["rows"] as IList<object>;

			foreach (IDictionary<string,object> enumerable_item in enumerable_rows)
			{
				IDictionary<string,object> doc_item = enumerable_item ["doc"] as IDictionary<string,object>;

				if (
					doc_item ["status"] != null &&
					doc_item ["status"].ToString ().StartsWith ("Deleted", StringComparison.OrdinalIgnoreCase))
				{
					export_queue_item item = new export_queue_item ();

					item._id = doc_item ["_id"].ToString ();
					item._rev = doc_item ["_rev"].ToString ();
					item._deleted = doc_item.ContainsKey("_deleted") ? doc_item["_deleted"] as bool?: null;
					item.date_created = doc_item ["date_created"] as DateTime?;
					item.created_by = doc_item.ContainsKey("created_by") && doc_item ["created_by"] != null ? doc_item ["created_by"].ToString () : null;
					item.date_last_updated = doc_item ["date_last_updated"] as DateTime?;
					item.last_updated_by = doc_item.ContainsKey("last_updated_by") && doc_item["last_updated_by"] != null ? doc_item ["last_updated_by"].ToString () : null;
					item.file_name = doc_item ["file_name"] != null ? doc_item ["file_name"].ToString () : null;
					item.export_type = doc_item ["export_type"] != null ? doc_item ["export_type"].ToString () : null;
					item.status = doc_item ["status"] != null ? doc_item ["status"].ToString () : null;

					result.Add (item);
				}
			}


			if (result.Count > 0)
			{
				if (result.Count > 1)
				{
					var comparer = Comparer<export_queue_item>.Create
						(
							(x, y) => x.date_created.Value.CompareTo (y.date_created.Value) 
						);

					result.Sort (comparer);
				}

				export_queue_item item_to_process = result [0];

				try
				{
					string item_directory_name = item_to_process.file_name.Substring (0, item_to_process.file_name.LastIndexOf ("."));
					string export_directory = System.IO.Path.Combine (System.Configuration.ConfigurationManager.AppSettings ["export_directory"], item_directory_name);

					try
					{
						if (System.IO.Directory.Exists(export_directory))
						{
							System.IO.Directory.Delete(export_directory, true);
						}
					}
					catch(Exception Ex)
					{
						// do nothing for now
						System.Console.WriteLine ("check_for_changes_job.Process_Export_Queue_Delete: Unable to Delete Directory {0}", export_directory);
					}

					string file_path = System.IO.Path.Combine (System.Configuration.ConfigurationManager.AppSettings ["export_directory"], item_to_process.file_name);
					try
					{
						
						if (System.IO.File.Exists(file_path))
						{
							System.IO.File.Delete(file_path);
						}

					}
					catch(Exception Ex)
					{
						// do nothing for now
                        System.Console.WriteLine ("Program.Process_Export_Queue_Delete: Unable to Delete File {0}", file_path);
					}

					item_to_process.status = "expunged";
					item_to_process.last_updated_by = "mmria-server";
					Newtonsoft.Json.JsonSerializerSettings settings = new Newtonsoft.Json.JsonSerializerSettings ();
					settings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
					string object_string = Newtonsoft.Json.JsonConvert.SerializeObject(item_to_process, settings); 
					var set_curl = new cURL ("PUT", null, Program.config_couchdb_url + "/export_queue/" + item_to_process._id, object_string, scheduleInfoMessage.user_name, scheduleInfoMessage.password);

					responseFromServer = get_curl.execute ();
				}
				catch(Exception ex)
				{
					// do nothing for now
				}

			}

		}

    }
}