using Microsoft.CodeAnalysis;
using Verify = EvilRoslynAnalyzers.Tests.Utilities.EvilVerifier<EvilRoslynAnalyzers.NoVarVariableDeclarationsAnalyzer>;

namespace EvilRoslynAnalyzers.Tests;

public class BanVarTests
{
    [Fact]
    public Task ShouldNotDetectNormalVariableDeclaration()
    {
        string testCode = @"
using System;
public class TestClass {
    public void TestMethod() {
        int a = 5;
    }
}
";
        return Verify.VerifyAnalyzer(testCode);
    }
    
    [Fact]
    public Task ShouldDetectVarVariableDeclaration()
    {
        string testCode = @"
using System;
public class TestClass {
    public void TestMethod() {
        var a = 5;
    }
}
";
        return Verify.VerifyAnalyzer(testCode,
            Verify.Diagnostic().WithSpan(5, 9, 5, 12).WithSeverity(DiagnosticSeverity.Error).WithArguments("var"));
    }
}