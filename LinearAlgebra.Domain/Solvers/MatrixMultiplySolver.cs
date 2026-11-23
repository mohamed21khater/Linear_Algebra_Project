using System;
using System.Collections.Generic;
using LinearAlgebra.Domain.Enums;
using LinearAlgebra.Domain.Models;

namespace LinearAlgebra.Domain.Solvers;

/// <summary>
/// Multiplies two compatible matrices (A×B).
/// </summary>
public class MatrixMultiplySolver : SolverBase
{
    public override string Name => "Matrix Multiplication";

    protected override SolveResult DoSolve(Matrix matrix)
    {
        throw new InvalidOperationException("Use Solve(Matrix a, Matrix b).");
    }

    public SolveResult Solve(Matrix a, Matrix b)
    {
        Steps.Clear();
        if (a.Cols != b.Rows)
            return new SolveResult
            {
                Title        = "Error",
                Summary      = $"Cannot multiply: A is {a.Rows}×{a.Cols}, B is {b.Rows}×{b.Cols} (A.Cols ≠ B.Rows).",
                SolutionType = SolutionType.Error,
                Steps        = new List<SolutionStep>(Steps)
            };

        AddStep($"Matrix A ({a.Rows}×{a.Cols}):", a);
        AddStep($"Matrix B ({b.Rows}×{b.Cols}):", b);
        AddStep($"Result C = A×B will be ({a.Rows}×{b.Cols}).");

        Matrix result = a * b;

        AddStep("C = A × B:", result);

        return new SolveResult
        {
            Title        = Name,
            Summary      = $"A({a.Rows}×{a.Cols}) × B({b.Rows}×{b.Cols}) = C({a.Rows}×{b.Cols})",
            SolutionType = SolutionType.Value,
            Steps        = new List<SolutionStep>(Steps),
            ResultMatrix = result
        };
    }
}
