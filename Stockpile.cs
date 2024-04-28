using Piles.Utility;
using Piles.Versioning;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Piles {
	public class Stockpile : Pileable {
		private string _directory = ".", _filename = "default.stock";

		public string SaveDirectory {
			get { return _directory; }
			set { _directory = value; }
		}

		public string FileName {
			get { return _filename; }
			set { _filename = value; }
		}

		public string FullPath {
			get { return string.Format("{0}/{1}", SaveDirectory, FileName); }
		}

		public Stockpile (string? dir = null, string? filename = null) {
			if (dir != null) { SaveDirectory = dir; }
			if (filename != null) { FileName = filename; }
		}

		public override string ToString() {
			List<string> vals = new List<string>();

			foreach (KeyValuePair<string, List<Pile>> pair in _collection) {
				foreach (Pile val in pair.Value) { 
					string? add = val.ToString();
					if (add == null) { continue; }
					vals.Add(add);
				}
			}

			return string.Join("", vals);
		}

		/// <summary>
		/// Saves the stockpile to the SaveDirectory with the file name FileName
		/// </summary>
		/// <param name="error"></param>
		/// <returns>true if the save was successful and false if unsuccessful, more details will be found in the error string</returns>
		public bool Save (out string error) {
			if (!Directory.Exists(SaveDirectory)) {
				error = "SaveDirectory \"" + SaveDirectory + "\" not found to be a directory";
				return false;
			}

			if (FileName.Equals(string.Empty)) {
				error = "FileName not specfied, has an empty value";
				return false;
			}

			string path = FullPath;

			if (!File.Exists(path)) {
				var create = File.Create(path);
				create.Close();
			}

			File.WriteAllText(path, ToString());
			
			error = "";
			return true;
		}

		/// <summary>
		/// Saves the stockpile to the SaveDirectory with the file name FileName
		/// </summary>
		/// <returns>true if the save was successful and false if unsuccessful</returns>
		public bool Save () {
			string err;
			return Save(out err);
		}

		// ----------------------------------------------
		//  Static
		// ----------------------------------------------

		public static Stockpile Load(string path) {
			Stockpile file = new Stockpile();
			if (!File.Exists(path)) { return file; }

			file.SaveDirectory = IO.GetFilePath(path);
			file.FileName = IO.GetFileName(path, true);

			string content = File.ReadAllText(path);
			content = content.Replace("\n", "").Replace("\t", "").Replace("\r", "");
			List<string> chunks = ChunkData(content);

			foreach (string chunk in chunks) {
				Pile? point = LoadPile(chunk);
				if (point == null) { continue; }
				file.Add(point);
			}

			return file;
		}

		private static List<string> ChunkData(string content) {
			List<string> chunks = new List<string>();
			List<char> wait = new List<char> {
				';'
			};

			string chunk = "";
			foreach (char single in content) {
				if (!wait[0].Equals('\"')) {
					if (wait.Count > 0) {
						if (single == wait[0]) {
							wait.RemoveAt(0);
						}
					}

					switch (single) {
						case '[':
							wait.Insert(0, ']');
							break;
						case '{':
							wait.Insert(0, '}');
							break;
						case '\"':
							wait.Insert(0, '\"');
							break;
					}
				} else {
					if (single == '\"') {
						wait.RemoveAt(0);
					}
				}

				if (wait.Count == 0) {
					wait.Add(';');
					chunks.Add(chunk);
					chunk = "";
				} else {
					chunk += single;
				}
			}

			return chunks;
		}

		private static Pile? LoadPile(string content) {
			string[] split = content.Split(new string[] { ": " }, 2, StringSplitOptions.None);
			if (split.Length < 2) { return null; }
			Pile val = new Pile(split[0]);

			string point = split[1];

			if (point[0] == '[') {
				string shed = split[1].Remove(split[1].Length - 1, 1).Remove(0, 1);

				List<string> parts = new List<string>();
				List<char> wait = new List<char>();

				string build = "";
				for (int i = 0; i < shed.Length; i++) {
					if (wait.Count <= 0) {
						string conjoin = (shed[i] + "" + shed[Math.Min(i + 1, shed.Length - 1)]);
						if (conjoin.Equals(", ")) {
							// cause a split
							parts.Add(build);
							build = "";
							i++;
						} else {
							if (shed[i] == '\"') {
								wait.Add('\"');
							} else if (shed[i] == '[') {
								wait.Add(']');
							}

							build += shed[i];
						}
					} else {
						if (shed[i] == wait[0]) {
							wait.RemoveAt(0);
						}

						build += shed[i];
					}
				}

				parts.Add(build);

				int count = parts.Count;
				if (parts[count - 1][0] == '{') {
					point = parts[count - 1];
					parts.RemoveAt(count - 1);
				}

				foreach (string part in parts) {
					ParseValue(val, part);
				}
			}

			if (point[0] == '{') {
				List<string> chunks = ChunkData(point.Remove(point.Length - 1, 1).Remove(0, 1));
				foreach (string chunk in chunks) {
					Pile? coin = LoadPile(chunk);
					if (coin == null) { continue; }
					val.Add(coin);
				}
			} else {
				ParseValue(val, point);
			}

			return val;
		}

		private static void ParseValue(Pile val, string toparse) {
			if (toparse.Equals("null")) { return; }

			if (toparse[0] == '\"') { // string
				val.Add(toparse.Replace("\"", ""));
			} else if (toparse.Equals("false") || toparse.Equals("true")) { // bool
				val.Add(toparse.Equals("true"));
			} else { // must be double
				double doub = 0;
				double.TryParse(toparse, out doub);
				val.Add(doub);
			}
		}

	}
}
