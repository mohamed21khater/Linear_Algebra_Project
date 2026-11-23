using System;

namespace LinearAlgebra.Domain.Models;

/// <summary>
/// A single record in the solution history. Part of the Domain so that
/// IHistoryRepository (also in Domain) can reference it without circular deps.
/// </summary>
public class HistoryEntry
{
    public Guid     Id            { get; set; } = Guid.NewGuid();
    public DateTime Timestamp     { get; set; } = DateTime.Now;
    public string   OperationName { get; set; } = string.Empty;
    public string   InputSummary  { get; set; } = string.Empty;
    public string   ResultSummary { get; set; } = string.Empty;
    public bool     WasSuccess    { get; set; }

    /// <summary>Full step-by-step text, serialized as one block.</summary>
    public string StepsText { get; set; } = string.Empty;

    public override string ToString() =>
        $"[{Timestamp:yyyy-MM-dd HH:mm:ss}] {OperationName}: {ResultSummary}";
}
