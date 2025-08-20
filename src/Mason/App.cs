using System;
using System.Collections.Generic;
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

public class App : IExternalApplication
{
    private readonly Logger Log = ConfigLog();

    public Result OnStartup(UIControlledApplication application)
    {
        Log.Info(Environment.OSVersion.ToString());
        string revitVersion = application.ControlledApplication.SubVersionNumber;
        string revitBuildVersion = application.ControlledApplication.VersionBuild;
        Log.Info($"Autodesk Revit {revitVersion} {revitBuildVersion}");
        string masonVersion = FileVersionInfo
            .GetVersionInfo(Assembly.GetExecutingAssembly().Location)
            .ProductVersion.Split('+')[0];
        Log.Info($"Mason {masonVersion}");

        new CommandRegister(application).Register();
        return Result.Succeeded;
    }

    public Result OnShutdown(UIControlledApplication application)
    {
        Log.Info("Shutdown Mason.");
        return Result.Succeeded;
    }

    private static Logger ConfigLog()
    {
        string logDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "/../log";
        NLog.Config.LoggingConfiguration config = new();
        NLog.Targets.FileTarget logfile = new("MasonLogFile")
        {
            FileName = $"{logDir}/{DateTime.Now:yyyyMMdd-HHmmss}.log",
        };
#if Debug
        config.AddRule(LogLevel.Debug, LogLevel.Fatal, logfile);
#endif
#if Release
        config.AddRule(LogLevel.Info, LogLevel.Fatal, logfile);
#endif
        LogManager.Configuration = config;
        Logger Logger = LogManager.GetCurrentClassLogger();
        Logger.Debug("Finish Initialize Logger.");
#if Debug
        Process.Start(logfile.FileName.ToString());
#endif
        return Logger;
    }
}

internal sealed class CommandRegister(UIControlledApplication uicapp)
{
    private readonly UIControlledApplication UICApp = uicapp;
    private readonly Logger Log = LogManager.GetCurrentClassLogger();

    private RibbonPanel AddRibbonPanel(string tabName, string ribbonPanelName)
    {
        // Create a custom ribbon tab
        try
        {
            UICApp.CreateRibbonTab(tabName);
            Log.Info($"Add Ribbon Tab: {tabName}");
        }
        catch
        {
            //tab already exist
            Log.Info($"Ribbon Tab already exist: {tabName}");
        }
        // Add a new ribbon panel
        RibbonPanel ribbonPanel;
        try
        {
            ribbonPanel = UICApp.CreateRibbonPanel(tabName, ribbonPanelName);
            Log.Info($"Add Panel: {tabName}");
        }
        catch
        {
            //ribbon panel already exist
            Log.Info($"Ribbon Panel already exist: {ribbonPanelName}");
            List<RibbonPanel> ribbonPanels = UICApp.GetRibbonPanels(tabName);
            ribbonPanel = ribbonPanels.Find(panel => panel.Name == ribbonPanelName);
        }
        return ribbonPanel;
    }

    private BitmapImage GetIcon(string iconName, bool large)
    {
        if (iconName == null)
        {
            iconName = "default";
            Log.Info("Use default icon.");
        }
        Assembly assembly = Assembly.GetExecutingAssembly();
        string resourceName = $"Mason.Icon.{iconName}.png";
        Stream stream = assembly.GetManifestResourceStream(resourceName);
        if (stream == null)
        {
            Log.Warn($"Icon file not found: {resourceName}.");
            stream = assembly.GetManifestResourceStream("Mason.Icon.default.png");
        }

        using Stream fs = stream;
        System.Drawing.Bitmap bitmap = new(fs);

        using MemoryStream memory = new();
        Log.Debug($"Pixel format of {iconName}: {bitmap.PixelFormat}");
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
        Log.Debug($"`HorizontalResolution` of {iconName}.png: {bitmap.HorizontalResolution}");
        Log.Debug($"`DecodePixelWidth` of {iconName}.png: {bitmapImage.DecodePixelWidth}");
        bitmapImage.EndInit();
        bitmapImage.Freeze();

        return bitmapImage;
    }

    private PushButtonData NewPushButtonData<T>(
        string text = null,
        string iconName = null,
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
        Log.Info($"Add Command to PushButtonData: {cls.FullName}");
        return pbd;
    }

    public void Register()
    {
        Log.Info("Start register.");
        RegisterCommand();
        RegisterClashAndJoin();
        Log.Info("Finish register.");
    }

    private void RegisterCommand()
    {
        const string tabName = "Mason";

        #region Structure
        {
            RibbonPanel ribbonPanel = AddRibbonPanel(tabName, "Structure");

            ribbonPanel.AddItem(
                NewPushButtonData<DisallowJoinBeam>(
                    text: "Disallow\nJoin Beam",
                    longDescription: "Sets the indicated end of the framing element to not be allowed to join to others."
                )
            );
            ribbonPanel.AddItem(
                NewPushButtonData<FlipBeam>(
                    text: "Flip\nBeam",
                    longDescription: "Flip ends order of beam element."
                )
            );
        }
        #endregion Structure

        #region MEP
        {
            RibbonPanel ribbonPanel = AddRibbonPanel(tabName, "MEP");

            ribbonPanel.AddItem(
                NewPushButtonData<FlipPipe>(
                    text: "Flip\nPipe",
                    longDescription: "Flip ends order of pipe element."
                )
            );
        }
        #endregion MEP

        #region Misc
        {
            RibbonPanel ribbonPanel = AddRibbonPanel(tabName, "Misc");

            ribbonPanel.AddItem(
                NewPushButtonData<ResetGraphicsOverride>(
                    text: "Reset\nGraphics",
                    longDescription: "Set graphics override to default."
                )
            );
            ribbonPanel.AddItem(
                NewPushButtonData<ExportWorksetsNWC>(
                    text: "Export\nWorksets NWC",
                    iconName: "navisworks",
                    toolTip: "Export NWC of each worksets in current 3D view."
                )
            );
            ribbonPanel.AddItem(
                NewPushButtonData<CreateWorksetView>(
                    text: "Create\nWorksets View",
                    toolTip: "Create Views for each Workset."
                )
            );
            ribbonPanel.AddItem(
                NewPushButtonData<ClearCAD>(
                    text: "Clear\nCAD",
                    longDescription: "Clear CAD files in current document."
                )
            );
        }
        #endregion Misc

        #region DevTool
        {
            RibbonPanel ribbonPanel = AddRibbonPanel(tabName, "DevTool");

            ribbonPanel.AddItem(
                NewPushButtonData<Command.DevTool.HelloWorld>(text: "Hello\nWorld")
            );
            ribbonPanel.AddItem(
                NewPushButtonData<Command.DevTool.ThrowError>(
                    text: "Throw\nError",
                    longDescription: "Throw a error for test."
                )
            );
            ribbonPanel.AddItem(
                NewPushButtonData<Command.DevTool.OpenLog>(
                    text: "Open\nLog",
                    iconName: "log",
                    longDescription: "Open log file."
                )
            );
            ribbonPanel.AddItem(
                NewPushButtonData<Command.DevTool.OpenRevitAPI>(
                    text: "Open\nRevit API",
                    iconName: "revit",
                    longDescription: "Open Revit API in browser."
                )
            );
        }
        #endregion DevTool
    }

    private void RegisterClashAndJoin()
    {
        #region Self Clash&Join

        {
            RibbonPanel ribbonPanel = AddRibbonPanel("Clash&Join", "Self Clash&Join");
            ribbonPanel.AddItem(NewPushButtonData<SelfClashDetection>(text: "Clash\nDetection"));
            ribbonPanel.AddItem(NewPushButtonData<SelfJoin>(text: "Join\nElements"));
            ribbonPanel.AddItem(NewPushButtonData<SelfUnjoin>(text: "Unjoin\nElements"));
            ribbonPanel.AddItem(NewPushButtonData<SelfJoinWallOpening>(text: "Join\nWall Opening"));
        }
        #endregion Self Clash&Join

        #region Group Clash&Join
        {
            RibbonPanel ribbonPanel = AddRibbonPanel("Clash&Join", "Group Clash&Join");

            ribbonPanel.AddItem(NewPushButtonData<GroupClashDetection>(text: "Clash\nDetection"));
            ribbonPanel.AddItem(NewPushButtonData<GroupJoin>(text: "Join\nElements"));
            ribbonPanel.AddItem(NewPushButtonData<GroupUnjoin>(text: "Unoin\nElements"));
            ribbonPanel.AddItem(NewPushButtonData<GroupSwitchJoin>(text: "Switch\nJoin"));

            SplitButtonData spbd1 = new("SplitButtonSelectJoinGroup1", "Group1");
            SplitButton spb1 = ribbonPanel.AddItem(spbd1) as SplitButton;
            Log.Info("Add SplitButton ClashAndJoin Select1.");
            spb1.AddPushButton(NewPushButtonData<SelectGroup1>(text: "Select\nGroup 1"));
            spb1.AddPushButton(NewPushButtonData<AppendGroup1>(text: "Append\nGroup 1"));
            spb1.AddPushButton(NewPushButtonData<ClearGroup1>(text: "Clear\nGroup 1"));
            spb1.IsSynchronizedWithCurrentItem = false;

            SplitButtonData spbd2 = new("SplitButtonSelectJoinGroup2", "Group2");
            SplitButton spb2 = ribbonPanel.AddItem(spbd2) as SplitButton;
            Log.Info("Add SplitButton ClashAndJoin Select2.");
            spb2.AddPushButton(NewPushButtonData<SelectGroup2>(text: "Select\nGroup 2"));
            spb2.AddPushButton(NewPushButtonData<AppendGroup2>(text: "Append\nGroup 2"));
            spb2.AddPushButton(NewPushButtonData<ClearGroup2>(text: "Clear\nGroup 2"));
            spb2.IsSynchronizedWithCurrentItem = false;

            ribbonPanel.AddItem(NewPushButtonData<ClearAll>(text: "Clear\nSelection"));
        }

        #endregion Group Clash&Join

        #region benchmark
        {
            RibbonPanel ribbonPanel = AddRibbonPanel("Clash&Join", "Benchmark");
            ribbonPanel.AddItem(
                NewPushButtonData<BenchBoundingBoxIntersect>(text: "BoundingBox\nIntersect")
            );
            ribbonPanel.AddItem(NewPushButtonData<BenchClashDetect>(text: "Clash\nDetection"));
        }
        #endregion benchmark
    }
}
