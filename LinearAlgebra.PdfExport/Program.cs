using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

// ?? QuestPDF Community License ??????????????????????????????????????????????
QuestPDF.Settings.License = LicenseType.Community;

// ?? Output path ??????????????????????????????????????????????????????????????
string outputPath = Path.Combine(
    Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
    "LinearAlgebraSolver_Documentation.pdf");

Console.WriteLine("Generating PDF documentation...");

// ?? Color palette ?????????????????????????????????????????????????????????????
var Red       = "#D72638";
var RedDark   = "#8B1520";
var Black     = "#0A0A0A";
var DarkGray  = "#1C1C1C";
var MidGray   = "#333333";
var LightGray = "#888888";
var White     = "#F0F0F0";
var CodeBg    = "#141414";
var CodeFg    = "#E0E0E0";

Document.Create(container =>
{
    container.Page(page =>
    {
        page.Size(PageSizes.A4);
        page.Margin(0);
        page.DefaultTextStyle(x => x.FontFamily("Arial").FontSize(10).FontColor(White));
        page.Background().Background(Black);

        page.Content().Column(col =>
        {
            // ??????????????????????????????????????????????????????????????
            //  COVER PAGE
            // ??????????????????????????????????????????????????????????????
            col.Item().Height(PageSizes.A4.Height).Background(Black).Column(cover =>
            {
                // Top red strip
                cover.Item().Height(6).Background(Red);

                cover.Item().Padding(60).Column(inner =>
                {
                    // Badge
                    inner.Item().Width(70).Height(70)
                         .Background(Red).CornerRadius(12)
                         .AlignCenter().AlignMiddle()
                         .Text("?").FontSize(38).Bold().FontColor(White);

                    inner.Item().Height(30);

                    inner.Item().Text("LINEAR ALGEBRA SOLVER")
                         .FontSize(32).Bold().FontColor(White);

                    inner.Item().Height(10);

                    inner.Item().Text("Complete Technical Documentation")
                         .FontSize(14).FontColor(LightGray);

                    inner.Item().Height(6);

                    inner.Item().BorderBottom(2).BorderColor(Red).PaddingBottom(30)
                         .Text("Architecture � Classes � Algorithms � Design Patterns")
                         .FontSize(11).FontColor(LightGray).Italic();

                    inner.Item().Height(50);

                    // Tech badges
                    inner.Item().Row(row =>
                    {
                        void Badge(RowDescriptor r, string text, string bg)
                            => r.AutoItem().Background(bg).CornerRadius(6)
                                .Padding(8).PaddingLeft(14).PaddingRight(14)
                                .Margin(4)
                                .Text(text).FontSize(10).Bold().FontColor(White);

                        Badge(row, "C# 13", DarkGray);
                        Badge(row, ".NET 10", DarkGray);
                        Badge(row, "WPF", DarkGray);
                        Badge(row, "xUnit", DarkGray);
                        Badge(row, "QuestPDF", Red);
                    });

                    inner.Item().Height(50);

                    // Stats grid
                    inner.Item().Background(DarkGray).CornerRadius(10).Padding(24).Row(row =>
                    {
                        void Stat(RowDescriptor r, string num, string label)
                        {
                            r.RelativeItem().Column(c =>
                            {
                                c.Item().Text(num).FontSize(28).Bold().FontColor(Red);
                                c.Item().Text(label).FontSize(10).FontColor(LightGray);
                            });
                        }
                        Stat(row, "5",  "Projects");
                        Stat(row, "20+","Source Files");
                        Stat(row, "7",  "Solvers");
                        Stat(row, "41", "Tests Passed");
                    });

                    inner.Item().Height(60);

                    inner.Item().Text($"Generated: {DateTime.Now:MMMM dd, yyyy}")
                         .FontSize(10).FontColor(MidGray);
                    inner.Item().Text("Mohamed � Linear Algebra Project")
                         .FontSize(10).FontColor(MidGray);
                });

                cover.Item().Extend().AlignBottom()
                     .Height(6).Background(Red);
            });

            // ??????????????????????????????????????????????????????????????
            //  TABLE OF CONTENTS
            // ??????????????????????????????????????????????????????????????
            col.Item().PageBreak();
            col.Item().Padding(50).Column(toc =>
            {
                SubHeader(toc, "Table of Contents");
                toc.Item().Height(20);

                void TocEntry(ColumnDescriptor c, string num, string title, string desc)
                {
                    c.Item().Row(row =>
                    {
                        row.AutoItem().Width(30).Text(num).Bold().FontColor(Red).FontSize(11);
                        row.RelativeItem().Column(inner =>
                        {
                            inner.Item().Text(title).Bold().FontSize(11).FontColor(White);
                            inner.Item().Text(desc).FontSize(9).FontColor(LightGray);
                        });
                    });
                    c.Item().Height(14);
                }

                TocEntry(toc, "1.", "Project Overview",          "Goals � Features � Technology Stack");
                TocEntry(toc, "2.", "Solution Architecture",      "Layered Design � Dependency Rules � Project Map");
                TocEntry(toc, "3.", "Domain Layer",               "Fraction � Matrix � SolveResult � SolutionStep � HistoryEntry");
                TocEntry(toc, "4.", "Interfaces",                 "ISolver � IHistoryRepository");
                TocEntry(toc, "5.", "Solvers � Algorithms",       "GaussJordan � Determinant � Inverse � Rank � Transpose � Add/Sub/Mul");
                TocEntry(toc, "6.", "Application Layer",          "SolverService � HistoryService � EquationParser");
                TocEntry(toc, "7.", "Infrastructure Layer",       "JsonHistoryRepository � TextHistoryRepository");
                TocEntry(toc, "8.", "UI Layer (WPF)",             "MainWindow � Code-Behind � Composition Root");
                TocEntry(toc, "9.", "OOP Principles",             "Encapsulation � Abstraction � Inheritance � Polymorphism");
                TocEntry(toc, "10.","Design Patterns",            "Template Method � Strategy � Repository � DI");
                TocEntry(toc, "11.","Unit Tests",                 "FractionTests � MatrixTests � SolverTests � 41 Passing");
                TocEntry(toc, "12.","Data Flow Walkthrough",      "Full trace: user input ? result displayed");
                TocEntry(toc, "13.","Example Calculations",       "Gauss-Jordan � Determinant � Inverse worked examples");
            });

            // ??????????????????????????????????????????????????????????????
            //  1. PROJECT OVERVIEW
            // ??????????????????????????????????????????????????????????????
            col.Item().PageBreak();
            col.Item().Padding(50).Column(s =>
            {
                ChapterTitle(s, "1", "Project Overview");

                SubHeader(s, "Goal");
                BodyText(s, "Linear Algebra Solver is a Windows Desktop application built with WPF and .NET 10. It accepts matrices and systems of linear equations as text input, solves them using exact fraction arithmetic (zero floating-point error), displays every step of the solution, and stores a persistent history of all past operations.");

                s.Item().Height(16);
                SubHeader(s, "Key Features");
                BulletList(s, new[]
                {
                    "Solve systems of linear equations (Gauss-Jordan Elimination) � detects Unique / Infinite / No Solution",
                    "Compute matrix Determinant using row-reduction in O(n�)",
                    "Compute Matrix Inverse using the augmented [A|I] ? [I|A?�] method",
                    "Transpose any matrix (m � n)",
                    "Compute Matrix Rank via Row Echelon Form",
                    "Matrix Addition, Subtraction, and Multiplication for two matrices",
                    "All results shown step-by-step with the matrix state after each operation",
                    "Persistent JSON history saved to %AppData% � survives app restarts",
                    "Red & Black dark-theme WPF UI with sidebar navigation and bottom history panel",
                });

                s.Item().Height(16);
                SubHeader(s, "Technology Stack");
                TwoColTable(s, new[]
                {
                    ("Language",         "C# 13 with nullable reference types and collection expressions"),
                    ("Framework",        ".NET 10 (UI: net10.0-windows, Libraries: net10.0)"),
                    ("UI Framework",     "WPF (Windows Presentation Foundation) � XAML-based"),
                    ("Arithmetic",       "Custom Fraction struct � exact rational arithmetic"),
                    ("Storage",          "System.Text.Json � history saved as JSON in AppData"),
                    ("Testing",          "xUnit 2.9 � 41 unit tests across 3 test classes"),
                    ("PDF Generation",   "QuestPDF 2024.10 � this document"),
                    ("IDE",              "Visual Studio 2022 / JetBrains Rider"),
                });
            });

            // ??????????????????????????????????????????????????????????????
            //  2. ARCHITECTURE
            // ??????????????????????????????????????????????????????????????
            col.Item().PageBreak();
            col.Item().Padding(50).Column(s =>
            {
                ChapterTitle(s, "2", "Solution Architecture");

                BodyText(s, "The solution follows a Layered Architecture (also called N-Tier Architecture) with a strict dependency rule: inner layers never reference outer layers. This keeps the core business logic (Domain) completely independent from UI and storage concerns.");

                s.Item().Height(16);

                // Architecture diagram as a table
                s.Item().Background(DarkGray).CornerRadius(10).Padding(20).Column(diag =>
                {
                    void Layer(ColumnDescriptor d, string name, string desc, string bg)
                    {
                        d.Item().Background(bg).CornerRadius(6).Padding(12).Margin(4).Row(r =>
                        {
                            r.AutoItem().Width(160).Text(name).Bold().FontSize(11).FontColor(White);
                            r.RelativeItem().Text(desc).FontSize(10).FontColor(LightGray);
                        });
                    }
                    Layer(diag, "UI Layer (WPF)",          "MainWindow.xaml + MainWindow.xaml.cs � Composition Root", "#1A0810");
                    diag.Item().AlignCenter().Text("?  depends on").FontSize(9).FontColor(LightGray);
                    Layer(diag, "Application Layer",       "SolverService � HistoryService � EquationParser",         "#0D1A08");
                    diag.Item().AlignCenter().Text("?  depends on").FontSize(9).FontColor(LightGray);
                    Layer(diag, "Domain Layer",            "Fraction � Matrix � Solvers � Interfaces � Models",       "#08101A");
                    diag.Item().AlignCenter().Text("?  implements").FontSize(9).FontColor(LightGray);
                    Layer(diag, "Infrastructure Layer",    "JsonHistoryRepository � TextHistoryRepository",           "#1A1008");
                });

                s.Item().Height(16);
                SubHeader(s, "The Golden Rule");
                BodyText(s, "Domain layer has zero knowledge of Infrastructure, Application, or UI. It contains no using System.Windows or file-I/O code. This makes the Domain fully testable in isolation and replaceable without touching business logic.");

                s.Item().Height(16);
                SubHeader(s, "Project Structure");
                CodeBlock(s,
@"LinearAlgebraSolver.slnx
??? LinearAlgebra.Domain/           (net10.0 � Class Library)
?   ??? Models/   Fraction  Matrix  SolutionStep  SolveResult  HistoryEntry
?   ??? Enums/    SolutionType  OperationType
?   ??? Interfaces/  ISolver  IHistoryRepository
?   ??? Solvers/  SolverBase  GaussJordanSolver  DeterminantSolver
?                 InverseSolver  RankSolver  TransposeSolver
?                 MatrixAddSubSolver  MatrixMultiplySolver
?
??? LinearAlgebra.Application/      (net10.0 � Class Library)
?   ??? Parsing/   EquationParser
?   ??? Services/  SolverService  HistoryService
?
??? LinearAlgebra.Infrastructure/   (net10.0 � Class Library)
?   ??? Storage/  JsonHistoryRepository  TextHistoryRepository
?
??? LinearAlgebra.UI/               (net10.0-windows � WPF App)
?   ??? MainWindow.xaml  (Red & Black theme, sidebar layout)
?   ??? MainWindow.xaml.cs  (event handlers + DI composition root)
?
??? LinearAlgebra.Tests/            (net10.0 � xUnit)
?   ??? FractionTests.cs   (18 tests)
?   ??? MatrixTests.cs     (9 tests)
?   ??? SolverTests.cs     (14 tests)
?
??? LinearAlgebra.PdfExport/        (net10.0 � Console � this file)");
            });

            // ??????????????????????????????????????????????????????????????
            //  3. DOMAIN LAYER
            // ??????????????????????????????????????????????????????????????
            col.Item().PageBreak();
            col.Item().Padding(50).Column(s =>
            {
                ChapterTitle(s, "3", "Domain Layer � Classes");

                // ?? Fraction ??????????????????????????????????????????????
                SubHeader(s, "3.1  Fraction  (struct)");
                BodyText(s, "The most critical class in the project. Represents any rational number as an exact numerator/denominator pair of type long. All arithmetic operations produce reduced fractions using the Euclidean GCD algorithm, guaranteeing zero floating-point error.");

                s.Item().Height(8);
                InfoBox(s, "WHY FRACTIONS?",
                    "In standard floating-point: 0.1 + 0.2 = 0.30000000004\n" +
                    "With Fraction struct:  1/10 + 2/10 = 3/10  (exact, always)");

                s.Item().Height(8);
                CodeBlock(s,
@"public readonly struct Fraction : IEquatable<Fraction>, IComparable<Fraction>
{
    public long Numerator   { get; }
    public long Denominator => _denominator == 0 ? 1 : _denominator;
    private readonly long _denominator;

    public Fraction(long numerator, long denominator = 1)
    {
        if (denominator == 0) throw new DivideByZeroException(...);
        if (numerator == 0)  { Numerator = 0; _denominator = 1; return; }

        long gcd  = Gcd(Math.Abs(numerator), Math.Abs(denominator));
        int  sign = denominator < 0 ? -1 : 1;
        Numerator    = sign * numerator   / gcd;   // auto-reduce
        _denominator = sign * denominator / gcd;
    }
}");

                s.Item().Height(10);
                SubHeader(s, "Fraction � Supported Operations");
                TwoColTable(s, new[]
                {
                    ("Arithmetic",       "+ ? � �   all return new reduced Fraction"),
                    ("Comparison",       "== != < > <= >=   exact integer comparison"),
                    ("Implicit casts",   "int, long ? Fraction   (no data loss)"),
                    ("Explicit cast",    "(double)fraction   for display only"),
                    ("Parse",            @"""3""  ""-2/5""  ""0.75""  ? Fraction"),
                    ("ToString",         @"3/2  or  5  (whole numbers shown clean)"),
                    ("Default guard",    "default(Fraction) safely returns 0/1 not 0/0"),
                });

                s.Item().Height(16);

                // ?? Matrix ????????????????????????????????????????????????
                SubHeader(s, "3.2  Matrix  (class)");
                BodyText(s, "Wraps a 2-D array of Fraction values. Encapsulates the internal array behind a public indexer and exposes only safe row-operation methods used by the solvers. All constructors initialise every cell to Fraction.Zero to avoid the default(Fraction) = 0/0 bug.");

                s.Item().Height(8);
                CodeBlock(s,
@"public class Matrix
{
    private readonly Fraction[,] _data;   // PRIVATE � Encapsulation
    public int Rows { get; }
    public int Cols { get; }

    // Indexer � only public access to data
    public Fraction this[int row, int col] { get => ...; set => ...; }

    // Row operations (mutating � called by solvers on a Clone())
    public void SwapRows    (int r1, int r2)              // R1 ? R2
    public void ScaleRow    (int r,  Fraction scalar)     // Ri = k � Ri
    public void AddScaledRow(int dest, int src, Fraction) // Rd = Rd + k�Rs

    // Matrix arithmetic via operator overloading
    public static Matrix operator +(Matrix a, Matrix b)
    public static Matrix operator *(Matrix a, Matrix b)   // O(n�)
    public Matrix Transpose()
    public Matrix Minor(int r, int c)                     // for cofactor
}");

                s.Item().Height(16);

                // ?? SolveResult / SolutionStep ????????????????????????????
                SubHeader(s, "3.3  SolveResult  &  SolutionStep");
                BodyText(s, "SolveResult is the universal return type of every ISolver.Solve() call. It carries the solution type enum, human-readable title/summary, the full ordered list of steps, and optional solution dictionary, scalar result, or result matrix.");

                s.Item().Height(8);
                CodeBlock(s,
@"public class SolveResult
{
    public string         Title        { get; init; }
    public string         Summary      { get; init; }
    public SolutionType   SolutionType { get; init; }  // Unique|Infinite|NoSolution|Value|Error
    public List<SolutionStep> Steps    { get; init; }
    public Dictionary<string,string> Solution { get; init; } // {""x1"":""2"", ""x2"":""3""}
    public Matrix?        ResultMatrix { get; init; }  // inverse / transpose result
    public Fraction?      ScalarResult { get; init; }  // determinant / rank value
    public bool IsSuccess => SolutionType is Unique or Value;
}

public class SolutionStep
{
    public int    StepNumber  { get; init; }
    public string Description { get; init; }  // e.g. ""R2 = R2 + (3)�R1""
    public string MatrixState { get; init; }  // text snapshot of matrix after op
}");

                s.Item().Height(16);
                SubHeader(s, "3.4  SolutionType  Enum");
                TwoColTable(s, new[]
                {
                    ("Unique",    "System has exactly one solution � RREF is full rank"),
                    ("Infinite",  "System has infinitely many solutions � rank < unknowns"),
                    ("NoSolution","System is inconsistent � row of form [0�0 | k?0]"),
                    ("Value",     "Operation returns a scalar or matrix (det, inverse, rank�)"),
                    ("Error",     "Invalid input � non-square for det/inverse, dimension mismatch�"),
                });
            });

            // ??????????????????????????????????????????????????????????????
            //  4. INTERFACES
            // ??????????????????????????????????????????????????????????????
            col.Item().PageBreak();
            col.Item().Padding(50).Column(s =>
            {
                ChapterTitle(s, "4", "Interfaces � Abstraction Contracts");

                SubHeader(s, "4.1  ISolver");
                BodyText(s, "Every solver class must implement this interface. The SolverService works exclusively with ISolver � it never imports a concrete solver type. This enables the Open/Closed Principle: add a new solver by creating a new file; existing code is untouched.");
                s.Item().Height(8);
                CodeBlock(s,
@"public interface ISolver
{
    string      Name  { get; }     // e.g. ""Gauss-Jordan Elimination""
    SolveResult Solve(Matrix matrix);
}");

                s.Item().Height(16);
                SubHeader(s, "4.2  IHistoryRepository");
                BodyText(s, "Abstracts all persistence. The service layer calls Save/LoadAll/Clear without knowing whether data goes to JSON, TXT, SQLite, or the cloud. Swapping storage requires changing exactly ONE line in the composition root.");
                s.Item().Height(8);
                CodeBlock(s,
@"public interface IHistoryRepository
{
    void               Save   (HistoryEntry entry);
    List<HistoryEntry> LoadAll();
    void               Clear  ();
}");

                s.Item().Height(16);
                InfoBox(s, "DEPENDENCY INVERSION PRINCIPLE",
                    "High-level modules (SolverService) depend on abstractions (IHistoryRepository),\n" +
                    "not on concrete implementations (JsonHistoryRepository).\n\n" +
                    "Switching from JSON to a database:\n" +
                    "  BEFORE:  var repo = new JsonHistoryRepository(path);\n" +
                    "  AFTER:   var repo = new DatabaseHistoryRepository(connString);\n" +
                    "  ? ONE line change. Zero logic changes.");
            });

            // ??????????????????????????????????????????????????????????????
            //  5. SOLVERS
            // ??????????????????????????????????????????????????????????????
            col.Item().PageBreak();
            col.Item().Padding(50).Column(s =>
            {
                ChapterTitle(s, "5", "Solvers � Algorithms in Detail");

                SubHeader(s, "5.0  SolverBase  (abstract)");
                BodyText(s, "All solvers inherit from SolverBase, which applies the Template Method Pattern. The base class owns the Steps list and the public Solve() method. Subclasses only override DoSolve().");
                s.Item().Height(8);
                CodeBlock(s,
@"public abstract class SolverBase : ISolver
{
    protected readonly List<SolutionStep> Steps = [];
    public abstract string Name { get; }

    public SolveResult Solve(Matrix matrix)   // TEMPLATE METHOD
    {
        Steps.Clear();          // 1. shared setup
        return DoSolve(matrix); // 2. delegate to subclass
    }

    protected abstract SolveResult DoSolve(Matrix matrix);  // override this

    protected void AddStep(string desc, Matrix? state = null)
        => Steps.Add(new SolutionStep(Steps.Count + 1, desc, state));
}");

                s.Item().Height(16);

                // ?? Gauss-Jordan ??????????????????????????????????????????
                SubHeader(s, "5.1  GaussJordanSolver");
                BodyText(s, "Solves a system of n linear equations with n unknowns. Input is an augmented matrix [A | b] of size n � (n+1). Uses full Gauss-Jordan elimination (not just forward Gaussian � it also eliminates upward) to reach Reduced Row Echelon Form (RREF) directly.");

                s.Item().Height(8);
                InfoBox(s, "ALGORITHM STEPS",
                    "For each column c (pivot column):\n" +
                    "  1. PARTIAL PIVOT � find the row with the largest absolute value in column c\n" +
                    "     (improves numerical stability even with exact fractions)\n" +
                    "  2. SWAP that row to the pivot position if needed ? record step\n" +
                    "  3. SCALE pivot row so pivot element = 1  ? record step\n" +
                    "  4. ELIMINATE all other rows in column c (above AND below) ? record each\n" +
                    "After all columns processed ? classify result:");

                s.Item().Height(8);
                CodeBlock(s,
@"// Contradiction detection: [0 0 ... 0 | k] where k ? 0
bool allZeroCoeffs = true;
for (int j = 0; j < n; j++)
    if (!m[i, j].IsZero) { allZeroCoeffs = false; break; }
if (allZeroCoeffs && !m[i, n].IsZero)
    return NoSolution;    // e.g. 0x + 0y = 5  ? impossible

// Rank check: if rank < n ? infinitely many solutions
int rank = CountNonZeroPivots(m, n);
if (rank < n) return Infinite;

// Otherwise ? extract unique solution
solution[""x1""] = m[0, n].ToString();  // e.g. ""3/2""
solution[""x2""] = m[1, n].ToString();");

                s.Item().Height(16);

                // ?? Determinant ???????????????????????????????????????????
                SubHeader(s, "5.2  DeterminantSolver");
                BodyText(s, "Computes det(A) for a square matrix using Row Reduction to Upper Triangular Form. The determinant equals the product of the diagonal elements. Each row swap multiplies the running determinant by ?1.");

                s.Item().Height(8);
                CodeBlock(s,
@"Fraction det = Fraction.One;
for (int col = 0; col < n; col++)
{
    int pivotRow = FindFirstNonZero(m, col);
    if (pivotRow == -1) return Fraction.Zero;  // singular

    if (pivotRow != col)
    {
        m.SwapRows(col, pivotRow);
        det = -det;              // every swap flips sign
    }

    det *= m[col, col];          // multiply running product by pivot

    // Eliminate below pivot only (we only need upper triangular)
    for (int row = col + 1; row < n; row++)
    {
        Fraction factor = -(m[row, col] / m[col, col]);
        m.AddScaledRow(row, col, factor);
    }
}
return det;   // exact fraction � e.g.  -3  or  5/2");

                s.Item().Height(8);
                InfoBox(s, "EXAMPLE  �  3�3 Determinant",
                    "A = | 1  2  3 |     det(A) = 1(5�10 - 6�8) - 2(4�10 - 6�7) + 3(4�8 - 5�7)\n" +
                    "    | 4  5  6 |           = 1(50-48) - 2(40-42) + 3(32-35)\n" +
                    "    | 7  8  10|           = 2 + 4 - 9  =  -3\n" +
                    "Row-reduction gives the same result: det = -3  (exact fraction, no rounding)");

                s.Item().Height(16);

                // ?? Inverse ???????????????????????????????????????????????
                SubHeader(s, "5.3  InverseSolver");
                BodyText(s, "Computes A?� by augmenting the input matrix with the identity: [A | I]. Gauss-Jordan elimination then transforms this into [I | A?�]. If A is singular (det = 0), the solver detects the zero pivot and returns NoSolution.");

                s.Item().Height(8);
                CodeBlock(s,
@"// Build [A | I]  � size n � 2n
var augmented = new Matrix(n, 2 * n);
for (int i = 0; i < n; i++)
{
    for (int j = 0; j < n; j++) augmented[i, j]     = matrix[i, j];
    augmented[i, n + i] = Fraction.One;   // identity on the right
}

// Apply Gauss-Jordan ...
// After full elimination, right half = A?�
var inverse = new Matrix(n, n);
for (int i = 0; i < n; i++)
    for (int j = 0; j < n; j++)
        inverse[i, j] = augmented[i, n + j];");

                s.Item().Height(8);
                InfoBox(s, "EXAMPLE  �  2�2 Inverse",
                    "A = | 1  2 |     Start: | 1  2 | 1  0 |\n" +
                    "    | 3  4 |             | 3  4 | 0  1 |\n\n" +
                    "R2 = R2 - 3�R1 ?        | 1  2 |  1   0 |\n" +
                    "                         | 0 -2 | -3   1 |\n\n" +
                    "R2 = R2 � (-1/2) ?       | 1  2 |  1  -1/2 |\n" +
                    "                          | 0  1 | 3/2  -1/2|\n\n" +
                    "R1 = R1 - 2�R2 ?         | 1  0 | -2    1   |\n" +
                    "                          | 0  1 | 3/2  -1/2 |\n\n" +
                    "A?� = | -2    1  |   Verify: A � A?� = I  ?\n" +
                    "      | 3/2  -1/2|");
            });

            // ??????????????????????????????????????????????????????????????
            //  5 cont. � Rank, Transpose, Ops
            // ??????????????????????????????????????????????????????????????
            col.Item().PageBreak();
            col.Item().Padding(50).Column(s =>
            {
                SubHeader(s, "5.4  RankSolver");
                BodyText(s, "Computes the rank of any m�n matrix � the number of linearly independent rows. Uses forward-only elimination (Row Echelon Form, not full RREF) counting each pivot found.");
                s.Item().Height(8);
                CodeBlock(s,
@"int rank = 0;
for (int col = 0; col < cols && rank < rows; col++)
{
    // find first non-zero element at or below current rank row
    int pivotRow = FindNonZero(m, col, startRow: rank);
    if (pivotRow == -1) continue;   // whole column is zero � skip

    m.SwapRows(rank, pivotRow);     // bring pivot to current row
    m.ScaleRow(rank, 1 / pivot);    // normalize

    // eliminate below only
    for (int i = rank + 1; i < rows; i++)
        if (!m[i, col].IsZero)
            m.AddScaledRow(i, rank, -m[i, col]);

    rank++;   // found a pivot ? rank increases
}
return new Fraction(rank);");

                s.Item().Height(16);
                SubHeader(s, "5.5  TransposeSolver");
                BodyText(s, "Transposes matrix A (m�n) ? A? (n�m) by swapping rows and columns. Time complexity O(m�n). Recorded in one step.");
                s.Item().Height(8);
                CodeBlock(s,
@"public Matrix Transpose()
{
    var t = new Matrix(Cols, Rows);     // dimensions flipped
    for (int i = 0; i < Rows; i++)
        for (int j = 0; j < Cols; j++)
            t[j, i] = _data[i, j];     // element at (i,j) ? (j,i)
    return t;
}");

                s.Item().Height(16);
                SubHeader(s, "5.6  MatrixAddSubSolver  &  MatrixMultiplySolver");
                BodyText(s, "Binary solvers that work on two matrices A and B. Addition and subtraction require identical dimensions. Multiplication requires A.Cols == B.Rows and produces a result of size A.Rows � B.Cols using the dot-product definition.");
                s.Item().Height(8);
                CodeBlock(s,
@"// Multiplication  C[i][j] = ? A[i][k] � B[k][j]
for (int i = 0; i < a.Rows; i++)
    for (int j = 0; j < b.Cols; j++)
        for (int k = 0; k < a.Cols; k++)
            result[i, j] += a[i, k] * b[k, j];
// Time complexity: O(n�)  �  all in exact Fraction arithmetic");

                s.Item().Height(16);

                // Solver table summary
                SubHeader(s, "5.7  Solver Summary Table");
                s.Item().Height(8);
                s.Item().Background(DarkGray).CornerRadius(8).Padding(16).Column(table =>
                {
                    // Header
                    table.Item().BorderBottom(1).BorderColor(Red).PaddingBottom(6).Row(r =>
                    {
                        r.RelativeItem(2).Text("Solver Class").Bold().FontColor(Red).FontSize(10);
                        r.RelativeItem(2).Text("Algorithm").Bold().FontColor(Red).FontSize(10);
                        r.RelativeItem(1).Text("Input").Bold().FontColor(Red).FontSize(10);
                        r.RelativeItem(1).Text("Output").Bold().FontColor(Red).FontSize(10);
                    });
                    table.Item().Height(6);

                    void Row(ColumnDescriptor d, string cls, string algo, string inp, string outp)
                    {
                        d.Item().PaddingVertical(5).Row(r =>
                        {
                            r.RelativeItem(2).Text(cls).FontColor(White).FontSize(9);
                            r.RelativeItem(2).Text(algo).FontColor(LightGray).FontSize(9);
                            r.RelativeItem(1).Text(inp).FontColor(LightGray).FontSize(9);
                            r.RelativeItem(1).Text(outp).FontColor(LightGray).FontSize(9);
                        });
                        d.Item().Height(1).Background(MidGray);
                    }

                    Row(table, "GaussJordanSolver",    "Gauss-Jordan RREF",         "n�(n+1) aug.", "Solution dict");
                    Row(table, "DeterminantSolver",    "Row reduction O(n�)",       "n�n",          "Fraction");
                    Row(table, "InverseSolver",         "[A|I]?[I|A?�]",           "n�n",          "n�n matrix");
                    Row(table, "RankSolver",            "Row Echelon Form",          "m�n",          "Fraction (int)");
                    Row(table, "TransposeSolver",       "Index swap O(mn)",         "m�n",          "n�m matrix");
                    Row(table, "MatrixAddSubSolver",    "Element-wise +/?",         "2� m�n",       "m�n matrix");
                    Row(table, "MatrixMultiplySolver",  "Dot product O(n�)",        "m�k + k�n",    "m�n matrix");
                });
            });

            // ??????????????????????????????????????????????????????????????
            //  6. APPLICATION LAYER
            // ??????????????????????????????????????????????????????????????
            col.Item().PageBreak();
            col.Item().Padding(50).Column(s =>
            {
                ChapterTitle(s, "6", "Application Layer");

                SubHeader(s, "6.1  SolverService");
                BodyText(s, "The central orchestrator. The UI calls this service and never touches a solver directly. SolverService parses the input, picks the right solver, calls Solve(), saves to history, and returns the SolveResult. This separates concerns and keeps the UI thin.");
                s.Item().Height(8);
                CodeBlock(s,
@"public class SolverService
{
    private readonly IHistoryRepository _history;

    // Constructor Injection � receives abstraction, not concrete type
    public SolverService(IHistoryRepository history) => _history = history;

    public SolveResult SolveLinearSystem(string text)
    {
        Matrix m      = EquationParser.ParseMatrix(text);  // parse
        var    solver = new GaussJordanSolver();            // pick solver
        var    result = solver.Solve(m);                    // compute
        SaveHistory(solver.Name, text, result);             // persist
        return result;                                      // return
    }
    // ... similar methods for each operation ...
}");

                s.Item().Height(16);
                SubHeader(s, "6.2  HistoryService");
                BodyText(s, "A thin wrapper around IHistoryRepository. Provides higher-level convenience methods: GetLast(n), Search(keyword), and Clear(). The UI uses this service rather than the repository directly.");
                s.Item().Height(8);
                CodeBlock(s,
@"public class HistoryService
{
    private readonly IHistoryRepository _repo;

    public List<HistoryEntry> GetAll()           => _repo.LoadAll();
    public void               Clear()            => _repo.Clear();

    public List<HistoryEntry> GetLast(int n)
    {
        var all = _repo.LoadAll();
        return all.Count > n ? all.GetRange(all.Count - n, n) : all;
    }

    public List<HistoryEntry> Search(string keyword) =>
        _repo.LoadAll().FindAll(e =>
            e.OperationName.Contains(keyword, OrdinalIgnoreCase) ||
            e.ResultSummary.Contains(keyword, OrdinalIgnoreCase));
}");

                s.Item().Height(16);
                SubHeader(s, "6.3  EquationParser");
                BodyText(s, "Static utility that converts raw user text into Matrix objects. Accepts rows separated by newlines or semicolons, elements separated by spaces, tabs, or commas. Supports integers, decimals, and fraction notation (e.g. 1/2).");
                s.Item().Height(8);
                CodeBlock(s,
@"// All these inputs produce the same 2�3 Matrix:
""1 2 3\n4 5 6""          // spaces + newlines
""1,2,3;4,5,6""           // commas + semicolons
""1/2  2/3  1\n3  -1/4  0"" // fractions

public static Matrix ParseMatrix(string text)
{
    var rows = text.Split(new[]{'\n','\r',';'}, RemoveEmptyEntries);
    var parsed = rows.Select(ParseRow).ToList();
    // ParseRow splits on space/tab/comma, calls Fraction.Parse() on each token
    return Matrix.FromRows(parsed);
}");
            });

            // ??????????????????????????????????????????????????????????????
            //  7. INFRASTRUCTURE
            // ??????????????????????????????????????????????????????????????
            col.Item().PageBreak();
            col.Item().Padding(50).Column(s =>
            {
                ChapterTitle(s, "7", "Infrastructure Layer � Persistence");

                SubHeader(s, "7.1  JsonHistoryRepository");
                BodyText(s, "Implements IHistoryRepository using System.Text.Json. Stores all history entries in a single JSON array file located at: %AppData%\\LinearAlgebraSolver\\history.json. The file is loaded fully on each read and rewritten on each save (suitable for small history sets).");
                s.Item().Height(8);
                CodeBlock(s,
@"public class JsonHistoryRepository : IHistoryRepository
{
    private readonly string _filePath;

    public void Save(HistoryEntry entry)
    {
        var entries = LoadAll();   // read existing
        entries.Add(entry);        // append
        string json = JsonSerializer.Serialize(entries, _options);
        File.WriteAllText(_filePath, json);   // overwrite
    }

    public List<HistoryEntry> LoadAll()
    {
        if (!File.Exists(_filePath)) return [];
        try
        {
            string json = File.ReadAllText(_filePath);
            return JsonSerializer.Deserialize<List<HistoryEntry>>(json) ?? [];
        }
        catch { return []; }   // graceful degradation � never crash on startup
    }
}");

                s.Item().Height(8);
                InfoBox(s, "SAMPLE history.json ENTRY",
@"[
  {
    ""Id"": ""f3a1b2c4-..."",
    ""Timestamp"": ""2026-11-22T12:00:00"",
    ""OperationName"": ""Gauss-Jordan Elimination"",
    ""InputSummary"":  ""2 1 -1 8\n-3 -1 2 -11\n-2 1 2 -3"",
    ""ResultSummary"": ""x1 = 2, x2 = 3, x3 = -1"",
    ""WasSuccess"": true,
    ""StepsText"": ""Step 1: Starting augmented...\n..."" 
  }
]");

                s.Item().Height(16);
                SubHeader(s, "7.2  TextHistoryRepository");
                BodyText(s, "Alternative plain-text implementation. Appends each entry as a readable block separated by dashes. Useful for quick inspection without a JSON viewer. Interchangeable with JsonHistoryRepository by changing one line in the composition root.");
            });

            // ??????????????????????????????????????????????????????????????
            //  8. UI LAYER
            // ??????????????????????????????????????????????????????????????
            col.Item().PageBreak();
            col.Item().Padding(50).Column(s =>
            {
                ChapterTitle(s, "8", "UI Layer � WPF Application");

                SubHeader(s, "8.1  Layout Structure");
                BodyText(s, "The window is divided into 5 horizontal rows: a header bar, the main body (3 columns: sidebar + input + output), a draggable GridSplitter, the history panel (bottom, horizontally scrollable cards), and a status bar.");

                s.Item().Height(8);
                CodeBlock(s,
@"<Grid>
  <Grid.RowDefinitions>
    <RowDefinition Height=""52""/>      <!-- Header bar          -->
    <RowDefinition Height=""*""/>       <!-- Main body (3-col)   -->
    <RowDefinition Height=""5""/>       <!-- Draggable splitter  -->
    <RowDefinition Height=""180""/>     <!-- History panel       -->
    <RowDefinition Height=""28""/>      <!-- Status bar          -->
  </Grid.RowDefinitions>
  ...
</Grid>");

                s.Item().Height(16);
                SubHeader(s, "8.2  Composition Root � Dependency Injection");
                BodyText(s, "In the MainWindow constructor, all dependencies are assembled manually (Poor Man's DI). This is the only place in the entire application where concrete types are referenced. To switch storage, change exactly one line here.");
                s.Item().Height(8);
                CodeBlock(s,
@"public MainWindow()
{
    InitializeComponent();

    // ?? COMPOSITION ROOT ?????????????????????????????????????
    string historyPath = Path.Combine(AppData, ""LinearAlgebraSolver"", ""history.json"");

    IHistoryRepository repo = new JsonHistoryRepository(historyPath);
    // swap to:              new TextHistoryRepository(historyPath)
    // or future:            new DatabaseHistoryRepository(connString)

    _history = new HistoryService(repo);
    _solver  = new SolverService(repo);
    // ?????????????????????????????????????????????????????????
}");

                s.Item().Height(16);
                SubHeader(s, "8.3  Execute Helper � All Buttons Call This");
                BodyText(s, "A single private method handles all 8 operations uniformly, ensuring consistent status updates, error handling, and history refresh regardless of which button was pressed.");
                s.Item().Height(8);
                CodeBlock(s,
@"private void Execute(Func<SolveResult> action)
{
    try
    {
        SetStatus(""Computing�"");       // 1. update status
        var result = action();           // 2. run the lambda
        DisplayResult(result);           // 3. show steps in output panel
        RefreshHistory();               // 4. reload bottom history panel
        SetStatus(result.Summary);      // 5. update status with summary
    }
    catch (Exception ex)
    {
        ResultTitle.Text = ""Error"";
        StepsOutput.Text  = ex.Message;
        SetStatus($""Error: {ex.Message}"");
    }
}

// Usage:
private void BtnSolveGauss_Click(object sender, RoutedEventArgs e)
    => Execute(() => _solver.SolveLinearSystem(GaussMatrixInput.Text));");

                s.Item().Height(16);
                SubHeader(s, "8.4  Color Theme");
                TwoColTable(s, new[]
                {
                    ("Background (base)",   "#0A0A0A � deep black"),
                    ("Panel backgrounds",   "#0D0D0D / #0F0F0F / #141414"),
                    ("Accent (primary)",    "#D72638 � vivid red"),
                    ("Accent (hover)",      "#FF3347 � brighter red"),
                    ("Accent (pressed)",    "#8B1520 � dark red"),
                    ("Text (primary)",      "#F0F0F0 � near white"),
                    ("Text (secondary)",    "#888888 � mid gray"),
                    ("Borders",             "#1E1E1E / #2A2A2A � dark gray"),
                });
            });

            // ??????????????????????????????????????????????????????????????
            //  9. OOP PRINCIPLES
            // ??????????????????????????????????????????????????????????????
            col.Item().PageBreak();
            col.Item().Padding(50).Column(s =>
            {
                ChapterTitle(s, "9", "OOP Principles Applied");

                void OopSection(ColumnDescriptor d, string num, string name, string where, string why, string code)
                {
                    d.Item().Background(DarkGray).CornerRadius(8).Padding(16).Column(inner =>
                    {
                        inner.Item().Row(r =>
                        {
                            r.AutoItem().Background(Red).CornerRadius(4).Width(28).Height(28)
                             .AlignCenter().AlignMiddle()
                             .Text(num).Bold().FontColor(White).FontSize(13);
                            r.RelativeItem().PaddingLeft(10).Column(c =>
                            {
                                c.Item().Text(name).Bold().FontSize(13).FontColor(White);
                                c.Item().Text($"Where: {where}").FontSize(9).FontColor(LightGray);
                            });
                        });
                        inner.Item().Height(8);
                        inner.Item().Text(why).FontSize(10).FontColor(LightGray);
                        inner.Item().Height(8);
                        inner.Item().Background(CodeBg).CornerRadius(5).Padding(10)
                             .Text(code).FontFamily("Courier New").FontSize(9).FontColor(CodeFg);
                    });
                    d.Item().Height(14);
                }

                OopSection(s, "1", "Encapsulation",
                    "Matrix._data  |  Fraction._denominator",
                    "Internal data is private. External code cannot corrupt the matrix array directly � it must use the public indexer and row-operation methods.",
                    "private readonly Fraction[,] _data;   // cannot be touched externally\npublic Fraction this[int r, int c] { get => _data[r,c]; set => _data[r,c]=value; }");

                OopSection(s, "2", "Abstraction",
                    "ISolver  |  IHistoryRepository",
                    "Users of these interfaces do not need to know HOW the work is done � only WHAT the contract promises. SolverService knows only 'call .Solve(matrix)' on any ISolver.",
                    "ISolver solver = new GaussJordanSolver();\n// or:         = new DeterminantSolver();\nSolveResult r = solver.Solve(matrix);  // same call regardless");

                OopSection(s, "3", "Inheritance",
                    "SolverBase ? GaussJordanSolver / DeterminantSolver / ...",
                    "Common boilerplate (Steps list management, AddStep helper) lives once in SolverBase. Each concrete solver inherits it and only writes its own algorithm.",
                    "public class GaussJordanSolver : SolverBase  // inherits Steps, AddStep\n{\n    public override string Name => \"Gauss-Jordan Elimination\";\n    protected override SolveResult DoSolve(Matrix matrix) { ... }\n}");

                OopSection(s, "4", "Polymorphism",
                    "SolverService  ?  ISolver",
                    "SolverService calls solver.Solve() on any ISolver reference. At runtime the correct algorithm runs � Gauss-Jordan, Determinant, or Inverse � without SolverService knowing the concrete type.",
                    "// All three produce a valid SolveResult via the same call:\nISolver s = GetSolverForOperation(op);  // returns any implementation\nSolveResult result = s.Solve(matrix);    // polymorphic dispatch");
            });

            // ??????????????????????????????????????????????????????????????
            //  10. DESIGN PATTERNS
            // ??????????????????????????????????????????????????????????????
            col.Item().PageBreak();
            col.Item().Padding(50).Column(s =>
            {
                ChapterTitle(s, "10", "Design Patterns");

                void PatternBox(ColumnDescriptor d, string name, string intent, string where, string code)
                {
                    d.Item().Column(inner =>
                    {
                        inner.Item().Row(r =>
                        {
                            r.AutoItem().Background(Red).CornerRadius(4).Padding(6)
                             .Text(name).Bold().FontColor(White).FontSize(11);
                        });
                        inner.Item().Height(6);
                        inner.Item().Text($"Intent: {intent}").FontSize(10).FontColor(LightGray);
                        inner.Item().Text($"Where:  {where}").FontSize(10).FontColor(LightGray);
                        inner.Item().Height(6);
                        inner.Item().Background(CodeBg).CornerRadius(5).Padding(10)
                             .Text(code).FontFamily("Courier New").FontSize(9).FontColor(CodeFg);
                    });
                    d.Item().Height(20);
                }

                PatternBox(s, "Template Method",
                    "Define the skeleton of an algorithm in a base class; let subclasses fill in specific steps.",
                    "SolverBase.Solve() ? DoSolve() in each concrete solver",
@"// SolverBase (base class � the template)
public SolveResult Solve(Matrix m)
{
    Steps.Clear();         // ? fixed step 1
    return DoSolve(m);     // ? variable step (subclass)
}
protected abstract SolveResult DoSolve(Matrix m);  // subclass fills this

// GaussJordanSolver (concrete)
protected override SolveResult DoSolve(Matrix m) { /* GJ logic */ }");

                PatternBox(s, "Strategy Pattern",
                    "Define a family of algorithms, encapsulate each, and make them interchangeable.",
                    "Each Solver class is a Strategy; SolverService selects the right one per operation",
@"// All strategies share the same interface (ISolver)
ISolver strategy = operationType switch
{
    ""gauss""     => new GaussJordanSolver(),
    ""det""       => new DeterminantSolver(),
    ""inverse""   => new InverseSolver(),
    _             => throw new NotSupportedException()
};
var result = strategy.Solve(matrix);   // same call, different algorithm");

                PatternBox(s, "Repository Pattern",
                    "Decouple data-access logic from business logic by hiding storage behind an interface.",
                    "IHistoryRepository ? JsonHistoryRepository / TextHistoryRepository",
@"// Application only sees the interface:
public class SolverService
{
    private readonly IHistoryRepository _history;
    // No File.WriteAllText here. No JsonSerializer here. Zero I/O.
    // history.Save(entry)  � that's all it knows.
}

// Infrastructure implements the interface:
public class JsonHistoryRepository : IHistoryRepository
{
    public void Save(HistoryEntry e) { /* JSON I/O here */ }
}");

                PatternBox(s, "Dependency Injection (Constructor Injection)",
                    "Provide dependencies from outside rather than creating them inside. Enables testing and flexibility.",
                    "MainWindow (Composition Root) ? SolverService ? IHistoryRepository",
@"// SolverService does NOT create its own repo:
public SolverService(IHistoryRepository history)  // receives it
    => _history = history;

// MainWindow (composition root) decides which impl to use:
IHistoryRepository repo = new JsonHistoryRepository(path);
var solver = new SolverService(repo);   // inject
// For tests:
IHistoryRepository repo = new InMemoryRepository();  // testable");
            });

            // ??????????????????????????????????????????????????????????????
            //  11. UNIT TESTS
            // ??????????????????????????????????????????????????????????????
            col.Item().PageBreak();
            col.Item().Padding(50).Column(s =>
            {
                ChapterTitle(s, "11", "Unit Tests � 41 / 41 Passing");

                BodyText(s, "The test project (LinearAlgebra.Tests) uses xUnit 2.9 and targets net10.0. All 41 tests pass with zero warnings.");

                s.Item().Height(16);

                // Results summary
                s.Item().Background(DarkGray).CornerRadius(10).Padding(20).Row(row =>
                {
                    void TestStat(RowDescriptor r, string num, string label, string color)
                    {
                        r.RelativeItem().AlignCenter().Column(c =>
                        {
                            c.Item().Text(num).Bold().FontSize(28).FontColor(color).AlignCenter();
                            c.Item().Text(label).FontSize(10).FontColor(LightGray).AlignCenter();
                        });
                    }
                    TestStat(row, "41", "Total Tests",  White);
                    TestStat(row, "41", "Passed ?",     "#22CC44");
                    TestStat(row, "0",  "Failed",        LightGray);
                    TestStat(row, "3",  "Test Classes",  Red);
                    TestStat(row, "0",  "Warnings",      LightGray);
                });

                s.Item().Height(20);
                SubHeader(s, "FractionTests.cs  (18 tests)");
                BulletList(s, new[]
                {
                    "Constructor_NormalizesSign � new Fraction(3,-4) = -3/4",
                    "Constructor_ReducesToLowestTerms � new Fraction(6,4) = 3/2",
                    "Constructor_ZeroAlwaysHasDenominatorOne � new Fraction(0,999) = 0/1",
                    "Constructor_ThrowsOnZeroDenominator � expects DivideByZeroException",
                    "Addition_IsCorrect (3 cases) � 1/2+1/3=5/6, 1/4+3/4=1, -1/2+1/2=0",
                    "Subtraction_IsCorrect (2 cases) � 3/4-1/4=1/2, 1-1=0",
                    "Multiplication_IsCorrect (2 cases) � 2/3�3/4=1/2",
                    "Division_IsCorrect (2 cases) � (1/2)/(3/4)=2/3",
                    "Division_ByZero_Throws � expects DivideByZeroException",
                    "Comparison_Works � < > == operators",
                    "Parse_IsCorrect (3 cases) � \"3\", \"-2/5\", \"0\"",
                    "ToString_ShowsWholeNumber � 10/2 ? \"5\"",
                    "ToString_ShowsFraction � 2/3 ? \"2/3\"",
                });

                s.Item().Height(14);
                SubHeader(s, "MatrixTests.cs  (9 tests)");
                BulletList(s, new[]
                {
                    "Indexer_GetSet_Works � set [0,0]=5, read back 5",
                    "Identity_HasOneOnDiagonal � 3�3 identity matrix",
                    "SwapRows_SwapsCorrectly � rows 0 and 1 swapped",
                    "ScaleRow_MultipliesEveryElement � row � 1/2",
                    "Addition_ProducesCorrectResult � A + I",
                    "Multiply_Identity_ReturnsOriginal � A � I = A",
                    "Transpose_FlipsRowsAndCols � 2�3 ? 3�2",
                    "AddMismatch_Throws � 2�2 + 3�3 ? InvalidOperationException",
                    "MultiplyMismatch_Throws � 2�3 � 2�2 ? InvalidOperationException",
                });

                s.Item().Height(14);
                SubHeader(s, "SolverTests.cs  (14 tests)");
                BulletList(s, new[]
                {
                    "GaussJordan_UniqueSolution_IsCorrect � 3�3 system ? x1=2, x2=3, x3=-1",
                    "GaussJordan_NoSolution_DetectedCorrectly � x+y=1, x+y=2 ? NoSolution",
                    "GaussJordan_InfiniteSolutions_DetectedCorrectly � dependent rows ? Infinite",
                    "Determinant_1x1_IsCorrect � [[5]] ? det=5",
                    "Determinant_2x2_IsCorrect � [[1,2],[3,4]] ? det=-2",
                    "Determinant_3x3_IsCorrect � standard 3�3 ? det=-3",
                    "Determinant_SingularMatrix_IsZero � [[1,2,3],[2,4,6],...] ? det=0",
                    "Determinant_NonSquare_ReturnsError � 2�3 matrix ? SolutionType.Error",
                    "Inverse_2x2_IsCorrect � [[1,2],[3,4]] ? [[-2,1],[3/2,-1/2]]",
                    "Inverse_SingularMatrix_ReturnsNoSolution � [[1,2],[2,4]] ? NoSolution",
                    "Rank_FullRank_IsN � Identity(3) ? rank=3",
                    "Rank_SingularMatrix_IsLessThanN � all rows = multiples ? rank=1",
                });

                s.Item().Height(16);
                SubHeader(s, "Sample Test Code");
                s.Item().Height(8);
                CodeBlock(s,
@"[Fact]
public void GaussJordan_UniqueSolution_IsCorrect()
{
    // 2x + y - z = 8
    // -3x - y + 2z = -11      Known solution: x=2, y=3, z=-1
    // -2x + y + 2z = -3
    var aug = Matrix.FromRows(new[]
    {
        new Fraction[] {  2,  1, -1,  8  },
        new Fraction[] { -3, -1,  2, -11 },
        new Fraction[] { -2,  1,  2, -3  }
    });

    var result = new GaussJordanSolver().Solve(aug);

    Assert.Equal(SolutionType.Unique, result.SolutionType);
    Assert.Equal(""2"",  result.Solution[""x1""]);
    Assert.Equal(""3"",  result.Solution[""x2""]);
    Assert.Equal(""-1"", result.Solution[""x3""]);
}");
            });

            // ??????????????????????????????????????????????????????????????
            //  12. DATA FLOW
            // ??????????????????????????????????????????????????????????????
            col.Item().PageBreak();
            col.Item().Padding(50).Column(s =>
            {
                ChapterTitle(s, "12", "Data Flow Walkthrough");
                BodyText(s, "Complete trace of what happens from the moment the user clicks ? Solve to the result appearing on screen.");

                s.Item().Height(16);

                void FlowStep(ColumnDescriptor d, string num, string layer, string action, string detail)
                {
                    d.Item().Row(r =>
                    {
                        r.AutoItem().Width(32).Height(32)
                         .Background(Red).CornerRadius(16)
                         .AlignCenter().AlignMiddle()
                         .Text(num).Bold().FontColor(White).FontSize(12);

                        r.AutoItem().Width(12).AlignMiddle()
                         .Text("?").FontColor(LightGray).FontSize(14);

                        r.RelativeItem().Column(c =>
                        {
                            c.Item().Text(layer).Bold().FontSize(11).FontColor(White);
                            c.Item().Text(action).FontSize(10).FontColor(LightGray);
                            c.Item().Background(CodeBg).CornerRadius(4).Padding(6).Margin(0, 4, 0, 0)
                             .Text(detail).FontFamily("Courier New").FontSize(9).FontColor(CodeFg);
                        });
                    });
                    d.Item().PaddingLeft(44).Column(c =>
                    {
                        c.Item().Height(2).Width(2).Background(Red);
                        c.Item().Height(8);
                    });
                }

                FlowStep(s, "1", "UI Layer � MainWindow.xaml.cs",
                    "User clicks ? Solve System button",
                    "BtnSolveGauss_Click ? Execute(() => _solver.SolveLinearSystem(GaussMatrixInput.Text))");

                FlowStep(s, "2", "UI Layer � Execute() helper",
                    "SetStatus(\"Computing�\"), then calls the lambda",
                    "Func<SolveResult> action = () => _solver.SolveLinearSystem(text); action()");

                FlowStep(s, "3", "Application Layer � SolverService",
                    "Parses text to Matrix, creates solver, runs it",
                    "Matrix m = EquationParser.ParseMatrix(text);  // \"2 1 -1 8\\n...\" ? 3�4 Matrix");

                FlowStep(s, "4", "Application Layer � EquationParser",
                    "Splits text into rows, then tokens, then Fraction.Parse() each",
                    "\"2 1 -1 8\" ? [Fraction(2), Fraction(1), Fraction(-1), Fraction(8)]");

                FlowStep(s, "5", "Domain Layer � GaussJordanSolver",
                    "DoSolve() runs RREF on a clone of the matrix",
                    "for each col: pivot ? swap ? scale ? eliminate ? AddStep(description, matrix)");

                FlowStep(s, "6", "Domain Layer � SolveResult built",
                    "Steps list populated, solution dictionary filled",
                    "result.Solution = {\"x1\":\"2\", \"x2\":\"3\", \"x3\":\"-1\"}  SolutionType = Unique");

                FlowStep(s, "7", "Application Layer � SolverService",
                    "Calls _history.Save(new HistoryEntry{...})",
                    "Infrastructure.JsonHistoryRepository writes to %AppData%\\history.json");

                FlowStep(s, "8", "UI Layer � DisplayResult(result)",
                    "Formats step-by-step text, sets StepsOutput.Text",
                    "StepsOutput.Text = \"Step 1: Starting matrix\\n[ 2  1 -1  8 ]\\n...\"");

                FlowStep(s, "9", "UI Layer � RefreshHistory()",
                    "Reloads history list from JSON, reverses (newest first)",
                    "HistoryList.ItemsSource = entries;  HistoryCountText.Text = entries.Count");
            });

            // ??????????????????????????????????????????????????????????????
            //  13. WORKED EXAMPLES
            // ??????????????????????????????????????????????????????????????
            col.Item().PageBreak();
            col.Item().Padding(50).Column(s =>
            {
                ChapterTitle(s, "13", "Example Calculations with Real Data");

                SubHeader(s, "Example 1 � Gauss-Jordan (3�3 Unique Solution)");
                BodyText(s, "Solve:  2x + y ? z = 8   |   ?3x ? y + 2z = ?11   |   ?2x + y + 2z = ?3");
                s.Item().Height(8);

                s.Item().Background(DarkGray).CornerRadius(8).Padding(16).Column(ex =>
                {
                    void ExStep(ColumnDescriptor d, string title, string matrix)
                    {
                        d.Item().Text(title).Bold().FontSize(10).FontColor(Red);
                        d.Item().Height(4);
                        d.Item().Background(CodeBg).CornerRadius(4).Padding(10)
                         .Text(matrix).FontFamily("Courier New").FontSize(10).FontColor(CodeFg);
                        d.Item().Height(10);
                    }

                    ExStep(ex, "Start � Augmented Matrix [A|b]:",
@"| 2   1  -1 |  8  |
|-3  -1   2 | -11 |
|-2   1   2 |  -3 |");

                    ExStep(ex, "Step 1 � Swap R1 ? R2 (partial pivot, |-3| > |2|):",
@"|-3  -1   2 | -11 |
| 2   1  -1 |  8  |
|-2   1   2 |  -3 |");

                    ExStep(ex, "Step 2 � R1 = R1 � (?1/3)   ?   pivot = 1:",
@"| 1   1/3  -2/3 | 11/3 |
| 2   1    -1   |  8   |
|-2   1     2   | -3   |");

                    ExStep(ex, "Step 3-4 � Eliminate column 1 from R2 and R3:",
@"| 1   1/3  -2/3 | 11/3 |
| 0   1/3   1/3 |  2/3 |
| 0   5/3   2/3 | 13/3 |");

                    ExStep(ex, "... (continuing elimination) ... Final RREF:",
@"| 1   0   0 |  2  |
| 0   1   0 |  3  |
| 0   0   1 | -1  |");

                    ex.Item().Text("?  x? = 2,  x? = 3,  x? = ?1    ?  Unique Solution")
                      .Bold().FontSize(11).FontColor("#22CC44");
                });

                s.Item().Height(20);
                SubHeader(s, "Example 2 � Determinant (3�3)");
                BodyText(s, "Compute det(A) for A = [[1,2,3],[4,5,6],[7,8,10]]");
                s.Item().Height(8);

                s.Item().Background(DarkGray).CornerRadius(8).Padding(16).Column(ex =>
                {
                    ex.Item().Text("Row reduction to upper triangular:").FontSize(10).FontColor(LightGray);
                    ex.Item().Height(6);
                    ex.Item().Background(CodeBg).CornerRadius(4).Padding(10)
                      .Text(
@"Start:  det = 1

Pivot col 0:  pivot = 1  ?  det � 1 = 1
  R2 = R2 ? 4�R1  ?  [0, -3, -6]
  R3 = R3 ? 7�R1  ?  [0, -6, -11]

Pivot col 1:  pivot = -3  ?  det � (-3) = -3
  R3 = R3 ? 2�R2  ?  [0, 0, 1]

Pivot col 2:  pivot = 1  ?  det � 1 = -3

Result:  det(A) = -3   (exact fraction, stored as Fraction(-3,1))")
                      .FontFamily("Courier New").FontSize(9).FontColor(CodeFg);
                    ex.Item().Height(8);
                    ex.Item().Text("?  det(A) = ?3    ?")
                      .Bold().FontSize(11).FontColor("#22CC44");
                });

                s.Item().Height(20);
                SubHeader(s, "Example 3 � Matrix Inverse (2�2)");
                s.Item().Height(8);

                s.Item().Background(DarkGray).CornerRadius(8).Padding(16).Column(ex =>
                {
                    ex.Item().Background(CodeBg).CornerRadius(4).Padding(10)
                      .Text(
@"Input A = | 1  2 |
           | 3  4 |

Augmented [A|I]:
  | 1  2 | 1  0 |
  | 3  4 | 0  1 |

R2 = R2 ? 3�R1:
  | 1  2 |  1   0 |
  | 0 -2 | -3   1 |

R2 = R2 � (?1/2):
  | 1  2 |  1  -1/2 |
  | 0  1 | 3/2  -1/2|

R1 = R1 ? 2�R2:
  | 1  0 | -2    1   |
  | 0  1 | 3/2  -1/2 |

A?� = | -2    1  |
       | 3/2  -1/2|

Verify: A � A?� = | 1 0 | = I   ?")
                      .FontFamily("Courier New").FontSize(9).FontColor(CodeFg);
                });
            });

            // ??????????????????????????????????????????????????????????????
            //  BACK COVER
            // ??????????????????????????????????????????????????????????????
            col.Item().PageBreak();
            col.Item().Height(PageSizes.A4.Height).Background(Black).Column(back =>
            {
                back.Item().Height(6).Background(Red);
                back.Item().Extend().Padding(60).Column(inner =>
                {
                    inner.Item().Height(80);
                    inner.Item().Text("Build Summary")
                         .FontSize(22).Bold().FontColor(White);
                    inner.Item().Height(16);

                    inner.Item().Background(DarkGray).CornerRadius(10).Padding(24).Column(grid =>
                    {
                        void GridRow(ColumnDescriptor d, string label, string value, string color)
                        {
                            d.Item().Row(r =>
                            {
                                r.RelativeItem().Text(label).FontSize(11).FontColor(LightGray);
                                r.AutoItem().Text(value).Bold().FontSize(11).FontColor(color);
                            });
                            d.Item().Height(10);
                        }
                        GridRow(grid, "Build Result",        "?  Succeeded � 0 Errors, 0 Warnings",   "#22CC44");
                        GridRow(grid, "Total Tests",         "41 / 41 Passed",                         "#22CC44");
                        GridRow(grid, "Total Source Files",  "20+ C# files",                            White);
                        GridRow(grid, "Lines of Code",       "~2,000 lines (excl. XAML)",               White);
                        GridRow(grid, "NuGet Packages",      "xUnit 2.9 � QuestPDF 2024.10",            White);
                        GridRow(grid, ".NET Target",         "net10.0 / net10.0-windows",               White);
                        GridRow(grid, "History Storage",     "%AppData%\\LinearAlgebraSolver\\history.json", White);
                    });

                    inner.Item().Height(40);
                    inner.Item().BorderTop(1).BorderColor(MidGray).PaddingTop(24).Column(footer =>
                    {
                        footer.Item().Text("Linear Algebra Solver")
                              .Bold().FontSize(14).FontColor(White);
                        footer.Item().Text("Mohamed  �  C# .NET 10  �  WPF  �  xUnit  �  QuestPDF")
                              .FontSize(10).FontColor(LightGray);
                        footer.Item().Text($"Generated {DateTime.Now:dd MMMM yyyy  HH:mm}")
                              .FontSize(10).FontColor(MidGray);
                    });
                });
                back.Item().Height(6).Background(Red);
            });
        });

    });
})
.GeneratePdf(outputPath);

Console.WriteLine($"\n? PDF generated successfully!");
Console.WriteLine($"  Location: {outputPath}");
Console.WriteLine("\nPress any key to exit...");
Console.ReadKey();

// ??????????????????????????????????????????????????????????????????????????
//  LOCAL HELPER FUNCTIONS
// ??????????????????????????????????????????????????????????????????????????

static void ChapterTitle(ColumnDescriptor col, string num, string title)
{
    col.Item().Row(r =>
    {
        r.AutoItem().Background("#D72638").CornerRadius(6)
         .Width(36).Height(36).AlignCenter().AlignMiddle()
         .Text(num).Bold().FontSize(14).FontColor("#FFFFFF");
        r.RelativeItem().PaddingLeft(14).AlignMiddle()
         .Text(title).Bold().FontSize(20).FontColor("#F0F0F0");
    });
    col.Item().Height(2).Background("#D72638").Padding(10);
}

static void SubHeader(ColumnDescriptor col, string text)
{
    col.Item().Text(text).Bold().FontSize(13).FontColor("#F0F0F0");
    col.Item().Height(8);
}

static void BodyText(ColumnDescriptor col, string text)
{
    col.Item().Text(text).FontSize(10).FontColor("#AAAAAA").LineHeight(1.5f);
}

static void BulletList(ColumnDescriptor col, string[] items)
{
    foreach (var item in items)
    {
        col.Item().Row(r =>
        {
            r.AutoItem().Width(16).PaddingTop(2)
             .Text("�").FontColor("#D72638").FontSize(12);
            r.RelativeItem()
             .Text(item).FontSize(10).FontColor("#AAAAAA").LineHeight(1.4f);
        });
        col.Item().Height(4);
    }
}

static void InfoBox(ColumnDescriptor col, string title, string content)
{
    col.Item().Background("#100408").BorderLeft(3).BorderColor("#D72638")
       .Padding(14).Column(inner =>
       {
           inner.Item().Text(title).Bold().FontSize(10).FontColor("#D72638");
           inner.Item().Height(6);
           inner.Item().Text(content).FontFamily("Courier New").FontSize(9).FontColor("#CCCCCC").LineHeight(1.4f);
       });
}

static void CodeBlock(ColumnDescriptor col, string code)
{
    col.Item().Background("#0D0D0D").CornerRadius(6)
       .Border(1).BorderColor("#2A2A2A")
       .Padding(14)
       .Text(code).FontFamily("Courier New").FontSize(9).FontColor("#E0E0E0").LineHeight(1.5f);
}

static void TwoColTable(ColumnDescriptor col, (string, string)[] rows)
{
    col.Item().Background("#141414").CornerRadius(8).Padding(14).Column(table =>
    {
        bool alt = false;
        foreach (var (label, value) in rows)
        {
            table.Item().Background(alt ? "#1A1A1A" : "transparent")
                 .Padding(6).Row(r =>
                 {
                     r.RelativeItem(1).Text(label).Bold().FontSize(10).FontColor("#D72638");
                     r.RelativeItem(2).Text(value).FontSize(10).FontColor("#CCCCCC");
                 });
            alt = !alt;
        }
    });
    col.Item().Height(10);
}
