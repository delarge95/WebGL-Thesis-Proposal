using System;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace UnityArchitectGuard
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class GodClassAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "AG001";
        private const string Title = "God Class Detected";
        private const string MessageFormat = "Class '{0}' has {1} dependencies (threshold: {2}). Consider refactoring.";
        private const string Description = "Classes with too many dependencies are hard to maintain.";
        private const string Category = "Architecture";

        private const int DependencyThreshold = 30; // Start strict

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(
            DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSyntaxNodeAction(AnalyzeClass, SyntaxKind.ClassDeclaration);
        }

        private void AnalyzeClass(SyntaxNodeAnalysisContext context)
        {
            var classDeclaration = (ClassDeclarationSyntax)context.Node;
            
            // Very simple heuristic: count unique field types + property types used
            // This is a "lite" version of coupling check
            var fields = classDeclaration.DescendantNodes().OfType<FieldDeclarationSyntax>();
            var properties = classDeclaration.DescendantNodes().OfType<PropertyDeclarationSyntax>();
            
            var dependencies = fields.SelectMany(f => f.Declaration.Variables) // simplistic, better to look at TypeSyntax
                .Count() + properties.Count();

            // A real "God Class" also has many methods and lines. 
            // Let's check for raw line count estimate as proxy for immediate feedback
            var lineCount = classDeclaration.ToString().Count(c => c == '\n');

            if (lineCount > 500) // Threshold for lines
            {
                 var diagnostic = Diagnostic.Create(Rule, classDeclaration.Identifier.GetLocation(), 
                     classDeclaration.Identifier.Text, lineCount, 500);
                 context.ReportDiagnostic(diagnostic);
            }
        }
    }
}
