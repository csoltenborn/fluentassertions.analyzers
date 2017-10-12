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
    public class CollectionShouldNotHaveCountAnalyzer : FluentAssertionsAnalyzer
    {
        public const string DiagnosticId = Constants.Tips.Collections.CollectionShouldNotHaveCount;
        public const string Category = Constants.Tips.Category;

        public const string Message = "Use {0} .Should() followed by NotHaveCount instead.";

        protected override DiagnosticDescriptor Rule => new DiagnosticDescriptor(DiagnosticId, Title, Message, Category, DiagnosticSeverity.Info, true);
        protected override IEnumerable<(FluentAssertionsCSharpSyntaxVisitor, BecauseArgumentsSyntaxVisitor)> Visitors
        {
            get
            {
                yield return (new CountShouldNotBeSyntaxVisitor(), new BecauseArgumentsSyntaxVisitor("NotBe", 1));
            }
        }

        private class CountShouldNotBeSyntaxVisitor : FluentAssertionsWithArgumentCSharpSyntaxVisitor
        {
            protected override string MethodContainingArgument => "NotBe";
            public CountShouldNotBeSyntaxVisitor() : base("Count", "Should", "NotBe")
            {
            }
        }
    }

    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(CollectionShouldNotHaveCountCodeFix)), Shared]
    public class CollectionShouldNotHaveCountCodeFix : FluentAssertionsCodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(CollectionShouldNotHaveCountAnalyzer.DiagnosticId);

        protected override StatementSyntax GetNewStatement(FluentAssertionsDiagnosticProperties properties)
            => SyntaxFactory.ParseStatement($"{properties.VariableName}.Should().NotHaveCount({properties.CombineWithBecauseArgumentsString(properties.ArgumentString)});");
    }
}
