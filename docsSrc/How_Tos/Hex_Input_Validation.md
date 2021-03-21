# Simple example - how to validate hexadecimal input

Suppose we want to use Regex as a format validator of an input string. We will use the [widely known definition](https://wiki.osdev.org/Hexadecimal_Notation#:~:text=Hexadecimal%20notation%20is%20just%20another%20base%20for%20representing,hexadecimal%20is%2010%20through%2015%20in%20decimal%20notation.).

Throughout history, there have been various ways to indicate a hex string. In the 1970's-1990's, a hex number was indicated with a leading '\$' ie "\$A0", but nowadays, a hex-number is notated with a leading '0x', ie "0xF5"

We will support both notations, and accept bytes (8-bit) and words (16-bit), and we don't care about upper/lower-case.

The patterns are composed from the ground up:.

1. Define the alphabet;
2. Define the value-format;
3. Define the format of notation style in conjunction wih the value-format;
4. Put it all together;
5. Create a validation function which returns the hex-value when the input is valid.

```fsharp
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


```
