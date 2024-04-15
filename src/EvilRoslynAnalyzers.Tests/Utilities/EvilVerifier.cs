using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Testing;

namespace EvilRoslynAnalyzers.Tests.Utilities;

/**
 * Code inspired by https://github.com/xunit/xunit.analyzers/blob/main/src/xunit.analyzers.tests/Utility/CSharpVerifier.cs
 */

[SuppressMessage("Design", "CA1000:Do not declare static members on generic types")]
public sealed class EvilVerifier<TAnalyzer> where TAnalyzer : DiagnosticAnalyzer, new()
{
    /// <summary>
    ///     Creates a diagnostic result for the diagnostic referenced in <see cref="TAnalyzer" />.
    /// </summary>
    public static DiagnosticResult Diagnostic()
    {
        return CSharpCodeFixVerifier<TAnalyzer, EmptyCodeFixProvider, DefaultVerifier>.Diagnostic();
    }

    public static Task VerifyAnalyzer(string source, params DiagnosticResult[] diagnostics)
    {
        return VerifyAnalyzer(new[] { source }, diagnostics);
    }

    public static Task VerifyAnalyzer(string[] sources, params DiagnosticResult[] diagnostics)
    {
        var test = new EvilTest();
#pragma warning disable CA1062
        foreach (var source in sources)
#pragma warning restore CA1062
            test.TestState.Sources.Add(source);

        test.ExpectedDiagnostics.AddRange(diagnostics);
        return test.RunAsync();
    }

    public static Task VerifyCodeFix(string before, string after, string fixerActionKey,
        params DiagnosticResult[] diagnostics)
    {
        var test = new EvilTest
        {
            TestCode = before,
            FixedCode = after,
            CodeActionEquivalenceKey = fixerActionKey
        };
        test.TestState.ExpectedDiagnostics.AddRange(diagnostics);
        return test.RunAsync();
    }

    public static Task VerifyCodeFix(
        string before,
        string after,
        string fixerActionKey,
        int incrementalIterations,
        CodeFixTestBehaviors codeFixBehaviors,
        DiagnosticResult[] diagnostics,
        DiagnosticResult[] fixedDiagnostics)
    {
        var test = new EvilTest
        {
            TestCode = before,
            FixedCode = after,
            CodeActionEquivalenceKey = fixerActionKey, 
            NumberOfIncrementalIterations = incrementalIterations,
            CodeFixTestBehaviors = codeFixBehaviors
        };
        test.TestState.ExpectedDiagnostics.AddRange(diagnostics);
        test.FixedState.ExpectedDiagnostics.AddRange(fixedDiagnostics);
        return test.RunAsync();
    }

    private sealed class EvilTest() : TestBase(ReferenceAssemblies.Default);

    private class TestBase : CSharpCodeFixTest<TAnalyzer, EmptyCodeFixProvider, DefaultVerifier>
    {
        protected TestBase(ReferenceAssemblies referenceAssemblies)
        {
            ReferenceAssemblies = referenceAssemblies;

            // Diagnostics are reported in both normal and generated code
            TestBehaviors |= TestBehaviors.SkipGeneratedCodeCheck;

            // Tests that check for messages should run independent of current system culture.
            CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;
        }

        protected override IEnumerable<CodeFixProvider> GetCodeFixProviders()
        {
            // This is a temporary workaround to ensure that the code fix provider is loaded.
            return new[] { new EmptyCodeFixProvider() };
        }
    }
}