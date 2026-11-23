Linear Algebra SolverA C# / .NET 10 / WPF desktop application for solving linear algebra problems with 100% exact fraction arithmetic, step-by-step explanations, and persistent operation history.  
Built around a 4-layer architecture, Object-Oriented Programming (OOP) principles, and Design Patterns to keep mathematical logic decoupled from the user interface and storage layer.  
Why Exact Fractions Instead of Floating-Point?
Standard floating-point math can introduce rounding drift across repeated operations 
(for example, decimals not behaving as exact rational numbers during multi-step matrix reductions).  
To avoid floating-point rounding errors, the project uses a custom Fraction value type (readonly struct) that stores simplified rational numbers (numerator / denominator): 
Automatic GCD Simplification: Automatically reduces fractions (such as 6/4 → 3/2) via Greatest Common Divisor (GCD). 
Safe Default Struct Handling: Returns a denominator of 1 when _denominator == 0 so default(Fraction) remains safe.  
Operator Overloading & Parsing: Supports +, -, *, and /, and parses integers, fractions (e.g., -2/5), and decimals (e.g., 0.75).  
Features & OperationsGauss-Jordan Elimination: Solves systems of linear equations from augmented matrices [A | b] with partial pivoting and classifies unique, infinite, and no-solution cases.  
Determinant: Computes determinants via row reduction.   Matrix Inverse: Computes A⁻¹ using the augmented [A | I] approach.  
Rank & Transpose: Calculates matrix rank via Row Echelon Form and computes matrix transposes.  
Matrix Arithmetic (A ⊕ B): Supports addition, subtraction, and multiplication of matrices.  
Step-by-Step Output: Displays intermediate transformations and row operations clearly. 
Persistent History: Saves operation history to disk (JsonHistoryRepository / TextHistoryRepository) with graceful fallback if a history file is corrupted.  
ArchitectureThe solution is organized into 4 layers so domain logic has no dependency on UI or infrastructure: 
Plaintext
┌─────────────────────────────────────────────┐
│          UI Layer  (WPF)                    │
│  MainWindow.xaml + MainWindow.xaml.cs       │
└──────────────────┬──────────────────────────┘
                   │ calls
                   ▼
┌─────────────────────────────────────────────┐
│          Application Layer                  │
│   SolverService + HistoryService            │
│   EquationParser                            │
└────────┬───────────────────────┬────────────┘
         │                       │
         ▼                       ▼
┌──────────────────┐   ┌──────────────────────┐
│   Domain Layer   │   │  Infrastructure      │
│  Fraction        │   │  JsonHistoryRepo     │
│  Matrix          │   │  TextHistoryRepo     │
│  Solvers         │   └──────────────────────┘
│  Interfaces      │
└──────────────────┘
OOP & Design Patterns UsedEncapsulation & Abstraction: Private internal data in Matrix (Fraction[,]) exposed through indexers and row operations, paired with ISolver and IHistoryRepository interfaces. 
Template Method (SolverBase):
Centralizes common execution steps in Solve(Matrix) while delegating solver-specific logic to DoSolve(Matrix). 
Strategy & Polymorphism: SolverService executes ISolver implementations dynamically based on the selected operation.  
Repository & Dependency Injection:
SolverService and HistoryService depend on IHistoryRepository injected at the composition root in MainWindow.  
Getting StartedPrerequisites.NET 10 SDKWindows (for running the WPF desktop application)
Run the ApplicationPowerShelldotnet run --project LinearAlgebra.UI
Run Unit TestsPowerShelldotnet test LinearAlgebra.Tests
The test suite includes 41 unit tests covering Fraction arithmetic and edge cases, Matrix operations, and solver outputs. 
Publish Release BuildPowerShelldotnet publish LinearAlgebra.UI -c Release -o publish/
The published executable is output to publish/LinearAlgebraSolver.exe. 
History Storage LocationBy default, JSON history is stored at:PlaintextC:\Users\{username}\AppData\Roaming\LinearAlgebraSolver\history.json
