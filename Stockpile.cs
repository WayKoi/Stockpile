﻿using Piles.Utility;
using Piles.Versioning;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Piles {
	public class Stockpile {
		public string Path { get; set; } = string.Empty;
		public string FileName { get; set; } = string.Empty;
		public string FileExtension { get; set; } = string.Empty;
		public Ver Version { get; set; } = new Ver();
		public bool HasVersion { get; set; } = true;

		protected Dictionary<string, List<Pile>> Collection = new Dictionary<string, List<Pile>>();

		public Stockpile (string? path = null) {
			if (path == null) { path = string.Empty; }
			Path = path;
		}

		public void AddToCollection(Pile val) {
			if (val == null) { return; }

			if (!Collection.ContainsKey(val.Handle)) {
				Collection.Add(val.Handle, new List<Pile>());
			}

			Collection[val.Handle].Add(val);
		}

		public void RemoveFromCollection(Pile val) {
			if (val == null) { return; }

			if (Collection.ContainsKey(val.Handle)) {
				Collection[val.Handle].Remove(val);
				if (Collection[val.Handle].Count <= 0) {
					Collection.Remove(val.Handle);
				}
			}
		}

		public Pile TopProperty(string handle) {
			handle = StandardizeHandle(handle);
			if (!Collection.ContainsKey(handle)) { Collection.Add(handle, new List<Pile>()); }

			List<Pile> point = Collection[handle];
			if (point.Count == 0) { point.Add(new Pile(handle)); }

			return point[0];
		}

		public List<Pile> Property(string handle) {
			handle = StandardizeHandle(handle);
			if (!Collection.ContainsKey(handle)) { Collection.Add(handle, new List<Pile>()); }
			return Collection[handle];
		}

		public override string ToString() {
			List<string> vals = new List<string>();

			if (HasVersion) {
				vals.Add(string.Format("Version: \"{0}.{1}.{2}\";\n", Version.Major, Version.Minor, Version.Patch));
			}

			foreach (KeyValuePair<string, List<Pile>> pair in Collection) {
				foreach (Pile val in pair.Value) { 
					string? add = val.ToString();
					if (add == null) { continue; }
					vals.Add(add);
				}
			}

			return string.Join("", vals);
		}

		public void Save (string? path = null) {
			path = GetFullPath(path);

			if (path == string.Empty) { return; }
			if (!File.Exists(path)) {
				var create = File.Create(path);
				create.Close();
			}

			File.WriteAllText(path, ToString());
		}

		public string GetFullPath (string? path) {
			if (path == null) { path = Path; }
			return string.Format("{0}/{1}.{2}", path, FileName, FileExtension);
		}

		// ----------------------------------------------
		//  Static
		// ----------------------------------------------
		private static string BannedHandleChars = "[]{}\":;\n\t\r";
		public static string StandardizeHandle(string handle) {
			if (handle.Equals("")) { handle = "None"; return handle; }

			handle = handle.Trim();

			foreach (char single in BannedHandleChars) {
				handle = handle.Replace(single + "", "");
			}

			handle = handle.Replace(" ", "-");

			return handle;
		}

		public static Stockpile Load(string path, bool versioned = true) {
			Stockpile file = new Stockpile();
			if (!File.Exists(path)) { return file; }

			file.Path = IO.GetFilePath(path);
			file.FileName = IO.GetFileName(path);
			file.FileExtension = IO.GetFileExtension(path);

			string content = File.ReadAllText(path);
			content = content.Replace("\n", "").Replace("\t", "").Replace("\r", "");
			List<string> chunks = ChunkData(content);

			foreach (string chunk in chunks) {
				Pile? point = LoadPile(chunk);
				if (point == null) { continue; }
				file.AddToCollection(point);
			}

			file.HasVersion = versioned;
			if (versioned) {
				Pile point = file.TopProperty("Version");
				string top = point.TopStringDefault("0.0.0");
				string[] segs = top.Split('.');

				if (segs.Length == 3) {
					int major = 0, minor = 0, patch = 0;
					int.TryParse(segs[0], out major);
					int.TryParse(segs[1], out minor);
					int.TryParse(segs[2], out patch);
					file.Version.Set( (major, minor, patch));
				}

				file.RemoveFromCollection(point);
			}

			return file;
		}

		protected static List<string> ChunkData(string content) {
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

		protected static Pile? LoadPile(string content) {
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
					val.AddToCollection(coin);
				}
			} else {
				ParseValue(val, point);
			}

			return val;
		}

		protected static void ParseValue(Pile val, string toparse) {
			if (toparse.Equals("null")) { return; }

			if (toparse[0] == '\"') { // string
				val.AddString = toparse.Replace("\"", "");
			} else if (toparse.Equals("false") || toparse.Equals("true")) { // bool
				val.AddBool = toparse.Equals("true");
			} else { // must be double
				double doub = 0;
				double.TryParse(toparse, out doub);
				val.AddDouble = doub;
			}
		}

	}
}
