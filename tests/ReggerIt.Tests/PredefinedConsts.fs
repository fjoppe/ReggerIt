namespace ReggerIt.Predefined.Construct.Tests

open System
open Expecto
open ReggerIt
open System.Text.RegularExpressions
open ReggerIt


module ``Predefined Macro Character Sets Tests`` =
    [<Tests>]
    let ``Macro Character Tests`` =
        testList "Macro Character Tests" [
            testCase $"FullString {nameof(Macro.any)} Text" <| fun _ ->
                let pattern =
                    Plain "A" + Macro.any + Plain "A"
                    |>  Convert.ToFullstringPattern

                let inputs = [
                    "AXA"
                    "ADA"
                    "AHA"
                    "AKA"
                ]

                inputs
                |>  List.iter(fun input ->
                    let subject = Regex.Match(input, pattern)
                    Expect.isTrue subject.Success $"Unsuccessful match on input '{input}'"
                    Expect.equal subject.Value input $"Match is not equal to input '{input}'"
                )

            testCase $"FullString {nameof(Macro.whitespace)} Text" <| fun _ ->
                let pattern =
                    Plain "A" + Macro.whitespace + Plain "A"
                    |>  Convert.ToFullstringPattern

                let inputs = [
                    "A A"
                    "A\tA"
                    "A\vA"
                ]

                inputs
                |>  List.iter(fun input ->
                    let subject = Regex.Match(input, pattern)
                    Expect.isTrue subject.Success $"Unsuccessful match on input '{input}'"
                    Expect.equal subject.Value input $"Match is not equal to input '{input}'"
                )

            testCase $"FullString {nameof(Macro.nonWhitespace)} Text" <| fun _ ->
                let pattern =
                    Plain "A" + Macro.nonWhitespace + Plain "A"
                    |>  Convert.ToFullstringPattern

                let inputs = [
                    "A9A"
                    "ARA"
                    "A\bA"
                ]

                inputs
                |>  List.iter(fun input ->
                    let subject = Regex.Match(input, pattern)
                    Expect.isTrue subject.Success $"Unsuccessful match on input '{input}'"
                    Expect.equal subject.Value input $"Match is not equal to input '{input}'"
                )

            testCase $"FullString {nameof(Macro.bell)} Text" <| fun _ ->
                let pattern =
                    Plain "A" + Macro.bell + Plain "A"
                    |>  Convert.ToFullstringPattern

                let inputs = [
                    "A\aA"
                ]

                inputs
                |>  List.iter(fun input ->
                    let subject = Regex.Match(input, pattern)
                    Expect.isTrue subject.Success $"Unsuccessful match on input '{input}'"
                    Expect.equal subject.Value input $"Match is not equal to input '{input}'"
                )


            testCase $"FullString {nameof(Macro.backspace)} Text" <| fun _ ->
                let pattern =
                    Plain "A" + Macro.backspace + Plain "A"
                    |>  Convert.ToFullstringPattern

                let inputs = [
                    "A\bA"
                ]

                inputs
                |>  List.iter(fun input ->
                    let subject = Regex.Match(input, pattern)
                    Expect.isTrue subject.Success $"Unsuccessful match on input '{input}'"
                    Expect.equal subject.Value input $"Match is not equal to input '{input}'"
                )

            testCase $"FullString {nameof(Macro.tab)} Text" <| fun _ ->
                let pattern =
                    Plain "A" + Macro.tab + Plain "A"
                    |>  Convert.ToFullstringPattern

                let inputs = [
                    "A\tA"
                ]

                inputs
                |>  List.iter(fun input ->
                    let subject = Regex.Match(input, pattern)
                    Expect.isTrue subject.Success $"Unsuccessful match on input '{input}'"
                    Expect.equal subject.Value input $"Match is not equal to input '{input}'"
                )

            testCase $"FullString {nameof(Macro.carriageReturn)} Text" <| fun _ ->
                let pattern =
                    Plain "A" + Macro.carriageReturn + Plain "A"
                    |>  Convert.ToFullstringPattern

                let inputs = [
                    "A\rA"
                ]

                inputs
                |>  List.iter(fun input ->
                    let subject = Regex.Match(input, pattern)
                    Expect.isTrue subject.Success $"Unsuccessful match on input '{input}'"
                    Expect.equal subject.Value input $"Match is not equal to input '{input}'"
                )

            testCase $"FullString {nameof(Macro.verticalTab)} Text" <| fun _ ->
                let pattern =
                    Plain "A" + Macro.verticalTab + Plain "A"
                    |>  Convert.ToFullstringPattern

                let inputs = [
                    "A\vA"
                ]

                inputs
                |>  List.iter(fun input ->
                    let subject = Regex.Match(input, pattern)
                    Expect.isTrue subject.Success $"Unsuccessful match on input '{input}'"
                    Expect.equal subject.Value input $"Match is not equal to input '{input}'"
                )

            testCase $"FullString {nameof(Macro.formFeed)} Text" <| fun _ ->
                let pattern =
                    Plain "A" + Macro.formFeed + Plain "A"
                    |>  Convert.ToFullstringPattern

                let inputs = [
                    "A\u000CA"
                ]

                inputs
                |>  List.iter(fun input ->
                    let subject = Regex.Match(input, pattern)
                    Expect.isTrue subject.Success $"Unsuccessful match on input '{input}'"
                    Expect.equal subject.Value input $"Match is not equal to input '{input}'"
                )

            testCase $"FullString {nameof(Macro.newLine)} Text" <| fun _ ->
                let pattern =
                    Plain "A" + Macro.newLine + Plain "A"
                    |>  Convert.ToFullstringPattern

                let inputs = [
                    "A\nA"
                ]

                inputs
                |>  List.iter(fun input ->
                    let subject = Regex.Match(input, pattern, RegexOptions.Multiline)
                    Expect.isTrue subject.Success $"Unsuccessful match on input '{input}'"
                    Expect.equal subject.Value input $"Match is not equal to input '{input}'"
                )


            testCase $"FullString {nameof(Macro.ascii)} Text" <| fun _ ->
                let pattern =
                    Plain "A" + Macro.ascii "40" + Plain "A"
                    |>  Convert.ToFullstringPattern

                let inputs = [
                    "A@A"
                ]

                inputs
                |>  List.iter(fun input ->
                    let subject = Regex.Match(input, pattern)
                    Expect.isTrue subject.Success $"Unsuccessful match on input '{input}'"
                    Expect.equal subject.Value input $"Match is not equal to input '{input}'"
                )

            testCase $"FullString {nameof(Macro.utf16)} Text" <| fun _ ->
                let pattern =
                    Plain "A" + Macro.utf16 "0040" + Plain "A"
                    |>  Convert.ToFullstringPattern

                let inputs = [
                    "A@A"
                ]

                inputs
                |>  List.iter(fun input ->
                    let subject = Regex.Match(input, pattern)
                    Expect.isTrue subject.Success $"Unsuccessful match on input '{input}'"
                    Expect.equal subject.Value input $"Match is not equal to input '{input}'"
                )
        ]
