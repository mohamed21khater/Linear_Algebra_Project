namespace LinearAlgebra.Domain.Enums;

public enum SolutionType
{
    /// <summary>System has exactly one solution.</summary>
    Unique,

    /// <summary>System has infinitely many solutions (underdetermined).</summary>
    Infinite,

    /// <summary>System has no solution (inconsistent).</summary>
    NoSolution,

    /// <summary>Used for operations that return a scalar/matrix value (det, inverse, etc.).</summary>
    Value,

    /// <summary>An error occurred during solving.</summary>
    Error
}
