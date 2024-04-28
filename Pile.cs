using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Piles {
	public class Pile : Pileable {
		private string _handle = string.Empty;
		public string Handle {
			get { return _handle; }
			private set {
				_handle = StandardizeHandle(value);
			}
		}

		private List<string> _strings = new List<string>();
		private List<double> _doubles = new List<double>();
		private List<bool> _bools = new List<bool>();

		public bool TopBool {
			get {
				if (_bools.Count == 0) { _bools.Add(false); }
				return _bools[0];
			}
			set {
				if (_bools.Count == 0) { _bools.Add(false); }
				_bools[0] = value;
			}
		}

		public bool[] Bools {
			get {
				return _bools.ToArray();
			}
		}

		public double TopDouble {
			get {
				if (_doubles.Count == 0) { _doubles.Add(0); }
				return _doubles[0];
			}
			set {
				if (_doubles.Count == 0) { _doubles.Add(0); }
				_doubles[0] = value;
			}
		}

		/// <summary>
		/// Allows you to ask for the top double and if there isnt one add one with the default value given
		/// </summary>
		/// <param name="defaultValue"></param>
		/// <returns>The top double</returns>
		public double TopDoubleDefault (double defaultValue = 0) {
			if (_doubles.Count == 0) { _doubles.Add(defaultValue); }
			return _doubles[0];
		}

		/// <summary>
		/// Allows you to ask for the top double and clamp it between a low and a high inclusive.
		/// If there is no top double it will default to the low
		/// </summary>
		/// <param name="low"></param>
		/// <param name="high"></param>
		/// <returns>The top double after being clamped</returns>
		public double TopDoubleRange (double low, double high) {
			if (_doubles.Count == 0) { _doubles.Add(low); }
			_doubles[0] = Math.Clamp(_doubles[0], low, high);
			return _doubles[0];
		}

		public double[] Doubles {
			get {
				return _doubles.ToArray();
			}
		}

		public string TopString {
			get {
				if (_strings.Count == 0) { _strings.Add(string.Empty); }
				return _strings[0];
			}
			set {
				if (_strings.Count == 0) { _strings.Add(string.Empty); }
				_strings[0] = value;
			}
		}

		/// <summary>
		/// Allows you to ask for the top string and if there isnt one add one with the default value given
		/// </summary>
		/// <param name="defaultValue"></param>
		/// <returns>The top string</returns>
		public string TopStringDefault (string defaultValue) {
			if (_strings.Count == 0) { _strings.Add(defaultValue); }
			return _strings[0];
		}

		public string[] Strings {
			get {
				return _strings.ToArray();
			}
		}

		public Pile(string handle) {
			Handle = handle;
		}

		public void Clear_strings() { _strings.Clear(); }
		public void Clear_doubles() { _doubles.Clear(); }
		public void Clear_bools() { _bools.Clear(); }

		public void Add (double val) {
			if (_doubles == null) { _doubles = new List<double>(); }
			_doubles.Add(val);
		}

		public void Add(string val) {
			if (_strings == null) { _strings = new List<string>(); }
			val = val.Replace("\"", "");
			_strings.Add(val);
		}

		public void Add(bool val) {
			if (_bools == null) { _bools = new List<bool>(); }
			_bools.Add(val);
		}

		public override string ToString() {
			string build = "";
			int count = 0;

			if (_strings.Count > 0) {
				foreach (string val in _strings) {
					build += (count > 0 ? ", " : "") + "\"" + val + "\"";
					count++;
				}
			}

			if (_doubles.Count > 0) {
				foreach (double val in _doubles) {
					build += (count > 0 ? ", " : "") + val;
					count++;
				}
			}

			if (_bools.Count > 0) {
				foreach (bool val in _bools) {
					build += (count > 0 ? ", " : "") + val.ToString().ToLower();
					count++;
				}
			}

			if (_collection.Count > 0) {
				string sub = (count > 0 ? ", " : "") + "{\n\t";

				bool append = false;
				List<string> _collections = new List<string>();
				foreach (KeyValuePair<string, List<Pile>> key in _collection) {
					if (key.Value == null) { continue; }
					foreach (Pile val in key.Value) {
						if (val != null) {
							_collections.Add(val.ToString());
							append = true;
						}
					}
				}

				foreach (string val in _collections) {
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
