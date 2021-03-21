# Composing an Email-address pattern

Suppose we wanted to construct a pattern which matches an email address.

We'll use an easy-going approach to the email-address specification according to the [syntax definition on Wikipedia](https://en.wikipedia.org/wiki/Email_address#Syntax).

The definition is split up in a local and domain part.

Let's start with the local part, which may be implemented as follows:
```fsharp
open ReggerIt

let ucase = Between 'A' 'Z'
let lcase = Between 'a' 'z' 
let printable = OneOf "!#$%&'*+-/=?^_`{|}~"

let local =  OnceOrMore (ucase ||| lcase ||| Macro.decimalDigit ||| printable ||| Plain ".")

```

The domain part is composed from a list of labels, seperated by a dot. A label may contain a hyphen, but not at the start or end and has maximum length of 63:
```fsharp
let dot = Plain "."

let labelUnrestricted = ucase ||| lcase ||| Macro.decimalDigit

let label = (labelUnrestricted + RepeatRange 0 61 (labelUnrestricted ||| Plain "-") + labelUnrestricted) |||  labelUnrestricted

let domain = OnceOrMore(label + dot) + label
```

The pattern for the email-address is finally constructed by gluing it all together:

```fsharp
let email = local + Plain "@" + domain
```


Test the pattern:
```fsharp
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

```



