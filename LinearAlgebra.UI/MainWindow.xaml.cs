using System;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using LinearAlgebra.Domain.Models;
using LinearAlgebra.Application.Services;
using LinearAlgebra.Infrastructure.Storage;

namespace LinearAlgebra.UI;

public partial class MainWindow : Window
{
    private readonly SolverService  _solver;
    private readonly HistoryService _history;
    private readonly DispatcherTimer _clock;

    // All input panels — toggled by sidebar nav
    private StackPanel[] _allPanels = null!;

    public MainWindow()
    {
        InitializeComponent();

        // ── Composition Root (Dependency Injection) ───────────────────────────
        string historyPath = System.IO.Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "LinearAlgebraSolver",
            "history.json");

        var repo  = new JsonHistoryRepository(historyPath);
        _history  = new HistoryService(repo);
        _solver   = new SolverService(repo);

        // ── Clock ─────────────────────────────────────────────────────────────
        _clock = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
        _clock.Tick += (_, _) => ClockText.Text = DateTime.Now.ToString("HH:mm:ss");
        _clock.Start();

        // ── Panel registry ────────────────────────────────────────────────────
        _allPanels = [PanelGauss, PanelDet, PanelInv, PanelTranspose, PanelRank, PanelMatOps];

        // ── Load history from disk ────────────────────────────────────────────
        RefreshHistory();
        SetStatus("Ready — select an operation from the sidebar.");
    }

    // ══════════════════════════════════════════════════════════════════════════
    //  Sidebar Navigation
    // ══════════════════════════════════════════════════════════════════════════

    private void NavBtn_Checked(object sender, RoutedEventArgs e)
    {
        if (_allPanels is null) return;   // not yet initialised

        // Hide all panels
        foreach (var p in _allPanels)
            p.Visibility = Visibility.Collapsed;

        // Show the right one and update header text
        if (sender == BtnNavGauss)
        {
            PanelGauss.Visibility      = Visibility.Visible;
            InputPanelTitle.Text       = "Linear System  [ A | b ]";
            InputPanelHint.Text        = "Augmented matrix — last column is the right-hand side (b)";
        }
        else if (sender == BtnNavDet)
        {
            PanelDet.Visibility        = Visibility.Visible;
            InputPanelTitle.Text       = "Determinant  det(A)";
            InputPanelHint.Text        = "Enter a square (n × n) matrix";
        }
        else if (sender == BtnNavInv)
        {
            PanelInv.Visibility        = Visibility.Visible;
            InputPanelTitle.Text       = "Matrix Inverse  A⁻¹";
            InputPanelHint.Text        = "Enter a square (n × n) matrix";
        }
        else if (sender == BtnNavTranspose)
        {
            PanelTranspose.Visibility  = Visibility.Visible;
            InputPanelTitle.Text       = "Matrix Transpose  Aᵀ";
            InputPanelHint.Text        = "Enter any (m × n) matrix";
        }
        else if (sender == BtnNavRank)
        {
            PanelRank.Visibility       = Visibility.Visible;
            InputPanelTitle.Text       = "Matrix Rank  rank(A)";
            InputPanelHint.Text        = "Enter any (m × n) matrix";
        }
        else if (sender == BtnNavMatOps)
        {
            PanelMatOps.Visibility     = Visibility.Visible;
            InputPanelTitle.Text       = "Matrix Operations  A ⊕ B";
            InputPanelHint.Text        = "Enter two matrices — A and B";
        }
    }

    // ══════════════════════════════════════════════════════════════════════════
    //  Solve Buttons
    // ══════════════════════════════════════════════════════════════════════════

    private void BtnSolveGauss_Click(object sender, RoutedEventArgs e)
        => Execute(() => _solver.SolveLinearSystem(GaussMatrixInput.Text));

    private void BtnDet_Click(object sender, RoutedEventArgs e)
        => Execute(() => _solver.ComputeDeterminant(DetMatrixInput.Text));

    private void BtnInverse_Click(object sender, RoutedEventArgs e)
        => Execute(() => _solver.ComputeInverse(InvMatrixInput.Text));

    private void BtnTranspose_Click(object sender, RoutedEventArgs e)
        => Execute(() => _solver.ComputeTranspose(TransposeInput.Text));

    private void BtnRank_Click(object sender, RoutedEventArgs e)
        => Execute(() => _solver.ComputeRank(RankInput.Text));

    private void BtnAdd_Click(object sender, RoutedEventArgs e)
        => Execute(() => _solver.AddMatrices(OpMatrixA.Text, OpMatrixB.Text));

    private void BtnSub_Click(object sender, RoutedEventArgs e)
        => Execute(() => _solver.SubtractMatrices(OpMatrixA.Text, OpMatrixB.Text));

    private void BtnMul_Click(object sender, RoutedEventArgs e)
        => Execute(() => _solver.MultiplyMatrices(OpMatrixA.Text, OpMatrixB.Text));

    // ── Utility buttons ───────────────────────────────────────────────────────

    private void BtnClearInput_Click(object sender, RoutedEventArgs e)
    {
        GaussMatrixInput.Text = string.Empty;
        DetMatrixInput.Text   = string.Empty;
        InvMatrixInput.Text   = string.Empty;
        TransposeInput.Text   = string.Empty;
        RankInput.Text        = string.Empty;
        OpMatrixA.Text        = string.Empty;
        OpMatrixB.Text        = string.Empty;
        SetStatus("Input cleared.");
    }

    private void BtnCopyResult_Click(object sender, RoutedEventArgs e)
    {
        if (!string.IsNullOrWhiteSpace(StepsOutput.Text))
        {
            Clipboard.SetText(StepsOutput.Text);
            SetStatus("Result copied to clipboard ✓");
        }
    }

    private void BtnClearHistory_Click(object sender, RoutedEventArgs e)
    {
        var confirm = MessageBox.Show(
            "Clear all history? This cannot be undone.",
            "Confirm",
            MessageBoxButton.YesNo,
            MessageBoxImage.Warning);

        if (confirm == MessageBoxResult.Yes)
        {
            _history.Clear();
            RefreshHistory();
            SetStatus("History cleared.");
        }
    }

    private void BtnRefreshHistory_Click(object sender, RoutedEventArgs e)
    {
        RefreshHistory();
        SetStatus("History refreshed.");
    }

    private void HistoryList_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (HistoryList.SelectedItem is not HistoryEntry entry) return;

        ResultTitle.Text    = $"[History]  {entry.OperationName}";
        ResultSubtitle.Text = entry.Timestamp.ToString("yyyy-MM-dd  HH:mm:ss");
        StepsOutput.Text    = BuildHistoryDisplay(entry);
        SetStatus($"Showing history: {entry.OperationName} — {entry.Timestamp:HH:mm:ss}");

        HistoryList.SelectedItem = null;
    }

    // ══════════════════════════════════════════════════════════════════════════
    //  Core Execute Helper
    // ══════════════════════════════════════════════════════════════════════════

    private void Execute(Func<SolveResult> action)
    {
        try
        {
            SetStatus("Computing…");
            var result = action();
            DisplayResult(result);
            RefreshHistory();

            if (result.IsSuccess)
                SetStatus($"✓  {result.Summary}");
            else
                SetStatus($"⚠  {result.Summary}");
        }
        catch (Exception ex)
        {
            ResultTitle.Text    = "Error";
            ResultSubtitle.Text = ex.GetType().Name;
            StepsOutput.Text    = ex.Message;
            SetStatus($"Error: {ex.Message}");
        }
    }

    private void DisplayResult(SolveResult result)
    {
        ResultTitle.Text    = result.Title;
        ResultSubtitle.Text = result.Summary;

        var sb = new StringBuilder();

        // Header
        sb.AppendLine($"{'─',0}{'─',0}  {result.Title}  {'─',0}{'─',0}");
        sb.AppendLine();

        // Steps
        foreach (var step in result.Steps)
        {
            sb.AppendLine($"Step {step.StepNumber}:  {step.Description}");
            if (!string.IsNullOrWhiteSpace(step.MatrixState))
            {
                foreach (var line in step.MatrixState.Split('\n'))
                    if (!string.IsNullOrWhiteSpace(line))
                        sb.AppendLine($"    {line.TrimEnd()}");
            }
            sb.AppendLine();
        }

        // Final solution (variable values)
        if (result.Solution.Count > 0)
        {
            sb.AppendLine("── Answer ──────────────────────────────────");
            foreach (var kv in result.Solution)
                sb.AppendLine($"    {kv.Key}  =  {kv.Value}");
            sb.AppendLine();
        }

        // Scalar (determinant / rank)
        if (result.ScalarResult.HasValue)
        {
            sb.AppendLine("── Result ──────────────────────────────────");
            sb.AppendLine($"    {result.ScalarResult.Value}");
            double dec = (double)result.ScalarResult.Value;
            if (Math.Abs(dec - Math.Round(dec)) > 1e-10)
                sb.AppendLine($"    ≈  {dec:F6}");
            sb.AppendLine();
        }

        // Matrix result (inverse / transpose / add / mul …)
        if (result.ResultMatrix is not null)
        {
            sb.AppendLine("── Result Matrix ───────────────────────────");
            foreach (var line in result.ResultMatrix.ToString().Split('\n'))
                if (!string.IsNullOrWhiteSpace(line))
                    sb.AppendLine($"    {line.TrimEnd()}");
        }

        StepsOutput.Text = sb.ToString();
    }

    // ══════════════════════════════════════════════════════════════════════════
    //  UI Helpers
    // ══════════════════════════════════════════════════════════════════════════

    private void RefreshHistory()
    {
        var entries = _history.GetAll();
        entries.Reverse();   // newest first
        HistoryList.ItemsSource  = entries;
        HistoryCountText.Text    = entries.Count.ToString();
    }

    private static string BuildHistoryDisplay(HistoryEntry entry)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"[History]  {entry.OperationName}");
        sb.AppendLine($"Time      :  {entry.Timestamp:yyyy-MM-dd  HH:mm:ss}");
        sb.AppendLine($"Result    :  {entry.ResultSummary}");
        sb.AppendLine();
        sb.AppendLine("── Input ───────────────────────────────────");
        sb.AppendLine(entry.InputSummary);
        sb.AppendLine();
        sb.AppendLine("── Steps ───────────────────────────────────");
        sb.AppendLine(entry.StepsText);
        return sb.ToString();
    }

    private void SetStatus(string message) => StatusText.Text = message;
}
