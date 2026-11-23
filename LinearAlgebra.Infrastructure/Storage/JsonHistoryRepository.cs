using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using LinearAlgebra.Domain.Models;
using LinearAlgebra.Domain.Interfaces;

namespace LinearAlgebra.Infrastructure.Storage;

/// <summary>
/// Persists history entries in a JSON file.
/// Each run appends to the same file so history survives across sessions.
/// </summary>
public class JsonHistoryRepository : IHistoryRepository
{
    private readonly string _filePath;

    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        WriteIndented = true
    };

    public JsonHistoryRepository(string filePath)
    {
        _filePath = filePath;
        EnsureDirectoryExists();
    }

    public void Save(HistoryEntry entry)
    {
        var entries = LoadAll();
        entries.Add(entry);
        string json = JsonSerializer.Serialize(entries, _jsonOptions);
        File.WriteAllText(_filePath, json);
    }

    public List<HistoryEntry> LoadAll()
    {
        if (!File.Exists(_filePath))
            return [];

        try
        {
            string json = File.ReadAllText(_filePath);
            return JsonSerializer.Deserialize<List<HistoryEntry>>(json, _jsonOptions) ?? [];
        }
        catch
        {
            // Corrupted file — return empty list (don't crash on startup)
            return [];
        }
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
