# ReggerIt

Regular Expressions are very powerful, though not very popular amongst software developers because it is very hard to create complex patterns.

ReggerIt helps to reduce this complexity by providing a Domain Specific Language. With ReggerIt, you can build up a complex regular expressing pattern from the ground up, without losing track of parenthesis counting.

ReggerIt is optimized to be used with [System.Text.RegularExpressions.Regex](https://docs.microsoft.com/en-us/dotnet/api/system.text.regularexpressions.regex?view=net-5.0) and provides for the ``pattern`` input parameter, in various matching functions/methods.


---

## Builds

GitHub Actions |
:---: |
[![GitHub Actions](https://github.com/fjoppe/ReggerIt/workflows/Build%20master/badge.svg)](https://github.com/frankjoppe@hotmail.com/ReggerIt/actions?query=branch%3Amaster) |
[![Build History](https://buildstats.info/github/chart/fjoppe/ReggerIt)](https://github.com/frankjoppe@hotmail.com/ReggerIt/actions?query=branch%3Amaster) |

## NuGet 

Package | Stable | Prerelease
--- | --- | ---
ReggerIt | [![NuGet Badge](https://buildstats.info/nuget/ReggerIt)](https://www.nuget.org/packages/ReggerIt/) | [![NuGet Badge](https://buildstats.info/nuget/ReggerIt?includePreReleases=true)](https://www.nuget.org/packages/ReggerIt/)

---

### Developing

Make sure the following **requirements** are installed on your system:

- [dotnet SDK](https://www.microsoft.com/net/download/core) 5.0 or higher

---

### Building


```sh
> build.cmd <optional buildtarget> // on windows
$ ./build.sh  <optional buildtarget>// on unix
```




