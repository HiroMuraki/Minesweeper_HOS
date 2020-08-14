using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using static MinesweepGameLite.MainCodes.GeneralAction;

namespace MinesweepGameLite {
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application {
        private Window gameWindow;
        public static bool IsSoundEnabled = true;
        public static string UserTempFilePath;
        private static readonly string[] SoundResources = new string[] {
            "BlockClickSound.wav",
            "BlockFlagSound.wav",
            "MenuMouseHoverSound.wav",
            "MenuButtonClickSound.wav"
        };
        private void Application_Startup(object sender, StartupEventArgs e) {
            if (e.Args.Length > 0) {
                string arg1 = e.Args[0].Trim('-', '+').ToUpper();
                if (arg1 == "NOSOUND" || arg1 == "SILENT") {
                    IsSoundEnabled = false;
                }
            }
            if (IsSoundEnabled) {
                UserTempFilePath = Environment.GetEnvironmentVariable("TEMP");
                InitializeResources();
            }
            gameWindow = new MinesweeperGameWindow();
            gameWindow.Show();
        }
        private void InitializeResources() {
            foreach (string soundName in SoundResources) {
                string fileFullPath = $@"{UserTempFilePath}\{soundName}";
                CopyInsertedFileToPath($"Resources.Sound.{soundName}", fileFullPath);
            }
        }
    }
}
