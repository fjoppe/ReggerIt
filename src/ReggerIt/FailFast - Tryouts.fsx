#I __SOURCE_DIRECTORY__
#r @"bin/Debug/net5.0/ReggerIt.dll"

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
