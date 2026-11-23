using System.Collections.Generic;
using LinearAlgebra.Domain.Enums;

namespace LinearAlgebra.Domain.Models;

/// <summary>
/// The complete result returned by any ISolver.
/// </summary>
public class SolveResult
{
    /// <summary>Human-readable title for the result (e.g., "Unique Solution").</summary>
    public string Title { get; init; } = string.Empty;

    /// <summary>Summary line shown in history.</summary>
    public string Summary { get; init; } = string.Empty;

    /// <summary>Type of solution (Unique / Infinite / NoSolution / Value).</summary>
    public SolutionType SolutionType { get; init; } = SolutionType.Unique;

    /// <summary>Ordered list of step-by-step explanations.</summary>
    public List<SolutionStep> Steps { get; init; } = [];

    /// <summary>
    /// Final answer as a dictionary: variable name → value.
    /// E.g. { "x1": "3/2", "x2": "-1" }
    /// </summary>
    public Dictionary<string, string> Solution { get; init; } = [];

    /// <summary>Final answer matrix (for operations that return a matrix).</summary>
    public Matrix? ResultMatrix { get; init; }

    /// <summary>Scalar result (for determinant).</summary>
    public Fraction? ScalarResult { get; init; }

    public bool IsSuccess =>
        SolutionType is SolutionType.Unique or SolutionType.Value;
}
