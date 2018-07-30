using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using System.Dynamic;
using mmria.common;

namespace mmria.server
{
    [Route("api/[controller]")]
	public class user_role_jurisdiction_viewController: ControllerBase
	{
        // GET api/values 
        [HttpGet]
        public async Task<mmria.common.model.couchdb.get_sortable_view_reponse_header<mmria.common.model.couchdb.user_role_jurisdiction>> Get
        (
            int skip = 0,
            int take = 25,
            string sort = "by_date_created",
            string search_key = null,
            bool descending = false
        ) 
		{
            /*
             * 
             * http://localhost:5984/de_id/_design/sortable/_view/conflicts
             * 

by_date_created
by_created_by
by_date_last_updated
by_last_updated_by
by_role_name
by_user_id
by_parent_id
by_jurisdiction_id
by_is_active
by_effective_start_date
by_effective_end_date


date_created
created_by
date_last_updated
last_updated_by
role_name
user_id
parent_id
jurisdiction_id
is_active
effective_start_date
effective_end_date

*/

            var jurisdiction_hashset = mmria.server.util.case_authorization.get_current_jurisdiction_id_set_for(User);
            string sort_view = sort.ToLower ();
            switch (sort_view)
            {
                    case "by_date_created":
                    case "by_created_by":
                    case "by_date_last_updated":
                    case "by_last_updated_by":
                    case "by_role_name":
                    case "by_user_id":
                    case "by_parent_id":
                    case "by_jurisdiction_id":
                    case "by_is_active":
                    case "by_effective_start_date":
                    case "by_effective_end_date":
                    break;

                default:
                    sort_view = "by_date_created";
                break;
            }



			try
			{
                System.Text.StringBuilder request_builder = new System.Text.StringBuilder ();
                request_builder.Append (Program.config_couchdb_url);
                request_builder.Append ($"/jurisdiction/_design/sortable/_view/{sort_view}?");


                if (string.IsNullOrWhiteSpace (search_key))
                {
                    if (skip > -1) 
                    {
                        request_builder.Append ($"skip={skip}");
                    } 
                    else 
                    {

                        request_builder.Append ("skip=0");
                    }


                    if (take > -1) 
                    {
                        request_builder.Append ($"&limit={take}");
                    }

                    if (descending) 
                    {
                        request_builder.Append ("&descending=true");
                    }
                } 
                else 
                {
                    request_builder.Append ("skip=0");

                    if (descending) 
                    {
                        request_builder.Append ("&descending=true");
                    }
                }



				var user_role_jurisdiction_curl = new cURL("GET", null, request_builder.ToString(), null, Program.config_timer_user_name, Program.config_timer_password);
				string response_from_server = await user_role_jurisdiction_curl.executeAsync ();

                var case_view_response = Newtonsoft.Json.JsonConvert.DeserializeObject<mmria.common.model.couchdb.get_sortable_view_reponse_header<mmria.common.model.couchdb.user_role_jurisdiction>>(response_from_server);

                if (string.IsNullOrWhiteSpace (search_key)) 
                {

                    var result = new mmria.common.model.couchdb.get_sortable_view_reponse_header<mmria.common.model.couchdb.user_role_jurisdiction>();
                    result.offset = case_view_response.offset;
                    result.total_rows = case_view_response.total_rows;

                    foreach(mmria.common.model.couchdb.get_sortable_view_response_item<mmria.common.model.couchdb.user_role_jurisdiction> cvi in case_view_response.rows)
                    {
                        bool is_jurisdiction_ok = false;
                        foreach(string jurisdiction_item in jurisdiction_hashset)
                        {
                            var regex = new System.Text.RegularExpressions.Regex("^" + @jurisdiction_item);
                            if(cvi.value.jurisdiction_id == null)
                            {
                                cvi.value.jurisdiction_id = "/";
                            }

                            if(regex.IsMatch(cvi.value.jurisdiction_id))
                            {
                                is_jurisdiction_ok = true;
                                break;
                            }
                        }

                        if(is_jurisdiction_ok) result.rows.Add (cvi);
                    }

                    return case_view_response;
                } 
                else 
                {
                    string key_compare = search_key.ToLower ().Trim (new char [] { '"' });

                    var result = new mmria.common.model.couchdb.get_sortable_view_reponse_header<mmria.common.model.couchdb.user_role_jurisdiction>();
                    result.offset = case_view_response.offset;
                    result.total_rows = case_view_response.total_rows;

                    //foreach(mmria.common.model.couchdb.user_role_jurisdiction cvi in case_view_response.rows)
                    foreach(mmria.common.model.couchdb.get_sortable_view_response_item<mmria.common.model.couchdb.user_role_jurisdiction> cvi in case_view_response.rows)
                    {
/*
date_created
created_by
date_last_updated
last_updated_by
role_name
user_id
parent_id
jurisdiction_id
is_active
effective_start_date
effective_end_date
 */ 

                        bool add_item = false;
                        if (cvi.value.jurisdiction_id != null && cvi.value.jurisdiction_id.Equals(key_compare, StringComparison.OrdinalIgnoreCase))
                        {
                            add_item = true;
                        }

                        if (cvi.value.is_active != null && cvi.value.is_active.HasValue)
                        {
                            if(bool.TryParse(key_compare, out bool is_active))
                            {
                                if(cvi.value.is_active.Value == is_active)
                                {
                                    add_item = true;
                                }
                            }
                        }

                        if(cvi.value.role_name != null && cvi.value.role_name.Equals(key_compare, StringComparison.OrdinalIgnoreCase))
                        {
                            add_item = true;
                        }

                        if(cvi.value.user_id != null && cvi.value.user_id.Equals (key_compare, StringComparison.OrdinalIgnoreCase))
                        {
                            add_item = true;
                        }


                        if(cvi.value.effective_start_date != null && cvi.value.effective_start_date.HasValue)
                        {
                            if(DateTime.TryParse(key_compare, out DateTime is_date))
                            {
                                if(cvi.value.effective_start_date.Value == is_date)
                                {
                                    add_item = true;
                                }
                            }
                        }

                        if(cvi.value.effective_end_date != null && cvi.value.effective_end_date.HasValue)
                        {
                            if(DateTime.TryParse(key_compare, out DateTime is_date))
                            {
                                if(cvi.value.effective_end_date.Value == is_date)
                                {
                                    add_item = true;
                                }
                            }
                        }



                        if(cvi.value.date_created != null && cvi.value.date_created.HasValue)
                        {
                            if(DateTime.TryParse(key_compare, out DateTime is_date))
                            {
                                if(cvi.value.date_created.Value == is_date)
                                {
                                    add_item = true;
                                }
                            }
                        }



                        if(cvi.value.date_last_updated != null && cvi.value.date_last_updated.HasValue)
                        {
                            if(DateTime.TryParse(key_compare, out DateTime is_date))
                            {
                                if(cvi.value.date_last_updated.Value == is_date)
                                {
                                    add_item = true;
                                }
                            }
                        }

                        if(cvi.value.created_by != null && cvi.value.created_by.Equals (key_compare, StringComparison.OrdinalIgnoreCase))
                        {
                            add_item = true;
                        }

                        if(cvi.value.last_updated_by != null && cvi.value.last_updated_by.Equals (key_compare, StringComparison.OrdinalIgnoreCase))
                        {
                            add_item = true;
                        }


                        bool is_jurisdiction_ok = false;
                        foreach(string jurisdiction_item in jurisdiction_hashset)
                        {
                            var regex = new System.Text.RegularExpressions.Regex("^" + @jurisdiction_item);

                            if(cvi.value.jurisdiction_id == null)
                            {
                                cvi.value.jurisdiction_id = "/";
                            }


                            if(regex.IsMatch(cvi.value.jurisdiction_id))
                            {
                                is_jurisdiction_ok = true;
                                break;
                            }
                        }

                        if(add_item && is_jurisdiction_ok) result.rows.Add (cvi);
                       
                    }

                    result.total_rows = result.rows.Count;
                    result.rows =  result.rows.Skip (skip).Take (take).ToList ();

                    return result;
                }


				/*
		< HTTP/1.1 200 OK
		< Set-Cookie: AuthSession=YW5uYTo0QUIzOTdFQjrC4ipN-D-53hw1sJepVzcVxnriEw;
		< Version=1; Path=/; HttpOnly
		> ...
		<
		{"ok":true}*/



			}
			catch(Exception ex)
			{
				Console.WriteLine (ex);

			} 

			return null;
		} 

	} 
}

