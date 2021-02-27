#I __SOURCE_DIRECTORY__
#r @"bin/Debug/net5.0/ReggerIt.dll"

open System.Text.RegularExpressions


let pattern =
    ReggerIt.OneOf "ABC" + ReggerIt.OneOf "DEF" |> ReggerIt.ToFullstringPattern


Regex.Match ("AD", pattern)


