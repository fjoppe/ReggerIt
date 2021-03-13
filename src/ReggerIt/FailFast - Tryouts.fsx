#I __SOURCE_DIRECTORY__
#r @"bin/Debug/net5.0/ReggerIt.dll"

open System.Text.RegularExpressions
open ReggerIt

let pattern =
    (OneOf "ABCDEF") - NotBetween 'C' 'D'
    |> Convert.ToFullstringPattern

"ABCDEFGH".ToCharArray()
|>  Array.iter(fun s -> printfn "%c - %b" s ((Regex.Match (string(s), pattern)).Success))


