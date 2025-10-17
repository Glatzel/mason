using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows.Media.Imaging;
using Autodesk.Revit.UI;
using Mason.ClashAndJoin.Command.Bench;
using Mason.ClashAndJoin.Command.Group;
using Mason.ClashAndJoin.Command.Self;
using Mason.Command.MEP;
using Mason.Command.Misc;
using Mason.Command.Structural;
using NLog;

namespace Mason;

/// <summary>
/// Entry point for the Mason Revit Add-in.
/// Handles initialization, logging, and command registration.
/// </summary>
public class App : IExternalApplication
{
    private readonly Logger Log = ConfigLog();

    /// <summary>
    /// Called when Revit starts up.
    /// Initializes the logger and registers all Mason commands.
    /// </summary>
    public Result OnStartup(UIControlledApplication application)
    {
        Log.Info("=== Mason Revit Add-in Startup ===");
        Log.Info($"Operating System: {Environment.OSVersion}");

        string revitVersion = application.ControlledApplication.SubVersionNumber;
        string revitBuildVersion = application.ControlledApplication.VersionBuild;
        Log.Info($"Detected Autodesk Revit {revitVersion} ({revitBuildVersion})");

        string masonVersion = FileVersionInfo
            .GetVersionInfo(Assembly.GetExecutingAssembly().Location)
            .ProductVersion.Split('+')[0];
        Log.Info($"Mason Version: {masonVersion}");

        Log.Debug("Starting command registration...");
        new CommandRegister(application).Register();
        Log.Info("Successfully registered all Mason commands.");

        Log.Info("=== Mason Startup Complete ===");
        return Result.Succeeded;
    }

    /// <summary>
    /// Called when Revit shuts down.
    /// Performs any necessary cleanup.
    /// </summary>
    public Result OnShutdown(UIControlledApplication application)
    {
        Log.Info("=== Mason Shutdown ===");
        Log.Info("Mason Revit Add-in is shutting down.");
        return Result.Succeeded;
    }

    /// <summary>
    /// Configures NLog logging behavior and output file location.
    /// </summary>
    private static Logger ConfigLog()
    {
        string assemblyLocation = Assembly.GetExecutingAssembly().Location;
        string? assemblyDir = Path.GetDirectoryName(assemblyLocation);

        if (string.IsNullOrEmpty(assemblyDir))
        {
            throw new InvalidOperationException("Cannot determine the assembly directory for logging.");
        }

        string logDir = Path.Combine(assemblyDir, "../log");

        if (!Directory.Exists(logDir))
        {
            Directory.CreateDirectory(logDir);
        }

        NLog.Config.LoggingConfiguration config = new();
        string logFilePath = Path.Combine(logDir, $"{DateTime.Now:yyyyMMdd-HHmmss}.log");

        NLog.Targets.FileTarget logfile = new("MasonLogFile")
        {
            FileName = logFilePath,
            Layout =
                "${longdate} | ${level:uppercase=true} | ${message} ${exception:format=toString,StackTrace}",
        };

        // Validate that the log file path is within the intended log directory before opening
        if (Path.GetFullPath(logFilePath).StartsWith(Path.GetFullPath(logDir), StringComparison.OrdinalIgnoreCase)
            && File.Exists(logFilePath))
        {
            Process.Start(new ProcessStartInfo(logFilePath) { UseShellExecute = true });
        }
        else
        {
            logger.Warn("Attempted to open log file with invalid path: " + logFilePath);
        }
        config.AddRule(LogLevel.Debug, LogLevel.Fatal, logfile);
        LogManager.Configuration = config;
        Logger logger = LogManager.GetCurrentClassLogger();
        logger.Debug("Logger initialized in Debug mode.");
        Process.Start(new ProcessStartInfo(logFilePath) { UseShellExecute = true });
#else
        config.AddRule(LogLevel.Info, LogLevel.Fatal, logfile);
        LogManager.Configuration = config;
        var logger = LogManager.GetCurrentClassLogger();
        logger.Info("Logger initialized in Release mode.");
#endif

        return logger;
    }
}

/// <summary>
/// Handles the creation of Revit ribbon panels and registration of Mason commands.
/// </summary>
internal sealed class CommandRegister(UIControlledApplication uicapp)
{
    private readonly UIControlledApplication UICApp = uicapp;
    private readonly Logger Log = LogManager.GetCurrentClassLogger();

    /// <summary>
    /// Adds or retrieves a Revit Ribbon Panel under the specified tab.
    /// </summary>
    private RibbonPanel AddRibbonPanel(string tabName, string ribbonPanelName)
    {
        try
        {
            UICApp.CreateRibbonTab(tabName);
            Log.Trace($"Created Ribbon Tab: {tabName}");
        }
        catch
        {
            Log.Debug($"Ribbon Tab already exists: {tabName}");
        }
            Log.Debug($"Ribbon Panel already exists: {ribbonPanelName}");
            ribbonPanel = UICApp.GetRibbonPanels(tabName).Find(p => p.Name == ribbonPanelName);
            if (ribbonPanel == null)
            {
                Log.Error($"Ribbon Panel '{ribbonPanelName}' not found under tab '{tabName}'.");
                throw new InvalidOperationException($"Ribbon Panel '{ribbonPanelName}' not found under tab '{tabName}'.");
            }
        }
        return ribbonPanel;
            ribbonPanel = UICApp.CreateRibbonPanel(tabName, ribbonPanelName);
            Log.Trace($"Created Ribbon Panel: {ribbonPanelName} under {tabName}");
        }
        catch
        {
            Log.Debug($"Ribbon Panel already exists: {ribbonPanelName}");
            ribbonPanel = UICApp.GetRibbonPanels(tabName).Find(p => p.Name == ribbonPanelName)!;
        }
        return ribbonPanel;
    }

    /// <summary>
    /// Loads an embedded icon resource by name.
    /// </summary>
    private BitmapImage GetIcon(string iconName, bool large)
    {
        iconName ??= "default";
        string resourceName = $"Mason.Icon.{iconName}.png";

        Assembly assembly = Assembly.GetExecutingAssembly();
        Stream stream = assembly.GetManifestResourceStream(resourceName);

        if (stream == null)
        {
            Log.Warn($"Icon not found: {resourceName}. Using default icon.");
            stream = assembly.GetManifestResourceStream("Mason.Icon.default.png");
        }

        using Stream fs = stream!;
        using System.Drawing.Bitmap bitmap = new(fs);
        using MemoryStream memory = new();

        Log.Trace($"Loaded icon '{iconName}' with pixel format {bitmap.PixelFormat}");
        bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Tiff);
        memory.Position = 0;

        double desiredSize = large ? 32.0 : 16.0;
        BitmapImage bitmapImage = new();
        bitmapImage.BeginInit();
        bitmapImage.StreamSource = memory;
        bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
        bitmapImage.DecodePixelWidth = (int)
            Math.Round(desiredSize * bitmap.VerticalResolution / 96.0);
        bitmapImage.DecodePixelHeight = (int)
            Math.Round(desiredSize * bitmap.HorizontalResolution / 96.0);
        bitmapImage.EndInit();
        bitmapImage.Freeze();

        Log.Trace(
            $"Icon '{iconName}' decoded as {bitmapImage.DecodePixelWidth}x{bitmapImage.DecodePixelHeight}"
        );
        return bitmapImage;
    }

    /// <summary>
    /// Creates a new Revit push button for a command type.
    /// </summary>
    private PushButtonData NewPushButtonData<T>(
        string text = "",
        string iconName = "",
        string longDescription = "",
        string toolTip = ""
    )
        where T : IExternalCommand
    {
        Type cls = typeof(T);
        text ??= cls.Name;

        PushButtonData pbd = new(cls.FullName, text, cls.Assembly.Location, cls.FullName)
        {
            Image = GetIcon(iconName, false),
            LargeImage = GetIcon(iconName, true),
            LongDescription = longDescription,
            ToolTip = toolTip,
        };

        Log.Debug($"Created PushButtonData for command: {cls.FullName}");
        return pbd;
    }

    /// <summary>
    /// Registers all Mason commands under their respective tabs and panels.
    /// </summary>
    public void Register()
    {
        Log.Info("=== Begin Command Registration ===");
        RegisterCommand();
        RegisterClashAndJoin();
        Log.Info("=== Command Registration Complete ===");
    }

    #region Command Categories

    private void RegisterCommand()
    {
        const string tabName = "Mason";
        Log.Debug($"Registering general commands under tab: {tabName}");

        #region Structure
        {
            RibbonPanel panel = AddRibbonPanel(tabName, "Structure");
            panel.AddItem(
                NewPushButtonData<DisallowJoinBeam>(
                    "Disallow\nJoin Beam",
                    longDescription: "Sets the indicated end of the framing element to not be allowed to join to others."
                )
            );
            panel.AddItem(
                NewPushButtonData<FlipBeam>(
                    "Flip\nBeam",
                    longDescription: "Flip ends order of beam element."
                )
            );
        }
        #endregion

        #region MEP
        {
            RibbonPanel panel = AddRibbonPanel(tabName, "MEP");
            panel.AddItem(
                NewPushButtonData<FlipPipe>(
                    "Flip\nPipe",
                    longDescription: "Flip ends order of pipe element."
                )
            );
        }
        #endregion

        #region Misc
        {
            RibbonPanel panel = AddRibbonPanel(tabName, "Misc");
            panel.AddItem(
                NewPushButtonData<ResetGraphicsOverride>(
                    "Reset\nGraphics",
                    longDescription: "Set graphics override to default."
                )
            );
            panel.AddItem(
                NewPushButtonData<ExportWorksetsNWC>(
                    "Export\nWorksets NWC",
                    iconName: "navisworks",
                    toolTip: "Export NWC of each workset in current 3D view."
                )
            );
            panel.AddItem(
                NewPushButtonData<CreateWorksetView>(
                    "Create\nWorksets View",
                    toolTip: "Create views for each Workset."
                )
            );
            panel.AddItem(
                NewPushButtonData<ClearCAD>(
                    "Clear\nCAD",
                    longDescription: "Clear CAD files in current document."
                )
            );
        }
        #endregion

        #region DevTool
        {
            RibbonPanel panel = AddRibbonPanel(tabName, "DevTool");
            panel.AddItem(NewPushButtonData<Command.DevTool.HelloWorld>("Hello\nWorld"));
            panel.AddItem(
                NewPushButtonData<Command.DevTool.ThrowError>(
                    "Throw\nError",
                    longDescription: "Throw a test error."
                )
            );
            panel.AddItem(
                NewPushButtonData<Command.DevTool.OpenLog>(
                    "Open\nLog",
                    iconName: "log",
                    longDescription: "Open log file."
                )
            );
            panel.AddItem(
                NewPushButtonData<Command.DevTool.OpenRevitAPI>(
                    "Open\nRevit API",
                    iconName: "revit",
                    longDescription: "Open Revit API documentation in browser."
                )
            );
        }
        #endregion
    }

    private void RegisterClashAndJoin()
    {
        Log.Debug("Registering Clash & Join commands...");

        #region Self Clash&Join
        {
            RibbonPanel panel = AddRibbonPanel("Clash&Join", "Self Clash&Join");
            panel.AddItem(NewPushButtonData<SelfClashDetection>("Clash\nDetection"));
            panel.AddItem(NewPushButtonData<SelfJoin>("Join\nElements"));
            panel.AddItem(NewPushButtonData<SelfUnjoin>("Unjoin\nElements"));
            panel.AddItem(NewPushButtonData<SelfJoinWallOpening>("Join\nWall Opening"));
        }
        #endregion

        #region Group Clash&Join
        {
            RibbonPanel panel = AddRibbonPanel("Clash&Join", "Group Clash&Join");

            panel.AddItem(NewPushButtonData<GroupClashDetection>("Clash\nDetection"));
            panel.AddItem(NewPushButtonData<GroupJoin>("Join\nElements"));
            panel.AddItem(NewPushButtonData<GroupUnjoin>("Union\nElements"));
            panel.AddItem(NewPushButtonData<GroupSwitchJoin>("Switch\nJoin"));

            SplitButtonData group1 = new("SplitButtonSelectJoinGroup1", "Group1");
            SplitButton spb1 = (SplitButton)panel.AddItem(group1);
            spb1.AddPushButton(NewPushButtonData<SelectGroup1>("Select\nGroup 1"));
            spb1.AddPushButton(NewPushButtonData<AppendGroup1>("Append\nGroup 1"));
            spb1.AddPushButton(NewPushButtonData<ClearGroup1>("Clear\nGroup 1"));
            spb1.IsSynchronizedWithCurrentItem = false;

            SplitButtonData group2 = new("SplitButtonSelectJoinGroup2", "Group2");
            SplitButton spb2 = (SplitButton)panel.AddItem(group2);
            spb2.AddPushButton(NewPushButtonData<SelectGroup2>("Select\nGroup 2"));
            spb2.AddPushButton(NewPushButtonData<AppendGroup2>("Append\nGroup 2"));
            spb2.AddPushButton(NewPushButtonData<ClearGroup2>("Clear\nGroup 2"));
            spb2.IsSynchronizedWithCurrentItem = false;

            panel.AddItem(NewPushButtonData<ClearAll>("Clear\nSelection"));
        }
        #endregion

        #region Benchmark
        {
            RibbonPanel panel = AddRibbonPanel("Clash&Join", "Benchmark");
            panel.AddItem(NewPushButtonData<BenchBoundingBoxIntersect>("BoundingBox\nIntersect"));
            panel.AddItem(NewPushButtonData<BenchClashDetect>("Clash\nDetection"));
        }
        #endregion
    }

    #endregion
}
