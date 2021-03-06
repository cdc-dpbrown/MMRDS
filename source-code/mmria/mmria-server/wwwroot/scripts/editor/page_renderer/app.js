function app_render(p_result, p_metadata, p_data, p_ui, p_metadata_path, p_object_path, p_dictionary_path, p_is_grid_context, p_post_html_render)
{

    p_result.push("<section id='app_summary'><h1>Line Listing Summary</h1>");
    // p_result.push("<input type='button'  class='btn-green' value='Add New Case' onclick='g_ui.add_new_case()' /><hr/>");

    p_result.push("<input type='button' class='btn btn-primary' value='Add New Case' onclick='g_ui.add_new_case()' /><hr/>");
    


    p_result.push("<fieldset><legend>Search and Sort Case Listings</legend>");


    // p_result.push("Search Text: <input type='text' id='search_text_box' onchange='g_ui.case_view_request.search_key = this.value;' value='");
    p_result.push("<label for='search_text_box'>Search Text</label>");
    p_result.push("Search Text: <input type='text1' class='form-control1' id='search_text_box' onchange='g_ui.case_view_request.search_key=this.value;' value='");
    if(g_ui.case_view_request.search_key!= null)
    {
        p_result.push(p_ui.case_view_request.search_key.replace(/'/g, "&quot;"));
    }
    p_result.push("' />  ");

    p_post_html_render.push("$('#search_text_box').bind(\"enterKey\",function(e){");
    //p_post_html_render.push("	g_ui.case_view_request.search_key = \"\";");
    p_post_html_render.push("	get_case_set();");
    p_post_html_render.push(" });");

    p_post_html_render.push("$('#search_text_box').keyup(function(e){");
        p_post_html_render.push("	if(e.keyCode == 13)");
        p_post_html_render.push("	{");
            p_post_html_render.push("	$(this).trigger(\"enterKey\");");
            p_post_html_render.push("	}");
            p_post_html_render.push("});");



    // p_result.push("<input type='button' alt='search' id='search_command_button' onclick='g_ui.case_view_request.search_key = \"\";get_case_set();' value='Clear Search Text' />");
    p_result.push("<label for='search_command_button'>Execute Search</label>");
    p_result.push("<input type='button' class='btn btn-primary' alt='search' id='search_command_button' onclick='g_ui.case_view_request.search_key = \"\";get_case_set();' value='Clear Search Text' />");


    p_result.push("<label for='search_sort_by'>Sort By</label>");
    p_result.push("<br/> Sort By:<select id='search_sort_by' onchange='g_ui.case_view_request.sort = \"by_\" + this.options[this.selectedIndex].value;'>");


    
    if(p_ui.case_view_request.sort=="by_date_created")
    {
        p_result.push("<option selected>date_created</option>");
    }
    else
    {
        p_result.push("<option>date_created</option>");
    }
    
    if(p_ui.case_view_request.sort=="by_jurisdiction_id")
    {
        p_result.push("<option selected>jurisdiction_id</option>");
    }
    else
    {
        p_result.push("<option>jurisdiction_id</option>");
    }
    
    if(p_ui.case_view_request.sort=="by_last_name")
    {
        p_result.push("<option selected>last_name</option>");
    }
    else
    {
        p_result.push("<option>last_name</option>");
    }
    
    if(p_ui.case_view_request.sort=="by_first_name")
    {
        p_result.push("<option selected>first_name</option>");
    }
    else
    {
        p_result.push("<option>first_name</option>");
    }
    
    if(p_ui.case_view_request.sort=="by_middle_name")
    {
        p_result.push("<option selected>middle_name</option>");
    }
    else
    {
        p_result.push("<option>middle_name</option>");
    }
    
    if(p_ui.case_view_request.sort=="by_state_of_death")
    {
        p_result.push("<option selected>state_of_death</option>");
    }
    else
    {
        p_result.push("<option>state_of_death</option>");
    }
    
    if(p_ui.case_view_request.sort=="by_record_id")
    {
        p_result.push("<option selected>record_id</option>");
    }
    else
    {
        p_result.push("<option>record_id</option>");
    }
    
    if(p_ui.case_view_request.sort=="by_year_of_death")
    {
        p_result.push("<option selected>year_of_death</option>");
    }
    else
    {
        p_result.push("<option>year_of_death</option>");
    }
    
    if(p_ui.case_view_request.sort=="by_month_of_death")
    {
        p_result.push("<option selected>month_of_death</option>");
    }
    else
    {
        p_result.push("<option>month_of_death</option>");
    }
    
    if(p_ui.case_view_request.sort=="by_committe_review_date")
    {
        p_result.push("<option selected>committe_review_date</option>");
    }
    else
    {
        p_result.push("<option>committe_review_date</option>");
    }
    


    if(p_ui.case_view_request.sort=="by_agency_case_id")
    {
        p_result.push("<option selected>agency_case_id</option>");
    }
    else
    {
        p_result.push("<option>agency_case_id</option>");
    }
    
    
    if(p_ui.case_view_request.sort=="by_created_by")
    {
        p_result.push("<option selected>created_by</option>");
    }
    else
    {
        p_result.push("<option>created_by</option>");
    }
    
    
    if(p_ui.case_view_request.sort=="by_last_updated_by")
    {
        p_result.push("<option selected>last_updated_by</option>");
    }
    else
    {
        p_result.push("<option>last_updated_by</option>");
    }
    
    if(p_ui.case_view_request.sort=="by_date_last_updated")
    {
        p_result.push("<option selected>date_last_updated</option>");
    }
    else
    {
        p_result.push("<option>date_last_updated</option>");
        
    }
    p_result.push("</select>");

    p_result.push("<label for='sort_decending'>Is Sort Decending?</label>");
    p_result.push(" Sort Descending: <input id='sort_decending' type='checkbox' onchange='g_ui.case_view_request.descending = this.checked;' ");
    if(p_ui.case_view_request.descending)
    {
        p_result.push(" checked='true' ");
    }
    p_result.push(" />");



    p_result.push("<br/>");
    p_result.push("<label for='search_records_per_page'>Records per page</label>");
    p_result.push("Records per page: <select id='search_records_per_page' onchange='g_ui.case_view_request.take = this.value;' >");
    if(p_ui.case_view_request.take==25)
    {
        p_result.push("<option selected>25</option>");
    }
    else
    {
        p_result.push("<option>25</option>");
        
    }
    if(p_ui.case_view_request.take==50)
    {
        p_result.push("<option selected>50</option>");
    }
    else
    {
        p_result.push("<option>50</option>");
        
    }
    if(p_ui.case_view_request.take==100)
    {
        p_result.push("<option selected>100</option>");
    }
    else
    {
        p_result.push("<option>100</option>");
        
    }
    if(p_ui.case_view_request.take==250)
    {
        p_result.push("<option selected>250</option>");
    }
    else
    {
        p_result.push("<option>250</option>");
        
    }
    if(p_ui.case_view_request.take==500)
    {
        p_result.push("<option selected>500</option>");
    }
    else
    {
        p_result.push("<option>500</option>");
        
    }
    p_result.push("</select>");


    // p_result.push("<br/><br/><p style='text-align:right;'><input type='button' alt='search' id='search_command_button' onclick='get_case_set()' value='Apply Search and Sort' /> ");
    p_result.push("<label for='search_command_button'>Apply Search</label>");
    p_result.push("<br/><br/><p style='text-align:right;'><input type='button' class='btn btn-primary'  alt='search' id='search_command_button' onclick='get_case_set()' value='Apply Search and Sort' /> ");
    p_result.push("</p><br/><hr/>");

    /*
    if(p_ui.case_view_request.page != 1)
    {
        p_result.push(" <input type='button' alt='search' value='previous' />");
    }*/
    p_result.push("Page: ");
    p_result.push(p_ui.case_view_request.page);
    p_result.push(" of ")
    p_result.push(Math.ceil(p_ui.case_view_request.total_rows / p_ui.case_view_request.take));
    p_result.push("<br/> ");
    p_result.push("Total Number of Records: ");
    p_result.push(p_ui.case_view_request.total_rows);

    p_result.push("<br/> Select Page: ");

    for(var current_page = 1; (current_page - 1) * p_ui.case_view_request.take < p_ui.case_view_request.total_rows; current_page++)
    {
        // p_result.push(" <input type='button' alt='search' onclick='g_ui.case_view_request.page=");
        p_result.push("<label><input type='button2' class='btn btn-primary' title='search' onclick='g_ui.case_view_request.page=");
        p_result.push(current_page);
        p_result.push(";get_case_set();' value='");
        p_result.push(current_page);
        p_result.push("' />");
        p_result.push("Select page " + current_page + "</label>");
    }

    //p_result.push(" <input type='button' alt='search' value='next' />");

    p_result.push("</fieldset><br/>");



    p_result.push("<table>");
    p_result.push("<tr><th colspan=3 style='text-align:center;'>");
    p_result.push("Case Listing");
    p_result.push("</th></tr>");
    p_result.push("<tr>");
    p_result.push("<td> Case Infomation</td>");
    //p_result.push("<td>state of death</td>");
    //p_result.push("<td>year / month of death</td>");
    //p_result.push("<td>committe review date</td>");
    p_result.push("<td align=center>Last Updated </td>");
    p_result.push("<td align=center>Actions</td>");
    p_result.push("</tr>");

    

/*
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
    
    for(var i = 0; i < p_ui.case_view_list.length; i++)
    {
        var item = p_ui.case_view_list[i];

        if(i % 2)
        {
            p_result.push('		  <tr style="background:#FFFFFF" path="');
        }
        else
        {
            p_result.push('		  <tr style="background:#EEEEEE" path="');
        }
        p_result.push(item.id);
        p_result.push('">');
        
        
        
        p_result.push("<td>");
        p_result.push("<a href='#/")
        p_result.push(i);
        p_result.push("/home_record' role='button' class='btn-purple'>");
        p_result.push(item.value.jurisdiction_id);p_result.push("  :");
        p_result.push(item.value.last_name);p_result.push(", ");
        p_result.push(item.value.first_name);p_result.push(" ");
        p_result.push(item.value.middle_name);
        if(item.value.record_id)
        {
            p_result.push(" - (");
            p_result.push(item.value.record_id);
            p_result.push(" )");
        }


        if(item.value.agency_case_id)
        {
            p_result.push("  ac_id: ");
            p_result.push(item.value.agency_case_id)
        }
        
        
        p_result.push("</a>");
        p_result.push("</td>");

        //p_result.push("<td>");p_result.push(item.value.state_of_death);p_result.push("</td>");
        
        /*
        p_result.push("<td>");
        p_result.push(item.value.year_of_death);
        p_result.push(" / ");
        p_result.push(item.value.month_of_death);
        p_result.push("</td>");
        */
        //p_result.push("<td>");p_result.push(item.value.committe_review_date);p_result.push("</td>");
        
        p_result.push("<td>");
        p_result.push(item.value.last_updated_by);
        p_result.push(" ");
        p_result.push(item.value.date_last_updated);
        p_result.push("</td>");

        p_result.push("<td>");
        p_result.push("&nbsp;");

        // p_result.push(" <input type='button' value='delete' onclick='delete_record(" + i + ")'/> ");

        p_result.push("<label for='id_for_record_" + i + "'>press twice to delete =></label>");
        p_result.push("<input type='button3' id='id_for_record_" + i + "' class='btn btn-primary' value='delete' onclick='delete_record(" + i + ")'/>");
        

        p_result.push("</td>");

        
        /*
        <p class="p_result">');
        p_result.push(item.value.last_name);
        p_result.push(', ');
        p_result.push(item.value.first_name);
        p_result.push(' - ');
        p_result.push(item.value.record_id);
        p_result.push('	(');
        p_result.push(item.value.state_of_death);
        p_result.push('	) <a href="#/'+ i + '/home_record" role="button" class="btn-purple">select</a> <input type="button" value="delete" onclick="delete_record(' + i + ')"/></p>');
        */
        p_result.push('</tr>');
        
    }
    p_result.push('		</table>');

    p_result.push('<hr/>')

    p_result.push("Records per page: <select id='search_records_per_page' onchange='g_ui.case_view_request.take = this.value;' >");
    if(p_ui.case_view_request.take==25)
    {
        p_result.push("<option selected>25</option>");
    }
    else
    {
        p_result.push("<option>25</option>");
        
    }
    if(p_ui.case_view_request.take==50)
    {
        p_result.push("<option selected>50</option>");
    }
    else
    {
        p_result.push("<option>50</option>");
        
    }
    if(p_ui.case_view_request.take==100)
    {
        p_result.push("<option selected>100</option>");
    }
    else
    {
        p_result.push("<option>100</option>");
        
    }
    if(p_ui.case_view_request.take==250)
    {
        p_result.push("<option selected>250</option>");
    }
    else
    {
        p_result.push("<option>250</option>");
        
    }
    if(p_ui.case_view_request.take==500)
    {
        p_result.push("<option selected>500</option>");
    }
    else
    {
        p_result.push("<option>500</option>");
        
    }
    p_result.push("</select>");

    p_result.push("<br/>");
    p_result.push("Page: ");
    p_result.push(p_ui.case_view_request.page);
    p_result.push(" of ")
    p_result.push(Math.ceil(p_ui.case_view_request.total_rows / p_ui.case_view_request.take));
    p_result.push("<br/>");

    p_result.push("Total Number of Records: ");
    p_result.push(p_ui.case_view_request.total_rows);
    p_result.push("<br/>");

    /*
    if(p_ui.case_view_request.page != 1)
    {
        p_result.push(" <input type='button' alt='search' value='previous' />");
    }*/

    p_result.push("Select Page: ");
    for(var current_page = 1; (current_page - 1) * p_ui.case_view_request.take < p_ui.case_view_request.total_rows; current_page++)
    {
        // p_result.push(" <input type='button' alt='search' onclick='g_ui.case_view_request.page=");
        p_result.push("<label><input type='button2' class='btn btn-primary' alt='select page " + current_page + "' onclick='g_ui.case_view_request.page=");
        p_result.push(current_page);
        p_result.push(";get_case_set();' value='");
        p_result.push(current_page);
        p_result.push("' />select page " + current_page + "</label>");
    }


    //p_result.push(" <input type='button' alt='search' value='next' />");




    p_result.push("</section>");

    if(p_ui.url_state.path_array.length > 1)
    {
        for(var i = 0; i < p_metadata.children.length; i++)
        {
            var child = p_metadata.children[i];
            if(child.type.toLowerCase() == 'form' && p_ui.url_state.path_array[1] == child.name)
            {
                if(p_data[child.name] || p_data[child.name] == 0)
                {
                    // do nothing 
                }
                else
                {
                    p_data[child.name] = create_default_object(child, {})[child.name];
                }

                Array.prototype.push.apply(p_result, page_render(child, p_data[child.name], p_ui, p_metadata_path  + ".children[" + i + "]", p_object_path + "." + child.name, p_dictionary_path + "/" + child.name, false, p_post_html_render));				 		
                
            }
        }
    }

    p_result.push('<footer class="footer_wrapper">');
    p_result.push('<p>&nbsp;</p>');
    p_result.push('</footer>');

}