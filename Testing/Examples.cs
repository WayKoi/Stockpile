using Piles.DataFiles;
using Piles.SettingsFiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Piles.Testing {
	internal static class Examples {
		public static void BasicStockpile () {
			// Basic Stockpile Usage
			Stockpile stock = Stockpile.Load("./Testing/Test.stp");

			Console.WriteLine(stock.ToString());

			List<Pile> props = stock.TopProperty("Test").Property("property");
			foreach (Pile pile in props) {
				Console.Write(pile.ToString());
			}
		}

		public static void AdvancedStockpile () {
			// More Advanced Usage
			Stockpile stock = Stockpile.Load("./Testing/Solar.stp");

			Console.WriteLine("\nPlanets\n");

			List<Pile> planets = stock.Property("planet");

			foreach (Pile planet in planets) {
				Console.WriteLine("Name : " + planet.TopPropString("name"));
				Console.WriteLine("Size (Radius) : " + planet.TopPropDouble("size") + "km");
				Console.WriteLine("Terrestrial : " + planet.TopPropBool("terrestrial") + "\n");
			}

			Console.WriteLine("The Star\n");

			Console.WriteLine("Name : " + stock.TopProperty("star").TopProperty("name").TopString);
			Console.WriteLine("Size (Radius) : " + stock.TopProperty("star").TopProperty("size").TopDouble + "km\n");

			List<Pile> terrestrials = stock.SearchTop("planet", "terrestrial", true);

			Console.WriteLine("Terrestrial Planets\n");

			foreach (Pile planet in terrestrials) {
				Console.WriteLine("Name : " + planet.TopPropString("name"));
				Console.WriteLine("Size (Radius) : " + planet.TopPropDouble("size") + "km");
				Console.WriteLine("Terrestrial : " + planet.TopPropBool("terrestrial") + "\n");
			}

			Console.WriteLine("Earth\n");

			Pile? found = stock.SearchSingle("planet", "name", "Earth");

			if (found != null) {
				Console.WriteLine(found);
			}

			// Saving the stockpile
			stock.FileName = ("out.stp");
			stock.Save();
		}

		public static void SettingsFiles () {
			// Loading and using / updating
			SettingsFile sett = SettingsFile.Load("./Testing/Test.sett");

			sett.Set("key", 200);
			bool flag = sett.Search("flag", false);

			// Be careful with adding comments on load / save, you could end up rewriting comments over and over again
			// comments are more for generated files, not loaded files
			sett.AddComment("otherKey", " needed for something");
			sett.FileName = "output.sett";
			sett.Save();
		}

		public static void CreatingSettingsFiles () {
			// Creating new
			SettingsFile sett = new SettingsFile() { SaveDirectory = "./Testing", FileName = "Created.sett" };

			sett.Set("SomeKey", true);
			sett.AddComment("SomeKey", "This is a value that is used for ...");

			sett.Set("Flag", true);
			sett.Set("Y", 10);
			sett.Set("X", 100);
			sett.Set("Z", -200);

			sett.Save();
		}

		public static void DataFiles () {
			// parse clean will remove all things that return null
			List<Item> items = DataFile.ParseClean("./Testing/Data.dat", Item.Parse);

			foreach (Item item in items) {
				Console.WriteLine(item);
			}
		}

	}

	internal class Item {
		public string Name { get; set; }
		public string Description { get; set; }
		public double Cost { get; set; }
		public int Stock { get; set; }
		public bool Perishable { get; set; }

		public Item(string name, string desc, double cost, int stock, bool perishable) {
			Name = name;
			Description = desc;
			Cost = cost;
			Stock = stock;
			Perishable = perishable;
		}

		public override string ToString() {
			return string.Format("\"{0}\" \"{1}\" {2} {3} {4}", Name, Description, Cost, Stock, Perishable);
		}

		// This functiion will turn the strings into the object
		public static Item? Parse(string[] parts) {
			if (parts.Length < 5) { return null; }

			try {
				return new Item(
					parts[0],
					parts[1],
					double.Parse(parts[2]),
					int.Parse(parts[3]),
					parts[4].ToLower().Equals("true")
				);
			} catch { return null; }
		}
	}
}
