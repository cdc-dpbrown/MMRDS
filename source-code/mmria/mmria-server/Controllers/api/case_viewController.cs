using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using System.Dynamic;
using mmria.common;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace mmria.server
{
    [Authorize(Roles  = "abstractor")]
    [Route("api/[controller]")]
	public class case_viewController: ControllerBase 
	{


        // GET api/values 
        [HttpGet]
        public async Task<mmria.common.model.couchdb.case_view_response> Get
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
             * http://localhost:5984/mmrds/_design/sortable/_view/conflicts
             * 
by_date_created
by_date_last_updated
by_last_name
by_first_name
by_middle_name
by_year_of_death
by_month_of_death
by_committe_review_date
by_created_by
by_last_updated_by
by_state_of_death


*/


            var jurisdiction_hashset = mmria.server.util.authorization.get_current_jurisdiction_id_set_for(User);

            string sort_view = sort.ToLower ();
            switch (sort_view)
            {
                case "by_date_created":
                case "by_date_last_updated":
                case "by_last_name":
                case "by_first_name":
                case "by_middle_name":
                case "by_year_of_death":
                case "by_month_of_death":
                case "by_committe_review_date":
                case "by_created_by":
                case "by_last_updated_by":
                case "by_state_of_death":
                    break;

                default:
                    sort_view = "by_date_created";
                break;
            }



			try
			{
                System.Text.StringBuilder request_builder = new System.Text.StringBuilder ();
                request_builder.Append (Program.config_couchdb_url);
                request_builder.Append ($"/mmrds/_design/sortable/_view/{sort_view}?");


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

                string request_string = request_builder.ToString();
                var case_view_curl = new cURL("GET", null, request_string, null, Program.config_timer_user_name, Program.config_timer_password);
                string responseFromServer = await case_view_curl.executeAsync();

                mmria.common.model.couchdb.case_view_response case_view_response = Newtonsoft.Json.JsonConvert.DeserializeObject<mmria.common.model.couchdb.case_view_response>(responseFromServer);

                if (string.IsNullOrWhiteSpace (search_key)) 
                {
                    mmria.common.model.couchdb.case_view_response result = new mmria.common.model.couchdb.case_view_response();
                    result.offset = case_view_response.offset;
                    result.total_rows = case_view_response.total_rows;
                    

                    foreach(mmria.common.model.couchdb.case_view_item cvi in case_view_response.rows)
                    {
                        bool is_jurisdiction_ok = false;

                        if(cvi.value.jurisdiction_id == null)
                        {
                            cvi.value.jurisdiction_id = "/";
                        }

                        foreach(var jurisdiction_item in jurisdiction_hashset)
                        {
                            var regex = new System.Text.RegularExpressions.Regex("^" + @jurisdiction_item.jurisdiction_id);


                            if(regex.IsMatch(cvi.value.jurisdiction_id) && jurisdiction_item.ResourceRight == mmria.server.util.ResourceRightEnum.ReadCase)
                            {
                                is_jurisdiction_ok = true;
                                break;
                            }
                        }

                        if(is_jurisdiction_ok) result.rows.Add (cvi);
                    }
                    
                    result.total_rows = result.rows.Count;

                    return result;
                } 
                else 
                {
                    string key_compare = search_key.ToLower ().Trim (new char [] { '"' });

                    mmria.common.model.couchdb.case_view_response result = new mmria.common.model.couchdb.case_view_response();
                    result.offset = case_view_response.offset;
                    result.total_rows = case_view_response.total_rows;

                    foreach(mmria.common.model.couchdb.case_view_item cvi in case_view_response.rows)
                    {
                        bool add_item = false;
                        if (cvi.value.first_name != null && cvi.value.first_name.IndexOf (key_compare, StringComparison.OrdinalIgnoreCase) > -1)
                        {
                            add_item = true;
                        }

                        if (cvi.value.middle_name != null && cvi.value.middle_name.IndexOf (key_compare, StringComparison.OrdinalIgnoreCase) > -1)
                        {
                            add_item = true;
                        }

                        if(cvi.value.last_name != null && cvi.value.last_name.IndexOf (key_compare, StringComparison.OrdinalIgnoreCase) > -1 )
                        {
                            add_item = true;
                        }

                        if(cvi.value.record_id != null && cvi.value.record_id.IndexOf (key_compare, StringComparison.OrdinalIgnoreCase) > -1)
                        {
                            add_item = true;
                        }

                        if(cvi.value.agency_case_id != null && cvi.value.agency_case_id.IndexOf (key_compare, StringComparison.OrdinalIgnoreCase) > -1 )
                        {
                            add_item = true;
                        }

                        if(cvi.value.created_by != null && cvi.value.created_by.IndexOf (key_compare, StringComparison.OrdinalIgnoreCase) > -1)
                        {
                            add_item = true;
                        }

                        if(cvi.value.last_updated_by != null && cvi.value.last_updated_by.IndexOf (key_compare, StringComparison.OrdinalIgnoreCase) > -1)
                        {
                            add_item = true;
                        }

                        if(cvi.value.date_of_death_month != null && cvi.value.date_of_death_month.HasValue && cvi.value.date_of_death_month.Value.ToString ().IndexOf (key_compare, StringComparison.OrdinalIgnoreCase) > -1)
                        {
                            add_item = true;
                        }
                        if(cvi.value.date_of_death_year != null && cvi.value.date_of_death_year.HasValue  && cvi.value.date_of_death_year.Value.ToString ().IndexOf (key_compare, StringComparison.OrdinalIgnoreCase) > -1 )
                        {
                            add_item = true;
                        }

                        if (cvi.value.date_of_committee_review != null && cvi.value.date_of_committee_review.HasValue && cvi.value.date_of_committee_review.Value.ToString ().IndexOf (key_compare, StringComparison.OrdinalIgnoreCase) > -1) 
                        {
                            add_item = true;
                        }


                        bool is_jurisdiction_ok = false;
                        foreach(var jurisdiction_item in jurisdiction_hashset)
                        {
                            var regex = new System.Text.RegularExpressions.Regex("^" + @jurisdiction_item.jurisdiction_id);

                            if(cvi.value.jurisdiction_id == null)
                            {
                                cvi.value.jurisdiction_id = "/";
                            }


                            if(regex.IsMatch(cvi.value.jurisdiction_id) && jurisdiction_item.ResourceRight == mmria.server.util.ResourceRightEnum.ReadCase)
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

