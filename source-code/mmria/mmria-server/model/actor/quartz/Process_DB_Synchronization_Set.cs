using System;
using System.Collections.Generic;
using System.Linq;
using Akka.Actor;

namespace mmria.server.model.actor.quartz
{
    public class Process_DB_Synchronization_Set : UntypedActor
    {
        protected override void PreStart() => Console.WriteLine("Process_DB_Synchronization_Set started");
        protected override void PostStop() => Console.WriteLine("Process_DB_Synchronization_Set stopped");

        protected override void OnReceive(object message)
        {
               
            Console.WriteLine($"Process_DB_Synchronization_Set Baby {System.DateTime.Now}");

            switch (message)
            {
                case ScheduleInfoMessage scheduleInfo:
				//System.Console.WriteLine ("{0} Beginning Change Synchronization.", System.DateTime.Now);
				//log.DebugFormat("iCIMS_Data_Call_Job says: Starting {0} executing at {1}", jobKey, DateTime.Now.ToString("r"));
				mmria.server.model.couchdb.c_change_result latest_change_set = get_changes (Program.Last_Change_Sequence, scheduleInfo);

				Dictionary<string, KeyValuePair<string,bool>> response_results = new Dictionary<string, KeyValuePair<string,bool>> (StringComparer.OrdinalIgnoreCase);
			
				if (Program.Last_Change_Sequence != latest_change_set.last_seq)
				{
					foreach (mmria.server.model.couchdb.c_seq seq in latest_change_set.results)
					{
						if (response_results.ContainsKey (seq.id)) 
						{
							if (
								seq.changes.Count > 0 &&
								response_results [seq.id].Key != seq.changes [0].rev)
							{
								if (seq.deleted == null)
								{
									response_results [seq.id] = new KeyValuePair<string, bool> (seq.changes [0].rev, false);
								}
								else
								{
									response_results [seq.id] = new KeyValuePair<string, bool> (seq.changes [0].rev, true);
								}
							
							}
						}
						else 
						{
							if (seq.deleted == null)
							{
								response_results.Add (seq.id, new KeyValuePair<string, bool> (seq.changes [0].rev, false));
							}
							else
							{
								response_results.Add (seq.id, new KeyValuePair<string, bool> (seq.changes [0].rev, true));
							}
						}
					}
				}

			
				if (Program.Change_Sequence_Call_Count < int.MaxValue)
				{
					Program.Change_Sequence_Call_Count++;
				}

				if (Program.DateOfLastChange_Sequence_Call.Count > 9)
				{
					Program.DateOfLastChange_Sequence_Call.Clear ();
				}

				Program.DateOfLastChange_Sequence_Call.Add (DateTime.Now);

				Program.Last_Change_Sequence = latest_change_set.last_seq;

				//List<System.Threading.Tasks.Task> TaskList = new List<System.Threading.Tasks.Task> ();

				foreach (KeyValuePair<string, KeyValuePair<string, bool>> kvp in response_results)
				{
					System.Threading.Tasks.Task.Run
					(
						new Action (() => 
						{
							if (kvp.Value.Value)
							{
								try
								{
									mmria.server.util.c_sync_document sync_document = new mmria.server.util.c_sync_document (kvp.Key, null, "DELETE");
									sync_document.execute ();
								
			
								}
								catch (Exception ex)
								{
									//System.Console.WriteLine ("Sync Delete case");
									//System.Console.WriteLine (ex);
								}
							}
							else
							{
								string document_url = Program.config_couchdb_url + "/mmrds/" + kvp.Key;
								var document_curl = new cURL ("GET", null, document_url, null, Program.config_timer_user_name, Program.config_timer_password);
								string document_json = null;
		
								try
								{
									document_json = document_curl.execute ();
									if (!string.IsNullOrEmpty (document_json) && document_json.IndexOf ("\"_id\":\"_design/") < 0)
									{
										mmria.server.util.c_sync_document sync_document = new mmria.server.util.c_sync_document (kvp.Key, document_json);
										sync_document.execute ();
									}
			
								}
								catch (Exception ex)
								{
									//System.Console.WriteLine ("Sync PUT case");
									//System.Console.WriteLine (ex);
								}
							}
					})
					);
				}
				//System.Threading.Tasks.Task.WhenAll (TaskList);

				try
				{
	
					HashSet<string> mmrds_id_set = new HashSet<string> (StringComparer.OrdinalIgnoreCase);
					HashSet<string> de_id_set = new HashSet<string> (StringComparer.OrdinalIgnoreCase);
					HashSet<string> report_id_set = new HashSet<string> (StringComparer.OrdinalIgnoreCase);
					HashSet<string> deleted_id_set = null;
	
					string json = null;
					mmria.server.model.couchdb.c_all_docs all_docs = null;
					cURL curl = null;
	
					// get all non deleted cases in mmrds
					curl = new cURL ("GET", null, Program.config_couchdb_url + "/mmrds/_all_docs", null, Program.config_timer_user_name, Program.config_timer_password);
					json = curl.execute ();
					all_docs = Newtonsoft.Json.JsonConvert.DeserializeObject<mmria.server.model.couchdb.c_all_docs> (json);
					foreach (mmria.server.model.couchdb.c_all_docs_row all_doc_row in all_docs.rows)
					{
						mmrds_id_set.Add (all_doc_row.id);
					}
				
				
					// get all non deleted cases in de_id
					curl = new cURL ("GET", null, Program.config_couchdb_url + "/de_id/_all_docs", null, Program.config_timer_user_name, Program.config_timer_password);
					json = curl.execute ();
					all_docs = Newtonsoft.Json.JsonConvert.DeserializeObject<mmria.server.model.couchdb.c_all_docs> (json);
					foreach (mmria.server.model.couchdb.c_all_docs_row all_doc_row in all_docs.rows)
					{
						de_id_set.Add (all_doc_row.id);
					}
	
					deleted_id_set = new HashSet<string> (StringComparer.OrdinalIgnoreCase);
					deleted_id_set.Union (de_id_set.Except (mmrds_id_set));
					foreach (string id in deleted_id_set)
					{
						string rev = all_docs.rows.Where (r => r.id == id).FirstOrDefault ().rev.rev;
						curl = new cURL ("DELETE", null, Program.config_couchdb_url + "/de_id/" + id + "?rev=" + rev, null, Program.config_timer_user_name, Program.config_timer_password);
						json = curl.execute ();
					}
	
					// get all non deleted cases in report
					curl = new cURL ("GET", null, Program.config_couchdb_url + "/report/_all_docs", null, Program.config_timer_user_name, Program.config_timer_password);
					json = curl.execute ();
					all_docs = Newtonsoft.Json.JsonConvert.DeserializeObject<mmria.server.model.couchdb.c_all_docs> (json);
					foreach (mmria.server.model.couchdb.c_all_docs_row all_doc_row in all_docs.rows)
					{
						report_id_set.Add (all_doc_row.id);
					}
					deleted_id_set = new HashSet<string> (StringComparer.OrdinalIgnoreCase);
					deleted_id_set.Union (report_id_set.Except (mmrds_id_set));
					foreach (string id in deleted_id_set)
					{
						string rev = all_docs.rows.Where (r => r.id == id).FirstOrDefault ().rev.rev;
						curl = new cURL ("DELETE", null, Program.config_couchdb_url + "/report/" + id + "?rev=" + rev, null, Program.config_timer_user_name, Program.config_timer_password);
						json = curl.execute ();
					}
				}
				catch (Exception ex)
				{
						System.Console.WriteLine ("Delete sync error:\n{0}", ex);
				}

				//System.Console.WriteLine ("{0}- Ending Change Synchronization.", System.DateTime.Now);
                break;
			}

        }

        public mmria.server.model.couchdb.c_change_result get_changes(string p_last_sequence, ScheduleInfoMessage p_scheduleInfo)
        {

			mmria.server.model.couchdb.c_change_result result = new mmria.server.model.couchdb.c_change_result();
			string url = null;

			if (string.IsNullOrWhiteSpace(p_last_sequence))
			{
				url = Program.config_couchdb_url + "/mmrds/_changes";
			}
			else
			{
				url = Program.config_couchdb_url + "/mmrds/_changes?since=" + p_last_sequence;
			}
			var curl = new cURL ("GET", null, url, null, p_scheduleInfo.user_name, p_scheduleInfo.password);
			string res = curl.execute();
			
			result = Newtonsoft.Json.JsonConvert.DeserializeObject<mmria.server.model.couchdb.c_change_result>(res);
			
			return result;
        }


    }
}