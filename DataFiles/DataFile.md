### Overview

Data Files are files with lines containing data where each line that isn't a comment has the same kind of data on it.
This allows a user to parse the entire file into a list of that object with the use of a parse function passed in on parse.

### Syntax

Data files can contain comments like this

```
# this is a comment
```

and those lines will be ignored on parse

Data files can also contain comments at the end of lines in the same manner

```
"some data" 234 443 222344 3 "other data" # this is a comment
```

This line will still be parsed up until the comment

To use the # symbol it must be contained within quotes like this

```
"#data" 23 3434 "#####"
```

This will not produce a comment

Data lines will be split by a space and sent off to the parse function but if the space is in quotes it will be ignored