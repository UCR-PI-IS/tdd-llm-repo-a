using System.Xml.Linq;
using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.MSBuild;

// Register MSBuild before any Roslyn workspace usage
MSBuildLocator.RegisterDefaults();

if (args.Length < 2)
{
    Console.Error.WriteLine("Usage: MetricsCalculator <project.csproj> <output.xml>");
    return 1;
}

var projectPath = Path.GetFullPath(args[0]);
var outputPath = Path.GetFullPath(args[1]);

if (!File.Exists(projectPath))
{
    Console.Error.WriteLine($"Error: Project file not found: {projectPath}");
    return 1;
}

var outputDir = Path.GetDirectoryName(outputPath);
if (!string.IsNullOrEmpty(outputDir))
    Directory.CreateDirectory(outputDir);

Console.WriteLine($"Analyzing: {projectPath}");

try
{
    using var workspace = MSBuildWorkspace.Create();
    workspace.WorkspaceFailed += (_, e) =>
    {
        if (e.Diagnostic.Kind == WorkspaceDiagnosticKind.Failure)
            Console.Error.WriteLine($"  Workspace issue: {e.Diagnostic.Message}");
    };

    var project = await workspace.OpenProjectAsync(projectPath);
    var compilation = await project.GetCompilationAsync();
    if (compilation is null)
    {
        Console.Error.WriteLine("Error: Could not compile project");
        return 1;
    }

    var assemblyMetrics = AnalyzeAssembly(compilation);
    var xml = GenerateXml(assemblyMetrics, project.Name);
    xml.Save(outputPath);
    Console.WriteLine($"  -> Metrics saved: {outputPath}");
    return 0;
}
catch (Exception ex)
{
    Console.Error.WriteLine($"Error: {ex.Message}");
    return 1;
}

// --- Metrics computation ---

static AssemblyMetrics AnalyzeAssembly(Compilation compilation)
{
    var namespaces = new List<NamespaceMetrics>();

    foreach (var tree in compilation.SyntaxTrees)
    {
        var root = tree.GetRoot();
        var semanticModel = compilation.GetSemanticModel(tree);

        // Group types by namespace
        var typeDeclarations = root.DescendantNodes()
            .OfType<TypeDeclarationSyntax>();

        foreach (var typeDecl in typeDeclarations)
        {
            var symbol = semanticModel.GetDeclaredSymbol(typeDecl);
            if (symbol is null) continue;

            var nsName = symbol.ContainingNamespace?.ToDisplayString() ?? "(global)";
            var ns = namespaces.FirstOrDefault(n => n.Name == nsName);
            if (ns is null)
            {
                ns = new NamespaceMetrics { Name = nsName };
                namespaces.Add(ns);
            }

            var typeMetrics = AnalyzeType(typeDecl, semanticModel);
            ns.Types.Add(typeMetrics);
        }
    }

    return new AssemblyMetrics
    {
        Name = compilation.AssemblyName ?? "Unknown",
        Namespaces = namespaces
    };
}

static TypeMetrics AnalyzeType(TypeDeclarationSyntax typeDecl, SemanticModel model)
{
    var symbol = model.GetDeclaredSymbol(typeDecl);
    var typeName = symbol?.Name ?? typeDecl.Identifier.Text;

    var methods = new List<MethodMetrics>();
    var memberDeclarations = typeDecl.Members;

    foreach (var member in memberDeclarations)
    {
        if (member is MethodDeclarationSyntax methodDecl)
        {
            methods.Add(AnalyzeMethod(methodDecl, model));
        }
        else if (member is ConstructorDeclarationSyntax ctorDecl)
        {
            methods.Add(AnalyzeConstructor(ctorDecl, model));
        }
        else if (member is PropertyDeclarationSyntax propDecl && propDecl.AccessorList != null)
        {
            foreach (var accessor in propDecl.AccessorList.Accessors)
            {
                if (accessor.Body != null || accessor.ExpressionBody != null)
                {
                    methods.Add(new MethodMetrics
                    {
                        Name = $"{propDecl.Identifier.Text}.{accessor.Keyword.Text}",
                        CyclomaticComplexity = ComputeCyclomaticComplexity(accessor),
                        SourceLines = CountLines(accessor),
                        ExecutableLines = CountExecutableLines(accessor)
                    });
                }
            }
        }
    }

    // Depth of inheritance
    int depthOfInheritance = 0;
    if (symbol is INamedTypeSymbol namedType)
    {
        var baseType = namedType.BaseType;
        while (baseType != null && baseType.SpecialType != SpecialType.System_Object)
        {
            depthOfInheritance++;
            baseType = baseType.BaseType;
        }
    }

    // Class coupling: count distinct referenced types
    int classCoupling = CountClassCoupling(typeDecl, model);

    // Aggregate metrics
    int totalCC = methods.Sum(m => m.CyclomaticComplexity);
    int totalSourceLines = CountLines(typeDecl);
    int totalExecLines = methods.Sum(m => m.ExecutableLines);

    // Maintainability Index (standard formula, 0-100 scale)
    double mi = ComputeMaintainabilityIndex(totalCC, totalSourceLines);

    return new TypeMetrics
    {
        Name = typeName,
        MaintainabilityIndex = (int)Math.Round(mi),
        CyclomaticComplexity = totalCC,
        ClassCoupling = classCoupling,
        DepthOfInheritance = depthOfInheritance,
        SourceLines = totalSourceLines,
        ExecutableLines = totalExecLines,
        Methods = methods
    };
}

static MethodMetrics AnalyzeMethod(MethodDeclarationSyntax method, SemanticModel model)
{
    int cc = ComputeCyclomaticComplexity(method);
    int sourceLines = CountLines(method);
    int execLines = CountExecutableLines(method);
    double mi = ComputeMaintainabilityIndex(cc, sourceLines);

    return new MethodMetrics
    {
        Name = method.Identifier.Text,
        CyclomaticComplexity = cc,
        SourceLines = sourceLines,
        ExecutableLines = execLines,
        MaintainabilityIndex = (int)Math.Round(mi)
    };
}

static MethodMetrics AnalyzeConstructor(ConstructorDeclarationSyntax ctor, SemanticModel model)
{
    int cc = ComputeCyclomaticComplexity(ctor);
    int sourceLines = CountLines(ctor);
    int execLines = CountExecutableLines(ctor);
    double mi = ComputeMaintainabilityIndex(cc, sourceLines);

    return new MethodMetrics
    {
        Name = $".ctor",
        CyclomaticComplexity = cc,
        SourceLines = sourceLines,
        ExecutableLines = execLines,
        MaintainabilityIndex = (int)Math.Round(mi)
    };
}

static int ComputeCyclomaticComplexity(SyntaxNode node)
{
    int complexity = 1; // Base path

    foreach (var descendant in node.DescendantNodes())
    {
        switch (descendant)
        {
            case IfStatementSyntax:
            case ConditionalExpressionSyntax: // ternary ?:
            case CaseSwitchLabelSyntax:
            case CasePatternSwitchLabelSyntax:
            case WhileStatementSyntax:
            case ForStatementSyntax:
            case ForEachStatementSyntax:
            case CatchClauseSyntax:
            case ConditionalAccessExpressionSyntax: // ?.
                complexity++;
                break;
            case BinaryExpressionSyntax binary:
                if (binary.IsKind(SyntaxKind.LogicalAndExpression) ||
                    binary.IsKind(SyntaxKind.LogicalOrExpression) ||
                    binary.IsKind(SyntaxKind.CoalesceExpression)) // ??
                    complexity++;
                break;
            case SwitchExpressionArmSyntax: // switch expression arms
                complexity++;
                break;
        }
    }

    return complexity;
}

static int CountLines(SyntaxNode node)
{
    var span = node.GetLocation().GetLineSpan();
    return span.EndLinePosition.Line - span.StartLinePosition.Line + 1;
}

static int CountExecutableLines(SyntaxNode node)
{
    return node.DescendantNodes()
        .OfType<StatementSyntax>()
        .Count(s => s is not BlockSyntax);
}

static int CountClassCoupling(TypeDeclarationSyntax typeDecl, SemanticModel model)
{
    var referencedTypes = new HashSet<string>();

    foreach (var node in typeDecl.DescendantNodes())
    {
        var typeInfo = model.GetTypeInfo(node);
        if (typeInfo.Type is INamedTypeSymbol namedType &&
            namedType.SpecialType == SpecialType.None &&
            !namedType.IsDefinition)
        {
            referencedTypes.Add(namedType.ToDisplayString());
        }

        if (node is IdentifierNameSyntax or QualifiedNameSyntax or GenericNameSyntax)
        {
            var symbolInfo = model.GetSymbolInfo(node);
            if (symbolInfo.Symbol?.ContainingType is INamedTypeSymbol containingType &&
                containingType.SpecialType == SpecialType.None)
            {
                referencedTypes.Add(containingType.ToDisplayString());
            }
        }
    }

    // Exclude self
    var selfName = model.GetDeclaredSymbol(typeDecl)?.ToDisplayString();
    if (selfName != null) referencedTypes.Remove(selfName);

    return referencedTypes.Count;
}

static double ComputeMaintainabilityIndex(int cyclomaticComplexity, int linesOfCode)
{
    if (linesOfCode <= 0) return 100;

    // Standard Maintainability Index formula (simplified, without Halstead Volume)
    // MI = MAX(0, (171 - 5.2 * ln(HV) - 0.23 * CC - 16.2 * ln(LOC)) * 100 / 171)
    // Without Halstead: MI = MAX(0, (171 - 0.23 * CC - 16.2 * ln(LOC)) * 100 / 171)
    double mi = (171.0 - 0.23 * cyclomaticComplexity - 16.2 * Math.Log(linesOfCode)) * 100.0 / 171.0;
    return Math.Max(0, Math.Min(100, mi));
}

// --- XML generation ---

static XDocument GenerateXml(AssemblyMetrics assembly, string projectName)
{
    var assemblyElement = new XElement("Assembly",
        new XAttribute("Name", assembly.Name));

    foreach (var ns in assembly.Namespaces)
    {
        var nsElement = new XElement("Namespace",
            new XAttribute("Name", ns.Name));

        foreach (var type in ns.Types)
        {
            var typeElement = new XElement("NamedType",
                new XAttribute("Name", type.Name),
                new XElement("Metrics",
                    Metric("MaintainabilityIndex", type.MaintainabilityIndex),
                    Metric("CyclomaticComplexity", type.CyclomaticComplexity),
                    Metric("ClassCoupling", type.ClassCoupling),
                    Metric("DepthOfInheritance", type.DepthOfInheritance),
                    Metric("SourceLines", type.SourceLines),
                    Metric("ExecutableLines", type.ExecutableLines)));

            foreach (var method in type.Methods)
            {
                typeElement.Add(new XElement("Method",
                    new XAttribute("Name", method.Name),
                    new XElement("Metrics",
                        Metric("MaintainabilityIndex", method.MaintainabilityIndex),
                        Metric("CyclomaticComplexity", method.CyclomaticComplexity),
                        Metric("SourceLines", method.SourceLines),
                        Metric("ExecutableLines", method.ExecutableLines))));
            }

            nsElement.Add(typeElement);
        }

        assemblyElement.Add(nsElement);
    }

    return new XDocument(
        new XDeclaration("1.0", "utf-8", null),
        new XElement("CodeMetricsReport",
            new XAttribute("Version", "1.0"),
            new XElement("Target",
                new XAttribute("Name", projectName),
                assemblyElement)));
}

static XElement Metric(string name, int value) =>
    new("Metric", new XAttribute("Name", name), new XAttribute("Value", value));

// --- Data models ---

record AssemblyMetrics
{
    public string Name { get; init; } = "";
    public List<NamespaceMetrics> Namespaces { get; init; } = new();
}

record NamespaceMetrics
{
    public string Name { get; set; } = "";
    public List<TypeMetrics> Types { get; init; } = new();
}

record TypeMetrics
{
    public string Name { get; init; } = "";
    public int MaintainabilityIndex { get; init; }
    public int CyclomaticComplexity { get; init; }
    public int ClassCoupling { get; init; }
    public int DepthOfInheritance { get; init; }
    public int SourceLines { get; init; }
    public int ExecutableLines { get; init; }
    public List<MethodMetrics> Methods { get; init; } = new();
}

record MethodMetrics
{
    public string Name { get; init; } = "";
    public int CyclomaticComplexity { get; init; }
    public int SourceLines { get; init; }
    public int ExecutableLines { get; init; }
    public int MaintainabilityIndex { get; init; }
}
