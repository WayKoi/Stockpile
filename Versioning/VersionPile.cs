using Piles;
using Piles.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Piles.Versioning {
	public class VersionPile : Stockpile {
		public Ver Version { get; set; } = new Ver();

		

		public override string ToString() {
			return string.Format("Version: \"{0}.{1}.{2}\";\n", Version.Major, Version.Minor, Version.Patch) + base.ToString();
		}
	}
}
