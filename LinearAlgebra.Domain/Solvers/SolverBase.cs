using System.Collections.Generic;
using LinearAlgebra.Domain.Interfaces;
using LinearAlgebra.Domain.Models;

namespace LinearAlgebra.Domain.Solvers;

/// <summary>
/// Base class shared by all solvers. Provides step-recording helpers.
/// Subclasses override DoSolve() — Template Method Pattern.
/// </summary>
public abstract class SolverBase : ISolver
{
    public abstract string Name { get; }

    protected readonly List<SolutionStep> Steps = [];

    public SolveResult Solve(Matrix matrix)
    {
        Steps.Clear();
        return DoSolve(matrix);
    }

    protected abstract SolveResult DoSolve(Matrix matrix);

    protected void AddStep(string description, Matrix? state = null)
    {
        Steps.Add(new SolutionStep(Steps.Count + 1, description, state));
    }
}
