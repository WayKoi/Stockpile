using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Piles.Versioning {
	public class Ver {
		public int Major, Minor, Patch;

		public Ver(int major = 0, int minor = 0, int patch = 0) {
			Major = major;
			Minor = minor;
			Patch = patch;
		}

		public Ver((int major, int minor, int patch) tuple) {
			Major = tuple.major;
			Minor = tuple.minor;
			Patch = tuple.patch;
		}


		public void Set((int Major, int Minor, int Patch) version) {
			Major = version.Major;
			Minor = version.Minor;
			Patch = version.Patch;
		}

		public static bool operator ==(Ver a, Ver b) {
			return a.Major == b.Major && a.Minor == b.Minor && a.Patch == b.Patch;
		}

		public static bool operator !=(Ver a, Ver b) {
			return a.Major != b.Major || a.Minor != b.Minor || a.Patch != b.Patch;
		}

		public override bool Equals(object? obj) {
			if (ReferenceEquals(this, obj)) {
				return true;
			}

			if (ReferenceEquals(obj, null)) {
				return false;
			}

			if (!(obj is Ver)) { return false; }

			return this == (Ver) obj;
		}

		public override int GetHashCode() {
			return Tuple.Create(Major, Minor, Patch).GetHashCode();
		}
	}
}
