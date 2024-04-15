namespace EvilRoslynAnalyzers;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class NoExtensionMethodsAnalyzer : DiagnosticAnalyzer
{
    public const string DiagnosticId = "NoExtensionMethods";

    private static readonly string Title = "Extension method invocation is forbidden";
    private static readonly string MessageFormat = "Extension method '{0}' invocation is not allowed";
    private static readonly string Description = "All extension method usages are forbidden in this codebase.";
    private const string Category = "Usage";

    private static DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Error, isEnabledByDefault: true, description: Description);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSyntaxNodeAction(AnalyzeSyntaxNode, SyntaxKind.InvocationExpression);
    }

    private void AnalyzeSyntaxNode(SyntaxNodeAnalysisContext context)
    {
        var invocationExpr = (InvocationExpressionSyntax)context.Node;

        if (invocationExpr.Expression is MemberAccessExpressionSyntax memberAccessExpr)
        {
            var symbol = context.SemanticModel.GetSymbolInfo(memberAccessExpr).Symbol;
            if (symbol?.Kind == SymbolKind.Method)
            {
                var methodSymbol = (IMethodSymbol)symbol;
                if (methodSymbol.IsExtensionMethod)
                {
                    var diag = Diagnostic.Create(Rule, memberAccessExpr.Name.GetLocation(), methodSymbol.Name);
                    context.ReportDiagnostic(diag);
                }
            }
        }
    }
}
