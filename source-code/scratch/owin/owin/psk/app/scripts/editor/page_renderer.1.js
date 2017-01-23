function page_render(p_metadata, p_data, p_ui, p_metadata_path, p_object_path, p_is_grid_context, p_row, p_column)
{

	
	var result = [];

	switch(p_metadata.type.toLowerCase())
  {
    case 'grid':
		var is_grid_context = true;

		result.push("<table id='");
		result.push(p_metadata_path);
		result.push("' class='grid'><tr><th colspan=");
		result.push(p_metadata.children.length + 1)
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
		result.push('<th>&nbsp;</th></tr>');

		for(var i = 0; i < p_data.length; i++)
		{
			result.push('<tr>');
			for(var j = 0; j < p_metadata.children.length; j++)
			{
				var child = p_metadata.children[j];
				result.push("<td>");
				Array.prototype.push.apply(result, page_render(child, p_data[i][child.name], p_ui, p_metadata_path + ".children[" + i + "]", p_object_path + "[" + i + "]." + child.name, is_grid_context, p_row, p_column));
				result.push("</td>");
			}
			result.push('<td> <input type="button" value="delete" id="delete_');
			result.push(p_object_path + "[" + i + "]");
			result.push('" onclick="g_delete_grid_item(\'');
			result.push(p_object_path + "[" + i + "]");
			result.push("', '");
			result.push(p_metadata_path);
			result.push('\')" /></td></tr>');
		}
    	result.push("<tr><td colspan=");
		result.push(p_metadata.children.length + 1);
		result.push(" align=right> <input type='button' value='Add Item' onclick='g_add_grid_item(\"");
		result.push(p_object_path);
		result.push("\", \"");
		result.push(p_metadata_path);
		result.push("\")' /></td></tr>");

		result.push("</table>");
		break;
    case 'group':
		if(p_column % 2 == 0)
		{
			result.push("</div>");
		}
		if(g_metadata_summary[p_metadata_path].group_level == 0)
		{
			result.push("<div class='row'>");
		}
		result.push("<fieldset id='");
		result.push(p_metadata.name);
		result.push("_id' class='group'><legend ");
		if(p_metadata.description && p_metadata.description.length > 0)
		{
			result.push("rel='tooltip' data-original-title='");
			result.push(p_metadata.description.replace(/'/g, "\\'"));
			result.push("'>");
		}
		else
		{
			result.push(">");
		}
		result.push(p_metadata.prompt);
		result.push("</legend>");
		for(var i = 0; i < p_metadata.children.length; i++)
		{
			var child = p_metadata.children[i];
			if(p_data[child.name])
			{

			}
			else
			{
				p_data[child.name] = create_default_object(child, {})[child.name];
			}
			Array.prototype.push.apply(result, page_render(child, p_data[child.name], p_ui, p_metadata_path + '.children[' + i + "]", p_object_path + "." + child.name, false, p_row, p_column + i));
		}
		result.push("</fieldset>");
		result.push("</div>");
		break;
    case 'form':
		if(
			 p_metadata.cardinality == "+" ||
			 p_metadata.cardinality == "*"
		
		)
		{
			result.push("<section id='");
			result.push(p_metadata.name);
			result.push("_id' class='form'><h2 ");
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
			result.push("</h2>");

			result.push('<input path="" type="button" value="add new ');
			result.push(p_metadata.prompt);
			result.push(' form" onclick="add_new_form_click(\'' + p_metadata_path + '\',\'' + p_object_path + '\')" />');

			result.push('<div class="search_wrapper">');
			for(var i = 0; i < p_data.length; i++)
			{
				var item = p_data[i];
				if(item)
				{
					if(i % 2)
					{
						result.push('		  <div class="result_wrapper_grey"> <a href="#/');
					}
					else
					{
						result.push('		  <div class="result_wrapper"> <a href="#/');
					}
					result.push(p_ui.url_state.path_array.join("/"));
					//result.push(p_metadata.name);
					result.push("/");
					result.push(i);
					result.push("\">");
					result.push('record ');
					result.push(i + 1);
					/*
					for(var j = 0; j < p_metadata.children.length && j < 5; j++)
					{
						result.push(item[p_metadata.children[j].name]);
						result.push(' ');
					}*/
					result.push('</a>');
					result.push('</div>');
				}

			}
			result.push('		</div>');
			result.push("</section>");


			

			if(p_ui.url_state.path_array.length > 2)
			{
				var data_index = p_ui.url_state.path_array[2];
				result.push("<section id='");
				result.push(p_metadata.name);
				result.push("' class='form'><h2 ");
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
				result.push("</h2>");
				
				for(var i = 0; i < p_metadata.children.length; i++)
				{
					var child = p_metadata.children[i];
					if(p_data[data_index][child.name])
					{

					}
					else
					{
						p_data[data_index][child.name] = create_default_object(child, {})[child.name];
					}
					Array.prototype.push.apply(result, page_render(child, p_data[data_index][child.name], p_ui, p_metadata_path + '.children[' + i + "]", p_object_path + "[" + data_index + "]." + child.name, false, 0, 0));
				}
				result.push("</section>");

			}

		}
		else
		{

			result.push("<section id='");
			result.push(p_metadata.name);
			result.push("_id' class='form'><h2 ");
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
			result.push("</h2>");

			if(g_data && p_metadata.name == "case_narrative")
			{
				//death_certificate/reviewer_note
				result.push("<h3>death_certificate/reviewer_note</h3>");
				result.push("<textarea cols=80 rows=7>");
				result.push(g_data.death_certificate.reviewer_note);
				result.push("</textarea>");

				//birth_fetal_death_certificate_parent/reviewer_note
				result.push("<h3>birth_fetal_death_certificate_parent/reviewer_note</h3>");
				result.push("<textarea cols=80 rows=7>");
				result.push(g_data.birth_fetal_death_certificate_parent.reviewer_note);
				result.push("</textarea>");

				
				//birth_certificate_infant_fetal_section/reviewer_note
				result.push("<h3>birth_certificate_infant_fetal_section/reviewer_note</h3>");
				for(var i = 0; i < g_data.birth_certificate_infant_fetal_section.length; i++)
				{
					result.push("<textarea cols=80 rows=7>");
					result.push(g_data.birth_certificate_infant_fetal_section[i].reviewer_note);
					result.push("</textarea>");
				}
				
				//autopsy_report/reviewer_note
				result.push("<h3>autopsy_report/reviewer_note</h3>");
				result.push("<textarea cols=80 rows=7>");
				result.push(g_data.autopsy_report.reviewer_note);
				result.push("</textarea>");

				
				//prenatal/reviewer_note
				result.push("<h3>prenatal/reviewer_note</h3>");
				result.push("<textarea cols=80 rows=7>");
				result.push(g_data.prenatal.reviewer_note);
				result.push("</textarea>");
				

				
				//er_visit_and_hospital_medical_records/reviewer_note
				result.push("<h3>er_visit_and_hospital_medical_records/reviewer_note</h3>");
				for(var i = 0; i < g_data.er_visit_and_hospital_medical_records.length; i++)
				{
					result.push("<textarea cols=80 rows=7>");
					result.push(g_data.er_visit_and_hospital_medical_records[i].reviewer_note);
					result.push("</textarea>");
				}
				
				//other_medical_office_visits/reviewer_note
				result.push("<h3>other_medical_office_visits/reviewer_note</h3>");
				for(var i = 0; i < g_data.er_visit_and_hospital_medical_records.length; i++)
				{
					result.push("<textarea cols=80 rows=7>");
					result.push(g_data.er_visit_and_hospital_medical_records[i].reviewer_note);
					result.push("</textarea>");
				}
///medical_transport/transport_narrative_summary
				result.push("<h3>medical_transport/transport_narrative_summary</h3>");
				for(var i = 0; i < g_data.medical_transport.length; i++)
				{
					result.push("<textarea cols=80 rows=7>");
					result.push(g_data.medical_transport[i].transport_narrative_summary);
					result.push("</textarea>");
				}
				
				//social_and_environmental_profile/reviewer_note
				result.push("<h3>social_and_environmental_profile/reviewer_note</h3>");
				result.push("<textarea cols=80 rows=7>");
				result.push(g_data.social_and_environmental_profile.reviewer_note);
				result.push("</textarea>");
				
			}

			for(var i = 0; i < p_metadata.children.length; i++)
			{
				var child = p_metadata.children[i];
				if(p_data[child.name])
				{

				}
				else
				{
					p_data[child.name] = create_default_object(child, {})[child.name];
				}
				Array.prototype.push.apply(result, page_render(child, p_data[child.name], p_ui, p_metadata_path + '.children[' + i + "]", p_object_path + "." + child.name, false, p_row, p_column));
			}
			result.push("</section>");
		}
		break;
    case 'app':
		result.push("<section id='app_summary'><h2>summary</h2>");
		result.push("<input type='button' class='btn-green' value='add new case' onclick='g_ui.add_new_case()' /><hr/>");
		result.push("<fieldset><legend>filter line listing</legend>");
		result.push("<input type='text' id='search_text_box' value='' /> ");
		result.push("<img src='/images/search.png' alt='search' height=8px width=8px valign=bottom class='btn-green' id='search_command_button'>");
		result.push("</fieldset>");

		result.push('<div class="search_wrapper">');
		for(var i = 0; i < p_ui.data_list.length; i++)
		{
			var item = p_ui.data_list[i];

			if(i % 2)
			{
				result.push('		  <div class="result_wrapper_grey" path="');
			}
			else
			{
				result.push('		  <div class="result_wrapper" path="');
			}
			result.push(item._id);
			result.push('"><p class="result">');
			result.push(item.home_record.last_name);
			result.push(', ');
			result.push(item.home_record.first_name);
			result.push(' - ');
			result.push(item.home_record.record_id);
			result.push('	(');
			result.push(item.home_record.state_of_death);
			result.push('	) <a href="#/'+ i + '/home_record" role="button" class="btn-purple">select</a> <input type="button" value="delete" onclick="delete_record(' + i + ')"/></p>');
			result.push('</div>');
			
		}
		result.push('		</div>');


		result.push("</section>");

		for(var i = 0; i < p_metadata.children.length; i++)
		{
			var child = p_metadata.children[i];
			if(child.type.toLowerCase() == 'form')
			{
					Array.prototype.push.apply(result, page_render(child, p_data[child.name], p_ui, p_metadata_path  + ".children[" + i + "]", p_object_path + "." + child.name, false, p_row, p_column));				 		
			}
		}

		result.push('<footer class="footer_wrapper">');
		result.push('<p>&nbsp;</p>');
		result.push('</footer>');

       break;
     case 'label':
			result.push("<div class='label' id='");
			result.push(p_object_path);
			result.push("' ");
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
			result.push("</div>");
			break;
     case 'button':
			result.push("<input class='button' type='button' id='");
			result.push(p_object_path);
			result.push("' ");
			if(p_metadata.description && p_metadata.description.length > 0)
			{
				result.push("rel='tooltip'  data-original-title='");
				result.push(p_metadata.description.replace(/'/g, "\\'"));
				result.push("' value='");
			}
			else
			{
				result.push(" value='");
			}

			result.push(p_metadata.prompt);
			result.push("' />");
			break;
		case 'string':
			result.push("<div class='col-sm-4 string' id='");
			result.push(p_object_path);
			result.push("'><span ");
			if(p_metadata.description && p_metadata.description.length > 0)
			{
				result.push("rel='tooltip'  data-original-title='");
				result.push(p_metadata.description.replace(/'/g, "\\'"));
				result.push("' ");
			}
			else
			{
				result.push(" ");
			}

			if(p_metadata.validation_description && p_metadata.validation_description.length > 0)
			{
				result.push(" validation-tooltip='");
				result.push(p_metadata.validation_description.replace(/'/g, "\\'"));
				result.push("'>");
			}
			else
			{
				result.push(">");
			}
			
			if(p_is_grid_context && p_is_grid_context == true)
			{

			}
			else
			{
				result.push(p_metadata.prompt);
			}
			result.push("</span><br/> <input type='text' name='");
			result.push(p_metadata.name);
			result.push("' value='");
			result.push(p_data);
			if(p_metadata.onfocus && p_metadata.onfocus != "")
			{
				var source_code = escodegen.generate(p_metadata.onfocus);
				var code_array = [];
				
				code_array.push(source_code.substring(0, source_code.length-1));
				code_array.push(".call(");
				code_array.push(p_object_path.substring(0, p_object_path.lastIndexOf(".")));
				code_array.push(", this);");
				result.push("' onfocus='");
				result.push(code_array.join('').replace(/'/g,"\""));
			}
			
			result.push("' onblur='g_set_data_object_from_path(\"");
			result.push(p_object_path);
			result.push("\",\"");
			result.push(p_metadata_path);
			result.push("\",this.value)' /></div>");
			
			
			break;
			   
	case 'address':
	case 'textarea':
				result.push("<div  class='col-sm-4 string' id='");
				result.push(p_object_path);
				result.push("'><span ");
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
				
				if(p_is_grid_context && p_is_grid_context == true)
				{

				}
				else
				{
					result.push(p_metadata.prompt);
				}
				result.push("</span><br/> <textarea  rows=5 cols=40 name='");
				result.push(p_metadata.name);
				result.push("'  onblur='g_set_data_object_from_path(\"");
				result.push(p_object_path);
				result.push("\",\"");
				result.push(p_metadata_path);
				result.push("\",this.value)' >");
				result.push(p_data);
				result.push("</textarea></div>");
           break;
     case 'number':
			result.push("<div class='col-sm-4 number' id='");
			result.push(p_object_path);
			result.push("'><span ");
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
			
			if(p_is_grid_context && p_is_grid_context == true)
			{

			}
			else
			{
				result.push(p_metadata.prompt);
			}
			result.push("</span><br/> <input type='Number' name='");
			result.push(p_metadata.name);
			result.push("' value='");
			result.push(p_data);
			result.push("'  onblur='g_set_data_object_from_path(\"");
			result.push(p_object_path);
			result.push("\",\"");
			result.push(p_metadata_path);
			result.push("\",this.value)'  /></div>");
           break;
     case 'boolean':
			result.push("<div class='col-sm-4 boolean' id='");
			result.push(p_object_path);
			result.push("'> <input type='checkbox' name='");
			result.push(p_metadata.name);
			if(p_data == true)
			{
				result.push("' checked='true'");
			}
			else
			{
				result.push("'  value='");
			}
			result.push(p_data);
			result.push("' onblur='g_set_data_object_from_path(\"");
			result.push(p_object_path);
			result.push("\",\"");
			result.push(p_metadata_path);
			result.push("\",this.checked)'  /><span ");
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
			
			if(p_is_grid_context && p_is_grid_context == true)
			{

			}
			else
			{
				result.push(p_metadata.prompt);
			}
			result.push("</span></div>");
            break;
    case 'list':
			if(p_metadata.control_style && p_metadata.control_style.toLowerCase().indexOf("editable") > -1)
			{
				result.push("<div class='col-sm-4 list' id='");
				result.push(p_object_path)
				
				result.push("'> <span ");
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
				
				if(p_is_grid_context && p_is_grid_context == true)
				{

				}
				else
				{
					result.push(p_metadata.prompt);
				}
				result.push("</span>");

				if(p_metadata.list_display_size && p_metadata.list_display_size!="")
				{
					result.push("<br/> <input type='text' name='");
					result.push(p_metadata.name);
					result.push("' value='");
					result.push(p_data);
					result.push("' onblur='g_set_data_object_from_path(\"");
					result.push(p_object_path);
					result.push("\",\"");
					result.push(p_metadata_path);
					result.push("\",this.value)' /> <br/> <select size=");
					result.push(p_metadata.list_display_size);
					result.push(" name='");
				}
				else if(p_metadata.is_multiselect && p_metadata.is_multiselect == true)
				{
					
					if(p_metadata.values.length > 6)
					{
						result.push("<br/> <select size='6' name='");
					}
					else
					{
						result.push("<br/> <select size=");
						result.push(p_metadata.values.length);
						result.push(" name='");
					}
					
				}
				else
				{
					result.push("<br/> <select size=");
					result.push(1);
					result.push(" name='");
				}

				result.push(p_metadata.name);
				result.push("'  onchange='g_set_data_object_from_path(\"");
				result.push(p_object_path);
				result.push("\",\"");
				result.push(p_metadata_path);
				result.push("\",this.value)'  ");

				if(p_metadata['is_multiselect'] && p_metadata.is_multiselect == true)
				{
					result.push(" multiple>");
					for(var i = 0; i < p_metadata.values.length; i++)
					{
						var item = p_metadata.values[i];
						if(p_data.indexOf(item.value) > -1)
						{
								result.push("<option value='");
								result.push(item.value.replace(/'/g, "\\'"));
								result.push("' selected>");
								if(p_metadata.is_save_value_display_description && p_metadata.is_save_value_display_description == true)
								{
									result.push(item.description);
								}
								else
								{
									result.push(item.value);
								}
								result.push("</option>");
						}
						else
						{
								result.push("<option value='");
								result.push(item.value.replace(/'/g, "\\'"));
								result.push("' >");
								if(p_metadata.is_save_value_display_description && p_metadata.is_save_value_display_description == true)
								{
									result.push(item.description);
								}
								else
								{
									result.push(item.value);
								}
								result.push("</option>");
						}
					}
					result.push("</select></div>");
				}
				else
				{
					result.push(">");

					for(var i = 0; i < p_metadata.values.length; i++)
					{
						var item = p_metadata.values[i];
						if(p_data == item.value)
						{
							result.push("<option value='");
							result.push(item.value.replace(/'/g, "\\'"));
							result.push("' selected>");
							if(p_metadata.is_save_value_display_description && p_metadata.is_save_value_display_description == true)
							{
								result.push(item.description);
							}
							else
							{
								result.push(item.value);
							}
							result.push("</option>");
						}
						else
						{
							result.push("<option value='");
							result.push(item.value.replace(/'/g, "\\'"));
							result.push("' >");
							if(p_metadata.is_save_value_display_description && p_metadata.is_save_value_display_description == true)
							{
								result.push(item.description);
							}
							else
							{
								result.push(item.value);
							}
							result.push("</option>");
						}
					}
					result.push("</select></div>");
				}
			}
			else
			{
				result.push("<div class='col-sm-4 list' id='");
				result.push(p_object_path)
				
				result.push("'> <span ");
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
				
				if(p_is_grid_context && p_is_grid_context == true)
				{

				}
				else
				{
					result.push(p_metadata.prompt);
				}
				result.push("</span>");

				if(p_metadata.list_display_size && p_metadata.list_display_size!="")
				{
					result.push("<br/> <select size=");
					result.push(p_metadata.list_display_size);
					result.push(" name='");
				}
				else if(p_metadata.is_multiselect && p_metadata.is_multiselect == true)
				{
					
					if(p_metadata.values.length > 6)
					{
						result.push("<br/> <select size='6' name='");
					}
					else
					{
						result.push("<br/> <select size=");
						result.push(p_metadata.values.length);
						result.push(" name='");
					}
					
				}
				else
				{
					result.push("<br/> <select size=");
					result.push(1);
					result.push(" name='");
				}

				result.push(p_metadata.name);
				result.push("'  onchange='g_set_data_object_from_path(\"");
				result.push(p_object_path);
				result.push("\",\"");
				result.push(p_metadata_path);
				result.push("\",this.value)'  ");

				if(p_metadata['is_multiselect'] && p_metadata.is_multiselect == true)
				{
					result.push(" multiple>");
					for(var i = 0; i < p_metadata.values.length; i++)
					{
						var item = p_metadata.values[i];
						if(p_data.indexOf(item.value) > -1)
						{
								result.push("<option value='");
								result.push(item.value.replace(/'/g, "\\'"));
								result.push("' selected>");
								if(p_metadata.is_save_value_display_description && p_metadata.is_save_value_display_description == true)
								{
									result.push(item.description);
								}
								else
								{
									result.push(item.value);
								}
								result.push("</option>");
						}
						else
						{
								result.push("<option value='");
								result.push(item.value.replace(/'/g, "\\'"));
								result.push("' >");
								if(p_metadata.is_save_value_display_description && p_metadata.is_save_value_display_description == true)
								{
									result.push(item.description);
								}
								else
								{
									result.push(item.value);
								}
								result.push("</option>");
						}
					}
					result.push("</select></div>");
				}
				else
				{
					result.push(">");

					for(var i = 0; i < p_metadata.values.length; i++)
					{
						var item = p_metadata.values[i];
						if(p_data == item.value)
						{
							result.push("<option value='");
							result.push(item.value.replace(/'/g, "\\'"));
							result.push("' selected>");
							if(p_metadata.is_save_value_display_description && p_metadata.is_save_value_display_description == true)
							{
								result.push(item.description);
							}
							else
							{
								result.push(item.value);
							}
							result.push("</option>");
						}
						else
						{
							result.push("<option value='");
							result.push(item.value.replace(/'/g, "\\'"));
							result.push("' >");
							if(p_metadata.is_save_value_display_description && p_metadata.is_save_value_display_description == true)
							{
								result.push(item.description);
							}
							else
							{
								result.push(item.value);
							}
							result.push("</option>");
						}
					}
					result.push("</select></div>");
				}
			}

           break;
	case 'date':
			if(typeof(p_data) != "date")
			{
				p_data = new Date(p_data);
			}
			result.push("<div class='col-sm-4 date' id='");
			result.push(p_object_path)
			
			result.push("'> ");
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
			
			if(p_is_grid_context && p_is_grid_context == true)
			{

			}
			else
			{
				result.push(p_metadata.prompt);
			}
			result.push("</span><br/> <input type='");
			//result.push(p_metadata.type.toLowerCase());
			result.push("date");
			result.push("' name='");
			result.push(p_metadata.name);
			result.push("' value='");
			result.push(p_data.toISOString().split("T")[0]);
			result.push("'  onblur='g_set_data_object_from_path(\"");
			result.push(p_object_path);
			result.push("\",\"");
			result.push(p_metadata_path);
			result.push("\",this.value)'  /></div>");
			 break;	
	case 'datetime':
			if(typeof(p_data) == "string")
			{
				p_data = new Date(p_data);
			}
			result.push("<div class='col-sm-4 date' id='");
			result.push(p_object_path)
			result.push("'> ");
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
			
			if(p_is_grid_context && p_is_grid_context == true)
			{

			}
			else
			{
				result.push(p_metadata.prompt);
			}
			result.push("</span><br/> <input type='");
			//result.push(p_metap_datadata.type.toLowerCase());
			result.push("date");
			result.push("'  name='");
			result.push(p_metadata.name);
			result.push("' value='");
			result.push(p_data.toISOString().split("T")[0]);
			result.push("'  onblur='g_set_data_object_from_path(\"");
			result.push(p_object_path);
			result.push("\",\"");
			result.push(p_metadata_path);
			result.push("\",this.value)'  />&nbsp;<input type='");
			//result.push(p_metadata.type.toLowerCase());
			result.push("time");
			result.push("' name='");
			result.push(p_metadata.name);
			result.push("' value='");
			result.push(p_data.toISOString().split("T")[1].replace("Z",""));
			result.push("'  onblur='g_set_data_object_from_path(\"");
			result.push(p_object_path);
			result.push("\",\"");
			result.push(p_metadata_path);
			result.push("\",this.value)'  /></div>");			
			 break;
		case 'time':
			if(typeof(p_data) == "string")
			{
				p_data = new Date(p_data);
			}
			result.push("<div class='col-sm-4 time' id='");
			result.push(p_object_path)
			
			result.push("'> ");
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
			
			if(p_is_grid_context && p_is_grid_context == true)
			{

			}
			else
			{
				result.push(p_metadata.prompt);
			}
			result.push("</span><br/> <input type='time' name='");
			result.push(p_metadata.name);
			result.push("' value='");
			result.push(p_data.toISOString().split("T")[1].replace("Z",""));
			result.push("' onblur='g_set_data_object_from_path(\"");
			result.push(p_object_path);
			result.push("\",\"");
			result.push(p_metadata_path);
			result.push("\",this.value)'   /></div>");
			break;
     default:
          console.log("page_render not processed", p_metadata);
       break;
  }

	return result;

}