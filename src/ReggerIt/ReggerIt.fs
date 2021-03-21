module ReggerIt

#nowarn "52" // "value has been copied to ensure the original is Not mutated"

open System.Diagnostics
open System.Text.RegularExpressions
open System.Text
open System
open System.Globalization
open System.Text.RegularExpressions


let private escaped (s:string) =
    [
        ("\\","\\\\")
        ("{","\{")
        ("}","\}")
        ("[","\[")
        ("]","\]")
        ("(","\(")
        (")","\)")
        ("|","\|")
        ("+","\+")
        ("*","\*")
        (".","\.")
        ("?","\?")
        ("$","\$")
    ]
    |>  List.fold(fun (st:string) (sch,rp) -> st.Replace(sch, rp) ) s


type ConstType =
    private {
        Text    : string
    }
    override this.ToString() =
        sprintf "%s" this.Text

    static member (+) (r1:ConstType, r2:ConstType) =
        let appd = r1.Text + r2.Text
        { Text = appd }

    static member Create r = { Text = r }


type PlainType =
    private {
        Text    : string
    }
    override this.ToString() = sprintf "%s" (escaped this.Text)

    static member (+) (r1:PlainType, r2:PlainType) =
        let appd = r1.Text + r2.Text
        { Text = appd }

    static member Create r = { Text = r }


and OneInSetCharacterType =
    |   CharacterString of bool * string
    |   CharacterRange  of bool * char * char

and OneInSetType =
    private {
        CharacterClass  : OneInSetCharacterType list
    }

    static member Optimize (ms:string) =
        ms.ToCharArray()
        |>  Array.distinct
        |>  fun ca -> new string(ca)


    static member TermToString (trm:OneInSetCharacterType) =
        let pfx r = if r then "^" else ""
        match trm with
        |   CharacterString (inv,cs)   -> sprintf "%s%s" (pfx inv) cs
        |   CharacterRange  (inv,s, e) -> sprintf "%s%c-%c" (pfx inv) s e



    override this.ToString() =
        let rec loop lst =
            match lst with
            |   []  -> failwith "Empty character class - this should never happen."
            |   [i] -> sprintf "[%s]" (OneInSetType.TermToString i)
            |   curr :: rest ->
                let subtract = loop rest
                sprintf "[%s-%s]" (OneInSetType.TermToString curr) subtract

        loop this.CharacterClass


    static member (-) (r1:OneInSetType, r2:OneInSetType) =
        { CharacterClass = r1.CharacterClass @ r2.CharacterClass }


    static member OrOptimized (r1:OneInSetType, r2:OneInSetType) =
        match (r1.CharacterClass, r2.CharacterClass) with
        |   ([CharacterString (b1,m1)], [CharacterString (b2,m2)]) ->
            if b1 = b2 then
               Some ({ CharacterClass = [CharacterString (b1, m1 + m2)]})
            else
                None
        |   _ -> None


    static member Create r = {CharacterClass = [r]}


and RexPatt =
    |   Const of ConstType
    |   Plain of PlainType
    |   OneInSet   of OneInSetType
    |   Or         of RexPatt list
    |   Concat     of RexPatt list
    |   IterRange  of RexPatt * int * (int option)
    |   ZeroOrMore of RexPatt
    |   ZeroOrMoreNonGreedy of RexPatt
    |   OneOrMore  of RexPatt
    |   OneOrMoreNonGreedy  of RexPatt
    |   Optional   of RexPatt
    |   Group      of RexPatt
    |   NamedGroup of string * RexPatt

    override this.ToString() =
        let str =
            match this with
            |   Const    c -> c.ToString()
            |   Plain    r -> r.ToString()
            |   OneInSet r -> r.ToString()
            |   Or       l ->
                    let l = l |> List.rev
                    let body = l.Tail |> List.fold(fun s e -> sprintf "(?:%s)|%O" s e) (sprintf "%O" l.Head)
                    sprintf "(?:%s)" body
            |   Concat   l ->
                    let l = l |> List.rev
                    l.Tail |> List.fold(fun s e -> sprintf "(?:%s)%O" s e) (sprintf "%O" l.Head)
            |   IterRange(t,mx,mno) ->
                    match mno with
                    |   Some(mn) ->  sprintf "(?:%O){%d,%d}" t mn mx
                    |   None     ->  sprintf "(?:%O){%d}" t mx
            |   ZeroOrMore t -> sprintf "(?:%O)*" t
            |   ZeroOrMoreNonGreedy t -> sprintf "(?:%O)*?" t
            |   OneOrMore  t -> sprintf "(?:%O)+" t
            |   OneOrMoreNonGreedy  t -> sprintf "(?:%O)+?" t
            |   Optional   t -> sprintf "(?:%O)?" t
            |   Group      t -> sprintf "(%O)" t
            |   NamedGroup (s,t) -> sprintf "(?<%s>%O)" s t
        str


    static member private DoConcat (r1:RexPatt, r2:RexPatt) =
        match (r1,r2) with
        |   (Concat c1, _) -> Concat(r2 :: c1)
        |   _   -> Concat([r2; r1])


    static member internal DoOr (r1:RexPatt, r2:RexPatt ) =
            match r1 with
            | Or     l ->   Or(r2 :: l)
            | _       ->    Or([r2; r1])


    static member (|||) (r1:RexPatt, r2:RexPatt) =
        match (r1,r2) with
        |   (OneInSet o1, OneInSet o2) ->
            OneInSetType.OrOptimized(o1, o2)
            |>  function
                |   Some rs -> OneInSet rs
                |   None    -> RexPatt.DoOr(r1, r2)
        |   _ -> RexPatt.DoOr(r1, r2)


    static member (-) (r1:RexPatt, r2:RexPatt) =
        match (r1,r2) with
        |   (OneInSet o1, OneInSet o2)  -> OneInSet(o1 - o2)
        |   _   -> failwith "Illegal subtraction, you can only subtract two OneOf sets"

    static member (+) (r1:RexPatt, r2:RexPatt) =
        match (r1,r2) with
        |   (Plain p1   , Plain p2)      -> Plain(p1 + p2)
        |   (Const c1   , Const c2)      -> Const(c1 + c2)
        |   _   ->  RexPatt.DoConcat(r1, r2)



/// Regex pattern must repeat exactly given value
let RepeatExact mx t = IterRange(t, mx, None)

/// Regex pattern may repeat within given range
let RepeatRange mn mx t = IterRange(t, mx, Some(mn))

/// Regex pattern may repeat zero or more
let ZeroOrMore t = ZeroOrMore(t)

/// Regex pattern may repeat zero or more - non greedy
let ZeroOrMoreNonGreedy t = ZeroOrMoreNonGreedy(t)

/// Regex pattern may repeat once or more
let OnceOrMore(t) = OneOrMore(t)

/// Regex pattern may repeat once or more - non greedy
let OnceOrMoreNonGreedy(t) = OneOrMoreNonGreedy(t)

/// Make Regex optional
let Optional(t) = Optional(t)

/// Plain regex pattern
let Plain c = Plain(PlainType.Create c)

/// One in Set regex pattern
let OneOf c = OneInSet(OneInSetType.Create (CharacterString (false, c)))

/// Not One in Set regex pattern
let NotOneOf c = OneInSet(OneInSetType.Create (CharacterString(true, c)))

/// Between - character range
let Between s e = OneInSet(OneInSetType.Create (CharacterRange (false, s,e)))

/// Not Between - character range
let NotBetween s e = OneInSet(OneInSetType.Create (CharacterRange (true, s,e)))

/// Creates Regex group
let Group p = Group(p)

/// Creates Regex named group
let NamedGroup s p = NamedGroup(s,p)

module Convert =
    /// Regex ToString - match from string start
    let ToStringStartPattern (p:RexPatt) = sprintf "\\A(%O)" p

    /// Regex ToString - full string match
    let ToFullstringPattern (p:RexPatt) = sprintf "\\A(%O)\\z" p

    /// Regex ToString - match anywhere in the string
    let ToPattern (p:RexPatt) = sprintf "(%O)" p


module Macro =
    let digit = Between 'A' 'F' ||| Between '0' '9'
    let validate input pattern errMessage =
        let mv = Regex.Match(input, (pattern |> Convert.ToFullstringPattern), RegexOptions.IgnoreCase)
        if mv.Success then
            mv.Value
        else
            failwith <| sprintf "Illegal input: '%s', %s" input errMessage

    /// Any character except newline
    let any = Const { Text = "."}

    /// Any whitespace character
    let whitespace = Const { Text = @"\s"}

    /// Any non-whitespace character
    let nonWhitespace = Const { Text = @"\S"}

    /// Bell character
    let bell = Const { Text = @"\a"}

    /// Backspace character
    let backspace = Const { Text = @"\u0008"}

    /// Tab character
    let tab = Const { Text = @"\t" }

    /// Carriage return character
    let carriageReturn = Const { Text = @"\r" }

    /// Vertical tab character
    let verticalTab = Const { Text = @"\v" }

    /// Form feed character
    let formFeed = Const { Text = @"\f" }

    /// Newline character
    let newLine = Const { Text = @"\n" }

    /// Escape character
    let escape = Const { Text = @"\a" }

    /// ASCII character, input is 8-bit hex, ie "00", "AF", "FF"
    let ascii (s:string) =
        let pattern =  digit + digit
        let hex = validate s pattern "expected hex-byte input, ie \"00\", \"AF\", \"FF\""
        Const { Text = sprintf @"\x%s" hex}

    /// UTF-16 character, input is 16-bit hex, ie "0000", "5AAF", "C0FF"
    let utf16 (s:string) =
        let pattern =  digit + digit + digit + digit
        let hex = validate s pattern "expected hex-word input, ie \"0000\", \"5AAF\", \"C0FF\""
        Const { Text = sprintf @"\u%s" hex}

    /// Word character
    let wordCharacter = Const { Text = @"\w" }

    /// Non Word character
    let nonWordCharacter = Const { Text = @"\W" }

    /// Decimal digit
    let decimalDigit = Const { Text = @"\d" }

    /// Non-decimal digit
    let nonDecimalDigit = Const { Text = @"\D" }

    /// Named character range
    let namedBlock n = Const { Text = sprintf @"\p{%s}" n}

