namespace ReggerIt.Simple.Construct.Tests

open System
open Expecto
open ReggerIt
open System.Text.RegularExpressions


module ``Simple ReggerIt Tests`` =
    [<Tests>]
    let ``Simple Text patterns`` =
        testList "Simple Text Patterns" [
            testCase $"FullString {nameof(Plain)} Text" <| fun _ ->
                let pattern =
                    Plain "AAB"
                    |>  Convert.ToFullstringPattern

                let inputs = [ "AAB" ]

                inputs
                |>  List.iter(fun input ->
                    let subject = Regex.Match(input, pattern)
                    Expect.isTrue subject.Success $"Unsuccessful match on input '{input}'"
                    Expect.equal subject.Value input $"Match is not equal to input '{input}'"
                )


            testCase $"FullString Concatenated {nameof(Plain)} Text" <| fun _ ->
                let pattern =
                    Plain "AB" + Plain "CD"
                    |>  Convert.ToFullstringPattern

                let inputs = [ "ABCD" ]

                inputs
                |>  List.iter(fun input ->
                    let subject = Regex.Match(input, pattern)
                    Expect.isTrue subject.Success $"Unsuccessful match on input '{input}'"
                    Expect.equal subject.Value input $"Match is not equal to input '{input}'"
                )

            testCase $"FullString {nameof(OneOf)} Text" <| fun _ ->
                let pattern =
                    OneOf "ABC"
                    |>  Convert.ToFullstringPattern

                let inputs = [ "A"; "B"; "C" ]

                inputs
                |>  List.iter(fun input ->
                    let subject = Regex.Match(input, pattern)
                    Expect.isTrue subject.Success $"Unsuccessful match on input '{input}'"
                    Expect.equal subject.Value input $"Match is not equal to input '{input}'"
                )


            testCase $"FullString concatenated {nameof(OneOf)} Text" <| fun _ ->
                let pattern =
                    OneOf "ABC" + OneOf "DEF"
                    |>  Convert.ToFullstringPattern

                let inputs = [ "AD"; "BE"; "CF" ]

                inputs
                |>  List.iter(fun input ->
                    let subject = Regex.Match(input, pattern)
                    Expect.isTrue subject.Success $"Unsuccessful match on input '{input}'"
                    Expect.equal subject.Value input $"Match is not equal to input '{input}'"
                )


            testCase $"FullString {nameof(NotOneOf)} Text - sunny day" <| fun _ ->
                let pattern =
                    Plain "X" + (NotOneOf "ABC") + Plain "X"
                    |>  Convert.ToFullstringPattern

                let inputs = [ "XEX"; "XFX"; "XGX" ]

                inputs
                |>  List.iter(fun input ->
                    let subject = Regex.Match(input, pattern)
                    Expect.isTrue subject.Success $"Unsuccessful match on input '{input}'"
                    Expect.equal subject.Value input $"Match is not equal to input '{input}'"
                )

            testCase $"FullString {nameof(NotOneOf)} Text - rainy day" <| fun _ ->
                let pattern =
                    Plain "X" + (NotOneOf "ABC") + Plain "X"
                    |>  Convert.ToFullstringPattern

                let inputs = [ "XAX"; "XBX"; "XCX" ]

                inputs
                |>  List.iter(fun input ->
                    let subject = Regex.Match(input, pattern)
                    Expect.isFalse subject.Success $"Successful match on input '{input}'"
                )


            testCase $"FullString Mixed Concatenated {nameof(Plain)} and {nameof(OneOf)} Text" <| fun _ ->
                let pattern =
                    Plain "Test" + OneOf "ABC" + Plain "After"
                    |>  Convert.ToFullstringPattern

                let inputs = [ "TestAAfter"; "TestBAfter"; "TestCAfter" ]

                inputs
                |>  List.iter(fun input ->
                    let subject = Regex.Match(input, pattern)
                    Expect.isTrue subject.Success $"Unsuccessful match on input '{input}'"
                    Expect.equal subject.Value input $"Match is not equal to input '{input}'"
                )

        ]

    [<Tests>]
    let ``Complex Text Patterns`` =
        testList "Complex Text Patterns" [
            testCase $"FullString {nameof(|||)} Once on {nameof(Plain)}" <| fun _ ->
                let pattern =
                    Plain "A" ||| Plain "B"
                    |>  Convert.ToFullstringPattern

                let inputs = [ "A"; "B" ]

                inputs
                |>  List.iter(fun input ->
                    let subject = Regex.Match(input, pattern)
                    Expect.isTrue subject.Success $"Unsuccessful match on input '{input}'"
                    Expect.equal subject.Value input $"Match is not equal to input '{input}'"
                )

            testCase $"FullString multiple {nameof(|||)} on {nameof(Plain)}" <| fun _ ->
                let pattern =
                    Plain "A" ||| Plain "B" ||| Plain "C"
                    |>  Convert.ToFullstringPattern

                let inputs = [ "A"; "B"; "C" ]

                inputs
                |>  List.iter(fun input ->
                    let subject = Regex.Match(input, pattern)
                    Expect.isTrue subject.Success $"Unsuccessful match on input '{input}'"
                    Expect.equal subject.Value input $"Match is not equal to input '{input}'"
                )

            testCase $"FullString {nameof(|||)} Once on {nameof(OneOf)}" <| fun _ ->
                let pattern =
                    OneOf "AC" ||| OneOf "BD"
                    |>  Convert.ToFullstringPattern

                let inputs = [ "A"; "C"; "B"; "D" ]

                inputs
                |>  List.iter(fun input ->
                    let subject = Regex.Match(input, pattern)
                    Expect.isTrue subject.Success $"Unsuccessful match on input '{input}'"
                    Expect.equal subject.Value input $"Match is not equal to input '{input}'"
                )

            testCase $"FullString multiple {nameof(|||)} on {nameof(OneOf)}" <| fun _ ->
                let pattern =
                    OneOf "AD" ||| OneOf "BE" ||| OneOf "CF"
                    |>  Convert.ToFullstringPattern

                let inputs = [ "A"; "B"; "C"; "D"; "E"; "F" ]

                inputs
                |>  List.iter(fun input ->
                    let subject = Regex.Match(input, pattern)
                    Expect.isTrue subject.Success $"Unsuccessful match on input '{input}'"
                    Expect.equal subject.Value input $"Match is not equal to input '{input}'"
                )

            testCase $"FullString hybrid multiple {nameof(|||)} on {nameof(OneOf)} and {nameof(Plain)}" <| fun _ ->
                let pattern =
                    OneOf "AD" ||| Plain "BE" ||| OneOf "CF" ||| Plain "HK"
                    |>  Convert.ToFullstringPattern

                let inputs = [ "A"; "BE"; "C"; "F"; "HK" ]

                inputs
                |>  List.iter(fun input ->
                    let subject = Regex.Match(input, pattern)
                    Expect.isTrue subject.Success $"Unsuccessful match on input '{input}'"
                    Expect.equal subject.Value input $"Match is not equal to input '{input}'"
                )


            testCase $"FullString {nameof(+)} Once" <| fun _ ->
                let pattern =
                    Plain "A" + Plain "B"
                    |>  Convert.ToFullstringPattern

                let inputs = [ "AB" ]

                inputs
                |>  List.iter(fun input ->
                    let subject = Regex.Match(input, pattern)
                    Expect.isTrue subject.Success $"Unsuccessful match on input '{input}'"
                    Expect.equal subject.Value input $"Match is not equal to input '{input}'"
                )

            testCase $"FullString multiple {nameof(+)}" <| fun _ ->
                let pattern =
                    Plain "A" + Plain "B" + Plain "C"
                    |>  Convert.ToFullstringPattern

                let inputs = [ "ABC" ]

                inputs
                |>  List.iter(fun input ->
                    let subject = Regex.Match(input, pattern)
                    Expect.isTrue subject.Success $"Unsuccessful match on input '{input}'"
                    Expect.equal subject.Value input $"Match is not equal to input '{input}'"
                )

        ]

    [<Tests>]
    let ``Sunny/Rainy day Tests`` =
        testList "Sunny/Rainy day Tests" [

            testCase $"FullString multiple {nameof(|||)} on {nameof(OneOf)} and {nameof(Between)}" <| fun _ ->
                let pattern =
                    OneOf "AD" ||| Between 'L' 'N'
                    |>  Convert.ToFullstringPattern

                let sunnyInputs = [ "A"; "D"; "L"; "M"; "N" ]
                let rainyInputs = [ "B"; "C"; "K"; "O" ]

                sunnyInputs
                |>  List.iter(fun input ->
                    let subject = Regex.Match(input, pattern)
                    Expect.isTrue subject.Success $"Unsuccessful match on input '{input}'"
                    Expect.equal subject.Value input $"Match is not equal to input '{input}'"
                )

                rainyInputs
                |>  List.iter(fun input ->
                    let subject = Regex.Match(input, pattern)
                    Expect.isFalse subject.Success $"Successful match on input '{input}'"
                )


            testCase $"FullString clean {nameof(Between)}" <| fun _ ->
                let pattern =
                    Between 'A' 'Z'
                    |>  Convert.ToFullstringPattern

                let sunnyInputs = [ "A"; "B"; "X"; "Z" ]
                let rainyInputs = [ "a"; "-"; "3"; "@" ]

                sunnyInputs
                |>  List.iter(fun input ->
                    let subject = Regex.Match(input, pattern)
                    Expect.isTrue subject.Success $"Unsuccessful match on input '{input}'"
                    Expect.equal subject.Value input $"Match is not equal to input '{input}'"
                )

                rainyInputs
                |>  List.iter(fun input ->
                    let subject = Regex.Match(input, pattern)
                    Expect.isFalse subject.Success $"Successful match on input '{input}'"
                )

            testCase $"FullString clean {nameof(NotBetween)}" <| fun _ ->
                let pattern =
                    (NotBetween 'A' 'Z')
                    |>  Convert.ToFullstringPattern

                let sunnyInputs = [ "a"; "-"; "3"; "@" ]
                let rainyInputs = [ "A"; "B"; "X"; "Z" ]

                sunnyInputs
                |>  List.iter(fun input ->
                    let subject = Regex.Match(input, pattern)
                    Expect.isTrue subject.Success $"Unsuccessful match on input '{input}'"
                    Expect.equal subject.Value input $"Match is not equal to input '{input}'"
                )

                rainyInputs
                |>  List.iter(fun input ->
                    let subject = Regex.Match(input, pattern)
                    Expect.isFalse subject.Success $"Successful match on input '{input}'"
                )

            testCase $"FullString clean {nameof(OneOf)} with {nameof(OneOf)} Subtraction" <| fun _ ->
                let pattern =
                    OneOf "ABCDEF" - OneOf "CD"
                    |>  Convert.ToFullstringPattern

                let sunnyInputs = [ "A"; "B"; "E"; "F" ]
                let rainyInputs = [ "C"; "D"; "G" ]

                sunnyInputs
                |>  List.iter(fun input ->
                    let subject = Regex.Match(input, pattern)
                    Expect.isTrue subject.Success $"Unsuccessful match on input '{input}'"
                    Expect.equal subject.Value input $"Match is not equal to input '{input}'"
                )

                rainyInputs
                |>  List.iter(fun input ->
                    let subject = Regex.Match(input, pattern)
                    Expect.isFalse subject.Success $"Successful match on input '{input}'"
                )


            testCase $"FullString clean {nameof(NotOneOf)} with {nameof(OneOf)} Subtraction" <| fun _ ->
                let pattern =
                    (NotOneOf "BCDE") - OneOf "FG"
                    |>  Convert.ToFullstringPattern

                let sunnyInputs = [ "A"; "H" ]
                let rainyInputs = [ "B"; "C"; "D"; "E"; "F"; "G" ]

                sunnyInputs
                |>  List.iter(fun input ->
                    let subject = Regex.Match(input, pattern)
                    Expect.isTrue subject.Success $"Unsuccessful match on input '{input}'"
                    Expect.equal subject.Value input $"Match is not equal to input '{input}'"
                )

                rainyInputs
                |>  List.iter(fun input ->
                    let subject = Regex.Match(input, pattern)
                    Expect.isFalse subject.Success $"Successful match on input '{input}'"
                )


            testCase $"FullString clean {nameof(Between)} with {nameof(OneOf)} Subtraction" <| fun _ ->
                let pattern =
                    Between 'A' 'F' - OneOf "CD"
                    |>  Convert.ToFullstringPattern

                let sunnyInputs = [ "A"; "B"; "E"; "F" ]
                let rainyInputs = [ "C"; "D"; "G" ]

                sunnyInputs
                |>  List.iter(fun input ->
                    let subject = Regex.Match(input, pattern)
                    Expect.isTrue subject.Success $"Unsuccessful match on input '{input}'"
                    Expect.equal subject.Value input $"Match is not equal to input '{input}'"
                )

                rainyInputs
                |>  List.iter(fun input ->
                    let subject = Regex.Match(input, pattern)
                    Expect.isFalse subject.Success $"Successful match on input '{input}'"
                )


            testCase $"FullString clean {nameof(NotBetween)} with {nameof(OneOf)} Subtraction" <| fun _ ->
                let pattern =
                    (NotBetween 'A' 'C') - OneOf "FG"
                    |>  Convert.ToFullstringPattern

                let sunnyInputs = [ "D"; "E"; "H" ]
                let rainyInputs = [ "A"; "B"; "C"; "F"; "G" ]

                sunnyInputs
                |>  List.iter(fun input ->
                    let subject = Regex.Match(input, pattern)
                    Expect.isTrue subject.Success $"Unsuccessful match on input '{input}'"
                    Expect.equal subject.Value input $"Match is not equal to input '{input}'"
                )

                rainyInputs
                |>  List.iter(fun input ->
                    let subject = Regex.Match(input, pattern)
                    Expect.isFalse subject.Success $"Successful match on input '{input}'"
                )


            testCase $"FullString clean {nameof(Between)} with {nameof(Between)} Subtraction" <| fun _ ->
                let pattern =
                    Between 'A' 'F' - Between 'C' 'D'
                    |>  Convert.ToFullstringPattern

                let sunnyInputs = [ "A"; "B"; "E"; "F" ]
                let rainyInputs = [ "C"; "D"; "G" ]

                sunnyInputs
                |>  List.iter(fun input ->
                    let subject = Regex.Match(input, pattern)
                    Expect.isTrue subject.Success $"Unsuccessful match on input '{input}'"
                    Expect.equal subject.Value input $"Match is not equal to input '{input}'"
                )

                rainyInputs
                |>  List.iter(fun input ->
                    let subject = Regex.Match(input, pattern)
                    Expect.isFalse subject.Success $"Successful match on input '{input}'"
                )


            testCase $"FullString clean {nameof(Between)} with {nameof(NotBetween)} Subtraction" <| fun _ ->
                let pattern =
                    (NotBetween 'B' 'F') - Between 'C' 'D'
                    |>  Convert.ToFullstringPattern

                let sunnyInputs = [ "A";  "G"]
                let rainyInputs = [ "B"; "C"; "D"; "E"; "F" ]

                sunnyInputs
                |>  List.iter(fun input ->
                    let subject = Regex.Match(input, pattern)
                    Expect.isTrue subject.Success $"Unsuccessful match on input '{input}'"
                    Expect.equal subject.Value input $"Match is not equal to input '{input}'"
                )

                rainyInputs
                |>  List.iter(fun input ->
                    let subject = Regex.Match(input, pattern)
                    Expect.isFalse subject.Success $"Successful match on input '{input}'"
                )


            testCase $"FullString clean {nameof(OneOf)} with {nameof(Between)} Subtraction" <| fun _ ->
                let pattern =
                    OneOf "ABCDEF" - Between 'C' 'D'
                    |>  Convert.ToFullstringPattern

                let sunnyInputs = [ "A"; "B"; "E"; "F" ]
                let rainyInputs = [ "C"; "D"; "G" ]

                sunnyInputs
                |>  List.iter(fun input ->
                    let subject = Regex.Match(input, pattern)
                    Expect.isTrue subject.Success $"Unsuccessful match on input '{input}'"
                    Expect.equal subject.Value input $"Match is not equal to input '{input}'"
                )

                rainyInputs
                |>  List.iter(fun input ->
                    let subject = Regex.Match(input, pattern)
                    Expect.isFalse subject.Success $"Successful match on input '{input}'"
                )


            testCase $"FullString clean {nameof(OneOf)} with {nameof(NotBetween)} Subtraction" <| fun _ ->
                let pattern =
                    (OneOf "ABCDEF") - NotBetween 'C' 'D'
                    |>  Convert.ToFullstringPattern

                let sunnyInputs = [ "C"; "D" ]
                let rainyInputs = [ "A"; "B"; "E"; "F" ]

                sunnyInputs
                |>  List.iter(fun input ->
                    let subject = Regex.Match(input, pattern)
                    Expect.isTrue subject.Success $"Unsuccessful match on input '{input}'"
                    Expect.equal subject.Value input $"Match is not equal to input '{input}'"
                )

                rainyInputs
                |>  List.iter(fun input ->
                    let subject = Regex.Match(input, pattern)
                    Expect.isFalse subject.Success $"Successful match on input '{input}'"
                )
        ]

    [<Tests>]
    let ``Exceptions Tests`` =
        testList "Exceptions Tests" [

            testCase $"FullString {nameof(OneOf)}/{nameof(Plain)} incorrect type for subtraction" <| fun _ ->
                Expect.throws(fun () ->OneOf "ABC" - Plain "B" |> ignore) "Plain cannot be subtracted"

        ]


    [<Tests>]
    let ``Repeating Patterns`` =
        testList "Repeating Patterns" [
            testCase $"FullString {nameof(ZeroOrMore)} Text" <| fun _ ->
                let pattern =
                    ZeroOrMore(Plain "A")
                    |>  Convert.ToFullstringPattern

                let inputs = [ ""; "A"; "AA"; "AAA" ]

                inputs
                |>  List.iter(fun input ->
                    let subject = Regex.Match(input, pattern)
                    Expect.isTrue subject.Success $"Unsuccessful match on input '{input}'"
                    Expect.equal subject.Value input $"Match is not equal to input '{input}'"
                )

            testCase $"FullString {nameof(OnceOrMore)} Text" <| fun _ ->
                let pattern =
                    OnceOrMore(Plain "A")
                    |>  Convert.ToFullstringPattern

                let inputs = [ "A"; "AA"; "AAA" ]

                inputs
                |>  List.iter(fun input ->
                    let subject = Regex.Match(input, pattern)
                    Expect.isTrue subject.Success $"Unsuccessful match on input '{input}'"
                    Expect.equal subject.Value input $"Match is not equal to input '{input}'"
                )


            testCase $"FullString {nameof(RepeatExact)} Text" <| fun _ ->
                let pattern =
                    RepeatExact 3 (Plain "A")
                    |>  Convert.ToFullstringPattern

                let inputs = [ "AAA" ]

                inputs
                |>  List.iter(fun input ->
                    let subject = Regex.Match(input, pattern)
                    Expect.isTrue subject.Success $"Unsuccessful match on input '{input}'"
                    Expect.equal subject.Value input $"Match is not equal to input '{input}'"
                )

            testCase $"FullString {nameof(RepeatRange)} Text" <| fun _ ->
                let pattern =
                    RepeatRange 2 3 (Plain "A")
                    |>  Convert.ToFullstringPattern

                let inputs = [ "AA"; "AAA" ]

                inputs
                |>  List.iter(fun input ->
                    let subject = Regex.Match(input, pattern)
                    Expect.isTrue subject.Success $"Unsuccessful match on input '{input}'"
                    Expect.equal subject.Value input $"Match is not equal to input '{input}'"
                )


            testCase $"FullString {nameof(Optional)} Text" <| fun _ ->
                let pattern =
                    Optional (Plain "A") + Plain "B"
                    |>  Convert.ToFullstringPattern

                let inputs = [ "AB"; "B" ]

                inputs
                |>  List.iter(fun input ->
                    let subject = Regex.Match(input, pattern)
                    Expect.isTrue subject.Success $"Unsuccessful match on input '{input}'"
                    Expect.equal subject.Value input $"Match is not equal to input '{input}'"
                )
        ]

    [<Tests>]
    let ``Grouping Patterns`` =
        testList "Grouping Patterns" [
            testCase $"FullString {nameof(ZeroOrMore)} Text" <| fun _ ->
                let pattern =
                    ZeroOrMore(Plain "A")
                    |>  Convert.ToFullstringPattern

                let inputs = [ ""; "A"; "AA"; "AAA" ]

                inputs
                |>  List.iter(fun input ->
                    let subject = Regex.Match(input, pattern)
                    Expect.isTrue subject.Success $"Unsuccessful match on input '{input}'"
                    Expect.equal subject.Value input $"Match is not equal to input '{input}'"
                )
        ]

    [<Tests>]
    let ``Escaped characters`` =
        testList "Escaped characters" [
            testCase $"FullString '$' Text" <| fun _ ->
                let pattern =
                    Plain "$"
                    |>  Convert.ToFullstringPattern

                let inputs = [ "$" ]

                inputs
                |>  List.iter(fun input ->
                    let subject = Regex.Match(input, pattern)
                    Expect.isTrue subject.Success $"Unsuccessful match on input '{input}'"
                    Expect.equal subject.Value input $"Match is not equal to input '{input}'"
                )
        ]
