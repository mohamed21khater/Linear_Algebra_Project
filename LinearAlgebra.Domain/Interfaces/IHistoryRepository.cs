using System.Collections.Generic;
using LinearAlgebra.Domain.Models;

namespace LinearAlgebra.Domain.Interfaces;

/// <summary>
/// Repository contract for persisting solution history.
/// Implementations (JSON / TXT) live in Infrastructure — Domain knows nothing about them.
/// </summary>
public interface IHistoryRepository
{
    void              Save(HistoryEntry entry);
    List<HistoryEntry> LoadAll();
    void              Clear();
}
