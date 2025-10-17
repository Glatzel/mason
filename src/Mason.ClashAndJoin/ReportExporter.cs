using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Autodesk.Revit.UI;
using Mason.Core.Utils;

namespace Mason.ClashAndJoin;

/// <summary>
/// Utility class for exporting clash reports.
/// </summary>
public static class ReportExporter
{
    /// <summary>
    /// Shows a dialog to the user with clash count and elapsed time, allowing optional export.
    /// </summary>
    /// <param name="clashset">Collection of clash pairs.</param>
    /// <param name="sw">Stopwatch measuring the clash detection duration.</param>
    public static void Dialog(ICollection<long[]> clashset, Stopwatch sw)
    {
        if (clashset.Count == 0)
        {
            TaskDialog.Show("Report Exporter", "No clashes detected.");
            return;
        }

        TaskDialog td = new("Clash Detection Report")
        {
            MainContent =
                $"Detected {clashset.Count} clashes.\nElapsed Time: {sw.Elapsed:hh\\:mm\\:ss\\.fff}",
        };

        td.AddCommandLink(TaskDialogCommandLinkId.CommandLink1, "Export to CSV");
        td.CommonButtons = TaskDialogCommonButtons.Ok;

        TaskDialogResult tdResult = td.Show();

        if (tdResult == TaskDialogResult.CommandLink1)
        {
            Export(clashset);
        }
    }

    /// <summary>
    /// Exports the clash pairs to a CSV file.
    /// </summary>
    /// <param name="clashset">Collection of clash pairs (each as an array of two long IDs).</param>
    public static void Export(IEnumerable<long[]> clashset)
    {
        string filePath = IO.SelectSaveFile("CSV files (*.csv)|*.csv");

        using StreamWriter sw = new(
            new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.Read)
        );
        foreach (long[] pair in clashset)
        {
            if (pair.Length >= 2)
            {
                sw.WriteLine($"{pair[0]};{pair[1]}");
            }
        }
    }
}
