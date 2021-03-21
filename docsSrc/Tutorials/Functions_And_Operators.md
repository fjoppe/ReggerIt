# Functions and Operators

The primary definition for a character match comes from:

*   ``Plain`` match, ie ``Plain "A"`` matches the "A" character, ``Plain "Hello"`` matches the word "Hello"
*   ``OneOf`` match, ie ``OneOf "AC"`` matches on "A" and "C"
*   ``Between`` match, ie ``Between 'a' 'z'``  matches all characters between 'a' and 'z'
*   Macro, the macro module provides various pre-defined character classes, including wildcard ``Macro.any``

The ``Plain`` and ``OneOf`` constructions do not require character escaping. So ``Plain "."`` matches the dot character. ``Macro.any`` is used as wildcard-character.


## Character Classes

Both ``OneOf`` and ``Between`` define character classes. Of wich also exist an inversion ``NotOneOf`` and ``NotBetween``.

Character classes can be subtracted.

For example:
    Between 'a' 'z' - OneOf "klm" // matches all lowercase leters except "klm"



## Operators

All Regular Expression (sub-)patterns can be used for these operators:

*   ``+`` - concatenation, ie ``Plain "A" + Plain "B"`` (which is the same as Plain "AB");
*   ``|||`` - or, either the left or right side must match, ie ``Plain "AB" ||| OneOf "CD"`` matches: "AB", "C", "D";

## Repetitions

One can use one of these repetitions:

*   ``ZeroOrMore`` - the input sub-pattern may repeat zero or more times;
*   ``OnceOrMore`` - the input sub-pattern may repeat once or more times;
*   ``RepeatRange`` - the input sub-pattern may repeat between a min and max times;
*   ``RepeatExact`` - the input sub-pattern must repeat exactly the indicated times;
*   ``Optional`` - Zero or one;

##  Grouping

These ways are supported for groups:

*   ``Group`` - Anonymous groups
*   ``NamedGroup`` - NamedGroup

Usually, anonymous groups are useful in lesser complex patterns. The named groups help in complex patterns. 
Naming groups make it a lot easier to extract sub-strings from a matching result.


##  Conversion

The conversion module is used to create the pattern-string:

*   ``ToStringStartPattern`` - Creates the pattern from the input, the start of the input string must match the start of the pattern;
*   ``ToFullstringPattern``  - Creates the pattern from the input, the full string must match the full pattern;
*   ``ToPattern`` - Creates the pattern from the input, the pattern-matching can start in the middle of the input string, and may match a substring;

The conversion module glues it together:

```fsharp
open ReggerIt
open System.Text.RegularExpressions

let pattern = Plain "HelloWorld" |> Convert.ToStringStartPattern

let input = "HelloWorld"

Regex.Match(input, pattern) // match success
```

##  Macro

The ``Macro`` module contains various frequently used character classes and definitions:

*   ``Macro.any`` - wildcard character, matches anything except newline
*   ``Macro.whitespace`` - matches any whitespace, including space, tab, and more
*   ``Macro.nonWhitespace`` - matches any non whitespace
*   ``Macro.bell`` - matches the bell character
*   ``Macro.backspace`` - matches the bachspace character
*   ``Macro.tab`` - matches the tab character
*   ``Macro.carriageReturn`` - matches the carriageReturn character
*   ``Macro.verticalTab`` - matches the verticalTab character
*   ``Macro.formFeed`` - matches the formFeed character
*   ``Macro.newLine`` - matches the newLine character
*   ``Macro.escape`` - matches the escape character
*   ``Macro.ascii`` - matches the specified 8-bit hex character
*   ``Macro.utf16`` - matches the specified 16-bit hex character
*   ``Macro.wordCharacter`` - matches a word character, letters, digits
*   ``Macro.nonWordCharacter`` - matches a non-word character
*   ``Macro.decimalDigit`` - matches a decimal digit character 0..9
*   ``Macro.nonDecimalDigit`` - matches a non-decimal digit character 0..9
*   ``Macro.namedBlock`` - matches a character from the specified named block




