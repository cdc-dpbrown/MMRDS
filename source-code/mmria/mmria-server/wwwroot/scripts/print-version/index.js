var g_metadata = null;
var g_data = null;
var g_copy_clip_board = null;
var g_delete_value_clip_board = null;
var g_delete_attribute_clip_board = null;
var g_delete_node_clip_board = null;

var g_ui = { is_collapsed : [] };




$(function ()
{//http://www.w3schools.com/html/html_layout.asp
  'use strict';

  	//profile.initialize_profile();

	  //load_metadata();
});


function create_print_version(p_metadata, p_data, p_section)
{
	g_data = p_data;
	var post_html_call_back = [];

	document.getElementById('form_content_id').innerHTML = print_version_render(p_metadata, p_data, "/", g_ui, "g_metadata", "g_data", post_html_call_back).join("");

    if(post_html_call_back.length > 0)
    {
		try
		{
			eval(post_html_call_back.join(""));
		}
		catch(ex)
		{
			console.log(ex);
		}
      
    }

	if(p_section && p_section.toLowerCase() != "all")
	{
		var section_list = document.getElementsByTagName("section");
		for(var i = 0; i < section_list.length; i++)
		{
			var section = section_list[i];
			if(section.id == p_section)
			{
				section.style.display = "block";
			}
			else
			{
				section.style.display = "none";
			}
		}
	}

}



function load_metadata()
{
	var metadata_url = location.protocol + '//' + location.host + '/api/metadata';

	$.ajax({
			url: metadata_url
	}).done(function(response) {
			g_metadata = response;
			g_data = create_default_object(g_metadata, {});
			g_ui.url_state = url_monitor.get_url_state(window.location.href);

			create_print_version();
	});
}



