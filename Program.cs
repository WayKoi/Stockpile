using Piles;
using Piles.Versioning;
using Piles.DataFiles;


Stockpile stock = Stockpile.Load("./Testing/Test.stp");

Console.WriteLine(stock.ToString());

List<Pile> props = stock.TopProperty("Test").Property("property");
foreach (Pile pile in props) {
	Console.Write(pile.ToString());
}

Console.WriteLine("\nPlanets\n");

stock = Stockpile.Load("./Testing/Solar.stp");

List<Pile> planets = stock.Property("planet");

foreach (Pile planet in planets) {
	Console.WriteLine("Name : " + planet.TopProperty("name").TopString);
	Console.WriteLine("Size (Radius) : " + planet.TopProperty("size").TopDouble + "km");
	Console.WriteLine("Terrestrial : " + planet.TopProperty("terrestrial").TopBool + "\n");
}

Console.WriteLine("The Star\n");

Console.WriteLine("Name : " + stock.TopProperty("star").TopProperty("name").TopString);
Console.WriteLine("Size (Radius) : " + stock.TopProperty("star").TopProperty("size").TopDouble + "km");


/*string path = Directory.GetCurrentDirectory() + "\\test.stp";

*//*Stockpile pile = Stockpile.Load(path);

StockVersion.Connect(
	"stp",
	new ((int, int, int), (int, int, int), Func<Stockpile, Stockpile>)[] {
		((0, 0, 0), (1, 0, 0), Updatever2)
	}
);

// StockVersion.Update(pile, (1, 0, 0));

Pile point = pile.TopProperty("Moon");
point.TopString = "test";
Pile sub = point.TopProperty("Size");
double size = sub.TopDoubleRange(20, 50);

sub = point.TopProperty("Name");
string name = sub.TopStringDefault("Luna");

sub = point.TopProperty("Material");
double dens = sub.TopProperty("Density").TopDoubleDefault(100);

Console.WriteLine(name + ": " + size + ", " + dens);

pile.Save();

Stockpile Updatever2(Stockpile pile) {
	pile.TopProperty("Moon").TopProperty("Name").TopString = "updated";
	return pile;
}*//*

// Data File Testing

string test = "";
DataFile.Parse<Object>("./test.txt", print, out test);
Console.WriteLine(test);

static object? print (string[] test) {
	foreach (string part in test) {
		Console.Write("\"" + part + "\" ");
	}

	Console.WriteLine();

	return null;
}*/