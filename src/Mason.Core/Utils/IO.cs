using System.Windows.Forms;

namespace Mason.Core.Utils;

public static class IO
{
    private static readonly NLog.Logger Log = NLog.LogManager.GetCurrentClassLogger();

    public static string SelectFolder()
    {
        FolderBrowserDialog folder = new();
        if (folder.ShowDialog() == DialogResult.OK)
        {
            Log.Info($"Select folder: {folder.SelectedPath}");
            return folder.SelectedPath;
        }
        return "./";
    }

    public static string SelectOpenFile(string filter = null)
    {
        Log.Debug($"File filter: {filter}");
        OpenFileDialog file = new() { Filter = filter?.Length == 0 ? null : filter };
        if (file.ShowDialog() == DialogResult.OK)
        {
            Log.Info($"Select file: {file.FileName}");
            return file.FileName;
        }
        Log.Warn("No file selected.");
        return null;
    }

    public static string SelectSaveFile(string filter = null)
    {
        Log.Debug($"File filter: {filter}");
        SaveFileDialog file = new() { Filter = filter?.Length == 0 ? null : filter };
        if (file.ShowDialog() == DialogResult.OK)
        {
            Log.Info($"Select file: {file.FileName}");
            return file.FileName;
        }
        Log.Warn("No file selected.");
        return null;
    }
}
