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

namespace Mason
{
    /// <summary>
    /// Main external application class for Mason Revit plugin.
    /// Initializes logger and registers commands and UI panels.
    /// </summary>
    public class App : IExternalApplication
    {
        private readonly Logger Log = ConfigLog();

        /// <summary>
        /// Called on Revit startup to initialize Mason.
        /// </summary>
        public Result OnStartup(UIControlledApplication application)
        {
            Log.Info($"OS: {Environment.OSVersion}");
            string revitVersion = application.ControlledApplication.SubVersionNumber;
            string revitBuild = application.ControlledApplication.VersionBuild;
            Log.Info($"Autodesk Revit {revitVersion} {revitBuild}");

            string masonVersion = FileVersionInfo
                .GetVersionInfo(Assembly.GetExecutingAssembly().Location)
                .ProductVersion.Split('+')[0];
            Log.Info($"Mason {masonVersion}");

            // Register all commands and Ribbon UI
            new CommandRegister(application).Register();

            return Result.Succeeded;
        }

        /// <summary>
        /// Called on Revit shutdown.
        /// </summary>
        public Result OnShutdown(UIControlledApplication application)
        {
            Log.Info("Shutdown Mason.");
            return Result.Succeeded;
        }

        /// <summary>
        /// Configures NLog for the plugin.
        /// </summary>
        private static Logger ConfigLog()
        {
            string logDir = Path.Combine(
                Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!,
                "../log"
            );
            NLog.Config.LoggingConfiguration config = new();
            NLog.Targets.FileTarget logfile = new("MasonLogFile")
            {
                FileName = $"{logDir}/{DateTime.Now:yyyyMMdd-HHmmss}.log",
            };

#if DEBUG
            config.AddRule(LogLevel.Debug, LogLevel.Fatal, logfile);
#endif
#if !DEBUG
            config.AddRule(LogLevel.Info, LogLevel.Fatal, logfile);
#endif
            LogManager.Configuration = config;

            Logger logger = LogManager.GetCurrentClassLogger();
            logger.Debug("Finish Initialize Logger.");

#if DEBUG
            // Open log automatically in Debug mode
            Process.Start(
                new ProcessStartInfo(logfile.FileName.ToString()) { UseShellExecute = true }
            );
#endif
            return logger;
        }
    }

    /// <summary>
    /// Handles all Ribbon UI creation and command registration.
    /// </summary>
    internal sealed class CommandRegister
    {
        private readonly UIControlledApplication UICApp;
        private readonly Logger Log = LogManager.GetCurrentClassLogger();

        public CommandRegister(UIControlledApplication uicapp)
        {
            UICApp = uicapp ?? throw new ArgumentNullException(nameof(uicapp));
        }

        /// <summary>
        /// Adds a Ribbon panel in a tab, creating tab if it does not exist.
        /// </summary>
        private RibbonPanel AddRibbonPanel(string tabName, string ribbonPanelName)
        {
            try
            {
                UICApp.CreateRibbonTab(tabName);
                Log.Info($"Added Ribbon Tab: {tabName}");
            }
            catch
            {
                Log.Info($"Ribbon Tab already exists: {tabName}");
            }

            RibbonPanel ribbonPanel;
            try
            {
                ribbonPanel = UICApp.CreateRibbonPanel(tabName, ribbonPanelName);
                Log.Info($"Added Ribbon Panel: {ribbonPanelName}");
            }
            catch
            {
                Log.Info($"Ribbon Panel already exists: {ribbonPanelName}");
                ribbonPanel = UICApp.GetRibbonPanels(tabName).Find(p => p.Name == ribbonPanelName)!;
            }

            return ribbonPanel;
        }

        /// <summary>
        /// Loads an embedded PNG icon as BitmapImage for buttons.
        /// </summary>
        private BitmapImage GetIcon(string iconName, bool large)
        {
            iconName ??= "default";
            Assembly assembly = Assembly.GetExecutingAssembly();
            string resourceName = $"Mason.Icon.{iconName}.png";

            Stream stream = assembly.GetManifestResourceStream(resourceName);
            if (stream == null)
            {
                Log.Warn($"Icon not found: {resourceName}, using default icon.");
                stream = assembly.GetManifestResourceStream("Mason.Icon.default.png");
            }

            using Stream fs = stream!;
            System.Drawing.Bitmap bitmap = new(fs);
            using MemoryStream memory = new();
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

            return bitmapImage;
        }

        /// <summary>
        /// Creates PushButtonData for a Revit command.
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
            Log.Info($"Created PushButtonData: {cls.FullName}");
            return pbd;
        }

        /// <summary>
        /// Registers all commands and UI buttons.
        /// </summary>
        public void Register()
        {
            Log.Info("Start register commands.");
            RegisterCommand();
            RegisterClashAndJoin();
            Log.Info("Finished registering commands.");
        }

        /// <summary>
        /// Registers generic commands for Structure, MEP, Misc, and DevTool panels.
        /// </summary>
        private void RegisterCommand()
        {
            const string tabName = "Mason";

            #region Structure
            {
                RibbonPanel ribbonPanel = AddRibbonPanel(tabName, "Structure");
                ribbonPanel.AddItem(
                    NewPushButtonData<DisallowJoinBeam>(
                        "Disallow\nJoin Beam",
                        longDescription: "Set end of framing element to disallow join."
                    )
                );
                ribbonPanel.AddItem(
                    NewPushButtonData<FlipBeam>(
                        "Flip\nBeam",
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
                        "Flip\nPipe",
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
                        "Reset\nGraphics",
                        longDescription: "Reset graphics override to default."
                    )
                );
                ribbonPanel.AddItem(
                    NewPushButtonData<ExportWorksetsNWC>(
                        "Export\nWorksets NWC",
                        iconName: "navisworks",
                        toolTip: "Export NWC of each workset in current 3D view."
                    )
                );
                ribbonPanel.AddItem(
                    NewPushButtonData<CreateWorksetView>(
                        "Create\nWorksets View",
                        toolTip: "Create Views for each Workset."
                    )
                );
                ribbonPanel.AddItem(
                    NewPushButtonData<ClearCAD>(
                        "Clear\nCAD",
                        longDescription: "Clear CAD files in current document."
                    )
                );
            }
            #endregion Misc

            #region DevTool
            {
                RibbonPanel ribbonPanel = AddRibbonPanel(tabName, "DevTool");
                ribbonPanel.AddItem(NewPushButtonData<Command.DevTool.HelloWorld>("Hello\nWorld"));
                ribbonPanel.AddItem(
                    NewPushButtonData<Command.DevTool.ThrowError>(
                        "Throw\nError",
                        longDescription: "Throw an error for test."
                    )
                );
                ribbonPanel.AddItem(
                    NewPushButtonData<Command.DevTool.OpenLog>(
                        "Open\nLog",
                        iconName: "log",
                        longDescription: "Open log file."
                    )
                );
                ribbonPanel.AddItem(
                    NewPushButtonData<Command.DevTool.OpenRevitAPI>(
                        "Open\nRevit API",
                        iconName: "revit",
                        longDescription: "Open Revit API in browser."
                    )
                );
            }
            #endregion DevTool
        }

        /// <summary>
        /// Registers Clash&Join commands for Self, Group, and Benchmark panels.
        /// </summary>
        private void RegisterClashAndJoin()
        {
            #region Self Clash&Join
            {
                RibbonPanel ribbonPanel = AddRibbonPanel("Clash&Join", "Self Clash&Join");
                ribbonPanel.AddItem(NewPushButtonData<SelfClashDetection>("Clash\nDetection"));
                ribbonPanel.AddItem(NewPushButtonData<SelfJoin>("Join\nElements"));
                ribbonPanel.AddItem(NewPushButtonData<SelfUnjoin>("Unjoin\nElements"));
                ribbonPanel.AddItem(NewPushButtonData<SelfJoinWallOpening>("Join\nWall Opening"));
            }
            #endregion Self Clash&Join

            #region Group Clash&Join
            {
                RibbonPanel ribbonPanel = AddRibbonPanel("Clash&Join", "Group Clash&Join");

                ribbonPanel.AddItem(NewPushButtonData<GroupClashDetection>("Clash\nDetection"));
                ribbonPanel.AddItem(NewPushButtonData<GroupJoin>("Join\nElements"));
                ribbonPanel.AddItem(NewPushButtonData<GroupUnjoin>("Union\nElements"));
                ribbonPanel.AddItem(NewPushButtonData<GroupSwitchJoin>("Switch\nJoin"));

                // Group1 split button
                SplitButton spb1 =
                    ribbonPanel.AddItem(
                        new SplitButtonData("SplitButtonSelectJoinGroup1", "Group1")
                    ) as SplitButton;
                spb1?.AddPushButton(NewPushButtonData<SelectGroup1>("Select\nGroup 1"));
                spb1?.AddPushButton(NewPushButtonData<AppendGroup1>("Append\nGroup 1"));
                spb1?.AddPushButton(NewPushButtonData<ClearGroup1>("Clear\nGroup 1"));
                if (spb1 != null)
                    spb1.IsSynchronizedWithCurrentItem = false;

                // Group2 split button
                SplitButton spb2 =
                    ribbonPanel.AddItem(
                        new SplitButtonData("SplitButtonSelectJoinGroup2", "Group2")
                    ) as SplitButton;
                spb2?.AddPushButton(NewPushButtonData<SelectGroup2>("Select\nGroup 2"));
                spb2?.AddPushButton(NewPushButtonData<AppendGroup2>("Append\nGroup 2"));
                spb2?.AddPushButton(NewPushButtonData<ClearGroup2>("Clear\nGroup 2"));
                if (spb2 != null)
                    spb2.IsSynchronizedWithCurrentItem = false;

                ribbonPanel.AddItem(NewPushButtonData<ClearAll>("Clear\nSelection"));
            }
            #endregion Group Clash&Join

            #region Benchmark
            {
                RibbonPanel ribbonPanel = AddRibbonPanel("Clash&Join", "Benchmark");
                ribbonPanel.AddItem(
                    NewPushButtonData<BenchBoundingBoxIntersect>("BoundingBox\nIntersect")
                );
                ribbonPanel.AddItem(NewPushButtonData<BenchClashDetect>("Clash\nDetection"));
            }
            #endregion Benchmark
        }
    }
}
