using System;
using System.Collections.Generic;
using LinearAlgebra.Domain.Enums;
using LinearAlgebra.Domain.Models;

namespace LinearAlgebra.Domain.Solvers;

/// <summary>
/// Solves a system of linear equations using the Gauss-Jordan elimination method.
/// Input: augmented matrix [A | b] of size n×(n+1).
/// </summary>
public class GaussJordanSolver : SolverBase
{
    public override string Name => "Gauss-Jordan Elimination";

    protected override SolveResult DoSolve(Matrix augmented)
    {
        int n = augmented.Rows;
        int cols = augmented.Cols; // = n + 1 for a square system

        if (cols < n + 1)
            return Error("Augmented matrix must have at least n+1 columns.");

        var m = augmented.Clone();
        AddStep("Starting augmented matrix [A|b]:", m);

        // ── Forward & backward elimination (full pivot on diagonal) ───────────
        for (int col = 0; col < n; col++)
        {
            // Find pivot (partial pivoting for numerical stability with fractions)
            int pivotRow = FindPivotRow(m, col, col);

            if (pivotRow == -1)
            {
                // No pivot in this column — might be infinite or no solution
                // We'll detect that after full elimination
                AddStep($"Column {col + 1}: no pivot found (column is zero below row {col + 1}).");
                continue;
            }

            if (pivotRow != col)
            {
                m.SwapRows(col, pivotRow);
                AddStep($"Swap R{col + 1} ↔ R{pivotRow + 1}:", m);
            }

            // Scale pivot row so pivot = 1
            Fraction pivot = m[col, col];
            if (!pivot.IsZero && pivot != Fraction.One)
            {
                m.ScaleRow(col, Fraction.One / pivot);
                AddStep($"R{col + 1} = R{col + 1} × (1/{pivot})  →  make pivot = 1:", m);
            }

            // Eliminate all other rows
            for (int row = 0; row < n; row++)
            {
                if (row == col || m[row, col].IsZero) continue;
                Fraction factor = -m[row, col];
                m.AddScaledRow(row, col, factor);
                string sign = factor > Fraction.Zero ? "+" : "-";
                AddStep($"R{row + 1} = R{row + 1} {sign} {factor.Abs()}×R{col + 1}:", m);
            }
        }

        // ── Classify & extract solution ───────────────────────────────────────
        return ClassifySolution(m, n);
    }

    private SolveResult ClassifySolution(Matrix m, int n)
    {
        // Check for inconsistency: a row [0 0 ... 0 | k] where k ≠ 0
        for (int i = 0; i < n; i++)
        {
            bool allZeroCoeffs = true;
            for (int j = 0; j < n; j++)
                if (!m[i, j].IsZero) { allZeroCoeffs = false; break; }

            if (allZeroCoeffs && !m[i, n].IsZero)
            {
                AddStep("Row of contradiction detected → No Solution.");
                return new SolveResult
                {
                    Title        = "No Solution",
                    Summary      = "The system is inconsistent (no solution).",
                    SolutionType = SolutionType.NoSolution,
                    Steps        = new List<SolutionStep>(Steps)
                };
            }
        }

        // Check rank of coefficient matrix vs augmented matrix
        int rank = ComputeRank(m, n);
        if (rank < n)
        {
            AddStep("System has fewer pivots than unknowns → Infinite Solutions.");
            return new SolveResult
            {
                Title        = "Infinite Solutions",
                Summary      = $"The system has infinitely many solutions (rank = {rank}, unknowns = {n}).",
                SolutionType = SolutionType.Infinite,
                Steps        = new List<SolutionStep>(Steps)
            };
        }

        // Unique solution
        var solution = new Dictionary<string, string>();
        for (int i = 0; i < n; i++)
        {
            string varName = $"x{i + 1}";
            solution[varName] = m[i, n].ToString();
        }

        string solutionText = string.Join(", ", System.Linq.Enumerable.Select(solution,
            kv => $"{kv.Key} = {kv.Value}"));

        AddStep($"Solution: {solutionText}");

        return new SolveResult
        {
            Title        = "Unique Solution",
            Summary      = solutionText,
            SolutionType = SolutionType.Unique,
            Steps        = new List<SolutionStep>(Steps),
            Solution     = solution
        };
    }

    private static int FindPivotRow(Matrix m, int col, int startRow)
    {
        // Choose the row with the largest absolute value (partial pivoting)
        int best = -1;
        Fraction bestVal = Fraction.Zero;
        for (int i = startRow; i < m.Rows; i++)
        {
            Fraction abs = m[i, col].Abs();
            if (abs > bestVal) { bestVal = abs; best = i; }
        }
        return best;
    }

    private static int ComputeRank(Matrix m, int cols)
    {
        int rank = 0;
        for (int i = 0; i < m.Rows && i < cols; i++)
            if (!m[i, i].IsZero) rank++;
        return rank;
    }

    private SolveResult Error(string message) => new()
    {
        Title        = "Error",
        Summary      = message,
        SolutionType = SolutionType.Error,
        Steps        = new List<SolutionStep>(Steps)
    };
}
