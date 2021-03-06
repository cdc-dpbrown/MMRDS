function print_version_render(p_metadata, p_data,  p_path, p_ui, p_metadata_path, p_object_path, p_post_html_render, p_muliform_index)
{
	var result = [];
	switch(p_metadata.type.toLowerCase())
	{
		case 'group':
				result.push('<fieldset>');
				//result.push(p_path)
				result.push('<legend>')
				result.push(p_metadata.prompt);
				result.push('</legend> ');
				//result.push(p_data[p_metadata.name]);

				for(var i = 0; i < p_metadata.children.length; i++)
				{
					var child = p_metadata.children[i];
					if(p_data[child.name] != null || child.type == "chart")
					Array.prototype.push.apply(result, print_version_render(child, p_data[child.name], p_path + "." + child.name, p_ui, p_metadata_path + '.children[' + i + "]", p_object_path + "." + child.name, p_post_html_render, p_muliform_index));
				}

				result.push('</fieldset>');
				break;	
		case 'grid':
		
				result.push('<table border="1">');
				//result.push(p_path)
				result.push('<tr><th colspan=')
				result.push(p_metadata.children.length);
				result.push('>')
				result.push(p_metadata.prompt);
				result.push('</th></tr>');
				//result.push(p_data[p_metadata.name]);
				result.push("<tr>");
				for(var i = 0; i < p_metadata.children.length; i++)
				{
					var child = p_metadata.children[i];
					result.push("<td>");
					result.push(child.prompt);
					result.push("</td>");
				}
				result.push("</tr>");

				for(var i = 0; i < p_data.length; i++)
				{
					result.push("<tr>");
					for(var j = 0; j < p_metadata.children.length; j++)
					{
						result.push("<td>");
						var child = p_metadata.children[j];
						if(p_data[i][child.name] == null)
						{
							result.push("&nbsp;");
						}
						else
						{
							var data = p_data[i][child.name];
							if(Array.isArray(data))
							{
								result.push("<ul>");
								for(var k = 0; k < data.length; k++)
								{
									result.push("<li>");
									result.push(data[k]);
									result.push("</li>");
								}
								result.push("</ul>");
							}
							else
							{
								result.push(data);
							}
						}
						result.push("</td>");
					}
					result.push("</tr>");
				}

				result.push('</table>');
				break;					
		case 'form':

		if(
			 p_metadata.cardinality == "+" ||
			 p_metadata.cardinality == "*"
		
		)
		{
			result.push('<section id="');
			result.push(p_metadata.name)
			result.push('">');

			for(var form_index = 0; form_index < p_data.length; form_index++)
			{
				var form_item = p_data[form_index];

				
				//result.push(p_metadata.name)
				result.push('<h2>')
				result.push(p_metadata.prompt);
				result.push(' Record: ');
				result.push(form_index + 1);
				result.push('</h2> ');
				//result.push(p_data[p_metadata.name]);
				
				if(p_metadata.children)
				{
					for(var i = 0; i < p_metadata.children.length; i++)
					{
						var child = p_metadata.children[i];
						if(form_item[child.name] != null || child.type == "chart")
						Array.prototype.push.apply(result, print_version_render(child, form_item[child.name], p_path + "." + child.name, p_ui, p_metadata_path + '.children[' + i + "]", p_object_path + "[" + i + "]." + child.name, p_post_html_render, form_index));
					}
				}
			}
			result.push('</section>');
		}
		else
		{
			result.push('<section id="');
				result.push(p_metadata.name)
				result.push('"> <h2>')
				result.push(p_metadata.prompt);
				result.push('</h2> ');
				//result.push(p_data[p_metadata.name]);
				
				if(p_metadata.children)
				{
					for(var i = 0; i < p_metadata.children.length; i++)
					{
						var child = p_metadata.children[i];
						if(p_data[child.name] != null || child.type == "chart")
						Array.prototype.push.apply(result, print_version_render(child, p_data[child.name], p_path + "." + child.name, p_ui, p_metadata_path + '.children[' + i + "]", p_object_path + "." + child.name, p_post_html_render, p_muliform_index));
					}
				}
				result.push('</section>');
		}
				
				break;	
		case "list":
		
				result.push('<p>');
				//result.push(p_path)
				result.push('<h9>');
				result.push(' <strong>')
				result.push(p_metadata.prompt);
				result.push('</strong>: ');
				
				//result.push(p_data[p_metadata.name]);
				if(Array.isArray(p_data))
				{
					result.push("<ul>");
					for(var i = 0; i < p_data.length; i++)
					{
						result.push("<li>");
						result.push(p_data[i]);
						result.push("</li>");

					}
					result.push("</ul>");
				}
				else
				{
					result.push(p_data);
				}
				result.push('</h9>');
				result.push('</p>');
				break;				
		case 'app':
				/*
				result.push('<p>');
				//result.push(p_path)
				result.push(' <strong>')
				result.push(p_metadata.prompt);
				result.push('</strong>: ');
				result.push(p_data[p_metadata.name]);
				result.push('</p>');
				*/
				if(p_metadata.children)
				{
					for(var i = 0; i < p_metadata.children.length; i++)
					{
						var child = p_metadata.children[i];
						if(child.type.toLowerCase() == "form" && p_data[child.name] != null)
						Array.prototype.push.apply(result, print_version_render(child, p_data[child.name], p_path + "." + child.name, p_ui, p_metadata_path + '.children[' + i + "]", p_object_path + "." + child.name, p_post_html_render, p_muliform_index));
					}
				}
				break;	
		case 'chart':
			result.push("<div  class='chart' id='");
			result.push(convert_object_path_to_jquery_id(p_object_path, p_muliform_index));
			
			result.push("' ");
			result.push(" mpath='");
			result.push(p_metadata_path);
			result.push("' ");
			result.push(">");
			result.push("<span ");
			if(p_metadata.description && p_metadata.description.length > 0)
			{
				result.push("rel='tooltip'  data-original-title='");
				result.push(p_metadata.description.replace(/'/g, "\\'"));
				result.push("'>");
			}
			else
			{
				result.push(">");
			} 
			

			result.push(p_metadata.prompt);

			result.push("</span>");
			result.push("</div>");


			p_post_html_render.push("  c3.generate({");
			p_post_html_render.push("  size: {");
			p_post_html_render.push("  height: 240,");
			p_post_html_render.push("  width: 480");
			p_post_html_render.push("  },");
			p_post_html_render.push("  bindto: '#");
			p_post_html_render.push(convert_object_path_to_jquery_id(p_object_path, p_muliform_index));
			p_post_html_render.push("',");





/*


d3.select('#chart svg').append('text')
    .attr('x', d3.select('#chart svg').node().getBoundingClientRect().width / 2)
    .attr('y', 16)
    .attr('text-anchor', 'middle')
    .style('font-size', '1.4em')
    .text('Title of this chart');
*/
			if(p_metadata.x_axis && p_metadata.x_axis != "")
			{
				p_post_html_render.push("axis: {");
				p_post_html_render.push("x: {");
				p_post_html_render.push("type: 'timeseries',");
				p_post_html_render.push("localtime: false,");
				p_post_html_render.push("tick: {");
				p_post_html_render.push(" format: '%Y-%m-%d %H:%M:%S'");
				p_post_html_render.push("}");
				p_post_html_render.push("        }},");
			}

			p_post_html_render.push("  data: {");

			if(p_metadata.x_axis && p_metadata.x_axis != "")
			{
				p_post_html_render.push("x: 'x', xFormat: '%Y-%m-%d %H:%M:%S',");
				//p_post_html_render.push("x: 'x', ");
			}

			p_post_html_render.push("      columns: [");

			if(p_metadata.x_axis && p_metadata.x_axis != "")
			{
				p_post_html_render.push(get_chart_x_range_from_path(p_metadata, p_metadata.x_axis, p_ui, p_muliform_index));
			}
			//p_post_html_render.push("  ['data1', 30, 200, 100, 400, 150, 250],");
			//p_post_html_render.push("['data2', 50, 20, 10, 40, 15, 25]")


			if(p_metadata.y_label && p_metadata.y_label != "")
			{
				var y_labels = p_metadata.y_label.split(",");
				var y_axis_paths = p_metadata.y_axis.split(",");
				for(var y_index = 0; y_index < y_axis_paths.length; y_index++)
				{
					p_post_html_render.push(get_chart_y_range_from_path(p_metadata, y_axis_paths[y_index], p_ui, y_labels[y_index], p_muliform_index));
					if(y_index < y_axis_paths.length-1)
					{
						p_post_html_render.push(",");
					}
				}
			}
			else
			{

				var y_axis_paths = p_metadata.y_axis.split(",");
				for(var y_index = 0; y_index < y_axis_paths.length; y_index++)
				{
					p_post_html_render.push(get_chart_y_range_from_path(p_metadata, y_axis_paths[y_index], p_ui, null, p_muliform_index));
					if(y_index < y_axis_paths.length-1)
					{
						p_post_html_render.push(",");
					}
				}
			}


			
			p_post_html_render.push("  ]");
			p_post_html_render.push("  }");
			p_post_html_render.push("  });");

			p_post_html_render.push(" d3.select('#" + convert_object_path_to_jquery_id(p_object_path, p_muliform_index) + " svg').append('text')");
			p_post_html_render.push("     .attr('x', d3.select('#" + convert_object_path_to_jquery_id(p_object_path, p_muliform_index) + " svg').node().getBoundingClientRect().width / 2)");
			p_post_html_render.push("     .attr('y', 16)");
			p_post_html_render.push("     .attr('text-anchor', 'middle')");
			p_post_html_render.push("     .style('font-size', '1.4em')");
			p_post_html_render.push("     .text('" + p_metadata.prompt + "');");

			break;	
		case 'boolean':
			result.push('<h9>')
			result.push('<p>');
			result.push(' <strong>')
			result.push(p_metadata.prompt);
			result.push('</strong>: ');
			if(p_data)
			{
				result.push(p_data);
			}
			else
			{
				result.push("&nbsp;");
			}
			result.push('</p>');
			result.push('</h9>')
			break;
		default:
				
			
				if(p_metadata.name != "case_opening_overview")
				{
					result.push('<h9>')
					result.push('<p>');
					result.push(' <strong>')
					result.push(p_metadata.prompt);
						result.push('</strong>: ');
					result.push(p_data);
					result.push('</p>');
					result.push('</h9>')
				}
			if  (p_metadata.name == "case_opening_overview") {
				
	            result.push('<div class="box">');
						
			
				if(g_data.home_record.record_id)
				{
					
					result.push(g_data.home_record.record_id);
					result.push('<br>');
				}
					
				result.push(p_data);
				
		        result.push('</div>');
				}
			
			
			
			
			
				/*

				
				if(p_metadata.children)
				{
					for(var i = 0; i < p_metadata.children.length; i++)
					{
						var child = p_metadata.children[i];
						if(p_data[child.name] != null)
						Array.prototype.push.apply(result, print_version_render(child, p_data[child.name], p_path + "." + child.name, p_ui));
					}
				}*/
				break;
	}

	return result;

}

function get_chart_x_ticks_from_path(p_metadata, p_metadata_path, p_ui, p_current_multiform_index )
{
	//prenatal/routine_monitoring/systolic_bp,prenatal/routine_monitoring/diastolic
	// p_ui.url_state.path_array.length
	var result = [];
	var array_field = eval(convert_dictionary_path_to_array_field(p_metadata_path, p_current_multiform_index));

	var array = eval(array_field[0]);

	if(array)
	{
		var field = array_field[1];

		result.push("[");
		//result.push("['x'");
		// ['data2', 50, 20, 10, 40, 15, 25]
		//result.push(50, 20, 10, 40, 15, 25);

		//result = ['data2', 50, 20, 10, 40, 15, 25];
		for(var i = 0; i < array.length; i++)
		{
			var val = array[i][field];
			if(val)
			{
				result.push(parseFloat(val));
			}
			else
			{
				result.push(0);
			}
			
		}

		result[result.length-1] = result[result.length-1] + "]";
		return result.join(",");
	}
	else
	{
		return "";
	}
	
}

function get_chart_x_range_from_path(p_metadata, p_metadata_path, p_ui, p_current_multiform_index)
{
	//prenatal/routine_monitoring/systolic_bp,prenatal/routine_monitoring/diastolic
	// p_ui.url_state.path_array.length
	var result = [];
	var array_field = eval(convert_dictionary_path_to_array_field(p_metadata_path, p_current_multiform_index));

	var array = eval(array_field[0]);
	if(array)
	{
		var field = array_field[1];


		result.push("['x'");
		// ['data2', 50, 20, 10, 40, 15, 25]
		//result.push(50, 20, 10, 40, 15, 25);

		//result = ['data2', 50, 20, 10, 40, 15, 25];
		for(var i = 0; i < array.length; i++)
		{
			var val = array[i][field];
			if(val)
			{
				var res = val.match(/^\d\d\d\d-\d\d-\d+$/);
				if(res)
				{
					result.push("'" + make_c3_date(val) +"'");
				}
				else 
				{
					res = val.match(/^\d\d\d\d-\d\d-\d\d[ T]?\d\d:\d\d:\d\d$/)
					if(res)
					{
						//var date_time = new Date(val);
						//result.push("'" + date_time.toISOString() + "'");
						result.push("'" + make_c3_date(val) +"'");
					}
					else
					{
						result.push(parseFloat(val));
					}
				}
			}
			else
			{
				result.push(0);
			}
			
		}

		result[result.length-1] = result[result.length-1] + "]";
		return result.join(",") + ",";
	}
	else
	{
		return "";
	}
}

function get_chart_y_range_from_path(p_metadata, p_metadata_path, p_ui, p_label, p_current_multiform_index)
{
	//prenatal/routine_monitoring/systolic_bp,prenatal/routine_monitoring/diastolic
	// p_ui.url_state.path_array.length
	var result = [];
	var array_field = eval(convert_dictionary_path_to_array_field(p_metadata_path, p_current_multiform_index));

	var array = eval(array_field[0]);

	var field = array_field[1];

	if(p_label)
	{
		result.push("['" + p_label + "'");
	}
	else
	{
		result.push("['" + array_field[1] + "'");
	}
	
	if(array)
	{
		// ['data2', 50, 20, 10, 40, 15, 25]
		//result.push(50, 20, 10, 40, 15, 25);

		//result = ['data2', 50, 20, 10, 40, 15, 25];
		for(var i = 0; i < array.length; i++)
		{
			var val = array[i][field];
			if(val)
			{
				result.push(parseFloat(val));
			}
			else
			{
				result.push(0);
			}
			
		}

		result[result.length-1] = result[result.length-1] + "]";
		return result.join(",");
	}
	else
	{
		return result.join("") + "]";;
	}
}

function convert_dictionary_path_to_array_field(p_path, p_current_multiform_index)
{

	//g_data.prenatal.routine_monitoring.systolic_bp
	//er_visit_and_hospital_medical_records/vital_signs/date_and_time
	//er_visit_and_hospital_medical_records[current_index]/vital_signs[]/date_and_time
	/* [
			er_visit_and_hospital_medical_records[current_index]/vital_signs,
			date_and_time
	 ]

	*/
	//g_data.er_visit_and_hospital_medical_records[current_index].vital_signs[].date_and_time
	//var temp = "g_data." + p_path.replace(new RegExp('/','gm'),".").replace(new RegExp('\\.(\\d+)\\.','gm'),"[$1].").replace(new RegExp('\\.(\\d+)$','g'),"[$1]");

	var result = []
	var temp = "g_data." + p_path.replace(new RegExp('/','gm'),".");
	
	var multi_form_check = temp.split(".") ;
	var check_path = eval(multi_form_check[0] + "." + multi_form_check[1]);
	if(Array.isArray(check_path))
	{
		var new_path = [];
		for(var i = 0; i < multi_form_check.length; i++)
		{
			
			if(i == 1)
			{
				new_path.push(multi_form_check[i] + "[" + p_current_multiform_index + "]");
			}
			else
			{
				new_path.push(multi_form_check[i]);
			}
		}
		var path = new_path.join('.');
		var index = path.lastIndexOf('.');
		result.push(path.substr(0, index));
		result.push(path.substr(index + 1, path.length - (index + 1)));
	}
	else
	{
		var index = temp.lastIndexOf('.');
		result.push(temp.substr(0, index));
		result.push(temp.substr(index + 1, temp.length - (index + 1)));
	}

	return result;
}


function convert_dictionary_path_to_lookup_object(p_path)
{

	//g_data.prenatal.routine_monitoring.systolic_bp
	var result = null;
	var temp_result = []
	var temp = "g_metadata." + p_path.replace(new RegExp('/','gm'),".").replace(new RegExp('\\.(\\d+)\\.','gm'),"[$1].").replace(new RegExp('\\.(\\d+)$','g'),"[$1]");
	var index = temp.lastIndexOf('.');
	temp_result.push(temp.substr(0, index));
	temp_result.push(temp.substr(index + 1, temp.length - (index + 1)));

	var lookup_list = eval(temp_result[0]);

	for(var i = 0; i < lookup_list.length; i++)
	{
		if(lookup_list[i].name == temp_result[1])
		{
			result = lookup_list[i].values;
			break;
		}
	}


	return result;
}

function convert_object_path_to_jquery_id(p_value, p_multiform_index)
{
	if(p_multiform_index)
	{
		return p_value.replace(/\./g,"_").replace(/\[/g,"_").replace(/\]/g,"_") + p_multiform_index;
	}
	else
	{
		return p_value.replace(/\./g,"_").replace(/\[/g,"_").replace(/\]/g,"_")
	}
}


function make_c3_date(p_value)
{
	//'%Y-%m-%d %H:%M:%S
	
	var date_time = new Date(p_value);

	var result = [];

	result.push(date_time.getFullYear());
	result.push("-");
	result.push(date_time.getMonth() + 1);
	result.push("-");
	result.push(date_time.getDate());

	result.push(" ");
	result.push(date_time.getHours());
	result.push(":");
	result.push(date_time.getMinutes());
	result.push(":");
	result.push(date_time.getSeconds());

	return result.join("");
}