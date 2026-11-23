using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using LinearAlgebra.Domain.Models;
using LinearAlgebra.Domain.Interfaces;

namespace LinearAlgebra.Infrastructure.Storage;

/// <summary>
/// Persists history entries as a plain human-readable text file.
/// Each entry is written as a block separated by dashes.
/// </summary>
public class TextHistoryRepository : IHistoryRepository
{
    private readonly string _filePath;

    private const string Separator = "──────────────────────────────────────────────";

    public TextHistoryRepository(string filePath)
    {
        _filePath = filePath;
        EnsureDirectoryExists();
    }

    public void Save(HistoryEntry entry)
    {
        var sb = new StringBuilder();
        sb.AppendLine(Separator);
        sb.AppendLine($"ID        : {entry.Id}");
        sb.AppendLine($"Time      : {entry.Timestamp:yyyy-MM-dd HH:mm:ss}");
        sb.AppendLine($"Operation : {entry.OperationName}");
        sb.AppendLine($"Success   : {entry.WasSuccess}");
        sb.AppendLine($"Input     :");
        sb.AppendLine(entry.InputSummary);
        sb.AppendLine($"Result    : {entry.ResultSummary}");
        sb.AppendLine("Steps     :");
        sb.AppendLine(entry.StepsText);

        File.AppendAllText(_filePath, sb.ToString());
    }

    public List<HistoryEntry> LoadAll()
    {
        if (!File.Exists(_filePath))
            return [];

        var entries = new List<HistoryEntry>();
        string[] lines = File.ReadAllLines(_filePath);

        HistoryEntry? current = null;
        var stepsBuffer = new StringBuilder();
        bool inSteps = false;

        foreach (string line in lines)
        {
            if (line.StartsWith(Separator))
            {
                if (current != null)
                {
                    current.StepsText = stepsBuffer.ToString();
                    entries.Add(current);
                }
                current   = new HistoryEntry();
                stepsBuffer.Clear();
                inSteps = false;
                continue;
            }

            if (current is null) continue;

            if (line.StartsWith("ID        : "))
                current.Id = Guid.TryParse(line[12..], out var g) ? g : Guid.NewGuid();
            else if (line.StartsWith("Time      : "))
                current.Timestamp = DateTime.TryParse(line[12..], out var dt) ? dt : DateTime.MinValue;
            else if (line.StartsWith("Operation : "))
                current.OperationName = line[12..];
            else if (line.StartsWith("Success   : "))
                current.WasSuccess = bool.TryParse(line[12..], out var b) && b;
            else if (line.StartsWith("Result    : "))
                current.ResultSummary = line[12..];
            else if (line.StartsWith("Steps     :"))
                inSteps = true;
            else if (line.StartsWith("Input     :"))
                inSteps = false; // switching to input section
            else if (inSteps)
                stepsBuffer.AppendLine(line);
            else
                current.InputSummary += line + "\n";
        }

        if (current != null)
        {
            current.StepsText = stepsBuffer.ToString();
            entries.Add(current);
        }

        return entries;
    }

    public void Clear()
    {
        if (File.Exists(_filePath))
            File.Delete(_filePath);
    }

    private void EnsureDirectoryExists()
    {
        string? dir = Path.GetDirectoryName(_filePath);
        if (!string.IsNullOrEmpty(dir))
            Directory.CreateDirectory(dir);
    }
}
