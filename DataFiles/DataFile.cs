using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Piles.DataFiles {
    public static class DataFile {
        public static List<T?> Parse<T>(string path, Func<string[], T?> convert, out string error) {
            List<T?> parsed = new List<T?>();    

            if (!File.Exists(path)) {
                error = "Error: No file was found at " + path + " to parse as a data file";
                return parsed;
            }

            List<string> lines = File.ReadAllLines(path).ToList();

            int linecount = lines.Count;
            for (int i = 0; i < linecount; i++) {
                string line = lines[i].Trim();

                // this whole line is a comment
                if (line.Length == 0 || line[0] == '#') { 
                    lines.RemoveAt(i); 
                    linecount--; 
                    i--;
                    continue;
                }

                // parse the line
                // tokenize the line

                List<string> tokens = new List<string>();

                List<char> wait = new List<char>();
                int waitCount = 0;

                string build = "";

                foreach (char chr in line) {
                    if (waitCount > 0) {
                        if (chr == wait[waitCount - 1]) {
                            wait.RemoveAt(waitCount - 1);
                            waitCount--;
                            continue;
                        }
                    } else {
                        if (chr == '\"') {
                            wait.Add(chr);
                            waitCount++;
                            continue;
                        } else if (chr == '#') {
                            break;
                        } else if (chr == ' ' || chr == '\t') {
                            if (!build.Equals("")) {
                                tokens.Add(build);
                                build = "";
                            }
							continue;
						}
                    }

                    build += chr;
                }

                if (!build.Equals("")) { tokens.Add(build); }

                T? converted = convert(tokens.ToArray());
                parsed.Add(converted);
            }

            error = "";
            return parsed;
        }

		public static List<T?> Parse<T>(string path, Func<string[], T?> convert) {
            string buff = "";
            return Parse(path, convert, out buff);
        }

		public static List<T> ParseClean<T>(string path, Func<string[], T?> convert, out string error) {
			List<T?> items = Parse(path, convert, out error);
            List<T> cleaned = new List<T>();

            foreach (T? item in items) {
                if (item == null) { continue; }
                cleaned.Add(item);
            }

            return cleaned;
		}

		public static List<T> ParseClean<T>(string path, Func<string[], T?> convert) {
            string err = "";
            return ParseClean(path, convert, out err);
        }
	}
}
