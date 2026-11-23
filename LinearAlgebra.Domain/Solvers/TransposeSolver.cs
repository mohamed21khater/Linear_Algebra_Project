using System.Collections.Generic;
using LinearAlgebra.Domain.Enums;
using LinearAlgebra.Domain.Models;

namespace LinearAlgebra.Domain.Solvers;

/// <summary>
/// Transposes a matrix. A simple solver that records every element swap as a step.
/// </summary>
public class TransposeSolver : SolverBase
{
    public override string Name => "Matrix Transpose";

    protected override SolveResult DoSolve(Matrix matrix)
    {
        AddStep($"Original matrix ({matrix.Rows}×{matrix.Cols}):", matrix);

        Matrix result = matrix.Transpose();

        AddStep($"Transposed matrix ({result.Rows}×{result.Cols}) — rows and columns swapped:", result);

        return new SolveResult
        {
            Title        = "Matrix Transpose",
            Summary      = $"Transposed {matrix.Rows}×{matrix.Cols} → {result.Rows}×{result.Cols}",
            SolutionType = SolutionType.Value,
            Steps        = new List<SolutionStep>(Steps),
            ResultMatrix = result
        };
    }
}
