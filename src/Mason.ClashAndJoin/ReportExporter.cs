using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

using Autodesk.Revit.UI;

using Mason.Core.Utils;

namespace Mason.ClashAndJoin;

public static class ReportExporter
{
    public static void Dialog(ICollection<long[]> clashset, Stopwatch sw)
    {
        TaskDialog td = new("??????")
        {
            MainContent = $"??{clashset.Count}?????\n???{sw.Elapsed:hh\\:mm\\:ss\\.fff}",
        };
        td.AddCommandLink(TaskDialogCommandLinkId.CommandLink1, "???��??");

        td.CommonButtons = TaskDialogCommonButtons.Ok;
        TaskDialogResult tdresult = td.Show();
        if (tdresult == TaskDialogResult.CommandLink1)
        {
            Export(clashset);
        }
    }

    public static void Export(IEnumerable<long[]> clashset)
    {
        using StreamWriter sw = new(new FileStream(IO.SelectSaveFile(), FileMode.Create));
        foreach (long[] item in clashset)
        {
            sw.WriteLine($"{item[0]};{item[1]}");
        }
    }
}
