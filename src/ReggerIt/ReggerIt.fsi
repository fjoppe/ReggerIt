module ReggerIt

open System.Text.RegularExpressions

/// Contains a Regular Expression Pattern
type RexPatt
type RexPatt
with
    /// Match either left or right pattern
    static member (|||) : RexPatt * RexPatt -> RexPatt

    /// Concatenate two sub-patterns
    static member (+) : RexPatt * RexPatt -> RexPatt

    /// Subtract two Character Classes - Only works on ``OneOf`` and ``Between`` constructs
    static member (-) : RexPatt * RexPatt -> RexPatt


/// Regex pattern must repeat exactly given value
val RepeatExact :  int -> RexPatt -> RexPatt

/// Regex pattern may repeat within given range
val RepeatRange : int -> int -> RexPatt -> RexPatt

/// Regex pattern may repeat zero or more
val ZeroOrMore : RexPatt -> RexPatt

/// Regex pattern may repeat zero or more - non greedy
val ZeroOrMoreNonGreedy : RexPatt -> RexPatt

/// Regex pattern may repeat once or more
val OnceOrMore : RexPatt -> RexPatt

/// Regex pattern may repeat once or more - non greedy
val OnceOrMoreNonGreedy : RexPatt -> RexPatt

/// Make Regex optional
val Optional : RexPatt -> RexPatt

/// Plain regex pattern
val Plain : string -> RexPatt

/// One in Set regex pattern
val OneOf : string -> RexPatt

/// Not One in Set regex pattern
val NotOneOf : string -> RexPatt

/// Between start and end character range
val Between : char -> char -> RexPatt

/// Not Between start and end character range
val NotBetween : char -> char -> RexPatt

/// Creates Regex group
val Group : RexPatt -> RexPatt

/// Creates Regex named group
val NamedGroup : string -> RexPatt -> RexPatt


/// Convert to pattern string
module Convert =
    /// Regex ToString - match from string start
    val ToStringStartPattern : RexPatt -> string

    /// Regex ToString - full string match
    val ToFullstringPattern : RexPatt -> string

    /// Regex ToString - match anywhere in the string
    val ToPattern : RexPatt -> string


/// Predefined macro's
module Macro =
    /// Any character except newline
    val any : RexPatt

    /// Any whitespace character
    val whitespace : RexPatt

    /// Any non-whitespace character
    val nonWhitespace : RexPatt

    /// Bell character
    val bell : RexPatt

    /// Backspace character
    val backspace : RexPatt

    /// Tab character
    val tab : RexPatt

    /// Carriage return character
    val carriageReturn : RexPatt

    /// Vertical tab character
    val verticalTab : RexPatt

    /// Form feed character
    val formFeed : RexPatt

    /// Newline character
    val newLine : RexPatt

    /// Escape character
    val escape : RexPatt

    /// ASCII character, input is 8-bit hex, ie "00", "AF", "FF"
    val ascii : string  -> RexPatt

    /// UTF-16 character, input is 16-bit hex, ie "0000", "5AAF", "C0FF"
    val utf16 : string -> RexPatt

    /// Word character
    val wordCharacter : RexPatt

    /// Non Word character
    val nonWordCharacter : RexPatt

    /// Decimal digit
    val decimalDigit : RexPatt

    /// Non-decimal digit
    val nonDecimalDigit : RexPatt

    /// Named character range
    val namedBlock : string -> RexPatt
