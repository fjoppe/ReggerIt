#I __SOURCE_DIRECTORY__
#r @"bin/Debug/net5.0/ReggerIt.dll"

open ReggerIt
open System.Text.RegularExpressions
open ReggerIt

let ucase = Between 'A' 'Z'
let lcase = Between 'a' 'z'
let printable = OneOf "!#$%&'*+-/=?^_`{|}~"

let local =  OnceOrMore (ucase ||| lcase ||| Macro.decimalDigit ||| printable ||| Plain ".")

let dot = Plain "."

let labelUnrestricted = ucase ||| lcase ||| Macro.decimalDigit

let label = (labelUnrestricted + RepeatRange 0 61 (labelUnrestricted ||| Plain "-") + labelUnrestricted) |||  labelUnrestricted

let domain = OnceOrMore(label + dot) + label

let email = local + Plain "@" + domain

open System.Text.RegularExpressions

let pattern = email |> Convert.ToStringStartPattern

[
    "simple@example.com"
    "very.common@example.com"
    "disposable.style.email.with+symbol@example.com"
    "other.email-with-hyphen@example.com"
]
|>  List.map(fun input ->
    Regex.Match(input, pattern)
)
