using LinearAlgebra.Domain.Models;
using Xunit;

namespace LinearAlgebra.Tests;

public class FractionTests
{
    // ── Construction & Normalization ──────────────────────────────────────────

    [Fact]
    public void Constructor_NormalizesSign()
    {
        var f = new Fraction(3, -4);
        Assert.Equal(-3, f.Numerator);
        Assert.Equal(4,  f.Denominator);
    }

    [Fact]
    public void Constructor_ReducesToLowestTerms()
    {
        var f = new Fraction(6, 4);
        Assert.Equal(3, f.Numerator);
        Assert.Equal(2, f.Denominator);
    }

    [Fact]
    public void Constructor_ZeroAlwaysHasDenominatorOne()
    {
        var f = new Fraction(0, 999);
        Assert.Equal(0, f.Numerator);
        Assert.Equal(1, f.Denominator);
    }

    [Fact]
    public void Constructor_ThrowsOnZeroDenominator()
    {
        Assert.Throws<DivideByZeroException>(() => new Fraction(1, 0));
    }

    // ── Arithmetic ────────────────────────────────────────────────────────────

    [Theory]
    [InlineData(1, 2, 1, 3, 5, 6)]   // 1/2 + 1/3 = 5/6
    [InlineData(1, 4, 3, 4, 1, 1)]   // 1/4 + 3/4 = 1
    [InlineData(-1, 2, 1, 2, 0, 1)]  // -1/2 + 1/2 = 0
    public void Addition_IsCorrect(long an, long ad, long bn, long bd, long rn, long rd)
    {
        var a = new Fraction(an, ad);
        var b = new Fraction(bn, bd);
        var expected = new Fraction(rn, rd);
        Assert.Equal(expected, a + b);
    }

    [Theory]
    [InlineData(3, 4, 1, 4, 1, 2)]   // 3/4 - 1/4 = 1/2
    [InlineData(1, 1, 1, 1, 0, 1)]   // 1 - 1 = 0
    public void Subtraction_IsCorrect(long an, long ad, long bn, long bd, long rn, long rd)
    {
        Assert.Equal(new Fraction(rn, rd), new Fraction(an, ad) - new Fraction(bn, bd));
    }

    [Theory]
    [InlineData(2, 3, 3, 4, 1, 2)]   // 2/3 × 3/4 = 1/2
    [InlineData(-1, 2, 2, 1, -1, 1)] // -1/2 × 2 = -1
    public void Multiplication_IsCorrect(long an, long ad, long bn, long bd, long rn, long rd)
    {
        Assert.Equal(new Fraction(rn, rd), new Fraction(an, ad) * new Fraction(bn, bd));
    }

    [Theory]
    [InlineData(1, 2, 3, 4, 2, 3)]   // (1/2) / (3/4) = 2/3
    [InlineData(5, 1, 2, 1, 5, 2)]   // 5 / 2 = 5/2
    public void Division_IsCorrect(long an, long ad, long bn, long bd, long rn, long rd)
    {
        Assert.Equal(new Fraction(rn, rd), new Fraction(an, ad) / new Fraction(bn, bd));
    }

    [Fact]
    public void Division_ByZero_Throws()
    {
        Assert.Throws<DivideByZeroException>(() => new Fraction(1) / Fraction.Zero);
    }

    // ── Comparison ────────────────────────────────────────────────────────────

    [Fact]
    public void Comparison_Works()
    {
        Assert.True(new Fraction(1, 3) < new Fraction(1, 2));
        Assert.True(new Fraction(2, 3) > new Fraction(1, 3));
        Assert.True(new Fraction(1, 2) == new Fraction(2, 4));
    }

    // ── Parsing ───────────────────────────────────────────────────────────────

    [Theory]
    [InlineData("3",    3, 1)]
    [InlineData("-2/5", -2, 5)]
    [InlineData("0",    0, 1)]
    public void Parse_IsCorrect(string input, long expectedN, long expectedD)
    {
        var f = Fraction.Parse(input);
        Assert.Equal(new Fraction(expectedN, expectedD), f);
    }

    // ── ToString ──────────────────────────────────────────────────────────────

    [Fact]
    public void ToString_ShowsWholeNumber()
    {
        Assert.Equal("5", new Fraction(10, 2).ToString());
    }

    [Fact]
    public void ToString_ShowsFraction()
    {
        Assert.Equal("2/3", new Fraction(2, 3).ToString());
    }
}
