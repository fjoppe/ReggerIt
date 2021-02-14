module ReggerIt

open System.Text.RegularExpressions

/// Contains a Regular Expression Pattern
type RexPatt
type RexPatt
    with
        static member (|||) : RexPatt * RexPatt -> RexPatt
        static member (+) : RexPatt * RexPatt -> RexPatt
        static member (-) : RexPatt * RexPatt -> RexPatt


/// Regex pattern must repeat exactly given value
val RepeatExact :  int -> RexPatt -> RexPatt

/// Regex pattern may repeat within given range
val RepeatRange : int -> int -> RexPatt -> RexPatt

/// Regex pattern may repeat zero or more
val ZeroOrMore : RexPatt -> RexPatt

/// Regex pattern may repeat once or more
val OnceOrMore : RexPatt -> RexPatt

/// Make Regex optional
val Optional : RexPatt -> RexPatt

/// Plain regex pattern
val Plain : string -> RexPatt 

/// One in Set regex pattern
val OneOf : string -> RexPatt

/// Exclude Set regex pattern
val Not : RexPatt -> RexPatt

/// Creates Regex group
val Group : RexPatt -> RexPatt

/// Creates Regex named group
val NamedGroup : string -> RexPatt -> RexPatt

/// Regex ToString - match from string start
val ToStringStartPattern : RexPatt -> string

/// Regex ToString - full string match
val ToFullstringPattern : RexPatt -> string

/// Regex ToString - match anywhere in the string 
val ToPattern : RexPatt -> string


