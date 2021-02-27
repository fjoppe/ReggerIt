# Background

Regular Expressions are useful, but it can be very hard to create complex patterns.
While creating the first version of [Legivel](https://github.com/fjoppe/Legivel), counting parenthesis became a problem, while many requirements were a [composition](https://yaml.org/spec/1.2/spec.html#nb-char) of other requirements.

Why does that need to be so hard? Regular Expressions are well supported in the .Net framework, but there still was no suitable method to create complex patterns in a convenient way. 

In Legivel, I first created this DSL, because I lost control over Regex patterns, which were a series of string concatenations. I did not create a separate DSL Library, because I did not want to support the full dotnet Regex featureset, and I was still discovering how Legivel would be build.

In a project for work, I wanted to parse many SQL queries, to extrapt the table-names. The Queries arrived in many different formats and linguistic forms. To solve this problem, I copy/pasted the Legivel code and solved this problem. That's when I thought, why is there no library for this?

Now there is..

