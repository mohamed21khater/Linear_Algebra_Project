namespace LinearAlgebra.Domain.Models;

/// <summary>
/// One step in a step-by-step solution.
/// Describes what operation was performed and shows the matrix state after it.
/// </summary>
public class SolutionStep
{
    public int    StepNumber   { get; init; }
    public string Description  { get; init; } = string.Empty;
    public string MatrixState  { get; init; } = string.Empty;  // text snapshot of matrix

    public SolutionStep() { }

    public SolutionStep(int number, string description, Matrix? matrix = null)
    {
        StepNumber  = number;
        Description = description;
        MatrixState = matrix?.ToString() ?? string.Empty;
    }

    public override string ToString() =>
        $"Step {StepNumber}: {Description}\n{MatrixState}";
}
