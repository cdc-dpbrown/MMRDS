﻿using System;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using System.Text;


namespace install.setup
{
	class MainClass
	{
		static System.Collections.Generic.Dictionary<string, string> name_hash_list;

		public static void Main(string[] args)
		{
			//C: \Users\jhaines\Downloads\samplefirst > "C:\Program Files\WiX Toolset v3.10\bin\candle" - o "C:\Users\jhaines\Downloads\samplefirst\output" output.xml

			//"C:\Program Files\WiX Toolset v3.10\bin\light" output.wixobj
			string wix_directory_path = System.Configuration.ConfigurationManager.AppSettings["wix_directory_path"];
			string input_directory_path = System.Configuration.ConfigurationManager.AppSettings["input_directory_path"];
			string output_directory_path = System.Configuration.ConfigurationManager.AppSettings["output_directory_path"];

			System.IO.Directory.Delete(output_directory_path, true);
			CopyFolder.CopyDirectory(input_directory_path, output_directory_path);

			//Console.WriteLine("Hello World!");
			string name_hash_file_name = "id.csv";
			string wix_file_name = "output.xml";


			name_hash_list = new System.Collections.Generic.Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);


			if (System.IO.File.Exists(name_hash_file_name))
			{

				using (System.IO.TextReader reader = System.IO.File.OpenText(name_hash_file_name))
				{
					string file = reader.ReadToEnd();
					string[] line_list = file.Split('\n');
					for (int i = 0; i < line_list.Length; i++)
					{
						string[] name_hash = line_list[i].Split(',');
						if (name_hash.Length > 1)
						{
							name_hash_list.Add(name_hash[0].Trim(), name_hash[1].Trim());
						}

					}
				}
			}

			string xml = get_xml_template();


			var xmlReader = XmlReader.Create(new StringReader(xml));
			XDocument wix_doc  = XDocument.Load(xmlReader);
			var namespaceManager = new XmlNamespaceManager(xmlReader.NameTable);
			namespaceManager.AddNamespace("prefix", "http://schemas.microsoft.com/wix/2006/wi");


			XElement ProductElement = wix_doc.XPathSelectElement("prefix:Wix/prefix:Product", namespaceManager);

			ProductElement.Attribute("Id").SetValue(get_id("PRODUCT_ID"));
			ProductElement.Attribute("UpgradeCode").SetValue(get_id("PRODUCT_UPGRADECODE"));

			Console.WriteLine("Product");
			Console.WriteLine(ProductElement.Attribute("Name"));
			Console.WriteLine(ProductElement.Attribute("Id"));
			Console.WriteLine(ProductElement.Attribute("UpgradeCode"));
			Console.WriteLine(ProductElement.Attribute("Manufacturer"));
			Console.WriteLine(ProductElement.Attribute("Version"));



			XElement PackageElement = ProductElement.XPathSelectElement("./prefix:Package", namespaceManager);
			Console.WriteLine("Package");
			Console.WriteLine(PackageElement.Attribute("Description"));
			Console.WriteLine(PackageElement.Attribute("Comments"));
			Console.WriteLine(PackageElement.Attribute("Manufacturer"));
			//Console.WriteLine(PackageElement.Attribute("Manufacturer"));
			//Console.WriteLine(PackageElement.Attribute("Version"));

			XElement MediaElement = ProductElement.XPathSelectElement("./prefix:Media", namespaceManager);
			Console.WriteLine("Media");
			Console.WriteLine(MediaElement.Attribute("Cabinet"));


			XElement PropertyElement = ProductElement.XPathSelectElement("./prefix:Property", namespaceManager);
			Console.WriteLine("Property");
			Console.WriteLine(PropertyElement.Attribute("Id"));
			Console.WriteLine(PropertyElement.Attribute("Value"));

			XElement IconElement = ProductElement.XPathSelectElement("./prefix:Icon", namespaceManager);
			Console.WriteLine("Icon");
			print_xattribute(IconElement.Attribute("Id"), IconElement.Attribute("SourceFile"));


			XElement DirectoryElement = ProductElement.XPathSelectElement(".//prefix:Directory[@Id='INSTALLDIR']", namespaceManager);
			Console.WriteLine("Directory");
			print_xattribute(DirectoryElement.Attribute("Id"), DirectoryElement.Attribute("Name"));



			XElement FeatureElement = ProductElement.XPathSelectElement("./prefix:Feature", namespaceManager);
			Console.WriteLine("Feature");
			print_xattribute(FeatureElement.Attribute("Id"), FeatureElement.Attribute("Level"));

			// removed components and features
			foreach (XElement ComponentElement in ProductElement.XPathSelectElements(".//prefix:Component", namespaceManager).ToList())
			{
				ComponentElement.Remove();
			}

			foreach (XElement ComponentRefElement in FeatureElement.XPathSelectElements(".//prefix:ComponentRef", namespaceManager).ToList())
			{
				ComponentRefElement.Remove();
			}

			AddComponents(DirectoryElement, FeatureElement, new System.IO.DirectoryInfo(output_directory_path));

			Console.WriteLine("Components");
			foreach (XElement ComponentElement in ProductElement.XPathSelectElements(".//prefix:Component", namespaceManager))
			{
				ComponentElement.Attribute("Guid").SetValue(get_id(ComponentElement.Attribute("Id").Value));

				print_xattribute(ComponentElement.Attribute("Id"), ComponentElement.Attribute("Guid"));
				XElement FileElement = ComponentElement.XPathSelectElement("./prefix:File", namespaceManager);
				if (FileElement != null)
				{
					print_xattribute(FileElement.Attribute("Id"), FileElement.Attribute("Name"));
				}

				/*
				XElement new_node = new_file_node();
				new_node.Add(new_short_cut_node());
				ComponentElement.Add(new_node);
				*/
			}
			Console.WriteLine("ComponentRefs");
			foreach (XElement ComponentRefElement in ProductElement.XPathSelectElements(".//prefix:ComponentRef", namespaceManager))
			{
				print_xattribute(ComponentRefElement.Attribute("Id"));
			}


			wix_doc.Save(wix_file_name);

			string text = File.ReadAllText(wix_file_name);
			text = System.Text.RegularExpressions.Regex.Replace(text, "xmlns=\"\"", "");
			File.WriteAllText(output_directory_path + "\\" + wix_file_name, text);

			System.Text.StringBuilder name_hash_file_builder = new StringBuilder();
			foreach (System.Collections.Generic.KeyValuePair<string, string> kvp in name_hash_list)
			{
				name_hash_file_builder.Append(kvp.Key);
				name_hash_file_builder.Append(",");
				name_hash_file_builder.AppendLine(kvp.Value);
			}
			System.IO.File.WriteAllText(name_hash_file_name, name_hash_file_builder.ToString());

			//var FieldsTypeIDs = from _FieldTypeID in wix_doc.Descendants("Field") select _FieldTypeID;

			//double width, height;



			//string checkcode = ViewElement.Attribute("CheckCode").Value.ToString();

			/*
			
			StringBuilder JavaScript = new StringBuilder();
			StringBuilder VariableDefinitions = new StringBuilder();

			XDocument xdocResponse = XDocument.Parse("");

			XDocMetadata.RequiredFieldsList = xdocResponse.Root.Attribute("RequiredFieldsList").Value;
			XDocMetadata.HiddenFieldsList = xdocResponse.Root.Attribute("HiddenFieldsList").Value;
			XDocMetadata.HighlightedFieldsList = xdocResponse.Root.Attribute("HighlightedFieldsList").Value;
			XDocMetadata.DisabledFieldsList = xdocResponse.Root.Attribute("DisabledFieldsList").Value;

			*/

		}

		static private string get_id(string p_key)
		{
			string result = null;

			if (name_hash_list.ContainsKey(p_key))
			{
				result = name_hash_list[p_key];
			}
			else
			{
				result = Guid.NewGuid().ToString();
				name_hash_list.Add(p_key, result);
			}

			return result;
		}



		static private void print_xattribute(XAttribute value)
		{
			Console.WriteLine(string.Format("\t{0}", value));
		}

		static private void print_xattribute(XAttribute value_1, XAttribute value_2)
		{
			Console.WriteLine(string.Format("\t{0} {1}", value_1, value_2));
		}

		static private void AddComponents(XElement DirectoryElement, XElement FeatureElement, System.IO.DirectoryInfo directoryInfo)
		{

			FileInfo[] fileInfoSet = directoryInfo.GetFiles();
			foreach (FileInfo fileInfo in fileInfoSet)
			{
				DirectoryElement.Add(get_component(fileInfo));
				FeatureElement.Add(create_component_ref(get_component_name(fileInfo.Name)));
			}

			foreach (System.IO.DirectoryInfo di in directoryInfo.GetDirectories())
			{
				string directoryName = di.FullName;

				System.Console.WriteLine(directoryName);

				XElement directory = create_directory(di);
				DirectoryElement.Add(directory);

				AddComponents(directory, FeatureElement, di);
			}


		}

		static private XElement create_directory(System.IO.DirectoryInfo p_directory_info)
		{
			string file_name = p_directory_info.Name;
			XElement result = new XElement
				(
					"Directory",
					new XAttribute("Id", file_name),
					new XAttribute("Name", file_name)
				);
			/*
			            < Directory Id = "HTML" Name = "app" >
*/

			return result;
		}

		static private XElement create_component_ref(string p_name)
		{
			//< ComponentRef Id = "ProgramMenuDir" />
			XElement result = new XElement
				(
					"ComponentRef",
					new XAttribute("Id", p_name)
				);
			return result;

		}

		static string get_component_name(string p_file_name)
		{
			string result = p_file_name.ToUpper().Replace('-', '_').Replace(".", "");

			return result;
		}

		static private XElement get_component(System.IO.FileInfo p_file_info)
		{
			string file_name = p_file_info.Name;
			XElement result = new XElement
				(
					"Component",
					new XAttribute("Id", get_component_name(file_name)),
					new XAttribute("Guid", get_id(p_file_info.FullName)),
					new_file_node(p_file_info)
				);
			/*
			            <Component Id="HelperLibrary" Guid="bc80dba1-5013-4bcb-9604-9cc9d4a30380">
              <File Id="HelperDLL" Name="Helper.dll" DiskId="1" Source="Helper.dll" KeyPath="yes" />
			
						</ Component >
*/

			return result;
		}


		static public XElement new_file_node(System.IO.FileInfo p_file_info)
		{
			/*
			  <File 
				Id = 'FoobarEXE'
				Name = 'FoobarAppl10.exe' 
				DiskId = '1' 
				Source = 'FoobarAppl10.exe' 
				KeyPath = 'yes' >	
			*/
			//XNamespace ns = "http://schemas.microsoft.com/wix/2006/wi";

			string file_name = get_component_name(p_file_info.Name);

			XElement result = new XElement
				(
				"File",
				new XAttribute("Id", file_name),
				new XAttribute("Name", file_name),
				new XAttribute("DiskId", "1"),
					new XAttribute("Source", p_file_info.FullName),
				new XAttribute("KeyPath", "yes")
				);


				return result;
		}


		static public XElement new_short_cut_node()
		{
			/*
			  <Shortcut
			  	Id="desktopFoobar10"
			  	Directory="DesktopFolder"
			  	Name="Foobar 1.0"
			  	WorkingDirectory="INSTALLDIR"
			  	Icon="Foobar10.exe"
			  	IconIndex="0"
			  	Advertise="yes"
			  	/>	
			*/

			XElement result = new XElement
				(
				"File",
				new XAttribute("Id", "desktopFoobar10"),
				new XAttribute("Directory", "DesktopFolder"),
				new XAttribute("Name", "Foobar 1.0"),
				new XAttribute("WorkingDirectory", "INSTALLDIR"),
				new XAttribute("Icon", "Foobar10.exe"),
				new XAttribute("IconIndex", "0"),
				new XAttribute("Advertise", "yes")
				);

			return result;
		}

		static public string get_xml_template()
		{
			Stream resourceStream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("install.setup.mmria.wxs");
			StreamReader reader = new StreamReader(resourceStream);
			string result = reader.ReadToEnd();




			return result;
		}


		static public string GetHash(string file_path)
		{
			String result;
			StringBuilder sb = new StringBuilder();
			System.Security.Cryptography.MD5 md5Hasher = System.Security.Cryptography.MD5.Create();

			using (System.IO.FileStream fs = System.IO.File.OpenRead(file_path))
			{
				foreach (Byte b in md5Hasher.ComputeHash(fs))
					sb.Append(b.ToString("X2").ToLowerInvariant());
			}

			result = sb.ToString();

			return result;
		}

	}
}
