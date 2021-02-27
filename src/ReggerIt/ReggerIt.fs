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
    |   CharacterString of string
    |   CharacterRange  of char * char

and OneInSetType =
    private {
        Invert      : bool
        Mainset     : OneInSetCharacterType
        Subtractset : OneInSetCharacterType option
    }

    static member Optimize (ms:string) =
        ms.ToCharArray()
        |>  Array.distinct
        |>  fun ca -> new string(ca)

    override this.ToString() =
        match (this.Mainset, this.Subtractset, this.Invert) with
        //  https://msdn.microsoft.com/en-us/library/20bw873z(v=vs.110).aspx#Anchor_13
        |   (CharacterString ms, Some(CharacterString sb), true) ->
            sprintf "[^%s-[%s]]" (escaped ms |> OneInSetType.Optimize) (escaped sb |> OneInSetType.Optimize)
        |   (CharacterString ms, Some(CharacterString sb), false) ->
            sprintf "[%s-[%s]]" (escaped ms |> OneInSetType.Optimize) (escaped sb |> OneInSetType.Optimize)
        |   (CharacterString ms, Some(CharacterRange (ss,se)), true) ->
            sprintf "[^%s-[%c-%c]]" (escaped ms |> OneInSetType.Optimize) ss se
        |   (CharacterString ms, Some(CharacterRange (ss,se)), false) ->
            sprintf "[%s-[%c-%c]]" (escaped ms |> OneInSetType.Optimize) ss se
        |   (CharacterRange (ms,me),  Some(CharacterRange (ss,se)), true) ->
            sprintf "[^%c-%c-[%c-%c]]"  ms me ss se
        |   (CharacterRange (ms,me),  Some(CharacterRange (ss,se)), false) ->
            sprintf "[%c-%c-[%c-%c]]"  ms me ss se
        |   (CharacterRange (ms,me), Some(CharacterString sb), true) ->
            sprintf "[^%c-%c-[%s]]" ms me (escaped sb |> OneInSetType.Optimize)
        |   (CharacterRange (ms,me), Some(CharacterString sb), false) ->
            sprintf "[%c-%c-[%s]]" ms me (escaped sb |> OneInSetType.Optimize)
        |   (CharacterString ms, None, true) ->
            sprintf "[^%s]" (escaped ms |> OneInSetType.Optimize)
        |   (CharacterString ms, None, false) ->
            sprintf "[%s]"  (escaped ms |> OneInSetType.Optimize)
        |   (CharacterRange (ms,me),  None, true) ->
            sprintf "[^%c-%c]" ms me
        |   (CharacterRange (ms,me),  None, false) ->
            sprintf "[%c-%c]" ms me


    static member (-) (r1:OneInSetType, r2:OneInSetType) =
        match (r1.Mainset, r1.Subtractset, r2.Mainset, r2.Subtractset) with
        |   (_, Some _, _, _)
        |   (_, _, _, Some _) -> failwith "Can only subtract clean OneOf values - neither can contain previous subtractions."
        |   (CharacterString m1, None, CharacterString m2, None) ->
            {Mainset = CharacterString m1; Subtractset = Some (CharacterString m2); Invert = r1.Invert }
        |   (CharacterString m1, None, CharacterRange(ms2, me2), None) ->
            {Mainset = CharacterString m1; Subtractset = Some (CharacterRange (ms2, me2)); Invert = r1.Invert }
        |   (CharacterRange(ms1, me1), None, CharacterRange(ms2, me2), None) ->
            {Mainset = CharacterRange (ms1, me1); Subtractset = Some (CharacterRange(ms2, me2)); Invert = r1.Invert }
        |   (CharacterRange(ms1, me1), None, CharacterString m2, None) ->
            {Mainset = CharacterRange(ms1, me1); Subtractset = Some (CharacterString m2); Invert = r1.Invert }


    static member (|||) (r1:OneInSetType, r2:OneInSetType) =
        match (r1.Mainset, r1.Subtractset, r2.Mainset, r2.Subtractset) with
        |   (_, Some _, _, _)
        |   (_, _, _, Some _) -> failwith "Can only OR clean OneOf values - neither can contain previous subtractions."
        |   (CharacterString m1, None, CharacterString m2, None) ->
            {Mainset = CharacterString m1; Subtractset = Some (CharacterString m2); Invert = r1.Invert }
        |   (CharacterString m1, None, CharacterRange(ms2, me2), None) ->
            {Mainset = CharacterString m1; Subtractset = Some (CharacterRange(ms2, me2)); Invert = r1.Invert }
        |   (CharacterRange(ms1, me1), None, CharacterString m2, None) ->
            {Mainset = CharacterRange(ms1, me1); Subtractset = Some (CharacterString m2); Invert = r1.Invert }
        |   (CharacterRange(ms1, me1), None, CharacterRange(ms2, me2), None) ->
            {Mainset = CharacterRange(ms1, me1); Subtractset = Some (CharacterRange(ms2, me2)); Invert = r1.Invert }


    static member Create r = {Mainset= r; Subtractset = None; Invert = false}

    member this.Not() =
        {this with Invert = true}


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
            |   NamedGroup (s,t) -> sprintf "(&<%s>%O)" s t
        str


    static member private DoConcat (r1:RexPatt, r2:RexPatt) =
        match (r1,r2) with
        |   (Concat c1, _) -> Concat(r2 :: c1)
        |   _   -> Concat([r2; r1])


    static member private DoOr (r1:RexPatt, r2:RexPatt ) =
            match r1 with
            | Or     l ->   Or(r2 :: l)
            | _       ->    Or([r2; r1])


    static member (|||) (r1:RexPatt, r2:RexPatt) =
        match (r1,r2) with
        |   (OneInSet o1, OneInSet o2)   -> OneInSet(o1 ||| o2)
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

    member this.Not =
        match this with
        |   OneInSet o1 ->  OneInSet(o1.Not())
        |   _   -> failwith "Unsupported Not-case"


/// Regex pattern must repeat exactly given value
let RepeatExact mx t = IterRange(t, mx, None)

/// Regex pattern may repeat within given range
let RepeatRange mn mx t = IterRange(t, mx, Some(mn))

/// Regex pattern may repeat zero or more
let ZeroOrMore t = ZeroOrMore(t)

/// Regex pattern may repeat once or more
let OnceOrMore(t) = OneOrMore(t)

/// Make Regex optional
let Optional(t) = Optional(t)

/// Plain regex pattern
let Plain c = Plain(PlainType.Create c)

/// One in Set regex pattern
let OneOf c = OneInSet(OneInSetType.Create (CharacterString c))

/// Between - character range
let Between s e = OneInSet(OneInSetType.Create (CharacterRange (s,e)))

/// Exclude Set regex pattern
let Not (c:RexPatt) = c.Not

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

