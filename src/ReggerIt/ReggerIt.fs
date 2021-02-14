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
        

type Plain =
    private {
        Text : string
    }
    override this.ToString() = 
        sprintf "%s" (escaped this.Text)

    static member (+) (r1:Plain, r2:Plain) = 
        let appd = r1.Text + r2.Text
        { Text = appd }

    static member Create r = { Text = r}


and OneInSet =
    private {
        Invert      : bool
        Mainset     : string
        Subtractset : string
        OneInSet    : Lazy<string>
    }

    static member OptimizeSet (ms:string) (ss:string) =
        let sar = ss.ToCharArray()
        ms.ToCharArray()
        |>  Array.filter(fun c -> not (Array.exists(fun ce -> ce = c) sar))
        |>  fun ca -> new string(ca)

    override this.ToString() =
        let subtract = this.Subtractset <> ""
        match (subtract, this.Invert) with 
        //  https://msdn.microsoft.com/en-us/library/20bw873z(v=vs.110).aspx#Anchor_13
        |   (true, true)    -> sprintf "[%s-[^%s]]" (escaped this.Subtractset) (escaped this.Mainset)
        |   (true, false)   -> sprintf "[%s-[%s]]"  (escaped this.Mainset) (escaped this.Subtractset)
        |   (false, true)   -> sprintf "[^%s]" (escaped this.Mainset)
        |   (false, false)  -> sprintf "[%s]" (escaped this.Mainset)

    static member (-) (r1:OneInSet, r2:OneInSet) =
        let ms = r1.Mainset
        let ss =  r1.Subtractset + r2.Mainset
        {Mainset = ms; Subtractset = ss; OneInSet = lazy(OneInSet.OptimizeSet ms ss); Invert = r1.Invert }

    static member (-) (r1:OneInSet, r2:Plain) =
        let ms = r1.Mainset
        let ss = r1.Subtractset + r2.Text
        {Mainset = ms; Subtractset = ss; OneInSet = lazy(OneInSet.OptimizeSet ms ss); Invert = r1.Invert }

    static member (+) (r1:OneInSet, r2:OneInSet) =
        let ms = r1.Mainset + r2.Mainset
        let ss = r1.Subtractset + r2.Subtractset
        {Mainset = ms; Subtractset = ss; OneInSet = lazy(OneInSet.OptimizeSet ms ss); Invert = r1.Invert }

    static member (+) (_:OneInSet, _:Plain) =
        failwith "Unsupported RGX addition"

    static member Create r = 
        {Mainset= r; Subtractset = ""; OneInSet = lazy(OneInSet.OptimizeSet r ""); Invert = false}

    member this.Not() = 
        {this with Invert = true}


and RexPatt =
    |   Plain of Plain
    |   OneInSet   of OneInSet
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

    static member (|||) (r1:RexPatt, r2:RexPatt) =
        match (r1,r2) with
        |   (OneInSet o1, OneInSet o2)   -> OneInSet(o1 + o2)
        |   _ ->
            match r1 with
            | Or     l ->   Or(r2 :: l)
            | _       ->    Or([r2; r1])

    static member (-) (r1:RexPatt, r2:RexPatt) =
        match (r1,r2) with
        |   (OneInSet o1, OneInSet o2)  -> OneInSet(o1 - o2)
        |   (OneInSet o1,    Plain p1)  -> OneInSet(o1 - p1)
        |   _   -> failwith "Unsupported RGX subtraction"

    static member (+) (r1:RexPatt, r2:RexPatt) =
        match (r1,r2) with
        |   (Plain p1   , Plain p2)      -> Plain(p1 + p2)
        |   (OneInSet o1, OneInSet o2)   -> OneInSet(o1 + o2)
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
let Plain c = Plain(Plain.Create c)

/// One in Set regex pattern
let OneOf c = OneInSet(OneInSet.Create c)

/// Exclude Set regex pattern
let Not (c:RexPatt) = c.Not

/// Creates Regex group
let Group p = Group(p)

/// Creates Regex named group
let NamedGroup s p = NamedGroup(s,p)

/// Regex ToString - match from string start
let ToStringStartPattern (p:RexPatt) = sprintf "\\A(%O)" p

/// Regex ToString - full string match
let ToFullstringPattern (p:RexPatt) = sprintf "\\A(%O)\\z" p

/// Regex ToString - match anywhere in the string 
let ToPattern (p:RexPatt) = sprintf "(%O)" p

