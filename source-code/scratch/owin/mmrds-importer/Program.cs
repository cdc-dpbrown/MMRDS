﻿using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Linq;

//using System.Threading.Tasks;
//using System.Data;
//using System.Linq;

using mmria.console.data;

namespace mmria.console
{
	interface icommand
	{
		string summary_help();
		string detailed_help(string method);
	}

	class MainClass
	{

		//import user_name:user1 password:password database_file_path:mapping-file-set/Maternal_Mortality.mdb url:http://localhost:12345
		//export user_name:user1 password:password url:http://localhost:12345
		//export-core user_name:user1 password:password url:http://localhost:12345


		public static void Main(string[] args)
		{

			if (args.Length > 0)
			{
				switch (args[0])
				{
					case "import":
						var importer = new mmria.console.import.mmrds_importer();
						importer.Execute(args);
						break;

					case "export":
						var exporter = new mmria.console.export.mmrds_exporter();
						exporter.Execute(args);
						break;
					case "export-core":
						var core_exporter = new mmria.console.export.core_element_exporter();
						core_exporter.Execute(args);
						break;


					default:
						return;
				}
			}
			else
			{
				System.Console.WriteLine("use:");
				System.Console.WriteLine("\timport");
				System.Console.WriteLine("\tupdate");
				System.Console.WriteLine("\tsetup");
				System.Console.WriteLine("\tconfigure");
				return;
			}
		}

		public static void Main2 (string[] args)
		{

			var data = new cData ("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=mapping-file-set/Maternal_Mortality.mdb;User ID=;Password=;");


			var rs = data.GetDataTable ("Select * from AutopsyReport");

			foreach (System.Data.DataRow row in rs.Rows) 
			{
				//if (row[5].ToString().ToLower() == "grid")
				//{
					Console.WriteLine(row[2].ToString());
				//}
			}

			var filename = @"mapping-file-set/MMRDS-Mapping-NO-GRIDS-test.csv";
			var connString = string.Format(
				@"Provider=Microsoft.Jet.OleDb.4.0; Data Source={0};Extended Properties=""Text;HDR=YES;FMT=Delimited""",
				System.IO.Path.GetDirectoryName(filename)
			);

			var data2 = new cData(connString);
			var rs2 = data2.GetDataTable("SELECT * FROM [" + System.IO.Path.GetFileName(filename) + "]");

			//Path,BaseTable,DataTablePath,f.Name,prompttext,ft.Name,DataType,MMRIA Path,MMRIA Group Name,Comments,


			foreach (System.Data.DataRow row in rs2.Rows)
			{
				if (row[5].ToString().ToLower() == "grid")
				{

					var grid_table = data.GetDataTable(string.Format("Select * from [{0}] Where 1=0", row[0].ToString().Replace(".", "")));
					Console.WriteLine(string.Format("{0}, {1}, \"\"", row[0].ToString().Replace(".",""), row["prompttext"].ToString().Replace(",","")));
					foreach (System.Data.DataColumn c in grid_table.Columns)
					{

						if(c.ColumnName != "UniqueKey" &&
							c.ColumnName != "UniqueRowId" &&
							c.ColumnName != "GlobalRecordId" &&
							c.ColumnName != "RECSTATUS" &&
							c.ColumnName != "FKEY"
						  )
						{
							Console.WriteLine(string.Format("\"\", \"\", {0}, {1}, \"\"", c.ColumnName, c.DataType));
						}
					}
				}
			}
			/*
			using (var conn = new System.Data.OleDb.OleDbConnection(connString))
			{
				conn.Open();
				var query = "SELECT * FROM [" + System.IO.Path.GetFileName(filename) + "]";
				using (var adapter = new System.Data.OleDb.OleDbDataAdapter(query, conn))
				{
					var ds = new DataSet("CSV File");
					adapter.Fill(ds);
				}
			}*/


			Console.WriteLine ("Hello World!");
		}
	}
}
