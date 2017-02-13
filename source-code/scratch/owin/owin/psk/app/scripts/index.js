'use strict';


//http://www.w3schools.com/css/css3_flexbox.asp

var g_metadata = null;
var g_data = null;
var g_metadata_path = [];
var g_validator_map = [];
var g_event_map = [];
var g_validation_description_map = [];
var g_selected_index = null;
var g_selected_delete_index = null;
var g_couchdb_url = null;
var g_localDB = null;
var g_remoteDB = null;
var g_metadata_summary = [];


var default_object = null;

function g_set_data_object_from_path(p_object_path, p_metadata_path, value)
{
  var current_value = eval(p_object_path);
  if(current_value != value)
  {
    if(g_validator_map[p_metadata_path])
    {
      if(g_validator_map[p_metadata_path](value))
      {
        var metadata = eval(p_metadata_path);

        if(metadata.type.toLowerCase() == "boolean")
        {
          eval(p_object_path + ' = ' + value);
        }
        else
        {
          eval(p_object_path + ' = "' + value.replace(/"/g, '\"').replace(/\n/g,"\\n") + '"');
        }
        g_data.date_last_updated = new Date();
        g_data.last_updated_by = profile.user_name;
		
		document.getElementById(p_object_path).innerHTML = page_render(metadata, eval(p_object_path), g_ui, p_metadata_path, p_object_path, false, 0, 0, 0).join("");
        if(g_ui.broken_rules[p_object_path])
        {
          g_ui.broken_rules[p_object_path] = false;
        } 

	  var db = new PouchDB("mmrds");
      db.put(g_data).then(function (doc)
      {
          if(g_data && g_data._id == doc.id)
          {
            g_data._rev = doc._rev;
          }

			for(var i = 0; i < g_ui.data_list.length; i++)
          {
            if(g_ui.data_list[i]._id == doc.id)
            {
                g_ui.data_list[i]._rev = doc.rev;
               console.log('set_value save finished');
                console.log(doc);
                break;
            }
          }
      });
		

      }
      else
      {
        g_ui.broken_rules[p_object_path] = true;
        //console.log("didn't pass validation");

      }
    }
    else
    {
      var metadata = eval(p_metadata_path);
      if(metadata.type.toLowerCase() == "list" && metadata['is_multiselect'] && metadata.is_multiselect == true)
      {
        var item = eval(p_object_path);
        if(item.indexOf(value) > -1)
        {
          item.splice(item.indexOf(value), 1);
        }
        else
        {
          item.push(value);
        }
    }
      else if(metadata.type.toLowerCase() == "boolean")
      {
        eval(p_object_path + ' = ' + value);
      }
      else
      {
        eval(p_object_path + ' = "' + value.replace(/"/g, '\"').replace(/\n/g,"\\n") + '"');
      }
      g_data.date_last_updated = new Date();
      g_data.last_updated_by = profile.user_name;

      var new_html = page_render(metadata, eval(p_object_path), g_ui, p_metadata_path, p_object_path, false, 0, 0, 0).join("");
      $("#" + p_object_path.replace(/\./g,"_").replace(/\[/, "\\[").replace(/\]/, "\\]")).replaceWith(new_html);

      switch(metadata.type.toLowerCase())
      {
        case 'time':

          $("#" + p_object_path.replace(/\./g,"_").replace(/\[/, "\\[").replace(/\]/, "\\]") + " .time" ).datetimepicker({ format: 'LT'});
          break;
          case 'date':
          flatpickr("#" + p_object_path.replace(/\./g,"_").replace(/\[/, "\\[").replace(/\]/, "\\]") + " .date", {
            utc: true,
            //defaultDate: "2016-12-27T00:00:00.000Z",
            enableTime: false,
          });

          break;

          case 'datetime':
            $("#" + p_object_path.replace(/\./g,"_").replace(/\[/, "\\[").replace(/\]/, "\\]") + " .datetime" ).datetimepicker();

          break;

          case 'number':
              $("#" + p_object_path.replace(/\./g,"_").replace(/\[/, "\\[").replace(/\]/, "\\]") + " input.number").TouchSpin({
                              verticalbuttons: true,
                              min: 0,
                              max: 10000,
                              step: 1,
                              maxboostedstep: 10
                          });

              $("#" + p_object_path.replace(/\./g,"_").replace(/\[/, "\\[").replace(/\]/, "\\]") + " input.number").attr("size", "15");
          break;
      }
      
	  
	  var db = new PouchDB("mmrds");
      db.put(g_data).then(function (doc)
      {
          if(g_data && g_data._id == doc.id)
          {
            g_data._rev = doc._rev;
          }

			for(var i = 0; i < g_ui.data_list.length; i++)
          {
            if(g_ui.data_list[i]._id == doc.id)
            {
                g_ui.data_list[i]._rev = doc.rev;
               console.log('set_value save finished');
                //console.log(doc);
                break;
            }
          }
      });
	  
    }

    apply_validation();
  }
}

function g_add_grid_item(p_object_path, p_metadata_path)
{
  var metadata = eval(p_metadata_path);
  var new_line_item = create_default_object(metadata, {});
  eval(p_object_path).push(new_line_item[metadata.name][0]);

  document.getElementById(p_metadata_path).innerHTML = page_render(metadata, eval(p_object_path), g_ui, p_metadata_path, p_object_path, false, 0, 0, 0).join("");
  apply_tool_tips();
}

function g_delete_grid_item(p_object_path, p_metadata_path)
{
  var metadata = eval(p_metadata_path);
  var index = p_object_path.match(new RegExp("\\[\\d+\\]$"))[0].replace("[","").replace("]","");
  var object_string = p_object_path.replace(new RegExp("(\\[\\d+\\]$)"), "");
  eval(object_string).splice(index, 1);

  document.getElementById(p_metadata_path).innerHTML = page_render(metadata, eval(object_string), g_ui, p_metadata_path, object_string, false, 0, 0, 0).join("");
}

var g_ui = {
  url_state: {
    selected_form_name: null,
    "selected_id": null,
    "selected_child_id": null,
    "path_array" : []

  },
  data_list : [],
  broken_rules : [],
  set_value: function(p_path, p_value)
  {
    console.log("g_ui.set_value: ", p_path, p_value);
    console.log("value: ", p_value.value);
    console.log("get_eval_string(p_path): ", g_ui.get_eval_string(p_path));

    eval(g_ui.get_eval_string(p_path + ' = "' + p_value.value.replace('"', '\\"')  + '"'));

    //var target = eval(g_ui.get_eval_string(p_path));
  },
  get_eval_string: function (p_path)
  {
  	var result = "g_data" + p_path.replace(new RegExp('/','gm'),".").replace(new RegExp('\\.(\\d+)\\.','g'),"[$1]\\.").replace(new RegExp('\\.(\\d+)$','g'),"[$1]");
    //return an  array with 2 parts.
      // g_data['attribute'].attribute...

  	return result;

  },
  add_new_case: function()
	{

    var result = create_default_object(g_metadata, {});

    result.date_created = new Date();
    result.created_by = profile.user_name;
    result.date_last_updated = new Date();
    result.last_updated_by = profile.user_name;

    result.home_record.last_name = "new-last-name";
    result.home_record.first_name = "new-first-name";
		var new_data = [];

		for(var i in g_ui.data_list)
		{
			new_data.push(g_ui.data_list[i]);
		}

		var new_record_id = new Date().toISOString();
		new_data.push
		(
      result
		);

		g_ui.data_list = new_data;

    g_data = result;

		g_ui.selected_record_id = result._id;
		g_ui.selected_record_index = g_ui.data_list.length -1;

    var db = new PouchDB("mmrds");

      db.put(g_data).then(function (doc)
      {
          if(g_data && g_data._id == doc.id)
          {
            g_data._rev = doc._rev;
          }

					for(var i = 0; i < g_ui.data_list.length; i++)
          {
            if(g_ui.data_list[i]._id == doc.id)
            {
                g_ui.data_list[i]._rev = doc.rev;
               console.log('save finished');
                console.log(doc);
                break;
            }
          }

          var url = location.protocol + '//' + location.host + '#/' + g_ui.selected_record_index + '/home_record';
          window.location = url;
      });

    return result;
	}
};
var $$ = {


 is_id: function(value){
   // 2016-06-12T13:49:24.759Z
    if(value)
    {
      var test = value.match(/^\d+-\d+-\d+T\d+:\d+:\d+.\d+Z$/);
      return (test)? true : false;
    }
    else
    {
        return false;
    }
  }
};

$(function ()
{
  load_values();
});

function load_values()
{
	$.ajax({
			url: location.protocol + '//' + location.host + '/api/values',
	}).done(function(response) {
			g_couchdb_url = response.couchdb_url;
      load_profile();

	});

}


function load_profile()
{
    profile.on_login_call_back = function ()
    {
      get_metadata();
    };

    profile.on_logout_call_back = function (p_user_name, p_password)
    {
      replicate_db_and_log_out(p_user_name, p_password);
    };


  	profile.initialize_profile();
}



function replicate_db_and_log_out(p_user_name, p_password)
{
    var db = new PouchDB('mmrds');
    var prefix = 'http://' + p_user_name + ":" + p_password + '@';
    var remoteDB = new PouchDB(prefix + g_couchdb_url.replace('http://','') + '/mmrds');

    db.replicate.to(remoteDB).on('complete', function () 
    {
        //Creating the database object
        var db = new PouchDB('mmrds');

        //deleting database
        db.destroy(function (err, response) {
          if (err) 
          {
              console.log(err);
          } 
          else 
          {
            console.log("database destroyed");
          }
          
          window.onhashchange = null;
          
          window.location.href = location.protocol + '//' + location.host;
          document.getElementById('navbar').innerHTML = navigation_render(g_metadata, 0, g_ui).join("");
          document.getElementById('form_content_id').innerHTML ="";
      });



  }).on('error', function (err) {
    console.log("db sync error", err);
  });

}


function get_case_set()
{
    var db = new PouchDB('mmrds');
    var prefix = 'http://' + profile.user_name + ":" + profile.password + '@';
    var remoteDB = new PouchDB(prefix + g_couchdb_url.replace('http://','') + '/mmrds');


    db.sync(remoteDB).on('complete', function () 
    {
        db.allDocs(
      {
        include_docs: true,
        attachments: true
      }).then(function (result) 
      {

        //console.log(result);
        g_ui.data_list = [];
        for(var i = 0; i < result.rows.length; i++)
        {
          if(result.rows[i].doc._id.indexOf("_design") < 0)
          {
            g_ui.data_list.push(result.rows[i].doc);
          }
          
        }

        document.getElementById('navbar').innerHTML = navigation_render(g_metadata, 0, g_ui).join("");
        document.getElementById('form_content_id').innerHTML = page_render(g_metadata, default_object, g_ui, "g_metadata", "default_object", false, 0, 0, 0).join("");

        var section_list = document.getElementsByTagName("section");
        for(var i = 0; i < section_list.length; i++)
        {
          var section = section_list[i];
          if(section.id == "app_summary")
          {
              section.style.display = "block";
          }
          else
          {
              section.style.display = "none";
          }
        }



      });



}).on('error', function (err) {
  console.log("db sync error", err);
});


}


function get_metadata()
{
  	$.ajax({
			url: location.protocol + '//' + location.host + '/api/metadata',
	}).done(function(response) {
			g_metadata = response;
      metadata_summary(g_metadata_summary, g_metadata, "g_metadata", 0, 0);
      default_object =  create_default_object(g_metadata, {});

      document.getElementById('form_content_id').innerHTML ="<br/><br/><br/><br/><h2>Fetching record from mmria database</h2>";
      //create_validator_map(g_validator_map, g_validation_description_map, g_metadata, "g_metadata");

      //window.location.href = location.protocol + '//' + location.host;

      get_case_set();

     g_ui.url_state = url_monitor.get_url_state(window.location.href);
      if(window.onhashchange)
      {
        window.onhashchange ({ isTrusted: true, newURL : window.location.href });
      }
      else
      {
        window.onhashchange = window_on_hash_change;
        window.onhashchange ({ isTrusted: true, newURL : window.location.href });
      }
	});
}

function window_on_hash_change(e)
{
  /*
  e = HashChangeEvent
  {
    isTrusted: true,
    oldURL: "http://localhost:12345/react-test/#/",
    newURL: "http://localhost:12345/rea
  }*/

  if(g_data)
  {

      var db = new PouchDB('mmrds');
      db.put(g_data).then(function (doc)
      {
          if(g_data && g_data._id == doc.id)
          {
            g_data._rev = doc._rev;
          }

					for(var i = 0; i < g_ui.data_list.length; i++)
          {
            if(g_ui.data_list[i]._id == doc.id)
            {
                g_ui.data_list[i]._rev = doc.rev;
               console.log('save finished');
                console.log(doc);
                break;
            }
          }
      });

        if(e.isTrusted)
        {

          var new_url = e.newURL || window.location.href;
          g_ui.url_state = url_monitor.get_url_state(new_url);

          if(g_ui.url_state.path_array && g_ui.url_state.path_array.length > 0 && (parseInt(g_ui.url_state.path_array[0]) >= 0))
          {
            /*
            if(g_data._id != g_ui.data_list[parseInt(g_ui.url_state.path_array[0])]._id)
            {
                save_queue.push(g_data._id);
            }*/

            g_data = g_ui.data_list[parseInt(g_ui.url_state.path_array[0])];

            document.getElementById('navbar').innerHTML = navigation_render(g_metadata, 0, g_ui).join("");
            document.getElementById('form_content_id').innerHTML = page_render(g_metadata, g_data, g_ui, "g_metadata", "g_data", false, 0, 0, 0).join("");
            apply_tool_tips();


            var section_list = document.getElementsByTagName("section");


            if(g_ui.url_state.path_array.length > 2 && (parseInt(g_ui.url_state.path_array[0]) >= 0))
            {
              for(var i = 0; i < section_list.length; i++)
              {
                var section = section_list[i];
                if(section.id == g_ui.url_state.path_array[1])
                {
                    section.style.display = "block";
                }
                else
                {
                    section.style.display = "none";
                }
              }
            }
            else
            {
              for(var i = 0; i < section_list.length; i++)
              {
                var section = section_list[i];
                if(section.id == g_ui.url_state.path_array[1] + "_id")
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
          else
          {
            /*
            if(g_data && !(save_queue.indexOf(g_data._id) > -1))
            {
              save_queue.push(g_data._id);
            }*/
            g_data = null;
            document.getElementById('navbar').innerHTML = navigation_render(g_metadata, 0, g_ui).join("");
            document.getElementById('form_content_id').innerHTML = page_render(g_metadata, default_object, g_ui, "g_metadata", "default_object", false, 0, 0, 0).join("");
            apply_tool_tips();

            var section_list = document.getElementsByTagName("section");
              for(var i = 0; i < section_list.length; i++)
              {
                var section = section_list[i];
                if(section.id == "app_summary")
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
      
    
   // g_data_access.set_data(g_data);
			
  }
  else if(e.isTrusted)
  {

    var new_url = e.newURL || window.location.href;

    g_ui.url_state = url_monitor.get_url_state(new_url);


    if(g_ui.url_state.path_array && g_ui.url_state.path_array.length > 0 && (parseInt(g_ui.url_state.path_array[0]) >= 0))
    {
      g_data = g_ui.data_list[parseInt(g_ui.url_state.path_array[0])];

      document.getElementById('navbar').innerHTML = navigation_render(g_metadata, 0, g_ui).join("");
      document.getElementById('form_content_id').innerHTML = page_render(g_metadata, g_data, g_ui, "g_metadata", "g_data", false, 0, 0, 0).join("");
      apply_tool_tips();


  		var section_list = document.getElementsByTagName("section");
      if(g_ui.url_state.path_array.length > 2 && (parseInt(g_ui.url_state.path_array[0]) >= 0))
      {
        for(var i = 0; i < section_list.length; i++)
        {
          var section = section_list[i];
          if(section.id == g_ui.url_state.path_array[1])
          {
              section.style.display = "block";
          }
          else
          {
              section.style.display = "none";
          }
        }
      }
      else
      {
        for(var i = 0; i < section_list.length; i++)
        {
          var section = section_list[i];
          if(section.id == g_ui.url_state.path_array[1] + "_id")
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
    else
    {
/*
      if(g_data && !(save_queue.indexOf(g_data._id) > -1))
      {
        save_queue.push(g_data._id);
      }*/
              
      g_data = null;

      document.getElementById('navbar').innerHTML = navigation_render(g_metadata, 0, g_ui).join("");
                                                            //page_render(p_metadata, p_data, p_ui, p_metadata_path, p_object_path, p_is_grid_context, p_row, p_column)
      document.getElementById('form_content_id').innerHTML = page_render(g_metadata, default_object, g_ui, "g_metadata", "default_object", false, 0, 0, 0).join("");
      apply_tool_tips();

      var section_list = document.getElementsByTagName("section");
        for(var i = 0; i < section_list.length; i++)
        {
          var section = section_list[i];
          if(section.id == "app_summary")
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
  else
  {
    // do nothing for now
  }
};


function show_print_version()
{
  window.open("./print-version", "_print_version");
}

function show_data_dictionary()
{
  window.open("./data-dictionary", "_data_dictionary");
}

function show_user_administration()
{
  window.open("./_users", "_users");
}


function apply_tool_tips()
{
  $('[rel=tooltip]').tooltip();
  $( ".time" ).datetimepicker({ format: 'LT'});
//$( "[metadata_type='date']" ).datetimepicker();

flatpickr(" .date", {
	utc: true,
	//defaultDate: "2016-12-27T00:00:00.000Z",
	enableTime: false,
});

$( ".datetime" ).datetimepicker();

$("input.number").TouchSpin({
                verticalbuttons: true,
                min: 0,
                max: 10000,
                step: 1,
                maxboostedstep: 10
            });

//$("input.number").mask("#,##0[.00", {reverse: true});
$("input.number").attr("size", "15");
    apply_validation();


}




function apply_validation()
{
    for(var i in g_ui.broken_rules)
    {
      var element = document.getElementById(i);
      if(g_ui.broken_rules[i] == true)
      {
        
        if
        (
          element &&
          element.className.indexOf('failed-validation') < 0
        )
        {
         element.className += ' failed-validation';
         /*
         var validation_text = element.getAttribute('validation-tooltip');
         if(validation_text)
         {
          var span_node = document.createElement("span");
          span_node.setAttribute('class', 'tooltip-content');
          span_node.innerText = element.getAttribute('validation-tooltip');
          element.appendChild(span_node);
         }
         */

        }
      }
      else
      {
        
        if
        (
          element &&
          element.className.indexOf('failed-validation') > 0
        )
        {
          var class_array = element.className.split(' ');
          class_array.splice(class_array.indexOf('failed-validation'),1);
          element.className = class_array.join(' ');
        }
        
      }
    }
}

function delete_record(p_index)
{
  if(p_index == g_selected_delete_index)
  {
    var data = g_ui.data_list[p_index];
    data._deleted = true;

    var db = new PouchDB("mmrds");

      db.put(data).then(function (doc)
      {
					for(var i = 0; i < g_ui.data_list.length; i++)
          {
            if(g_ui.data_list[i]._id == data._id)
            {
                g_ui.data_list.splice(i,1);
                break;
            }
          }

        document.getElementById('navbar').innerHTML = navigation_render(g_metadata, 0, g_ui).join("");
        document.getElementById('form_content_id').innerHTML = page_render(g_metadata, default_object, g_ui, "g_metadata", "default_object", false, 0, 0, 0).join("");

        var section_list = document.getElementsByTagName("section");
        for(var i = 0; i < section_list.length; i++)
        {
          var section = section_list[i];
          if(section.id == "app_summary")
          {
              section.style.display = "block";
          }
          else
          {
              section.style.display = "none";
          }
        }
      });


      g_selected_delete_index = null;


  }
  else
  {
      if(g_selected_delete_index != null && g_selected_delete_index > -1)
      {
          var old_id = g_ui.data_list[g_selected_delete_index]._id;
          $("div[path='" + old_id + "']").css("background", "");
      }

      g_selected_delete_index = p_index;
      var id = g_ui.data_list[p_index]._id;
      $("div[path='" + id + "']").css("background", "#BBBBBB");
      
  }
}


var save_interval_id = null;
var save_queue = [];

function save_change_task()
{

  if(profile.is_logged_in && profile.user_name && profile.password)
  {
    var db = new PouchDB('mmrds');
    var prefix = 'http://' + profile.user_name + ":" + profile.password + '@';
    var remoteDB = new PouchDB(prefix + g_couchdb_url.replace('http://','') + '/mmrds');

    db.replicate.to(remoteDB).on('complete', function (err, response) {
      
            console.log("replicate to server: complete");
  
        
    }).on('error', function (err) {
      console.log("replicate to server error:", err);
    });


  }

}

	window.setInterval(save_change_task, 30000);	

function open_print_version(p_section)
{

	var print_window = window.open('./print-version','_print_version',null,false);

	window.setTimeout(function()
	{
		print_window.create_print_version(g_metadata, g_data, p_section)
	}, 1000);	
}


function open_core_summary(p_section)
{

	var print_window = window.open('./core-elements','_core_summary',null,false);

	window.setTimeout(function()
	{
		print_window.create_print_version(g_metadata, g_data, p_section)
	}, 1000);	
}


function open_blank_version(p_section)
{

	var blank_window = window.open('./print-version','_blank_version',null,false);

	window.setTimeout(function()
	{
		blank_window.create_print_version(g_metadata, default_object, p_section)
	}, 1000);	
}


function open_aggregate_report_version(p_section)
{

	var report_window = window.open('./aggregate-report','_aggregate_report',null,false);

	window.setTimeout(function()
	{
		report_window.load_data(profile.user_name, profile.password)
	}, 1000);	
}

function add_new_form_click(p_metadata_path, p_object_path)
{
  console.log("add_new_form_click: " + p_metadata_path + " , " + p_object_path);
  var metadata = eval(p_metadata_path);
  var form_array = eval(p_object_path);

  var new_form = create_default_object(metadata, {});
  var item = new_form[metadata.name][0];
  form_array.push(item);

  document.getElementById(metadata.name + "_id").innerHTML = page_render(metadata, form_array, g_ui, p_metadata_path, p_object_path, false, 0, 0, 0).join("");

}