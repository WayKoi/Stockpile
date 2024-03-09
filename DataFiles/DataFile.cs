﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stockpile.DataFiles {
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
                if (line[0] == '#') { 
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
                        }
                    } else {
                        if (chr == '\"') {
                            wait.Add(chr);
                            continue;
                        } else if (chr == '#') {
                            break;
                        } else if (chr == ' ') {
                            tokens.Add(build);
                            build = "";
                            continue;
                        }   
                    }

                    build += chr;
                }
            }

            error = "";
            return parsed;
        }
    }
}
