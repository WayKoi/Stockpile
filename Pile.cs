using System;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Piles {
	public class Pile {
		private string _handle = string.Empty;
		public string Handle {
			get { return _handle; }
			protected set {
				_handle = Stockpile.StandardizeHandle(value);
			}
		}

		protected List<string> Strings = new List<string>();
		protected List<double> Doubles = new List<double>();
		protected List<bool> Bools = new List<bool>();

		protected Dictionary<string, List<Pile>> Collection = new Dictionary<string, List<Pile>>();

		public double AddDouble {
			set {
				if (Doubles == null) { Doubles = new List<double>(); }
				Doubles.Add(value);
			}
		}

		public string AddString {
			set {
				if (Strings == null) { Strings = new List<string>(); }
				value = value.Replace("\"", "");
				Strings.Add(value);
			}
		}

		public bool AddBool {
			set {
				if (Bools == null) { Bools = new List<bool>(); }
				Bools.Add(value);
			}
		}

		public bool TopBool {
			get {
				if (Bools.Count == 0) { Bools.Add(false); }
				return Bools[0];
			}
			set {
				if (Bools.Count == 0) { Bools.Add(false); }
				Bools[0] = value;
			}
		}

		public bool[] AllBools {
			get {
				return Bools.ToArray();
			}
		}

		public double TopDouble {
			get {
				if (Doubles.Count == 0) { Doubles.Add(0); }
				return Doubles[0];
			}
			set {
				if (Doubles.Count == 0) { Doubles.Add(0); }
				Doubles[0] = value;
			}
		}

		/// <summary>
		/// Allows you to ask for the top double and if there isnt one add one with the default value given
		/// </summary>
		/// <param name="defaultValue"></param>
		/// <returns>The top double</returns>
		public double TopDoubleDefault (double defaultValue = 0) {
			if (Doubles.Count == 0) { Doubles.Add(defaultValue); }
			return Doubles[0];
		}

		/// <summary>
		/// Allows you to ask for the top double and clamp it between a low and a high inclusive.
		/// If there is no top double it will default to the low
		/// </summary>
		/// <param name="low"></param>
		/// <param name="high"></param>
		/// <returns>The top double after being clamped</returns>
		public double TopDoubleRange (double low, double high) {
			if (Doubles.Count == 0) { Doubles.Add(low); }
			Doubles[0] = Math.Min(Math.Max(Doubles[0], low), high);
			return Doubles[0];
		}

		public double[] AllDoubles {
			get {
				return Doubles.ToArray();
			}
		}

		public string TopString {
			get {
				if (Strings.Count == 0) { Strings.Add(string.Empty); }
				return Strings[0];
			}
			set {
				if (Strings.Count == 0) { Strings.Add(string.Empty); }
				Strings[0] = value;
			}
		}

		/// <summary>
		/// Allows you to ask for the top string and if there isnt one add one with the default value given
		/// </summary>
		/// <param name="defaultValue"></param>
		/// <returns>The top string</returns>
		public string TopStringDefault (string defaultValue) {
			if (Strings.Count == 0) { Strings.Add(defaultValue); }
			return Strings[0];
		}

		public string[] AllStrings {
			get {
				return Strings.ToArray();
			}
		}

		public Pile(string handle) {
			Handle = handle;
		}

		public void ClearStrings() { Strings.Clear(); }
		public void ClearDoubles() { Doubles.Clear(); }
		public void ClearBools() { Bools.Clear(); }
		public void ClearCollection() { Collection.Clear(); }

		public void AddToCollection(Pile val) {
			if (val == null) { return; }
			if (!Collection.ContainsKey(val.Handle)) {
				Collection.Add(val.Handle, new List<Pile>());
			}

			Collection[val.Handle].Add(val);
		}

		public Pile TopProperty(string handle) {
			handle = Stockpile.StandardizeHandle(handle);
			if (!Collection.ContainsKey(handle)) { Collection.Add(handle, new List<Pile>()); }

			List<Pile> point = Collection[handle];
			if (point.Count == 0) { point.Add(new Pile(handle)); }

			return point[0];
		}

		public List<Pile> Property(string handle) {
			handle = Stockpile.StandardizeHandle(handle);
			if (!Collection.ContainsKey(handle)) { Collection.Add(handle, new List<Pile>()); }
			return Collection[handle];
		}

		public override string ToString() {
			string build = "";
			int count = 0;

			if (Strings.Count > 0) {
				foreach (string val in Strings) {
					build += (count > 0 ? ", " : "") + "\"" + val + "\"";
					count++;
				}
			}

			if (Doubles.Count > 0) {
				foreach (double val in Doubles) {
					build += (count > 0 ? ", " : "") + val;
					count++;
				}
			}

			if (Bools.Count > 0) {
				foreach (bool val in Bools) {
					build += (count > 0 ? ", " : "") + val.ToString().ToLower();
					count++;
				}
			}

			if (Collection.Count > 0) {
				string sub = (count > 0 ? ", " : "") + "{\n\t";

				bool append = false;
				List<string> collections = new List<string>();
				foreach (KeyValuePair<string, List<Pile>> key in Collection) {
					if (key.Value == null) { continue; }
					foreach (Pile val in key.Value) {
						if (val != null) {
							collections.Add(val.ToString());
							append = true;
						}
					}
				}

				foreach (string val in collections) {
					sub += val.Replace("\n", "\n\t");
				}

				sub = sub.Remove(sub.Length - 1) + "}";

				if (append) {
					build += sub;
					count++;
				}
			}

			if (count > 1) {
				build = "[" + build + "]";
			} else if (count == 0) {
				build = "null";
			}

			build = Handle + ": " + build + ";\n";

			return build;
		}
	}
}
