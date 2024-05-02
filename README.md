# Evil Roslyn Analyzers

Enterprise-approved insane C# Roslyn Analyzers under the motto: "don't let productivity triumph over orthodoxy."

* [`NoExtensionMethods`](https://www.reddit.com/r/dotnet/comments/1c4hz1z/linq_forbidden/) - especially LINQ. If classes were supposed to have these methods, they would have implemented via inheritance obviously.
* [`NoVarDeclarations`](https://twitter.com/michaeljolley/status/1782767354184667538) - the `var` keyword is dangerous; we should all write C# with explicit types in every expression, like a 1990s Java program.
* [`NoAndOperators`](https://twitter.com/willboulton) - disallow usage of the `&&` operator, instead you must be explicit by ruling falsehood.