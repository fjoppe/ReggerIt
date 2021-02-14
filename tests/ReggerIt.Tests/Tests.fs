namespace ReggerIt.Tests

open System
open Expecto
open ReggerIt
open System.Text.RegularExpressions

module ``ReggerIt Tests`` =
    [<Tests>]
    let ``Simple Text patterns`` =
        testList "Simple Text Patterns" [
            testCase $"FullString {nameof(ReggerIt.Plain)} Text" <| fun _ ->
                let pattern =
                    ReggerIt.Plain "AAB"
                    |>  ReggerIt.ToFullstringPattern

                let inputs = [ "AAB" ]

                inputs
                |>  List.iter(fun input ->
                    let subject = Regex.Match(input, pattern)
                    Expect.isTrue subject.Success "Successful match"
                    Expect.equal subject.Value input "Simple plain "
                )

            testCase $"FullString Concatenated {nameof(ReggerIt.Plain)} Text" <| fun _ ->
                let pattern =
                    ReggerIt.Plain "AB"
                    +   ReggerIt.Plain "CD"
                    |>  ReggerIt.ToFullstringPattern

                let inputs = [ "ABCB" ]

                inputs
                |>  List.iter(fun input ->
                    let subject = Regex.Match(input, pattern)
                    Expect.isTrue subject.Success "Successful match"
                    Expect.equal subject.Value input "Simple plain "
                )

            testCase $"FullString {nameof(ReggerIt.OneOf)} Text" <| fun _ ->
                let pattern =
                    ReggerIt.OneOf "ABC"
                    |>  ReggerIt.ToFullstringPattern

                let inputs = [ "A"; "B"; "C" ]

                inputs
                |>  List.iter(fun input ->
                    let subject = Regex.Match(input, pattern)
                    Expect.isTrue subject.Success "Successful match"
                    Expect.equal subject.Value input "OneOf Text"
                )

            testCase $"FullString concatenated {nameof(ReggerIt.OneOf)} Text" <| fun _ ->
                let pattern =
                    ReggerIt.OneOf "ABC"
                    +   ReggerIt.OneOf "DEF"
                    |>  ReggerIt.ToFullstringPattern

                let inputs = [ "AD"; "BE"; "CF" ]

                inputs
                |>  List.iter(fun input ->
                    let subject = Regex.Match(input, pattern)
                    Expect.isTrue subject.Success "Successful match"
                    Expect.equal subject.Value input "OneOf text "
                )

            testCase $"FullString Mixed Concatenated {nameof(ReggerIt.Plain)} and {nameof(ReggerIt.OneOf)} Text" <| fun _ ->
                let pattern =
                    ReggerIt.Plain "Test"
                    +   ReggerIt.OneOf "ABC"
                    +   ReggerIt.Plain "After"
                    |>  ReggerIt.ToFullstringPattern

                let inputs = [ "TestAAfter"; "TestBAfter"; "TestCAfter" ]

                inputs
                |>  List.iter(fun input ->
                    let subject = Regex.Match(input, pattern)
                    Expect.isTrue subject.Success "Successful match"
                    Expect.equal subject.Value input "Mixed text patterns "
                )

        ]

    [<Tests>]
    let ``Repeating Patterns`` =
        testList "Repeating Patterns" [
            testCase $"FullString {nameof(ReggerIt.ZeroOrMore)} Text" <| fun _ ->
                let pattern =
                    ReggerIt.Plain "A"
                    |>  ReggerIt.ZeroOrMore
                    |>  ReggerIt.ToFullstringPattern

                let inputs = [ ""; "A"; "AA"; "AAA" ]

                inputs
                |>  List.iter(fun input ->
                    let subject = Regex.Match(input, pattern)
                    Expect.isTrue subject.Success "Successful match"
                    Expect.equal subject.Value input "Zero or more "
                )

            testCase $"FullString {nameof(ReggerIt.OnceOrMore)} Text" <| fun _ ->
                let pattern =
                    ReggerIt.Plain "A"
                    |>  ReggerIt.OnceOrMore
                    |>  ReggerIt.ToFullstringPattern

                let inputs = [ "A"; "AA"; "AAA" ]

                inputs
                |>  List.iter(fun input ->
                    let subject = Regex.Match(input, pattern)
                    Expect.isTrue subject.Success "Successful match"
                    Expect.equal subject.Value input "One or more "
                )


            testCase $"FullString {nameof(ReggerIt.RepeatExact)} Text" <| fun _ ->
                let pattern =
                    ReggerIt.Plain "A"
                    |>  ReggerIt.RepeatExact 3
                    |>  ReggerIt.ToFullstringPattern

                let inputs = [ "AAA" ]

                inputs
                |>  List.iter(fun input ->
                    let subject = Regex.Match(input, pattern)
                    Expect.isTrue subject.Success "Successful match"
                    Expect.equal subject.Value input "Repeat exact"
                )

            testCase $"FullString {nameof(ReggerIt.RepeatRange)} Text" <| fun _ ->
                let pattern =
                    ReggerIt.Plain "A"
                    |>  ReggerIt.RepeatRange 2 3
                    |>  ReggerIt.ToFullstringPattern

                let inputs = [ "AA"; "AAA" ]

                inputs
                |>  List.iter(fun input ->
                    let subject = Regex.Match(input, pattern)
                    Expect.isTrue subject.Success "Successful match"
                    Expect.equal subject.Value input "Repeat Range "
                )
        ]

    [<Tests>]
    let ``Grouping Patterns`` =
        testList "Repeating Patterns" [
            testCase $"FullString {nameof(ReggerIt.ZeroOrMore)} Text" <| fun _ ->
                let pattern =
                    ReggerIt.Plain "A"
                    |>  ReggerIt.ZeroOrMore
                    |>  ReggerIt.ToFullstringPattern

                let inputs = [ ""; "A"; "AA"; "AAA" ]

                inputs
                |>  List.iter(fun input ->
                    let subject = Regex.Match(input, pattern)
                    Expect.isTrue subject.Success "Successful match"
                    Expect.equal subject.Value input "Zero or more "
                )
        ]
