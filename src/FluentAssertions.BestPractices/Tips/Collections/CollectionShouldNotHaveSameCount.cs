﻿using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Composition;

namespace FluentAssertions.BestPractices
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class CollectionShouldNotHaveSameCountAnalyzer : FluentAssertionsAnalyzer
    {
        public const string DiagnosticId = Constants.Tips.Collections.CollectionShouldNotHaveSameCount;
        public const string Category = Constants.Tips.Category;

        public const string Message = "Use {0} .Should() followed by .NotHaveSameCount() instead.";

        protected override DiagnosticDescriptor Rule => new DiagnosticDescriptor(DiagnosticId, Title, Message, Category, DiagnosticSeverity.Info, true);
        protected override IEnumerable<(FluentAssertionsCSharpSyntaxVisitor, BecauseArgumentsSyntaxVisitor)> Visitors
        {
            get
            {
                yield return (new CountShouldNotBeOtherCollectionCountSyntaxVisitor(), new BecauseArgumentsSyntaxVisitor("NotBe", 1));
            }
        }
        private class CountShouldNotBeOtherCollectionCountSyntaxVisitor : FluentAssertionsWithArgumentCSharpSyntaxVisitor
        {
            protected override string MethodContainingArgument => "NotBe";
            public CountShouldNotBeOtherCollectionCountSyntaxVisitor() : base("Count", "Should", "NotBe")
            {
            }

            protected override ExpressionSyntax ModifyArgument(ExpressionSyntax expression)
            {
                var invocation = expression as InvocationExpressionSyntax;
                var memberAccess = invocation?.Expression as MemberAccessExpressionSyntax;
                var identifier = memberAccess?.Expression as IdentifierNameSyntax;

                return identifier;
            }
        }
    }

    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(CollectionShouldNotHaveSameCountCodeFix)), Shared]
    public class CollectionShouldNotHaveSameCountCodeFix : FluentAssertionsCodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(CollectionShouldNotHaveSameCountAnalyzer.DiagnosticId);

        protected override StatementSyntax GetNewStatement(FluentAssertionsDiagnosticProperties properties)
            => SyntaxFactory.ParseStatement($"{properties.VariableName}.Should().NotHaveSameCount({properties.CombineWithBecauseArgumentsString(properties.ArgumentString)});");
    }
}
