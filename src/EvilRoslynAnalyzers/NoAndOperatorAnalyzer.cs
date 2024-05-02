namespace EvilRoslynAnalyzers;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class NoAndOperatorAnalyzer : DiagnosticAnalyzer
{
    public const string DiagnosticId = "NoAndOperator";
    
    private static readonly string Title = "AndAlso operator is forbidden";
    private static readonly string MessageFormat = "Using the && operator is not allowed";
    private static readonly string Description = "All boolean and operator (&&) usages are forbidden in this codebase. You must rule out your expressions being false!";
    private const string Category = "Usage";
    
    private static DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Error, isEnabledByDefault: true, description: Description);
    
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

    
    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSyntaxNodeAction(AnalyzeSyntaxNode, SyntaxKind.LogicalAndExpression);
    }

    private void AnalyzeSyntaxNode(SyntaxNodeAnalysisContext context)
    {
        var diag = Diagnostic.Create(Rule, context.Node.GetLocation());
        context.ReportDiagnostic(diag);
    }
}