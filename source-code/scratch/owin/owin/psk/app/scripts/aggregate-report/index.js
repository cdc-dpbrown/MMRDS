var g_data = null;
var g_couchdb_url = null;
var g_uid = null;
var g_pwd = null;


var g_ui = { 
	user_summary_list:[],
	user_list:[],
	data:null,
	url_state: {
    selected_form_name: null,
    selected_id: null,
    selected_child_id: null,
    path_array : []

  }
};



$(function ()
{//http://www.w3schools.com/html/html_layout.asp
  'use strict';

	load_values();
});

function load_values()
{
	$.ajax({
			url: location.protocol + '//' + location.host + '/api/values',
	}).done(function(response) {
			g_couchdb_url = response.couchdb_url;
	});

}

function load_data(p_uid, p_pwd)
{
	var url =  location.protocol + '//' + location.host + '/api/aggregate_report?' + p_uid

//	var prefix = 'http://' + p_uid + ":" + p_pwd + '@';
    //var url = prefix + g_couchdb_url.replace('http://','') + '/mmrds/_design/aggregate_report/_view/all';

	$.ajax({
			url: url
	}).done(function(response) {
			
			g_data = response.rows;

			document.getElementById('generate_report_button').disabled = false;
			//process_rows();
			//document.getElementById('navigation_id').innerHTML = navigation_render(g_user_list, 0, g_ui).join("");

			//document.getElementById('form_content_id').innerHTML = aggregate_report_render(g_ui, "", g_ui).join("");

	});
}

function generate_report_click()
{

	var year_of_death = document.getElementById('year_of_death').value;
	var month_of_case_review = document.getElementById('month_of_case_review').value;
	var  year_of_case_review = document.getElementById('year_of_case_review').value;

	document.getElementById('report_output_id').innerHTML ="process rows: "  + g_data.length;
}


function process_rows()
{
	for(var i = 0; i < g_data.length; i++)
	{
		var current_row = g_data[i];
	}
	
}


var summary_row = {
'id': [],
'hr_date_of_death_year': [],
'dc_date_of_death': [],
'date_of_review':  [],
'was_this_death_preventable':  [],
'pregnancy_relatedness':  [],
'bc_is_of_hispanic_origin':  [],
'dc_is_of_hispanic_origin':  [],
'age':  [],
'pmss':  [],
'did_obesity_contribute_to_the_death': [],
'did_mental_health_conditions_contribute_to_the_death': [],
'did_substance_use_disorder_contribute_to_the_death': [],
'was_this_death_a_sucide': [],
'was_this_death_a_homicide': [],
};

var total_row = 
{
Pregnancy_Relatedness : [],
Pregnancy_Related_Deaths_by_Race_Ethnicity : [],
Pregnancy_Related_Deaths_by_Timing_of_Death_in_Relation_to_Pregnancy : [],
Pregnancy_Related_Deaths_Determined_to_be_Preventable : [],
Pregnancy_Related_Deaths_by_Age_at_Death : [],
Pregnancy_Related_Deaths_Where_Obesity_Contributed_to_the_Death : [],
Pregnancy_Associated_but_NOT_Related_Deaths_by_Race_Ethnicity : [],
Pregnancy_Associated_Deaths_by_Timing_of_Death_in_Relation_to_Pregnancy : [],
Pregnancy_Associated_but_NOT_Related_Deaths_Determined_to_be_Preventable : [],
Pregnancy_Associated_but_NOT_Related_Deaths_by_Age_at_Death : []
}

/*
Pregnancy Relatedness
Pregnancy Related Deaths by Race-Ethnicity
Pregnancy Related Deaths by Timing of Death in Relation to Pregnancy
Pregnancy Related Deaths Determined to be Preventable
Pregnancy Related Deaths by Age at Death
Pregnancy Related Deaths Where Obesity Contributed to the Death

Pregnancy Associated but NOT Related Deaths by Race-Ethnicity
Pregnancy-Associated Deaths by Timing of Death in Relation to Pregnancy
Pregnancy Associated but NOT Related Deaths Determined to be Preventable
Pregnancy Associated but NOT Related Deaths by Age at Death
*/

/*
var row = {
'id': doc._id,
'hr_date_of_death_year': doc.home_record.date_of_death.year,
'dc_date_of_death':doc.death_certificate.certificate_identification.date_of_death,
'date_of_review': doc.committee_review.date_of_review,
'was_this_death_preventable': doc.committee_review.was_this_death_preventable,
'pregnancy_relatedness': doc.committee_review.pregnancy_relatedness,
'bc_is_of_hispanic_origin': doc.birth_fetal_death_certificate_parent.demographic_of_mother.is_of_hispanic_origin,
'dc_is_of_hispanic_origin': doc.death_certificate.demographics.is_of_hispanic_origin,
'age':doc.death_certificate.demographics.age,
'pmss': doc.committee_review.pmss_mm,
'did_obesity_contribute_to_the_death':doc.committee_review.did_obesity_contribute_to_the_death,
'did_mental_health_conditions_contribute_to_the_death':doc.committee_review.did_mental_health_conditions_contribute_to_the_death,
'did_substance_use_disorder_contribute_to_the_death':doc.committee_review.did_substance_use_disorder_contribute_to_the_death,
'was_this_death_a_sucide':doc.committee_review.was_this_death_a_sucide,
'was_this_death_a_homicide':doc.committee_review.homicide_relatedness.was_this_death_a_homicide 
};
*/