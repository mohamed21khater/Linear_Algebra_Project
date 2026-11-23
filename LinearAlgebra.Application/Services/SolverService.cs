using System;
using System.Collections.Generic;
using System.Text;
using LinearAlgebra.Application.Parsing;
using LinearAlgebra.Domain.Enums;
using LinearAlgebra.Domain.Interfaces;
using LinearAlgebra.Domain.Models;
using LinearAlgebra.Domain.Solvers;

namespace LinearAlgebra.Application.Services;

/// <summary>
/// Orchestrates all solver operations.
/// The UI only talks to this service — it never touches Domain solvers directly.
/// Implements Open/Closed: new solvers are added without touching this class.
/// </summary>
public class SolverService
{
    private readonly IHistoryRepository _history;

    public SolverService(IHistoryRepository history)
    {
        _history = history;
    }

    // ── Gauss-Jordan ──────────────────────────────────────────────────────────

    public SolveResult SolveLinearSystem(string augmentedMatrixText)
    {
        try
        {
            Matrix m = EquationParser.ParseMatrix(augmentedMatrixText);
            var solver = new GaussJordanSolver();
            var result = solver.Solve(m);
            SaveHistory(solver.Name, augmentedMatrixText, result);
            return result;
        }
        catch (Exception ex)
        {
            return BuildError("Gauss-Jordan", ex.Message);
        }
    }

    public SolveResult SolveLinearSystem(string coeffsText, string rhsText)
    {
        try
        {
            Matrix m = EquationParser.ParseAugmented(coeffsText, rhsText);
            var solver = new GaussJordanSolver();
            var result = solver.Solve(m);
            SaveHistory(solver.Name, $"Coefficients:\n{coeffsText}\nRHS:\n{rhsText}", result);
            return result;
        }
        catch (Exception ex)
        {
            return BuildError("Gauss-Jordan", ex.Message);
        }
    }

    // ── Determinant ───────────────────────────────────────────────────────────

    public SolveResult ComputeDeterminant(string matrixText)
    {
        try
        {
            Matrix m = EquationParser.ParseMatrix(matrixText);
            var solver = new DeterminantSolver();
            var result = solver.Solve(m);
            SaveHistory(solver.Name, matrixText, result);
            return result;
        }
        catch (Exception ex)
        {
            return BuildError("Determinant", ex.Message);
        }
    }

    // ── Inverse ───────────────────────────────────────────────────────────────

    public SolveResult ComputeInverse(string matrixText)
    {
        try
        {
            Matrix m = EquationParser.ParseMatrix(matrixText);
            var solver = new InverseSolver();
            var result = solver.Solve(m);
            SaveHistory(solver.Name, matrixText, result);
            return result;
        }
        catch (Exception ex)
        {
            return BuildError("Inverse", ex.Message);
        }
    }

    // ── Transpose ─────────────────────────────────────────────────────────────

    public SolveResult ComputeTranspose(string matrixText)
    {
        try
        {
            Matrix m = EquationParser.ParseMatrix(matrixText);
            var solver = new TransposeSolver();
            var result = solver.Solve(m);
            SaveHistory(solver.Name, matrixText, result);
            return result;
        }
        catch (Exception ex)
        {
            return BuildError("Transpose", ex.Message);
        }
    }

    // ── Rank ──────────────────────────────────────────────────────────────────

    public SolveResult ComputeRank(string matrixText)
    {
        try
        {
            Matrix m = EquationParser.ParseMatrix(matrixText);
            var solver = new RankSolver();
            var result = solver.Solve(m);
            SaveHistory(solver.Name, matrixText, result);
            return result;
        }
        catch (Exception ex)
        {
            return BuildError("Rank", ex.Message);
        }
    }

    // ── Matrix Add / Subtract ─────────────────────────────────────────────────

    public SolveResult AddMatrices(string aText, string bText)
        => BinaryOp(aText, bText, isAdd: true);

    public SolveResult SubtractMatrices(string aText, string bText)
        => BinaryOp(aText, bText, isAdd: false);

    private SolveResult BinaryOp(string aText, string bText, bool isAdd)
    {
        try
        {
            Matrix a = EquationParser.ParseMatrix(aText);
            Matrix b = EquationParser.ParseMatrix(bText);
            var solver = new MatrixAddSubSolver(isAdd);
            var result = solver.Solve(a, b);
            SaveHistory(solver.Name, $"A:\n{aText}\nB:\n{bText}", result);
            return result;
        }
        catch (Exception ex)
        {
            return BuildError(isAdd ? "Matrix Addition" : "Matrix Subtraction", ex.Message);
        }
    }

    // ── Matrix Multiply ───────────────────────────────────────────────────────

    public SolveResult MultiplyMatrices(string aText, string bText)
    {
        try
        {
            Matrix a = EquationParser.ParseMatrix(aText);
            Matrix b = EquationParser.ParseMatrix(bText);
            var solver = new MatrixMultiplySolver();
            var result = solver.Solve(a, b);
            SaveHistory(solver.Name, $"A:\n{aText}\nB:\n{bText}", result);
            return result;
        }
        catch (Exception ex)
        {
            return BuildError("Matrix Multiplication", ex.Message);
        }
    }

    // ── History helpers ───────────────────────────────────────────────────────

    private void SaveHistory(string opName, string input, SolveResult result)
    {
        var stepsText = new StringBuilder();
        foreach (var s in result.Steps)
            stepsText.AppendLine(s.ToString()).AppendLine();

        _history.Save(new HistoryEntry
        {
            OperationName = opName,
            InputSummary  = Truncate(input, 300),
            ResultSummary = result.Summary,
            WasSuccess    = result.IsSuccess,
            StepsText     = stepsText.ToString()
        });
    }

    private static string Truncate(string s, int max) =>
        s.Length <= max ? s : s[..max] + "…";

    private static SolveResult BuildError(string op, string msg) => new()
    {
        Title        = $"{op} — Error",
        Summary      = msg,
        SolutionType = SolutionType.Error,
        Steps        = [new SolutionStep(1, $"Error: {msg}")]
    };
}
