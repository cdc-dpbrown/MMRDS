'use strict';

var g_jurisdiction_list = null;
var g_policy_values = null;

$(function ()
{


	$(document).keydown(function(evt){
		if (evt.keyCode==90 && (evt.ctrlKey)){
			evt.preventDefault();
			undo_click();
		}

	});
  load_policy_values();

  document.getElementById("password_div").innerHTML = render_password().join("");
  
});



function load_policy_values()
{
	$.ajax({
			url: location.protocol + '//' + location.host + '/api/policyvalues',
	}).done(function(response) {
			g_policy_values = response;
			load_user_role_jurisdiction();
	});

}


function load_user_role_jurisdiction()
{

  /*            int skip = 0,
            int take = 25,
            string sort = "by_date_created",
            string search_key = null,
            bool descending = false
            */



	$.ajax({
			url: location.protocol + '//' + location.host + '/api/user_role_jurisdiction_view?skip=0&take=25&sort=by_user_id&search_key=' + $mmria.getCookie("uid"),
	}).done(function(response) {

      g_jurisdiction_list = []

      var role_list_html = [];
      role_list_html.push("<p>[ " + $mmria.getCookie("uid") + " ] Your password will expire in X days.</p>");
      role_list_html.push("<table border=1>");
      role_list_html.push("<tr bgcolor='#CCCCCC'><th colspan=7>Role assignment list</th></tr>");
      role_list_html.push("<tr bgcolor='#CCCCCC'>");
      role_list_html.push("<th>Role Name</th>");
      role_list_html.push("<th>Jurisdiction</th>");
      role_list_html.push("<th>Is Active</th>");
      role_list_html.push("<th>Start Date</th>");
      role_list_html.push("<th>End Date</th>");
      role_list_html.push("<th>Days Till<br/>Role Expires</th>");
      role_list_html.push("<th>Jurisdiction<br/>Admin</th>");
      role_list_html.push("</tr>");
      for(var i in response.rows)
      {

          var current_date = new Date();
          var oneDay = 24*60*60*1000; // hours*minutes*seconds*milliseconds

          var value = response.rows[i].value;
          if(value.user_id == $mmria.getCookie("uid"))
          {
            g_jurisdiction_list.push(value);

            var diffDays = 0;

            var effective_start_date = "";
            var effective_end_date = "never";

            if(value.effective_start_date && value.effective_start_date != "")
            {
              effective_start_date = value.effective_start_date.split('T')[0];
            }

            if(value.effective_end_date && value.effective_end_date != "")
            {
              effective_end_date = value.effective_end_date.split('T')[0];
              diffDays = Math.round(Math.abs((new Date(value.effective_end_date).getTime() - current_date.getTime())/(oneDay)));
              
            }

            if(i % 2 == 0)
            {
              role_list_html.push("<tr>");
              
            }
            else
            {
              role_list_html.push("<tr bgcolor='silver'>");
            }
            
            role_list_html.push("<td>" + value.role_name + "</td>");
            role_list_html.push("<td>" + value.jurisdiction_id + "</td>");
            role_list_html.push("<td>" + value.is_active + "</td>");
            role_list_html.push("<td>" + effective_start_date + "</td>");
            role_list_html.push("<td>" + effective_end_date + "</td>");
            role_list_html.push("<td align='right'>" + diffDays + "</td>");
            role_list_html.push("<td>" + value.last_updated_by + "</td>");
            role_list_html.push("</tr>");
          }
          
      }

      role_list_html.push("</table>");
      

      document.getElementById("role_list").innerHTML = role_list_html.join("");
      

	});

}

function render_password()
{
  var result = [];
  
	result.push("<table>");
	result.push("<tr><th colspan=2>Change Password</th></tr>");
	result.push("<tr><td>New Password</td><td><input type='password' value=''  /></td></tr>");
	result.push("<tr><td>Verify Password</td><td><input type='password' value=''  /></td></tr>");
	result.push("<tr><td>&nbsp;</td><td><input type='button' value='Update password' onclick='change_password_user_click()'/></td></tr>");

  result.push("</table>");
  
  return result;
}

function change_password_user_click(p_user_id)
{
	
	var new_user_password = document.querySelector('[role="confirm_1"][path="' + p_user_id + '"]').value;
	var new_confirm_password = document.querySelector('[role="confirm_2"][path="' + p_user_id + '"]').value;

	var user_index = -1;
	var user_list = g_ui.user_summary_list;
	var user = null;
	for(var i = 0; i < user_list.length; i++)
	{
		if(user_list[i]._id == p_user_id)
		{
			user = user_list[i];
			break;
		}
	}


	if(
		new_user_password == new_confirm_password &&
		is_valid_password(new_user_password) && 
		is_valid_password(new_confirm_password)
		
	)
	{



		if(user)
		{
			user.password = new_user_password;

			//var current_auth_session = profile.get_auth_session_cookie();

			//if(current_auth_session)
			//{ 
				$.ajax({
					url: location.protocol + '//' + location.host + '/api/user',
					contentType: 'application/json; charset=utf-8',
					dataType: 'json',
					data: JSON.stringify(user),
					type: "POST"/*,
					beforeSend: function (request)
					{
						request.setRequestHeader("AuthSession", current_auth_session);
					}*/
				}).done(function(response) 
				{
					var response_obj = eval(response);
					if(response_obj.ok)
					{
						for(var i = 0; i < g_ui.user_summary_list.length; i++)
						{
							if(g_ui.user_summary_list[i]._id == response_obj.id)
							{
								g_ui.user_summary_list[i]._rev = response_obj.rev; 
								break;
							}
						}

						if(response_obj.auth_session)
						{
							//profile.auth_session = response_obj.auth_session;
							$mmria.addCookie("AuthSession", response_obj.auth_session);
						}

						document.getElementById('form_content_id').innerHTML = user_render(g_ui, "", g_ui).join("");
						create_status_message("user information saved", convert_to_jquery_id(user._id));
						console.log("password saved sent", response);


					}
					//{ok: true, id: "2016-06-12T13:49:24.759Z", rev: "3-c0a15d6da8afa0f82f5ff8c53e0cc998"}
					
				});
			//}
		}
		else
		{
			document.getElementById('form_content_id').innerHTML = user_render(g_ui, "", g_ui).join("");
			//console.log("greatness awaits.");
		}
	}
	else
	{

		create_status_warning("invalid password.<br/>be sure that verify and password match,<br/>  minimum length is: " + g_policy_values.password_minimum_length + " and should only include characters [a-zA-Z0-9!@#$%?* ]", convert_to_jquery_id(user._id));
		//create_status_warning("invalid password and confirm", convert_to_jquery_id(user._id));
		console.log("got nothing.");
	}
}


function is_valid_user_name(p_value)
{
	var result = true;

	if(
		p_value && 
		p_value.length > 4
	)
	{
		//console.log("greatness awaits.");
	}
	else
	{
		result = false;
	}

	return result;
}

function is_valid_password(p_value)
{
	var result = true;

    var valid_character_re = /^[a-zA-Z0-9!@#$\%\?\* ]+$/g;


	if(
		p_value &&
		p_value.length >= g_policy_values.password_minimum_length &&
		p_value.match(valid_character_re)
	)
	{
		//console.log("greatness awaits.");
	}
	else
	{
		result = false;
	}

	return result;
	
}

