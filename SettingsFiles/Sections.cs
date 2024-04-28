using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Piles.SettingsFiles {
	internal abstract class Section {
		internal abstract string GetLine ();
	}

	internal class PropSection : Section {
		public Prop Property;

		public PropSection(Prop prop) {
			Property = prop;
		}

		internal override string GetLine () {
			return Property.ToString();
		}
	}

	internal class WhiteSpaceSection : Section {
		private int _amount = 0;

		public int Amount {
			get { return _amount; }
			set { _amount = Math.Max(value, 0); }
		}

		public WhiteSpaceSection(int amount) {
			Amount = amount;
		}

		internal override string GetLine() {
			return new string('\n', Math.Max(Amount - 1, 0));
		}
	}

	internal class CommentSection : Section {
		public string Comment;

		public CommentSection(string comment) {
			Comment = comment;
		}

		internal override string GetLine() {
			return "#" + Comment;
		}
	}
}
