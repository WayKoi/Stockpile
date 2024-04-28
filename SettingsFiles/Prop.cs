using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Piles.SettingsFiles {
	internal class Prop {
		public string Handle;
		public string Value;
		

		internal Prop (string handle, string value) {
			Handle = handle;
			Value = value;
		}

		public bool AsBool () {
			return Value.Equals("True");
		}

		public double AsDouble () {
			double val = 0;

			double.TryParse(Value, out val);

			return val;
		}

		public string AsString () {
			return Value;
		}

		public override string ToString () {
			return Handle + "=" + Value;
		}
	}
}
