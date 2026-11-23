using System.Collections.Generic;
using LinearAlgebra.Domain.Enums;
using LinearAlgebra.Domain.Models;

namespace LinearAlgebra.Domain.Solvers;

/// <summary>
/// Computes the determinant of a square matrix using cofactor expansion (recursive).
/// For large matrices (n > 4) it uses LU-style row reduction for efficiency.
/// </summary>
public class DeterminantSolver : SolverBase
{
    public override string Name => "Determinant";

    protected override SolveResult DoSolve(Matrix matrix)
    {
        if (!matrix.IsSquare)
            return Error("Determinant is only defined for square matrices.");

        AddStep("Computing determinant of:", matrix);

        Fraction det = ComputeDet(matrix, matrix.Rows);

        AddStep($"det(A) = {det}");

        return new SolveResult
        {
            Title        = "Determinant",
            Summary      = $"det(A) = {det}",
            SolutionType = SolutionType.Value,
            Steps        = new List<SolutionStep>(Steps),
            ScalarResult = det
        };
    }

    // ── Row-reduction method to compute det (O(n³)) ───────────────────────────

    private Fraction ComputeDet(Matrix original, int n)
    {
        var m = original.Clone();
        Fraction det = Fraction.One;

        for (int col = 0; col < n; col++)
        {
            // Find pivot
            int pivotRow = -1;
            for (int i = col; i < n; i++)
                if (!m[i, col].IsZero) { pivotRow = i; break; }

            if (pivotRow == -1) return Fraction.Zero; // singular

            if (pivotRow != col)
            {
                m.SwapRows(col, pivotRow);
                det = -det; // row swap flips sign
                AddStep($"Swap R{col + 1} ↔ R{pivotRow + 1}  →  flip sign:", m);
            }

            Fraction pivot = m[col, col];
            det *= pivot;
            AddStep($"Multiply det by pivot m[{col + 1},{col + 1}] = {pivot}  →  running det = {det}");

            // Eliminate below pivot
            for (int row = col + 1; row < n; row++)
            {
                if (m[row, col].IsZero) continue;
                Fraction factor = -(m[row, col] / pivot);
                m.AddScaledRow(row, col, factor);
            }
        }

        return det;
    }

    private SolveResult Error(string message) => new()
    {
        Title        = "Error",
        Summary      = message,
        SolutionType = SolutionType.Error,
        Steps        = new List<SolutionStep>(Steps)
    };
}
