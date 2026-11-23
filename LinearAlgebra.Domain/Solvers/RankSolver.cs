using System.Collections.Generic;
using LinearAlgebra.Domain.Enums;
using LinearAlgebra.Domain.Models;

namespace LinearAlgebra.Domain.Solvers;

/// <summary>
/// Computes the rank of a matrix using row-reduction to Row Echelon Form.
/// </summary>
public class RankSolver : SolverBase
{
    public override string Name => "Matrix Rank";

    protected override SolveResult DoSolve(Matrix matrix)
    {
        var m = matrix.Clone();
        int rows = m.Rows;
        int cols = m.Cols;

        AddStep("Starting matrix:", m);

        int rank = 0;
        for (int col = 0; col < cols && rank < rows; col++)
        {
            // Find pivot row at or below current rank row
            int pivotRow = -1;
            for (int i = rank; i < rows; i++)
                if (!m[i, col].IsZero) { pivotRow = i; break; }

            if (pivotRow == -1)
            {
                AddStep($"Column {col + 1}: all zeros below row {rank + 1} — skip.");
                continue;
            }

            if (pivotRow != rank)
            {
                m.SwapRows(rank, pivotRow);
                AddStep($"Swap R{rank + 1} ↔ R{pivotRow + 1}:", m);
            }

            Fraction pivot = m[rank, col];
            if (pivot != Fraction.One)
            {
                m.ScaleRow(rank, Fraction.One / pivot);
                AddStep($"R{rank + 1} = R{rank + 1} / {pivot}:", m);
            }

            // Eliminate below only (REF, not RREF)
            for (int i = rank + 1; i < rows; i++)
            {
                if (m[i, col].IsZero) continue;
                Fraction factor = -m[i, col];
                m.AddScaledRow(i, rank, factor);
                AddStep($"R{i + 1} = R{i + 1} + ({factor})×R{rank + 1}:", m);
            }

            rank++;
        }

        AddStep($"Row Echelon Form reached. Rank = {rank}", m);

        return new SolveResult
        {
            Title        = "Matrix Rank",
            Summary      = $"rank(A) = {rank}",
            SolutionType = SolutionType.Value,
            Steps        = new List<SolutionStep>(Steps),
            ScalarResult = new Fraction(rank)
        };
    }
}
