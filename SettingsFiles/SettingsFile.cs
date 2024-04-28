using Piles.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Piles.SettingsFiles {
	public class SettingsFile {
		private Dictionary<string, Prop> _table = new Dictionary<string, Prop>();
		private List<Section> _sections = new List<Section>();

		private string _directory = ".", _filename = "default.sett";

		public string SaveDirectory {
			get	{ return _directory; }
			set { _directory = value; }
		}

		public string FileName {
			get { return _filename; }
			set { _filename = value; }
		}

		public string FullPath {
			get { return string.Format("{0}/{1}", SaveDirectory, FileName); }
		}

		public string Search (string handle, string defaultValue) {
			if (!_table.ContainsKey(handle)) {
				Set(handle, defaultValue);
			}

			return _table[handle].AsString();
		}

		public double Search (string handle, double defaultValue) {
			if (!_table.ContainsKey(handle)) {
				Set(handle, defaultValue);
			}

			return _table[handle].AsDouble();
		}

		public bool Search(string handle, bool defaultValue) {
			if (!_table.ContainsKey(handle)) {
				Set(handle, defaultValue);
			}

			return _table[handle].AsBool();
		}

		public void Set(string handle, string val) {
			handle = handle.Replace("=", "");

			if (_table.ContainsKey(handle)) {
				_table[handle].Value = val;
				return;
			}

			Prop prop = new Prop(handle, val);
			_table.Add(handle, prop);
			_sections.Add(new PropSection(prop));
		}

		public void Set(string handle, double val) {
			Set(handle, val.ToString());
		}

		public void Set (string handle, bool val) {
			Set(handle, val.ToString());
		}

		public void AddComment (string message) {
			_sections.Add(new CommentSection(message));
		}

		public void AddComment (string property, string message) {
			int count = _sections.Count;

			for (int i = 0; i < count; i++) {
				if (_sections[i] is PropSection) {
					PropSection prop = (PropSection) _sections[i];
					if (prop.Property.Handle.Equals(property)) {
						_sections.Insert(i, new CommentSection(message));
						if (i > 0) { _sections.Insert(i, new WhiteSpaceSection(1)); }
						return;
					}
				}
			}

			// prop wasnt found
			AddComment(message);
		}

		public void Remove (string handle) {
			if (!_table.ContainsKey(handle)) { return; }

			Prop prop = _table[handle];

			int count = _sections.Count;
			for (int i = 0; i < count; i++) {
				Section sec	= _sections[i];

				if (sec is PropSection) {
					PropSection temp = (PropSection) sec;
					if (temp.Property == prop) {
						_sections.Remove(temp);
						break; // there cannot be more than one section containing this prop
					}
				}
			}
		}

		public bool Save (out string error) {
			if (!Directory.Exists(SaveDirectory)) {
				error = "SaveDirectory \"" + SaveDirectory + "\" not found to be a directory";
				return false;
			}

			if (FileName.Equals(string.Empty)) {
				error = "FileName not specfied, has an empty value";
				return false;
			}

			List<string> lines = new List<string>();

			foreach (Section sec in _sections) {
				lines.Add(sec.GetLine());
			}

			File.WriteAllLines(FullPath, lines.ToArray());

			error = string.Empty;
			return true;
		}

		public bool Save () {
			string err;
			return Save(out err);
		}

		// ----------------------------------------------
		//  Static
		// ----------------------------------------------

		public static SettingsFile Load(string filepath) {
			SettingsFile file = new SettingsFile();
		
			if (!File.Exists(filepath)) { return file; }

			file.SaveDirectory = IO.GetFilePath(filepath);
			file.FileName = IO.GetFileName(filepath, true);

			// parse the file
			string[] lines = File.ReadAllLines(filepath);

			for (int i = 0; i < lines.Length; i++) {
				lines[i] = lines[i].Trim();

				if (lines[i].Length <= 1) { // no possible way for this to contain anything useful
					file._sections.Add(new WhiteSpaceSection(1));
					continue;
				}

				if (lines[i][0] == '#') {
					// this is a comment
					file._sections.Add(new CommentSection(lines[i].Substring(1)));
				} else {
					// this line has data
					string[] parts = lines[i].Split("=", 2);

					if (parts.Length <= 1) { continue; } // not a valid key value pair

					for (int ii = 0; ii < parts.Length; ii++) {
						parts[ii] = parts[ii].Trim();	
					}

					// double definition
					if (file._table.ContainsKey(parts[0])) {
						file.Remove(parts[0]);
					}

					Prop prop = new Prop(parts[0], parts[1]);

					file._table.Add(parts[0], prop);
					file._sections.Add(new PropSection(prop));
				}
			}

			return file;
		}
	}
}
