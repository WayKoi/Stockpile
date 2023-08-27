using Piles;
using Piles.Versioning;

string path = Directory.GetCurrentDirectory() + "\\test.stp";

Stockpile pile = Stockpile.Load(path);

StockVersion.Connect(
	"stp",
	new ((int, int, int), (int, int, int), Func<Stockpile, Stockpile>)[] {
		((0, 0, 0), (1, 0, 0), Updatever2)
	}
);

StockVersion.Update(pile, (1, 0, 0));

/*Pile point = pile.TopProperty("Moon");
point.TopString = "test";
Pile sub = point.TopProperty("Size");
double size = sub.TopDoubleRange(20, 50);

sub = point.TopProperty("Name");
string name = sub.TopStringDefault("Luna");

sub = point.TopProperty("Material");
double dens = sub.TopProperty("Density").TopDoubleDefault(100);

Console.WriteLine(name + ": " + size + ", " + dens);*/

pile.Save();

Stockpile Updatever2 (Stockpile pile) {
	pile.TopProperty("Moon").TopProperty("Name").TopString = "updated";
	return pile;
}