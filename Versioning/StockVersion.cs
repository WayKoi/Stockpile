﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Piles.Versioning {
	public static class StockVersion {
		private static Dictionary<string, Dictionary<Ver, UpdateNode>> Trees = new Dictionary<string, Dictionary<Ver, UpdateNode>>();
		
		public static void Add (string ext, Ver version) {
			if (!Trees.ContainsKey(ext)) {
				Trees.Add(ext, new Dictionary<Ver, UpdateNode>());
			}

			Dictionary<Ver, UpdateNode> Nodes = Trees[ext];
			Nodes.Add(version, new UpdateNode(version));
		}

		public static void Connect (string ext, Ver start, Ver end, Func<VersionPile, VersionPile> update) {
			if (!Trees.ContainsKey(ext)) {
				Trees.Add(ext, new Dictionary<Ver, UpdateNode>());
			}

			Dictionary<Ver, UpdateNode> Nodes = Trees[ext];
			
			if (!Nodes.ContainsKey(start)) { Nodes.Add(start, new UpdateNode(start)); }
			if (!Nodes.ContainsKey(end)) { Nodes.Add(end, new UpdateNode(end)); }

			Nodes[start].Edges.Add(new UpdateEdge(Nodes[start], Nodes[end], update));
		}

		public static void Connect (string ext, ((int M, int m, int p) start, (int M, int m, int p) end, Func<VersionPile, VersionPile> update)[] tuples) {
			foreach (((int M, int m, int p) start, (int M, int m, int p) end, Func<VersionPile, VersionPile> update) tuple in tuples) {
				Connect(ext, new Ver(tuple.start), new Ver(tuple.end), tuple.update);
			}
		}

		public static void Update (VersionPile pile, Ver end) {
			List<UpdateEdge> path = GetPath(pile.FileExtension, pile.Version, end);

			foreach (UpdateEdge edge in path) {
				pile = edge.Update(pile);
				pile.Version = edge.End.Version;
			}

			pile.Save();
		}

		public static void Update(VersionPile pile, (int M, int m, int p) end) {
			Update(pile, new Ver(end));
		}

		private static List<UpdateEdge> GetPath (string ext, Ver start, Ver end) {
			List<UpdateEdge> edges = new List<UpdateEdge>();
			if (start == end) { return edges; }

			List<List<UpdateEdge>> Queue = new List<List<UpdateEdge>>();
			Dictionary<Ver, UpdateNode> Tree = Trees[ext];
			UpdateNode? point;
			
			if (!Tree.ContainsKey(start)) { return edges; }
			point = Tree[start];

			foreach (UpdateEdge edge in point.Edges) {
				if (!edge.Traversed) {
					Queue.Add(new List<UpdateEdge> { edge });
					edge.Traversed = true;
				}
			}

			List<UpdateEdge> current;

			// breadth first search to find the path
			do {
				current = Queue[0];
				point = current[current.Count - 1].End;

				foreach (UpdateEdge edge in point.Edges) {
					if (!edge.Traversed) {
						List<UpdateEdge> copy = Copy(current);
						copy.Add(edge);
						Queue.Add(copy);
						edge.Traversed = true;
					}
				}

				Queue.RemoveAt(0);
			} while (Queue.Count > 0 && point.Version != end);

			// need to reset for next search
			foreach (KeyValuePair<Ver, UpdateNode> pair in Tree) {
				pair.Value.ResetTraversal();
			}

			if (point.Version == end) {
				return current;
			}

			return edges;
		}

		private static List<UpdateEdge> Copy (List<UpdateEdge> edges) {
			return new List<UpdateEdge>(edges);
		}

		/// <summary>
		/// Default method for updating from version to version so that the graph can be complete
		/// </summary>
		/// <param name="change"></param>
		/// <returns></returns>
		public static Stockpile NoChange (Stockpile change) {
			return change;
		}
	}

	internal class UpdateNode {
		public Ver Version;
		public List<UpdateEdge> Edges = new List<UpdateEdge>();

		public UpdateNode (Ver version) {
			Version = version;
		}

		public void ResetTraversal () {
			foreach (UpdateEdge edge in Edges) {
				edge.Traversed = false;
			}
		}
	}

	internal class UpdateEdge {
		public UpdateNode Start, End;
		public Func<VersionPile, VersionPile> Update;
		public bool Traversed = false;

		public UpdateEdge (UpdateNode start, UpdateNode end, Func<VersionPile, VersionPile> update) {
			Start = start;
			End = end;
			Update = update;
		}
	}
}
