using Piles;

string path = Directory.GetCurrentDirectory() + "\\test.stp";

Stockpile pile = Stockpile.Load(path);

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