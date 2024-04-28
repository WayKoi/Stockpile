using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Piles {
	public abstract class Pileable {
		protected Dictionary<string, List<Pile>> _collection = new Dictionary<string, List<Pile>>();

		public void Add(Pile val) {
			if (val == null) { return; }

			if (!_collection.ContainsKey(val.Handle)) {
				_collection.Add(val.Handle, new List<Pile>());
			}

			_collection[val.Handle].Add(val);
		}

		public void Remove(Pile val) {
			if (val == null) { return; }

			if (_collection.ContainsKey(val.Handle)) {
				_collection[val.Handle].Remove(val);
				if (_collection[val.Handle].Count <= 0) {
					_collection.Remove(val.Handle);
				}
			}
		}

		public bool TopPropBool(string propertyName) {
			Pile prop = TopProperty(propertyName);
			return prop.TopBool;
		}

		public double TopPropDouble(string propertyName, double defaultValue = 0) {
			Pile prop = TopProperty(propertyName);
			return prop.TopDoubleDefault(defaultValue);
		}

		public double TopPropDouble(string propertyName, double min, double max) {
			Pile prop = TopProperty(propertyName);
			return prop.TopDoubleRange(min, max);
		}

		public string TopPropString(string propertyName, string defaultValue = "") {
			Pile prop = TopProperty(propertyName);
			return prop.TopStringDefault(defaultValue);
		}

		public Pile TopProperty(string handle) {
			handle = StandardizeHandle(handle);
			if (!_collection.ContainsKey(handle)) { _collection.Add(handle, new List<Pile>()); }

			List<Pile> point = _collection[handle];
			if (point.Count == 0) { point.Add(new Pile(handle)); }

			return point[0];
		}

		public List<Pile> Property(string handle) {
			handle = StandardizeHandle(handle);
			if (!_collection.ContainsKey(handle)) { _collection.Add(handle, new List<Pile>()); }
			return _collection[handle];
		}

		public bool HasProperty (string handle) {
			return _collection.ContainsKey(handle) && _collection[handle].Count > 0;
		}


		/// <summary>
		/// Searches for piles with the given handle containing the given property with the given value
		/// </summary>
		/// <param name="handle"></param>
		/// <param name="property"></param>
		/// <param name="value"></param>
		/// <returns>A list of All piles that match this criterea.</returns>
		public List<Pile> Search (string handle, string property, string value) {
			List<Pile> collect = new List<Pile>();

			List<Pile> search = Property(handle);

			foreach (Pile check in search) {
				if (check.HasProperty(property)) {
					Pile obtain = check.TopProperty(property);

					string[] parts = obtain.Strings;

					if (parts.Contains(value)) {
						collect.Add(check);
					}
				}
			}

			return collect;
		}

		public List<Pile> SearchTop(string handle, string property, string value) {
			List<Pile> collect = new List<Pile>();

			List<Pile> search = Property(handle);

			foreach (Pile check in search) {
				if (check.HasProperty(property) && check.TopPropString(property).Equals(value)) {
					collect.Add(check);
				}
			}

			return collect;
		}

		public List<Pile> SearchTop(string handle, string property, double value) {
			List<Pile> collect = new List<Pile>();

			List<Pile> search = Property(handle);

			foreach (Pile check in search) {
				if (check.HasProperty(property) && check.TopPropDouble(property) == value) {
					collect.Add(check);
				}
			}

			return collect;
		}

		public List<Pile> SearchTop(string handle, string property, bool value) {
			List<Pile> collect = new List<Pile>();

			List<Pile> search = Property(handle);

			foreach (Pile check in search) {
				if (check.HasProperty(property) && check.TopPropBool(property) == value) {
					collect.Add(check);
				}
			}

			return collect;
		}

		/// <summary>
		/// Searches for a pile with the given handle containing the given property with the given value
		/// </summary>
		/// <param name="handle"></param>
		/// <param name="property"></param>
		/// <param name="value"></param>
		/// <returns>The first pile found with the given criterea, if none are found to match the criterea returns null.</returns>
		public Pile? SearchSingle(string handle, string property, string value) {
			List<Pile> search = Property(handle);

			foreach (Pile check in search) {
				if (check.HasProperty(property) && check.TopPropString(property).Equals(value)) {
					return check;
				}
			}

			return null;
		}

		public void Clear_collection() { _collection.Clear(); }

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
	}
}
