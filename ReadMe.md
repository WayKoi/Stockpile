# Overview

This is a library I use for creating and managing data / settings files.

There are three main types that this library can handle.

- [Stockpiles](#stockpiles)
- [Settings Files](#settings-files)
- [Data Files](#data-files)

---

# Stockpiles

Stockpiles are my own file format similar to JSON. They support strings, doubles, booleans, lists, and collections / objects.

The biggest difference is that everything in Stockpile is named. No more floating random objects that have random structure.

Stockpiles currently have no validation and no versioning, these are both to come in the future.

A Stockpile file looks something like this

```
Handle: {
	X: 20;
	Y: 40;
	Z: -2:
};
```

These files also support objects / collections inside of other collections, like this

```
Handle: {
	Position: {
		X: 20;
		Y: 40;
		Z: -20;
	};
};
```

Loading in Stockpiles is done in one command

```C#
Stockpile stock = Stockpile.Load("./Testing/Solar.stp");
```

Stockpiles also allow for multiple definitions of the same handle, like this

```
planet: {
	name: "Earth";
	size: 6371;
	terrestrial: true;
};
planet: {
	name: "Mars";
	size: 3390;
	terrestrial: true;
};
```

This is super easy in code to search for, this would be done like so (assuming "stock" is a loaded Stockpile of the previously shown file)

```C#
List<Pile> planets = stock.Property("planet");
```

Stockpiles also allow for you to assign lists to handles, like this using square brackets

```
Test: [false, {
	property: false;
	property: false;
}];
```

These lists can be filled with as many strings, bools and doubles you like but only one collection.

Stockpiles let you ask for all the values a property has or just the top one. This is handy if the property only has one value.

```C#
// Accessing the top property with the handle "name"
Pile prop = stock.TopProperty("name");

// Accessing the top string of that property
string name = prop.TopString;

// Doing it all at once
name = stock.TopPropString("name");
```

If the property you are looking for does not exist it will be created for you with the default value given.

```C#
// Giving a single default value
double cost = prop.TopPropDouble("cost", 20);

// Giving a valid range of values
// Giving a range will make clamp the value of the property between the given min and max
// This will also update the actual value of the property, so no need to check afterwards
cost = prop.TopPropDouble("cost", 0.01, 99.99);
```

Stockpiles also remember where they were loaded from and can be saved very easily

```C#
stock.Save();
```

You can change the location it will be saved as well as the file name it will have on save

```C#
stock.SaveDirectory = "./Testing";
stock.FileName = "out.stp";
```

*Note: examples of Stockpile usage can be found in the Examples section of this project*

---

# Settings Files

Settings files are a stripped down version of Stockpiles. They allow for saving and loading of key value pairs, but no collections or lists.

A basic settings file looks like this

```
# this is a comment
# comments can be handy

Key=some value
OtherKey=10.20
Enabled=false
```

They can be read in like this

```C#
SettingsFile sett = SettingsFile.Load("./Testing/Test.sett");
```

Every key value pair is read in as a string and can be interpreted as a string, double or bool

```
string str = file.Search("key", "default");
double num = file.Search("key", 10);
bool truth = file.Search("key", false);
```

The default value given is used if the file does not contain the key given. It will add the key to the settings file with the default value if the key is not found.

Saving Settings files is just as easy as saving Stockpiles

```C#
sett.Save();
```

And you can also change the location and name of the file saved the same way as stockpiles

```C#
sett.SaveDirectory = "./Testing";
sett.FileName = "out.sett";
```

*Note: Examples of settings file usage can be found in the examples section of this project*

---

# Data Files

Data files are read-only files that allow for easy parsing of human readable files.

These files will look something like this.

```
# comments are nice to have

"Barbecue" "Outdoor Equipment" 899.99 3 false
Table Furniture 49.99 12 false
```

As seen above you can define strings either with the quotation marks or without them. The difference is you can put spaces and the comment symbol, #, 
between quotation marks and they will be put into the string on parse.

Data files are allowed to have whitespace and comments throughout, these will be ignored on parse.
Comments can be done one of two ways

```
# this is a comment that has its own line
"this is data" 10 20 40 20 # this comment is at the end of a data line
```

The file below will be parsed using a parse function to turn each line into a class

```
# this is a file containing stock for items in a store
# the format is
# "<name>" "<description>" <cost> <stock> <perishable (true or false)>

"Apple" "Fruit" 1.19 30 true
"Orange" "Fruit" 1.80 20 true
"Watermelon" "Fruit" 3.50 8 true

"Barbecue" "Outdoor Equipment" 899.99 3 false
Table Furniture 49.99 12 false
```

Given the Item class shown

```C#
internal class Item {
	public string Name { get; set; }
	public string Description { get; set; }
	public double Cost { get; set; }
	public int Stock { get; set; }
	public bool Perishable { get; set; }
}
```

On parse, each line will be tokenized into a string array, which will be sent off to the passed in parse function.
The DataFile Parse function requires a function to parse each line of the file into this class, so we will write one like this

```C#
public static Item? Parse(string[] parts) {
	// make sure the data is the right length
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
```

We simply return null if the lines data could not be parsed properly.

To use this function we send it to the DataFile parser like so

```C#
List<Item> items = DataFile.ParseClean("./Testing/Data.dat", Item.Parse);
```

The DataFile class allows for you to either parse regularily, with null values in the list, or parse cleanly removing all null return values from the list.
In the case above we use the clean version of the parse because we don't want any null values.

*Note: this example is found in the examples section of the project*

