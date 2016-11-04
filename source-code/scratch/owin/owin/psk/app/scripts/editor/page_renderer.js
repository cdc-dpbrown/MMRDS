function page_render(p_metadata, p_data, p_ui, p_path)
{

	var result = [];

	switch(p_metadata.type.toLowerCase())
  {
		case 'address':
			result.push("<fieldset id='");
			result.push(p_metadata.name);
			result.push("_id' class='group address'><legend>");
			result.push(p_metadata.prompt);
			result.push("</legend>");
			for(var i = 0; i < p_metadata.children.length; i++)
      {
        var child = p_metadata.children[i];
        Array.prototype.push.apply(result, page_render(child, p_data[child.name], p_ui, p_path + '/' + p_metadata.name));
      }
			result.push("<input type='button' value='get location' /></fieldset>");
      break;
    case 'grid':
		result.push("<table id='");
		result.push(p_metadata.name);
		result.push("_id' class='grid'><tr><th colspan=");
		result.push(p_metadata.children.length)
		result.push(">");
		result.push(p_metadata.prompt);
		result.push("</th></tr>");
		result.push('<tr>');
		for(var i = 0; i < p_metadata.children.length; i++)
		{
			var child = p_metadata.children[i];
			result.push('<th>');
			result.push(child.prompt);
			result.push('</th>')

		}
		result.push('</tr>');

		for(var i = 0; i < p_data[child.name].length; i++)
		{
			result.push('<tr>');
			for(var j = 0; j < p_data[child.name][i].length; j++)
			{
				result.push("<th>");
				result.push(p_data[child.name][i][j]);
				result.push("</th>");
			}
			result.push('</tr>');
		}
    result.push("<tr><td colspan=");
		result.push(p_metadata.children.length)
		result.push(" align=right> <input type='button' value='Add Item' /></td></tr>");

		result.push("</table>");
		break;
    case 'group':
			result.push("<fieldset id='");
			result.push(p_metadata.name);
			result.push("_id' class='group'><legend>");
			result.push(p_metadata.prompt);
			result.push("</legend>");
			for(var i = 0; i < p_metadata.children.length; i++)
      {
        var child = p_metadata.children[i];
        Array.prototype.push.apply(result, page_render(child, p_data[child.name], p_ui, p_path + '/' + p_metadata.name));
      }
			result.push("</fieldset>");
      break;
    case 'form':
			result.push("<section id='");
			result.push(p_metadata.name);
			result.push("_id' class='form'><h2>");
			result.push(p_metadata.prompt);
			result.push("</h2>");
			for(var i = 0; i < p_metadata.children.length; i++)
      {
        var child = p_metadata.children[i];
        Array.prototype.push.apply(result, page_render(child, p_data[child.name], p_ui, p_path + '/' + p_metadata.name));
      }
			result.push("</section>");
      break;
    case 'app':
		result.push("<section id='app_summary'><h2>summary</h2>");
		result.push("<input type='button' class='btn-green' value='add new case aa' onclick='g_ui.add_new_case()' /><hr/>");
		result.push("<fieldset><legend>filter line listing</legend>");
		result.push("<input type='text' id='search_text_box' value='' /> ");
		result.push("<img src='/images/search.png' alt='search' height=8px width=8px valign=bottom class='btn-green' id='search_command_button'>");
		result.push("</fieldset>");

		result.push('<div class="search_wrapper">');
		for(var i = 0; i < g_ui.data_list.length; i++)
		{
			var item = g_ui.data_list[i];

			if(i % 2)
			{
				result.push('		  <div class="result_wrapper_grey">');
			}
			else
			{
				result.push('		  <div class="result_wrapper">');
			}
			result.push('<p class="result">');
			result.push(item.home_record.last_name);
			result.push(', ');
			result.push(item.home_record.first_name);
			result.push('	(');
			result.push(item.home_record.date_of_death);
			result.push('	(');
			result.push(item.home_record.state_of_death);
			result.push('	) <a href="#/'+ i + '/home_record" role="button" class="btn-purple">select</a></p>');
			result.push('</div>');
			
		}
		result.push('		</div>');


		result.push("</section>");

       for(var i = 0; i < p_metadata.children.length; i++)
       {
					var child = p_metadata.children[i];
					if(child.type.toLowerCase() == 'form')
					{
				 		Array.prototype.push.apply(result, page_render(child, p_data[child.name], p_ui, ""));
					}
			 }

		result.push('<footer class="footer_wrapper">');
		result.push('<p>&nbsp;</p>');
		result.push('</footer>');

       break;
     case 'string':
					result.push("<div class='string'>");
					result.push(p_metadata.prompt);
					result.push("<br/> <input type='text' name='");
					result.push(p_metadata.name);
					result.push("' value='");
					result.push(p_data);
					result.push("' onblur='g_ui.set_value(\"");
          result.push(p_path);
					result.push('/');
					result.push(p_metadata.name);
          result.push("\",this)' /></div>");

           break;
     case 'textarea':
					result.push("<div class='string'>");
					result.push(p_metadata.prompt);
					result.push("<br/> <textarea  name='");
					result.push(p_metadata.name);
					result.push("'>");
					result.push(p_data);
					result.push("'</textarea></div>");

           break;
     case 'number':
						result.push("<div class='number'>");
						result.push(p_metadata.prompt);
						result.push("<br/> <input type='Number' name='");
						result.push(p_metadata.name);
						result.push("' value='");
						result.push(p_data);
						result.push("' onblur='g_ui.set_value(\"");
	          result.push(p_path);
						result.push('/');
						result.push(p_metadata.name);
	          result.push("\",this)'  /></div>");
           break;
     case 'boolean':
						result.push("<div class='boolean'>");
						result.push(p_metadata.prompt);
						result.push(" <input type='checkbox' name='");
						result.push(p_metadata.name);
						result.push("' checked='");
						result.push(p_data);
						result.push("' onblur='g_ui.set_value(\"");
	          result.push(p_path);
						result.push('/');
						result.push(p_metadata.name);
	          result.push("\",this)' /></div>");
            break;
    case 'list':
    case 'yes_no':
    case 'radio_group':
					 result.push("<div class='list'>");
					 result.push(p_metadata.prompt);
					 if(p_metadata.values.length > 6)
					 {
						 result.push("<br/> <select size=7 name='");
					 }
					 else
					 {
							result.push("<br/> <select size=");
							result.push(p_metadata.values.length);
							result.push(" name='");
					 }

					result.push(p_metadata.name);
					result.push("' onblur='g_ui.set_value(\"");
					result.push(p_path);
					result.push('/');
					result.push(p_metadata.name);
					result.push("\",this)' >");
					 for(var i = 0; i < p_metadata.values.length; i++)
					 {
						 var item = p_metadata.values[i];
						 if(p_data == item)
						 {
							 	result.push("<option value='");
								result.push("' selected>");
								result.push(item);
								result.push("</option>");
						 }
						 else
						 {
							 result.push("<option value='");
							 result.push("'>");
							 result.push(item);
							 result.push("</option>");
						 }
					 }
					 result.push("</select></div>");
           break;

		 case 'multilist':
     case 'race':
			 result.push("<div class='multilist'>");
			 result.push(p_metadata.prompt);
       result.push(' ( select all that apply )');
			 if(p_metadata.values.length > 6)
			 {
				 result.push("<br/> <select size=7 multiple name='");
			 }
			 else
			 {
					result.push("<br/> <select size=");
					result.push(p_metadata.values.length);
					result.push(" name='");
			 }

			 result.push(p_metadata.name);
			 result.push("' onblur='g_ui.set_value(\"");
			 result.push(p_path);
			 result.push('/');
			 result.push(p_metadata.name);
			 result.push("\",this)' >");
			 for(var i = 0; i < p_metadata.values.length; i++)
			 {
				 var item = p_metadata.values[i];
				 if(p_data == item)
				 {
						result.push("<option value='");
						result.push("' selected>");
						result.push(item);
						result.push("</option>");
				 }
				 else
				 {
					 result.push("<option value='");
					 result.push("'>");
					 result.push(item);
					 result.push("</option>");
				 }
			 }
			 result.push("</select></div>");
			 break;
			case 'date':
				result.push("<div class='date'>");
				result.push(p_metadata.prompt);
				result.push("<br/> <input type='date' name='");
				result.push(p_metadata.name);
				result.push("' value='");
				result.push(p_data);
				result.push("' onblur='g_ui.set_value(\"");
				result.push(p_path);
				result.push('/');
				result.push(p_metadata.name);
				result.push("\",this)'  /></div>");
			 break;
	    case 'time':
					result.push("<div class='time'>");
					result.push(p_metadata.prompt);
					result.push("<br/> <input type='text' name='");
					result.push(p_metadata.name);
					result.push("' value='");
					result.push(p_data);
					result.push("' onblur='g_ui.set_value(\"");
          result.push(p_path);
					result.push('/');
					result.push(p_metadata.name);
          result.push("\",this)'  /></div>");
				 break;
/*            break;
    case 'radiolist':
           p_parent[p_metadata.name] = "";
           break;
     case 'date':
            p_parent[p_metadata.name] = new Date();
            break;
    case 'time':
           p_parent[p_metadata.name] = "";
           break;*/
     default:
          console.log("page_render not processed", p_metadata);
       break;
  }

	return result;

}
