using LinearAlgebra.Domain.Models;

namespace LinearAlgebra.Domain.Interfaces;

/// <summary>
/// Contract for all solvers. Each concrete solver implements one operation.
/// The service layer works with this interface only (Polymorphism + DIP).
/// </summary>
public interface ISolver
{
    /// <summary>Human-readable name of this solver, e.g. "Gauss-Jordan".</summary>
    string Name { get; }

    /// <summary>
    /// Run the solve operation on the given input matrix.
    /// For equation systems the matrix should be the augmented matrix [A|b].
    /// </summary>
    SolveResult Solve(Matrix matrix);
}
