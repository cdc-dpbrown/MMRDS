using System;
namespace mmria.common.metadata
{
	public class node
	{
		public string prompt { get; set; }
		public string name { get; set; }
		public string type { get; set; }
		public string cardinality { get; set; } 
		public value_node[] values  { get; set; } 
		public node[] children { get; set; } 

		public bool? is_core_summary { get; set; } 
		public bool? is_required { get; set; } 
		public bool? is_read_only { get; set; } 

		public bool? is_multiselect{ get; set; } 
		public bool? is_save_value_display_description { get; set; } 
		public int? list_display_size { get; set; }
		public string control_style { get; set; } 

		public string default_value { get; set; } 
		public string pre_fill { get; set; } 

		public string regex_pattern { get; set; } 


		public string description { get; set; }
		public string validation_description { get; set; }

		public System.Dynamic.ExpandoObject validation { get; set; } 
		public System.Dynamic.ExpandoObject  onfocus { get; set; } 
		public System.Dynamic.ExpandoObject  onchange { get; set; } 
		public System.Dynamic.ExpandoObject  onblur { get; set; } 
		public System.Dynamic.ExpandoObject  onclick { get; set; } 

		public string max_value { get; set; } 
		public string min_value { get; set; } 




		public node()
		{

		} 
			//

	}
}
