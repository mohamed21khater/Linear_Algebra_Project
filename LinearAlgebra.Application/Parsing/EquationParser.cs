using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinearAlgebra.Domain.Models;

namespace LinearAlgebra.Application.Parsing;

/// <summary>
/// Converts user text input into Matrix objects.
///
/// Supported input formats:
///   • Rows separated by newlines (or semicolons).
///   • Elements separated by spaces, commas, or tabs.
///   • Elements may be integers, decimals, or fractions like "2/3".
///
/// Example:
///   "1 2 3
///    4 5 6
///    7 8 9"
/// </summary>
public static class EquationParser
{
    /// <summary>
    /// Parse a plain matrix from raw text.
    /// </summary>
    public static Matrix ParseMatrix(string text)
    {
        var rows = SplitRows(text);
        if (rows.Count == 0)
            throw new FormatException("Input is empty.");

        var parsedRows = rows.Select(ParseRow).ToList();

        int cols = parsedRows[0].Length;
        if (parsedRows.Any(r => r.Length != cols))
            throw new FormatException("All rows must have the same number of columns.");

        return Matrix.FromRows(parsedRows.Select(r => r).ToArray());
    }

    /// <summary>
    /// Parse an augmented matrix [A|b] from two separate text boxes.
    /// coeffsText = coefficient part, rhsText = right-hand side column.
    /// </summary>
    public static Matrix ParseAugmented(string coeffsText, string rhsText)
    {
        var coeffMatrix = ParseMatrix(coeffsText);
        var rhsRows     = SplitRows(rhsText);

        if (rhsRows.Count != coeffMatrix.Rows)
            throw new FormatException(
                $"RHS has {rhsRows.Count} rows but coefficient matrix has {coeffMatrix.Rows} rows.");

        var augmented = new Matrix(coeffMatrix.Rows, coeffMatrix.Cols + 1);
        for (int i = 0; i < coeffMatrix.Rows; i++)
        {
            for (int j = 0; j < coeffMatrix.Cols; j++)
                augmented[i, j] = coeffMatrix[i, j];

            Fraction[] rhsRow = ParseRow(rhsRows[i]);
            if (rhsRow.Length != 1)
                throw new FormatException($"RHS row {i + 1} must have exactly 1 value.");
            augmented[i, coeffMatrix.Cols] = rhsRow[0];
        }
        return augmented;
    }

    /// <summary>
    /// Parse a system of equations from a single augmented-matrix text block.
    /// All rows must have n+1 columns (last column = RHS).
    /// </summary>
    public static Matrix ParseAugmentedBlock(string text) => ParseMatrix(text);

    // ── Helpers ───────────────────────────────────────────────────────────────

    private static List<string> SplitRows(string text)
    {
        return text
            .Split(new[] { '\n', '\r', ';' }, StringSplitOptions.RemoveEmptyEntries)
            .Select(r => r.Trim())
            .Where(r => r.Length > 0)
            .ToList();
    }

    private static Fraction[] ParseRow(string row)
    {
        var tokens = row.Split(new[] { ' ', '\t', ',' }, StringSplitOptions.RemoveEmptyEntries);
        var result = new Fraction[tokens.Length];
        for (int i = 0; i < tokens.Length; i++)
        {
            if (!Fraction.TryParse(tokens[i], out result[i]))
                throw new FormatException($"Cannot parse '{tokens[i]}' as a number.");
        }
        return result;
    }

    /// <summary>
    /// Format a matrix as a nicely aligned string for display.
    /// </summary>
    public static string FormatMatrix(Matrix m)
    {
        // Compute max width per column
        int[] widths = new int[m.Cols];
        for (int j = 0; j < m.Cols; j++)
            for (int i = 0; i < m.Rows; i++)
                widths[j] = Math.Max(widths[j], m[i, j].ToString().Length);

        var sb = new StringBuilder();
        for (int i = 0; i < m.Rows; i++)
        {
            sb.Append("│ ");
            for (int j = 0; j < m.Cols; j++)
            {
                sb.Append(m[i, j].ToString().PadLeft(widths[j]));
                if (j < m.Cols - 1) sb.Append("  ");
            }
            sb.AppendLine(" │");
        }
        return sb.ToString();
    }
}
