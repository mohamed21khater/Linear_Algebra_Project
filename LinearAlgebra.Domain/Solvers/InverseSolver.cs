using System.Collections.Generic;
using LinearAlgebra.Domain.Enums;
using LinearAlgebra.Domain.Models;

namespace LinearAlgebra.Domain.Solvers;

/// <summary>
/// Computes the inverse of a square matrix using Gauss-Jordan augmented with identity:
///   [ A | I ]  →  [ I | A⁻¹ ]
/// </summary>
public class InverseSolver : SolverBase
{
    public override string Name => "Matrix Inverse";

    protected override SolveResult DoSolve(Matrix matrix)
    {
        if (!matrix.IsSquare)
            return Error("Inverse is only defined for square matrices.");

        int n = matrix.Rows;

        // Build augmented [A | I]
        var augmented = new Matrix(n, 2 * n);
        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < n; j++)
                augmented[i, j] = matrix[i, j];
            augmented[i, n + i] = Fraction.One;
        }

        AddStep("Build augmented matrix [A | I]:", augmented);

        // Gauss-Jordan on augmented
        for (int col = 0; col < n; col++)
        {
            // Partial pivot
            int pivotRow = -1;
            Fraction bestAbs = Fraction.Zero;
            for (int i = col; i < n; i++)
            {
                Fraction abs = augmented[i, col].Abs();
                if (abs > bestAbs) { bestAbs = abs; pivotRow = i; }
            }

            if (pivotRow == -1 || augmented[pivotRow, col].IsZero)
            {
                AddStep("Zero column encountered → matrix is singular (not invertible).");
                return new SolveResult
                {
                    Title        = "Not Invertible",
                    Summary      = "Matrix is singular (determinant = 0). No inverse exists.",
                    SolutionType = SolutionType.NoSolution,
                    Steps        = new List<SolutionStep>(Steps)
                };
            }

            if (pivotRow != col)
            {
                augmented.SwapRows(col, pivotRow);
                AddStep($"Swap R{col + 1} ↔ R{pivotRow + 1}:", augmented);
            }

            Fraction pivot = augmented[col, col];
            // Guard: pivot must be nonzero after the row swap
            if (pivot.IsZero)
            {
                AddStep("Pivot is zero after swapping → matrix is singular.");
                return new SolveResult
                {
                    Title        = "Not Invertible",
                    Summary      = "Matrix is singular (determinant = 0). No inverse exists.",
                    SolutionType = SolutionType.NoSolution,
                    Steps        = new List<SolutionStep>(Steps)
                };
            }

            if (pivot != Fraction.One)
            {
                augmented.ScaleRow(col, Fraction.One / pivot);
                AddStep($"R{col + 1} = R{col + 1} / {pivot}  →  pivot = 1:", augmented);
            }

            for (int row = 0; row < n; row++)
            {
                if (row == col || augmented[row, col].IsZero) continue;
                Fraction factor = -augmented[row, col];
                augmented.AddScaledRow(row, col, factor);
                AddStep($"R{row + 1} = R{row + 1} + ({factor})×R{col + 1}:", augmented);
            }
        }

        // Extract inverse (right half of augmented)
        var inverse = new Matrix(n, n);
        for (int i = 0; i < n; i++)
            for (int j = 0; j < n; j++)
                inverse[i, j] = augmented[i, n + j];

        AddStep("Inverse A⁻¹:", inverse);

        return new SolveResult
        {
            Title        = "Matrix Inverse",
            Summary      = $"Successfully computed A⁻¹ for {n}×{n} matrix.",
            SolutionType = SolutionType.Value,
            Steps        = new List<SolutionStep>(Steps),
            ResultMatrix = inverse
        };
    }

    private SolveResult Error(string message) => new()
    {
        Title        = "Error",
        Summary      = message,
        SolutionType = SolutionType.Error,
        Steps        = new List<SolutionStep>(Steps)
    };
}
