using System;
using System.IO;
using System.Windows.Forms;

namespace Mason.Core.Utils;

/// <summary>
/// Provides helper methods for user file and folder selection dialogs.
/// </summary>
public static class IO
{
    private static readonly NLog.Logger Log = NLog.LogManager.GetCurrentClassLogger();

    /// <summary>
    /// Opens a folder browser dialog for the user to select a directory.
    /// </summary>
    /// <returns>The selected folder path.</returns>
    /// <exception cref="OperationCanceledException">Thrown if the user cancels folder selection or selects an invalid folder.</exception>
    public static string SelectFolder()
    {
        using FolderBrowserDialog folder = new() { Description = "Select a folder" };

        DialogResult result = folder.ShowDialog();
        if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(folder.SelectedPath))
        {
            Log.Info($"User selected folder: {folder.SelectedPath}");
            return folder.SelectedPath;
        }

        Log.Error("User canceled folder selection or returned an empty path.");
        throw new OperationCanceledException("Folder selection was canceled or invalid.");
    }

    /// <summary>
    /// Opens a file dialog for the user to select an existing file to open.
    /// </summary>
    /// <param name="filter">An optional file filter (e.g. "Text files (*.txt)|*.txt").</param>
    /// <returns>The selected file path.</returns>
    /// <exception cref="OperationCanceledException">Thrown if the user cancels file selection or selects a non-existing file.</exception>
    public static string SelectOpenFile(string filter = null)
    {
        Log.Debug($"OpenFileDialog filter: {filter}");
        using OpenFileDialog dialog = new()
        {
            Filter = string.IsNullOrWhiteSpace(filter) ? "All files (*.*)|*.*" : filter,
            CheckFileExists = true,
            Multiselect = false,
            Title = "Select a file to open",
            InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
        };

        DialogResult result = dialog.ShowDialog();
        if (result == DialogResult.OK && File.Exists(dialog.FileName))
        {
            Log.Info($"User selected file to open: {dialog.FileName}");
            return dialog.FileName;
        }

        Log.Error("User canceled file open dialog or selected invalid file.");
        throw new OperationCanceledException("File open selection was canceled or invalid.");
    }

    /// <summary>
    /// Opens a file dialog for the user to specify a path for saving a file.
    /// </summary>
    /// <param name="filter">An optional file filter (e.g. "CSV files (*.csv)|*.csv").</param>
    /// <returns>The file path specified by the user.</returns>
    /// <exception cref="OperationCanceledException">Thrown if the user cancels the save file dialog or provides an empty path.</exception>
    public static string SelectSaveFile(string filter = null)
    {
        Log.Debug($"SaveFileDialog filter: {filter}");
        using SaveFileDialog dialog = new()
        {
            Filter = string.IsNullOrWhiteSpace(filter) ? "All files (*.*)|*.*" : filter,
            OverwritePrompt = true,
            Title = "Select a file to save",
            InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
        };

        DialogResult result = dialog.ShowDialog();
        if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(dialog.FileName))
        {
            Log.Info($"User selected file to save: {dialog.FileName}");
            return dialog.FileName;
        }

        Log.Error("User canceled save file dialog or returned an empty path.");
        throw new OperationCanceledException("Save file selection was canceled or invalid.");
    }
}
