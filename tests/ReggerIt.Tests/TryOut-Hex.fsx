#I __SOURCE_DIRECTORY__
#r @"bin/Debug/net5.0/ReggerIt.dll"

open ReggerIt
open System.Text.RegularExpressions

//  Define the alphabet
let HexDigit = Between '0' '9' |||  Between 'A' 'F'

//  Define the value-format
let hexByte = RepeatExact 2 HexDigit
let hexWord = RepeatExact 4 HexDigit

//  With leading indicators - old fashioned
let formatHexByteAntique = Plain "$" + NamedGroup "value" hexByte
let formatHexWordAntique = Plain "$" + NamedGroup "value" hexWord

//  With leading indicators - modern
let formatHexByte = Plain "0x" + NamedGroup "value" hexByte
let formatHexWord = Plain "0x" + NamedGroup "value" hexWord

//  Put it all together
let pattern = formatHexByteAntique ||| formatHexWordAntique ||| formatHexByte ||| formatHexWord

let validate s =
    let parse = Regex.Match(s, pattern |> Convert.ToFullstringPattern, RegexOptions.IgnoreCase)
    if parse.Success then Some (parse.Groups.["value"].Value)
    else None

//  Some test values
[
    "$AF"
    "$C001"
    "0x9F"
    "0xAAF1"
    "$eF"
    "illegal"
]
|>  List.iter(fun input ->
    validate input
    |>  function
        |   Some v -> printfn "Hex value: %s" v
        |   None   -> printfn "Not a hex value: %s" input
)

