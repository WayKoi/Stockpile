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

		public static new VersionPile Load (string path) {
			VersionPile file = new VersionPile();
			if (!File.Exists(path)) { return file; }

			file.Path = IO.GetFilePath(path);
			file.FileName = IO.GetFileName(path);
			file.FileExtension = IO.GetFileExtension(path);

			string content = File.ReadAllText(path);
			content = content.Replace("\n", "").Replace("\t", "").Replace("\r", "");
			List<string> chunks = ChunkData(content);

			Pile? point;

			foreach (string chunk in chunks) {
				point = LoadPile(chunk);
				if (point == null) { continue; }
				file.Add(point);
			}

			point = file.TopProperty("Version");
			string top = point.TopStringDefault("0.0.0");
			string[] segs = top.Split('.');

			if (segs.Length == 3) {
				int major = 0, minor = 0, patch = 0;
				int.TryParse(segs[0], out major);
				int.TryParse(segs[1], out minor);
				int.TryParse(segs[2], out patch);
				file.Version.Set((major, minor, patch));
			}

			file.Remove(point);

			return file;
		}

		public override string ToString() {
			return string.Format("Version: \"{0}.{1}.{2}\";\n", Version.Major, Version.Minor, Version.Patch) + base.ToString();
		}
	}
}
