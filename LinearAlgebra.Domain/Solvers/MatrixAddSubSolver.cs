using System;
using System.Collections.Generic;
using LinearAlgebra.Domain.Enums;
using LinearAlgebra.Domain.Models;

namespace LinearAlgebra.Domain.Solvers;

/// <summary>
/// Adds or subtracts two matrices of equal dimensions.
/// </summary>
public class MatrixAddSubSolver : SolverBase
{
    private readonly bool _isAddition;

    public MatrixAddSubSolver(bool isAddition = true)
    {
        _isAddition = isAddition;
    }

    public override string Name => _isAddition ? "Matrix Addition" : "Matrix Subtraction";

    protected override SolveResult DoSolve(Matrix matrix)
    {
        throw new InvalidOperationException(
            "Use Solve(Matrix a, Matrix b) overload for binary operations.");
    }

    public SolveResult Solve(Matrix a, Matrix b)
    {
        Steps.Clear();
        if (a.Rows != b.Rows || a.Cols != b.Cols)
            return new SolveResult
            {
                Title        = "Error",
                Summary      = $"Cannot {(_isAddition ? "add" : "subtract")}: dimensions {a.Rows}×{a.Cols} ≠ {b.Rows}×{b.Cols}.",
                SolutionType = SolutionType.Error,
                Steps        = new List<SolutionStep>(Steps)
            };

        AddStep($"Matrix A ({a.Rows}×{a.Cols}):", a);
        AddStep($"Matrix B ({b.Rows}×{b.Cols}):", b);

        Matrix result = _isAddition ? a + b : a - b;
        string op = _isAddition ? "A + B" : "A - B";

        AddStep($"Result {op}:", result);

        return new SolveResult
        {
            Title        = Name,
            Summary      = $"{op} computed for {a.Rows}×{a.Cols} matrices.",
            SolutionType = SolutionType.Value,
            Steps        = new List<SolutionStep>(Steps),
            ResultMatrix = result
        };
    }
}
