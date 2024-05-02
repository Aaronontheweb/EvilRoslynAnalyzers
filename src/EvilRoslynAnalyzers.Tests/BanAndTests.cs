using Microsoft.CodeAnalysis;
using Verify = EvilRoslynAnalyzers.Tests.Utilities.EvilVerifier<EvilRoslynAnalyzers.NoAndOperatorAnalyzer>;

namespace EvilRoslynAnalyzers.Tests;

public class BanAndTests
{
    [Fact]
    public void DemonstrateDeMorgansLaw()
    {
        bool DeMorgansLawAnd(bool a, bool b) => !(!a || !b);
        
        Assert.Equal(true && false, DeMorgansLawAnd(true, false));
        Assert.Equal(false && true, DeMorgansLawAnd(false, true));
        Assert.Equal(true && true, DeMorgansLawAnd(true, true));
        Assert.Equal(false && false, DeMorgansLawAnd(false, false));
    }
    
    
    [Fact]
    public Task ShouldNotDetectDeMorgansLawUsage()
    {
        string testCode = @"
using System;
public class TestClass {
    public void TestMethod() {
        bool a = true;
        bool b = false;
        if(!(!a || !b))
        {
        }
    }
}
";
        return Verify.VerifyAnalyzer(testCode);
    }
    
    
    [Fact]
    public Task ShouldDetectAndOperatorUsage()
    {
        string testCode = @"
using System;
public class TestClass {
    public void TestMethod() {
        bool a = true;
        bool b = false;
        if(a && b)
        {
        }
    }
}
";
        return Verify.VerifyAnalyzer(testCode,Verify.Diagnostic().WithSpan(7, 12, 7, 18).WithSeverity(DiagnosticSeverity.Error));
    }
}