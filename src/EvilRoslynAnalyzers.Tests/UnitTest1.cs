using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;
using EvilRoslynAnalyzers;
using Microsoft.CodeAnalysis.Testing;
using Verify = EvilRoslynAnalyzers.Tests.Utilities.EvilVerifier<EvilRoslynAnalyzers.NoExtensionMethodsAnalyzer>;

public class NoExtensionMethodsAnalyzerTests
{

    [Fact]
    public Task ShouldNotDetectNormalMethodUsage()
    {
        // Normal method usage should not trigger the analyzer
        string testCode = @"
using System;
public class TestClass {
    public void TestMethod() {
        Console.WriteLine(""Hello, World!"");
    }
}";
        
        return Verify.VerifyAnalyzer(testCode);
    }

    [Fact]
    public Task ShouldDetectExtensionMethodUsage()
    {
        // Example extension method usage in source code
        string testCode = @"
using System;
using System.Linq;
public class TestClass {
    public void TestMethod() {
        var numbers = new int[] { 1, 2, 3 };
        var count = numbers.Count();
    }
}";
        return Verify.VerifyAnalyzer(testCode,
            Verify.Diagnostic().WithSpan(7, 29, 7, 34).WithSeverity(DiagnosticSeverity.Error).WithArguments("Count"));
    }
}
