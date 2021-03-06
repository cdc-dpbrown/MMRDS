//http://stackoverflow.com/questions/5558873/changing-user-name-and-password-in-couchdb-user-database

function aggregate_report_render(p_ui, p_data, p_metadata_path, p_object_path, p_is_grid_context)
{
	var result = [];

	result.push("<div style='clear:both;margin-left:10px;'>");
	result.push("<table border=1><tr style='background:#BBBBBB;'><th colspan=4>user list</th></tr>");
	
	for(var i = 0; i < p_ui.user_summary_list.length; i++)
	{
		var item = p_ui.user_summary_list[i];
		if(item._id != "org.couchdb.user:mmrds")
		{
			Array.prototype.push.apply(result, user_entry_render(item, i));
		}
	}
	result.push("<tr><td colspan=4 align=right>&nbsp;</tr>")
	result.push("<tr><td colspan=4 align=right>user name:<input type='text' id='new_user_name' value=''/>password:<input type='text' id='new_user_password' value=''/><input type='button' value='add new user' onclick='add_new_user_click()' /></tr>")
	result.push("</table></div><br/><br/>");


	return result;

}


function create_total_number_of_cases_by_pregnancy_relatedness(p_row_set)
{
	var result = [];
	var count = {
		'':0,
		'Pregnancy Related':0,
		'Pregnancy Associated But NOT Related':0,
		'Not Pregnancy Related or Associated (i.e. False Positive)':0,
		'Unable to Determine if Pregnancy Related or Associated':0
}


	for(var i = 0; i < p_row_set.length; i++)
	{
		

		switch(p_row_set.pregnancy_relatedness)
		{
		
		case 'Pregnancy Related':
		//case 'Pregnancy-Related':
			count['Pregnancy Related']++;
			break;
		case 'Pregnancy Associated But NOT Related':
		//case 'Pregnancy-Associated, but NOT -Related':
			count['Pregnancy-Associated but NOT Related']++;
			break;
		case 'Not Pregnancy Related or Associated (i.e. False Positive)':
			count['Not Pregnancy Related or Associated (i.e. False Positive)']++;
			break;
		case 'Unable to Determine if Pregnancy Related or Associated':
		//case 'Pregnancy-Associated but Unable to Determine Pregnancy-Relatedness':
			count['Unable to Determine if Pregnancy Related or Associated']++;
			break;
		case '':
		default:
			count['']++;
			break;
		}

	}

	result.push("<h3>Total Number of Cases by Pregnancy Relatedness</h3>")
	result.push("<ul>");
	
	result.push("<li>Pregnancy Related: ");
	result.push(count['Pregnancy Related']);
	result.push("</li><li>Pregnancy Associated But NOT Related: ");
	result.push(count['Pregnancy Associated But NOT Related']);
	result.push("</li><li>Not Pregnancy Related or Associated (i.e. False Positive): ");
	result.push(count['Not Pregnancy Related or Associated (i.e. False Positive)']);
	result.push("</li><li>Unable to Determine if Pregnancy Related or Associated: ");
	result.push(count['Unable to Determine if Pregnancy Related or Associated']);
	result.push("</li><li>blank: ");
	result.push(count['']);
	result.push('</li></ul>');

	return result;

}


function create_number_of_pregnancy_related_deaths_by_race_ethnicity(p_row_set)
{
	var result = [];
	var count = {
		'':0,
		'Hispanic':0,
		'Non-Hispanic Black':0,
		'Non-Hispanic White':0,
		'American Indian / Alaska Native':0,
		'Native Hawaiian':0,
		'Guamanian or Chamorro':0,
		'Samoan':0,
		'Other Pacific Islander':0,
		'Asian Indian':0,
		'Filipino':0,
		'Korean':0,
		'Other Asian':0,
		'Chinese':0,
		'Japanese':0,
		'Vietnamese':0,
		'Other':0
}

/*
committee_review/pregnancy_relatedness = Pregnancy Related; 

birth_fetal_death_certificate_parent/demographic_of_mother/is_of_hispanic_origin = Yes, Mexican, Mexican American, Chicano + Yes, Puerto Rican + Yes, Cuban + Yes, Other Spanish/Hispanic/Latino +Yes, Origin Unknown

IF NO BC present:
death_certificate/demographics/is_of_hispanic_origin = Yes, Mexican, Mexican American, Chicano + Yes, Puerto Rican + Yes, Cuban + Yes, Other Spanish/Hispanic/Latino +Yes, Origin Unknown
*/

	for(var i = 0; i < p_row_set.length; i++)
	{
		var row = p_row_set[i];
		if(p_row_set.pregnancy_relatedness && p_row_set.pregnancy_relatedness == 'Pregnancy Related')
		{
			if(row.bc_is_of_hispanic_origin && row.bc_is_of_hispanic_origin.indexOf('Yes') > -1)
			{
				count.Hispanic++;
			}
			else if(row.dc_is_of_hispanic_origin && row.dc_is_of_hispanic_origin.indexOf('Yes') > -1)
			{
				count.Hispanic++;
			}
		}


	}

	result.push("<h3>Total Number of Cases by Pregnancy Relatedness</h3>")
	result.push("<ul>");
	
	result.push("<li>Pregnancy Related: ");
	result.push(count['Pregnancy Related']);
	result.push("</li><li>Pregnancy Associated But NOT Related: ");
	result.push(count['Pregnancy Associated But NOT Related']);
	result.push("</li><li>Not Pregnancy Related or Associated (i.e. False Positive): ");
	result.push(count['Not Pregnancy Related or Associated (i.e. False Positive)']);
	result.push("</li><li>Unable to Determine if Pregnancy Related or Associated: ");
	result.push(count['Unable to Determine if Pregnancy Related or Associated']);
	result.push("</li><li>blank: ");
	result.push(count['']);
	result.push('</li></ul>');

	return result;

}