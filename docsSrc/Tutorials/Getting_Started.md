# Getting Started

ReggerIt is used to compose regular expression patterns, compatible for [Regex in the Microsoft DotNet framework](https://docs.microsoft.com/en-us/dotnet/api/system.text.regularexpressions.regex).


The following is an example how to compose a pattern, which enables to read table-names from a SQL query.

The composition is layered:
* Definition of a label;
* Definition how a table name is constructed;
* The usage of tables in a SQL query - in the from and join clauses

With this approach, it is also very easy to "debug" a regex pattern. You can temporarily out-comment parts of the pattern and test its remains, to check your expectations with the actual outcome. In the example below, you can the "from" clause without the "join"s.

Another example, if you change the definition of ``label`` - ie it must start with a letter - then this change is only applied in one place.


```fsharp
open ReggerIt
open System.Text.RegularExpressions

///  Cleanup an input string
let cleanup (s:string) = Regex.Replace(s.ToLower(), "\s+", " ").Trim()

//  Build up pattern to extract table names from a SQL query string
let groupName = "table"

//  Tablename basics
let brackets s = Plain "[" + s + Plain "]"
let label = OnceOrMore Macro.wordCharacter
let labelNotation = label ||| brackets label

//  Notation basics
let dot = Plain "."
let white = OnceOrMore Macro.whitespace
let optWhite = ZeroOrMore (Macro.whitespace)
let commaSeperated = optWhite + Plain "," + optWhite

//  Tablename notations
let tableName = NamedGroup groupName labelNotation
let ``database..table`` = labelNotation + dot + dot + tableName
let ``schema.table``    = labelNotation + dot + tableName
let ``database.schema.table`` = labelNotation + dot + labelNotation + dot + tableName
let table = ``database..table`` ||| ``database.schema.table`` ||| ``schema.table`` ||| tableName
let tableAlias = table + Optional((white + Plain "as " + label) ||| (white + label))

//  from clause
let fromTables = tableAlias + ZeroOrMore(commaSeperated + tableAlias)

//  join clauses
let joinKind = Plain "inner" ||| Plain "outer" |||  Plain "left" |||  Plain "right"
let join = white + joinKind + white + Plain "join" + white + tableAlias

let select = Plain "select " + ZeroOrMore(Macro.any) + Plain "from " + fromTables + ZeroOrMore(join)

//  Prepare for Regex.Match
let pattern = select |> Convert.ToStringStartPattern

let input = """
SELECT p.Name AS ProductName,
NonDiscountSales = (OrderQty * UnitPrice),
Discounts = ((OrderQty * UnitPrice) * UnitPriceDiscount)
FROM Production.Product AS p
INNER JOIN Sales.SalesOrderDetail AS sod
ON p.ProductID = sod.ProductID
ORDER BY ProductName DESC;
"""

let extract = Regex.Match(input |> cleanup, pattern)

extract.Groups.[groupName].Captures
|>  Seq.iter(fun c -> printfn "%s" c.Value)

```


## Here is the path to downloading

    [lang=bash]
    paket install ReggerIt


