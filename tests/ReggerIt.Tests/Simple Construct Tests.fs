namespace ReggerIt.Simple.Construct.Tests

open System
open Expecto
open ReggerIt
open System.Text.RegularExpressions
open ReggerIt


module ``ReggerIt Tests`` =
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


            testCase $"FullString {nameof(Not)}/{nameof(Plain)} Text - rainy day" <| fun _ ->
                Expect.throws(fun () -> Not (Plain "A") |> ignore) "Plain cannot be negated"


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


            testCase $"FullString {nameof(Not)}/{nameof(OneOf)} Text - sunny day" <| fun _ ->
                let pattern =
                    Plain "X" + Not (OneOf "ABC") + Plain "X"
                    |>  Convert.ToFullstringPattern

                let inputs = [ "XEX"; "XFX"; "XGX" ]

                inputs
                |>  List.iter(fun input ->
                    let subject = Regex.Match(input, pattern)
                    Expect.isTrue subject.Success $"Unsuccessful match on input '{input}'"
                    Expect.equal subject.Value input $"Match is not equal to input '{input}'"
                )

            testCase $"FullString {nameof(Not)}/{nameof(OneOf)} Text - rainy day" <| fun _ ->
                let pattern =
                    Plain "X" + Not (OneOf "ABC") + Plain "X"
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
