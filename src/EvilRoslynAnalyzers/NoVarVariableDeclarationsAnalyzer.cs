namespace EvilRoslynAnalyzers;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class NoVarVariableDeclarationsAnalyzer : DiagnosticAnalyzer
{
    public const string DiagnosticId = "NoVarVariableDeclarations";

    private static readonly string Title = "Variable declaration through 'var' keyword is forbidden";
    private static readonly string MessageFormat = "Variable declaration through 'var' keyword is forbidden";
    private static readonly string Description = "Variable declaration through 'var' keyword is forbidden.";
    private const string Category = "Usage";

    private static DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Error, isEnabledByDefault: true, description: Description);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSyntaxNodeAction(AnalyzeSyntaxNode, SyntaxKind.VariableDeclaration);
    }
    
    private void AnalyzeSyntaxNode(SyntaxNodeAnalysisContext context)
    {
        var declaratorExpr = (VariableDeclarationSyntax)context.Node;
        var identifier = declaratorExpr.Type as IdentifierNameSyntax;
    
        if (identifier?.Identifier.Text == "var")
        {
            var diag = Diagnostic.Create(Rule, identifier.GetLocation(), identifier.Identifier.Text);
            context.ReportDiagnostic(diag);
        }
    }
}
