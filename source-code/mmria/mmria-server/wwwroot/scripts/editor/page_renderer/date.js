function date_render(p_result, p_metadata, p_data, p_ui, p_metadata_path, p_object_path, p_dictionary_path, p_is_grid_context, p_post_html_render)
{
    if(p_metadata.name == "date_of_screening")
    {
        console.log("break");
    }
    p_result.push("<div class='date' id='");
    p_result.push(convert_object_path_to_jquery_id(p_object_path));
    
    p_result.push("' ");
    p_result.push(" mpath='");
    p_result.push(p_metadata_path);
    p_result.push("' ");

    p_result.push(" style='");
    if(p_metadata.grid_row && p_metadata.grid_row!= "")
    {
        p_result.push("grid-row:");
        p_result.push(p_metadata.grid_row);
        p_result.push(";");
    }


    if(p_metadata.grid_column && p_metadata.grid_column!= "")
    {
        p_result.push("grid-column:");
        p_result.push(p_metadata.grid_column);
        p_result.push(";");
    }

    if(p_metadata.grid_area && p_metadata.grid_area!= "")
    {
        p_result.push("grid-area:");
        p_result.push(p_metadata.grid_area);
        p_result.push(";");
    }
    p_result.push("' ");

    p_result.push(">");
    p_result.push("<label ");
    if(p_metadata.description && p_metadata.description.length > 0)
    {
        p_result.push("rel='tooltip'  data-original-title='");
        p_result.push(p_metadata.description.replace(/'/g, "\\'"));
        p_result.push("'>");
    }
    else
    {
        p_result.push(">");
    }
    
    if(p_is_grid_context && p_is_grid_context == true)
    {

    }
    else
    {
        p_result.push(p_metadata.prompt);
    }

    /*
    p_result.push("</span><br/> <input  class='date' type='");
    //p_result.push(p_metadata.type.toLowerCase());
    p_result.push("text");
    p_result.push("' name='");
    p_result.push(p_metadata.name);
    p_result.push("' value='");
    p_result.push(p_data.toISOString().split("T")[0]);
    p_result.push("'  onblur='g_set_data_object_from_path(\"");
    p_result.push(p_object_path);
    p_result.push("\",\"");
    p_result.push(p_metadata_path);
    p_result.push("\",this.value)'  /></div>");
    */
    
    //p_result.push("<div style='position:relative'>");
    page_render_create_input(p_result, p_metadata, p_data, p_metadata_path, p_object_path, p_dictionary_path);
    //p_result.push("</div>");
    p_result.push("</label> ");
    p_result.push("</div>");

    p_post_html_render.push('flatpickr("#' + convert_object_path_to_jquery_id(p_object_path) + ' .date", {');
    p_post_html_render.push('	utc: true,');
    p_post_html_render.push('	defaultDate: "');
    p_post_html_render.push(p_data);
    p_post_html_render.push('",');
    p_post_html_render.push('	enableTime: false,');
    p_post_html_render.push('  onChange: function(selectedDates, p_value, instance)  ');
    p_post_html_render.push('  {');
    p_post_html_render.push('                g_set_data_object_from_path("' + p_object_path + '", "' + p_metadata_path + '", p_value);');
    p_post_html_render.push('  }');
    p_post_html_render.push('});');


}