using LinearAlgebra.Domain.Models;
using Xunit;

namespace LinearAlgebra.Tests;

public class MatrixTests
{
    // ── Indexer ───────────────────────────────────────────────────────────────

    [Fact]
    public void Indexer_GetSet_Works()
    {
        var m = new Matrix(2, 2);
        m[0, 0] = new Fraction(5);
        Assert.Equal(new Fraction(5), m[0, 0]);
    }

    // ── Identity ──────────────────────────────────────────────────────────────

    [Fact]
    public void Identity_HasOneOnDiagonal()
    {
        var id = Matrix.Identity(3);
        for (int i = 0; i < 3; i++)
            for (int j = 0; j < 3; j++)
                Assert.Equal(i == j ? Fraction.One : Fraction.Zero, id[i, j]);
    }

    // ── Row operations ────────────────────────────────────────────────────────

    [Fact]
    public void SwapRows_SwapsCorrectly()
    {
        var m = Matrix.FromRows(new[]
        {
            new[] { new Fraction(1), new Fraction(2) },
            new[] { new Fraction(3), new Fraction(4) }
        });
        m.SwapRows(0, 1);
        Assert.Equal(new Fraction(3), m[0, 0]);
        Assert.Equal(new Fraction(1), m[1, 0]);
    }

    [Fact]
    public void ScaleRow_MultipliesEveryElement()
    {
        var m = new Matrix(1, 3);
        m[0, 0] = 2; m[0, 1] = 4; m[0, 2] = 6;
        m.ScaleRow(0, new Fraction(1, 2));
        Assert.Equal(Fraction.One, m[0, 0]);
        Assert.Equal(new Fraction(2), m[0, 1]);
        Assert.Equal(new Fraction(3), m[0, 2]);
    }

    // ── Arithmetic ────────────────────────────────────────────────────────────

    [Fact]
    public void Addition_ProducesCorrectResult()
    {
        var a = Matrix.FromRows(new[]
        {
            new[] { new Fraction(1), new Fraction(2) },
            new[] { new Fraction(3), new Fraction(4) }
        });
        var b = Matrix.Identity(2);
        var c = a + b;
        Assert.Equal(new Fraction(2), c[0, 0]);
        Assert.Equal(new Fraction(5), c[1, 1]);
    }

    [Fact]
    public void Multiply_Identity_ReturnsOriginal()
    {
        var a = Matrix.FromRows(new[]
        {
            new[] { new Fraction(1), new Fraction(2) },
            new[] { new Fraction(3), new Fraction(4) }
        });
        var result = a * Matrix.Identity(2);
        Assert.Equal(a[0, 0], result[0, 0]);
        Assert.Equal(a[1, 1], result[1, 1]);
    }

    // ── Transpose ─────────────────────────────────────────────────────────────

    [Fact]
    public void Transpose_FlipsRowsAndCols()
    {
        var m = Matrix.FromRows(new[]
        {
            new[] { new Fraction(1), new Fraction(2), new Fraction(3) },
            new[] { new Fraction(4), new Fraction(5), new Fraction(6) }
        });
        var t = m.Transpose();
        Assert.Equal(3, t.Rows);
        Assert.Equal(2, t.Cols);
        Assert.Equal(new Fraction(2), t[1, 0]);
        Assert.Equal(new Fraction(4), t[0, 1]);
    }

    // ── Dimension checks ──────────────────────────────────────────────────────

    [Fact]
    public void AddMismatch_Throws()
    {
        var a = new Matrix(2, 2);
        var b = new Matrix(3, 3);
        Assert.Throws<InvalidOperationException>(() => { var _ = a + b; });
    }

    [Fact]
    public void MultiplyMismatch_Throws()
    {
        var a = new Matrix(2, 3);
        var b = new Matrix(2, 2);
        Assert.Throws<InvalidOperationException>(() => { var _ = a * b; });
    }
}
