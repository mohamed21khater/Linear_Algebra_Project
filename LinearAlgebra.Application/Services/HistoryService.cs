using System.Collections.Generic;
using LinearAlgebra.Domain.Models;
using LinearAlgebra.Domain.Interfaces;


namespace LinearAlgebra.Application.Services;

/// <summary>
/// Thin wrapper around IHistoryRepository.
/// Provides higher-level operations (last N entries, search, clear).
/// </summary>
public class HistoryService
{
    private readonly IHistoryRepository _repo;

    public HistoryService(IHistoryRepository repo)
    {
        _repo = repo;
    }

    public List<HistoryEntry> GetAll()           => _repo.LoadAll();
    public void               Clear()             => _repo.Clear();
    public void               Add(HistoryEntry e) => _repo.Save(e);

    public List<HistoryEntry> GetLast(int n)
    {
        var all = _repo.LoadAll();
        int skip = all.Count - n;
        return skip > 0 ? all.GetRange(skip, n) : all;
    }

    public List<HistoryEntry> Search(string keyword)
    {
        keyword = keyword.ToLowerInvariant();
        return _repo.LoadAll()
            .FindAll(e =>
                e.OperationName.ToLower().Contains(keyword) ||
                e.ResultSummary.ToLower().Contains(keyword) ||
                e.InputSummary .ToLower().Contains(keyword));
    }
}
