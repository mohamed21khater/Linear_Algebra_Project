using System;
using System.Text;
using LinearAlgebra.Domain.Models;

namespace LinearAlgebra.Domain.Models;

/// <summary>
/// An immutable matrix of exact Fraction values.
/// Exposes safe row-operation helpers used by solvers.
/// </summary>
public class Matrix
{
    private readonly Fraction[,] _data;

    public int Rows { get; }
    public int Cols { get; }

    // ── Constructors ──────────────────────────────────────────────────────────

    public Matrix(int rows, int cols)
    {
        if (rows <= 0 || cols <= 0)
            throw new ArgumentOutOfRangeException("Matrix dimensions must be positive.");
        Rows  = rows;
        Cols  = cols;
        _data = new Fraction[rows, cols];
        // Initialize all cells to Fraction.Zero (0/1) instead of default struct (0/0)
        for (int i = 0; i < rows; i++)
            for (int j = 0; j < cols; j++)
                _data[i, j] = Fraction.Zero;
    }

    public Matrix(Fraction[,] data)
    {
        Rows  = data.GetLength(0);
        Cols  = data.GetLength(1);
        _data = (Fraction[,])data.Clone();
    }

    // ── Indexer ───────────────────────────────────────────────────────────────

    public Fraction this[int row, int col]
    {
        get => _data[row, col];
        set => _data[row, col] = value;
    }

    // ── Static factories ──────────────────────────────────────────────────────

    public static Matrix Identity(int n)
    {
        var m = new Matrix(n, n);
        for (int i = 0; i < n; i++)
            m[i, i] = Fraction.One;
        return m;
    }

    public static Matrix FromRows(Fraction[][] rows)
    {
        int r = rows.Length;
        int c = rows[0].Length;
        var m = new Matrix(r, c);
        for (int i = 0; i < r; i++)
            for (int j = 0; j < c; j++)
                m[i, j] = rows[i][j];
        return m;
    }

    // ── Deep copy ─────────────────────────────────────────────────────────────

    public Matrix Clone()
    {
        var m = new Matrix(Rows, Cols);
        Array.Copy(_data, m._data, _data.Length);
        return m;
    }

    // ── Row operations (mutating — used inside solvers on a clone) ────────────

    /// <summary>Swap two rows.</summary>
    public void SwapRows(int r1, int r2)
    {
        for (int j = 0; j < Cols; j++)
            (_data[r1, j], _data[r2, j]) = (_data[r2, j], _data[r1, j]);
    }

    /// <summary>Multiply every element in a row by a scalar.</summary>
    public void ScaleRow(int r, Fraction scalar)
    {
        for (int j = 0; j < Cols; j++)
            _data[r, j] *= scalar;
    }

    /// <summary>Add a scalar multiple of srcRow to destRow.</summary>
    public void AddScaledRow(int destRow, int srcRow, Fraction scalar)
    {
        for (int j = 0; j < Cols; j++)
            _data[destRow, j] += scalar * _data[srcRow, j];
    }

    // ── Arithmetic operators ──────────────────────────────────────────────────

    public static Matrix operator +(Matrix a, Matrix b)
    {
        AssertSameDimensions(a, b);
        var result = new Matrix(a.Rows, a.Cols);
        for (int i = 0; i < a.Rows; i++)
            for (int j = 0; j < a.Cols; j++)
                result[i, j] = a[i, j] + b[i, j];
        return result;
    }

    public static Matrix operator -(Matrix a, Matrix b)
    {
        AssertSameDimensions(a, b);
        var result = new Matrix(a.Rows, a.Cols);
        for (int i = 0; i < a.Rows; i++)
            for (int j = 0; j < a.Cols; j++)
                result[i, j] = a[i, j] - b[i, j];
        return result;
    }

    public static Matrix operator *(Matrix a, Matrix b)
    {
        if (a.Cols != b.Rows)
            throw new InvalidOperationException(
                $"Cannot multiply {a.Rows}×{a.Cols} by {b.Rows}×{b.Cols}.");
        var result = new Matrix(a.Rows, b.Cols);
        for (int i = 0; i < a.Rows; i++)
            for (int j = 0; j < b.Cols; j++)
                for (int k = 0; k < a.Cols; k++)
                    result[i, j] += a[i, k] * b[k, j];
        return result;
    }

    public static Matrix operator *(Fraction scalar, Matrix m)
    {
        var result = m.Clone();
        for (int i = 0; i < m.Rows; i++)
            for (int j = 0; j < m.Cols; j++)
                result[i, j] = scalar * m[i, j];
        return result;
    }

    // ── Utilities ─────────────────────────────────────────────────────────────

    public Matrix Transpose()
    {
        var t = new Matrix(Cols, Rows);
        for (int i = 0; i < Rows; i++)
            for (int j = 0; j < Cols; j++)
                t[j, i] = _data[i, j];
        return t;
    }

    public bool IsSquare => Rows == Cols;

    /// <summary>Extract a sub-matrix by removing row r and column c.</summary>
    public Matrix Minor(int r, int c)
    {
        var minor = new Matrix(Rows - 1, Cols - 1);
        int mi = 0;
        for (int i = 0; i < Rows; i++)
        {
            if (i == r) continue;
            int mj = 0;
            for (int j = 0; j < Cols; j++)
            {
                if (j == c) continue;
                minor[mi, mj] = _data[i, j];
                mj++;
            }
            mi++;
        }
        return minor;
    }

    public Fraction[] GetRow(int r)
    {
        var row = new Fraction[Cols];
        for (int j = 0; j < Cols; j++) row[j] = _data[r, j];
        return row;
    }

    public Fraction[] GetCol(int c)
    {
        var col = new Fraction[Rows];
        for (int i = 0; i < Rows; i++) col[i] = _data[i, c];
        return col;
    }

    // ── String representation ─────────────────────────────────────────────────

    public override string ToString()
    {
        var sb = new StringBuilder();
        for (int i = 0; i < Rows; i++)
        {
            sb.Append("[ ");
            for (int j = 0; j < Cols; j++)
            {
                sb.Append(_data[i, j].ToString().PadLeft(8));
                if (j < Cols - 1) sb.Append("  ");
            }
            sb.AppendLine(" ]");
        }
        return sb.ToString();
    }

    // ── Private helpers ───────────────────────────────────────────────────────

    private static void AssertSameDimensions(Matrix a, Matrix b)
    {
        if (a.Rows != b.Rows || a.Cols != b.Cols)
            throw new InvalidOperationException(
                $"Matrix dimensions don't match: {a.Rows}×{a.Cols} vs {b.Rows}×{b.Cols}.");
    }
}
