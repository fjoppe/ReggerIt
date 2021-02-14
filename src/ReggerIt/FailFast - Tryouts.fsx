#I __SOURCE_DIRECTORY__
#r @"bin\Debug\net5.0\ReggerIt.dll"


open System.Text.RegularExpressions

let pattern = 
    ReggerIt.Plain "AA" 
    + ReggerIt.OneOf "CB" 
    + ReggerIt.Plain "+"
    + ReggerIt.Plain "*"
    + ReggerIt.Plain "\\w"
    |>  ReggerIt.ToPattern


Regex.Match ("AAB+*\\w", pattern, RegexOptions.IgnoreCase)

Regex.Match ("aab+*\\W", pattern, RegexOptions.IgnoreCase)


