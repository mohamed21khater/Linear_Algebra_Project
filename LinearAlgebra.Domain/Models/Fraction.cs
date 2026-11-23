using System;

namespace LinearAlgebra.Domain.Models;

/// <summary>
/// Represents an exact rational number as a fraction (numerator/denominator).
/// All arithmetic is done exactly with no floating-point rounding.
/// </summary>
public readonly struct Fraction : IEquatable<Fraction>, IComparable<Fraction>
{
    public static readonly Fraction Zero = new(0, 1);
    public static readonly Fraction One  = new(1, 1);

    public long Numerator   { get; }

    /// <summary>
    /// Returns the stored denominator, or 1 if the struct was default-initialized (0/0).
    /// This safely handles the C# default(Fraction) edge-case.
    /// </summary>
    public long Denominator => _denominator == 0 ? 1 : _denominator;
    private readonly long _denominator;

    public Fraction(long numerator, long denominator = 1)
    {
        if (denominator == 0)
            throw new DivideByZeroException("Fraction denominator cannot be zero.");

        if (numerator == 0)
        {
            Numerator    = 0;
            _denominator = 1;
            return;
        }

        long gcd  = Gcd(Math.Abs(numerator), Math.Abs(denominator));
        int  sign = denominator < 0 ? -1 : 1;

        Numerator    = sign * numerator   / gcd;
        _denominator = sign * denominator / gcd;
    }

    // ── Arithmetic ────────────────────────────────────────────────────────────

    public static Fraction operator +(Fraction a, Fraction b) =>
        new(a.Numerator * b.Denominator + b.Numerator * a.Denominator,
            a.Denominator * b.Denominator);

    public static Fraction operator -(Fraction a, Fraction b) =>
        new(a.Numerator * b.Denominator - b.Numerator * a.Denominator,
            a.Denominator * b.Denominator);

    public static Fraction operator *(Fraction a, Fraction b) =>
        new(a.Numerator * b.Numerator, a.Denominator * b.Denominator);

    public static Fraction operator /(Fraction a, Fraction b)
    {
        if (b.Numerator == 0)
            throw new DivideByZeroException("Cannot divide a fraction by zero.");
        return new(a.Numerator * b.Denominator, a.Denominator * b.Numerator);
    }

    public static Fraction operator -(Fraction a) => new(-a.Numerator, a.Denominator);

    public Fraction Abs() => new(Math.Abs(Numerator), Denominator);

    public bool IsZero => Numerator == 0;

    // ── Comparison ────────────────────────────────────────────────────────────

    public int CompareTo(Fraction other) =>
        (Numerator * other.Denominator).CompareTo(other.Numerator * Denominator);

    public static bool operator ==(Fraction a, Fraction b) => a.Equals(b);
    public static bool operator !=(Fraction a, Fraction b) => !a.Equals(b);
    public static bool operator  <(Fraction a, Fraction b) => a.CompareTo(b) <  0;
    public static bool operator  >(Fraction a, Fraction b) => a.CompareTo(b) >  0;
    public static bool operator <=(Fraction a, Fraction b) => a.CompareTo(b) <= 0;
    public static bool operator >=(Fraction a, Fraction b) => a.CompareTo(b) >= 0;

    // ── Implicit conversions ──────────────────────────────────────────────────

    public static implicit operator Fraction(int    value) => new(value);
    public static implicit operator Fraction(long   value) => new(value);
    public static implicit operator Fraction(double value)
    {
        // Convert double → fraction via continued-fraction approximation
        const int precision = 1_000_000;
        long num = (long)(value * precision);
        return new Fraction(num, precision);
    }

    public static explicit operator double(Fraction f) =>
        (double)f.Numerator / f.Denominator;

    // ── Parsing ───────────────────────────────────────────────────────────────

    /// <summary>
    /// Parses strings like "3", "-2/5", "0.75", "1.5"
    /// </summary>
    public static Fraction Parse(string s)
    {
        s = s.Trim();
        if (s.Contains('/'))
        {
            string[] parts = s.Split('/');
            if (parts.Length != 2)
                throw new FormatException($"Invalid fraction format: '{s}'");
            return new Fraction(long.Parse(parts[0].Trim()), long.Parse(parts[1].Trim()));
        }
        if (s.Contains('.'))
        {
            double d = double.Parse(s, System.Globalization.CultureInfo.InvariantCulture);
            return (Fraction)d;
        }
        return new Fraction(long.Parse(s));
    }

    public static bool TryParse(string s, out Fraction result)
    {
        try { result = Parse(s); return true; }
        catch  { result = Zero;  return false; }
    }

    // ── Display ───────────────────────────────────────────────────────────────

    public override string ToString() =>
        Denominator == 1 ? Numerator.ToString() : $"{Numerator}/{Denominator}";

    public string ToDecimalString(int decimals = 4) =>
        ((double)this).ToString($"F{decimals}");

    // ── Equality & helpers ────────────────────────────────────────────────────

    public bool Equals(Fraction other) =>
        Numerator == other.Numerator && Denominator == other.Denominator;

    public override bool Equals(object? obj) => obj is Fraction f && Equals(f);

    public override int GetHashCode() => HashCode.Combine(Numerator, Denominator);

    private static long Gcd(long a, long b)
    {
        while (b != 0) { long t = b; b = a % b; a = t; }
        return a;
    }
}
