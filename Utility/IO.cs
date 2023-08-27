using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Piles.Utility {
	public static class IO {
		public static string GetFilePath(string path) {
			path = path.Replace("\\", "/");
			List<string> chop = path.Split('/').ToList();

			if (!File.Exists(path)) { return string.Join('/', chop); }
			chop.RemoveAt(chop.Count - 1);

			return string.Join('/', chop);
		}

		public static string GetFileName(string path) {
			path = path.Replace("\\", "/");
			string[] chop = path.Split('/');

			List<string> slice = chop[chop.Length - 1].Split('.').ToList();
			if (slice.Count > 1) { slice.RemoveAt(slice.Count - 1); }
			
			return String.Join(".", slice);
		}

		public static string GetFileExtension(string path) {
			path = path.Replace("\\", "/");
			string[] chop = path.Split('/');
			string[] slice = chop[chop.Length - 1].Split('.');

			if (slice.Length >= 2) {
				return slice[slice.Length - 1];
			}

			return ""; // no Extension, ie this is a folder or a file with no extension
		}
	}
}
