using LinearAlgebra.Domain.Enums;
using LinearAlgebra.Domain.Models;
using LinearAlgebra.Domain.Solvers;
using Xunit;

namespace LinearAlgebra.Tests;

public class SolverTests
{
    // ══════════════════════════════════════════════════════════════════════════
    //  Gauss-Jordan
    // ══════════════════════════════════════════════════════════════════════════

    [Fact]
    public void GaussJordan_UniqueSolution_IsCorrect()
    {
        // 2x + y - z = 8
        // -3x - y + 2z = -11
        // -2x + y + 2z = -3
        // Solution: x=2, y=3, z=-1
        var aug = Matrix.FromRows(new[]
        {
            new Fraction[] {  2,  1, -1,  8 },
            new Fraction[] { -3, -1,  2, -11 },
            new Fraction[] { -2,  1,  2, -3  }
        });

        var solver = new GaussJordanSolver();
        var result = solver.Solve(aug);

        Assert.Equal(SolutionType.Unique, result.SolutionType);
        Assert.Equal("2",  result.Solution["x1"]);
        Assert.Equal("3",  result.Solution["x2"]);
        Assert.Equal("-1", result.Solution["x3"]);
    }

    [Fact]
    public void GaussJordan_NoSolution_DetectedCorrectly()
    {
        // x + y = 1
        // x + y = 2  (contradiction)
        var aug = Matrix.FromRows(new[]
        {
            new Fraction[] { 1, 1, 1 },
            new Fraction[] { 1, 1, 2 }
        });

        var result = new GaussJordanSolver().Solve(aug);
        Assert.Equal(SolutionType.NoSolution, result.SolutionType);
    }

    [Fact]
    public void GaussJordan_InfiniteSolutions_DetectedCorrectly()
    {
        // x + y = 1
        // 2x + 2y = 2  (dependent)
        var aug = Matrix.FromRows(new[]
        {
            new Fraction[] { 1, 1, 1 },
            new Fraction[] { 2, 2, 2 }
        });

        var result = new GaussJordanSolver().Solve(aug);
        Assert.Equal(SolutionType.Infinite, result.SolutionType);
    }

    // ══════════════════════════════════════════════════════════════════════════
    //  Determinant
    // ══════════════════════════════════════════════════════════════════════════

    [Fact]
    public void Determinant_1x1_IsCorrect()
    {
        var m = Matrix.FromRows(new[] { new Fraction[] { 5 } });
        var result = new DeterminantSolver().Solve(m);
        Assert.Equal(new Fraction(5), result.ScalarResult);
    }

    [Fact]
    public void Determinant_2x2_IsCorrect()
    {
        // |1 2|
        // |3 4|  = 1*4 - 2*3 = -2
        var m = Matrix.FromRows(new[]
        {
            new Fraction[] { 1, 2 },
            new Fraction[] { 3, 4 }
        });
        var result = new DeterminantSolver().Solve(m);
        Assert.Equal(new Fraction(-2), result.ScalarResult);
    }

    [Fact]
    public void Determinant_3x3_IsCorrect()
    {
        // |1 2 3|
        // |4 5 6|  → det = -3  (singular-ish matrix, known result)
        // |7 8 10|
        var m = Matrix.FromRows(new[]
        {
            new Fraction[] {  1, 2,  3 },
            new Fraction[] {  4, 5,  6 },
            new Fraction[] {  7, 8, 10 }
        });
        var result = new DeterminantSolver().Solve(m);
        Assert.Equal(new Fraction(-3), result.ScalarResult);
    }

    [Fact]
    public void Determinant_SingularMatrix_IsZero()
    {
        var m = Matrix.FromRows(new[]
        {
            new Fraction[] { 1, 2, 3 },
            new Fraction[] { 4, 5, 6 },
            new Fraction[] { 7, 8, 9 }
        });
        var result = new DeterminantSolver().Solve(m);
        Assert.Equal(Fraction.Zero, result.ScalarResult);
    }

    [Fact]
    public void Determinant_NonSquare_ReturnsError()
    {
        var m = Matrix.FromRows(new[]
        {
            new Fraction[] { 1, 2, 3 },
            new Fraction[] { 4, 5, 6 }
        });
        var result = new DeterminantSolver().Solve(m);
        Assert.Equal(SolutionType.Error, result.SolutionType);
    }

    // ══════════════════════════════════════════════════════════════════════════
    //  Inverse
    // ══════════════════════════════════════════════════════════════════════════

    [Fact]
    public void Inverse_2x2_IsCorrect()
    {
        // A = |1 2|   A⁻¹ = |-2    1  |
        //     |3 4|          | 3/2 -1/2|
        var m = Matrix.FromRows(new[]
        {
            new Fraction[] { 1, 2 },
            new Fraction[] { 3, 4 }
        });
        var result = new InverseSolver().Solve(m);

        Assert.Equal(SolutionType.Value, result.SolutionType);
        Assert.NotNull(result.ResultMatrix);
        Assert.Equal(new Fraction(-2),    result.ResultMatrix![0, 0]);
        Assert.Equal(new Fraction(1),     result.ResultMatrix![0, 1]);
        Assert.Equal(new Fraction(3, 2),  result.ResultMatrix![1, 0]);
        Assert.Equal(new Fraction(-1, 2), result.ResultMatrix![1, 1]);
    }

    [Fact]
    public void Inverse_SingularMatrix_ReturnsNoSolution()
    {
        var m = Matrix.FromRows(new[]
        {
            new Fraction[] { 1, 2 },
            new Fraction[] { 2, 4 }
        });
        var result = new InverseSolver().Solve(m);
        Assert.Equal(SolutionType.NoSolution, result.SolutionType);
    }

    // ══════════════════════════════════════════════════════════════════════════
    //  Rank
    // ══════════════════════════════════════════════════════════════════════════

    [Fact]
    public void Rank_FullRank_IsN()
    {
        var m = Matrix.Identity(3);
        var result = new RankSolver().Solve(m);
        Assert.Equal(new Fraction(3), result.ScalarResult);
    }

    [Fact]
    public void Rank_SingularMatrix_IsLessThanN()
    {
        // All rows are multiples of [1,2,3] → rank = 1
        var m = Matrix.FromRows(new[]
        {
            new Fraction[] { 1, 2, 3 },
            new Fraction[] { 2, 4, 6 },
            new Fraction[] { 3, 6, 9 }
        });
        var result = new RankSolver().Solve(m);
        Assert.Equal(new Fraction(1), result.ScalarResult);
    }
}
