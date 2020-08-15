using System;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using static Common.GeneralAction;

namespace Common {
    /// <summary>
    /// 应用载入操作，包括载入资源，读取启动参数
    /// </summary>
    public partial class App : Application {
        public static bool IsSoundEnabled = true;
        public static GameType DefaultGame = GameType.Minesweeper;
        public static string UserTempFilePath;
        
        private void Application_Startup(object sender, StartupEventArgs e) {
            foreach (string arg in e.Args) {
                if (arg.StartsWith("-")) {
                    string arg1 = arg.Trim('-', '+').ToUpper();
                    if (arg1 == "NOSOUND" || arg1 == "SILENT") {
                        IsSoundEnabled = false;
                        continue;
                    }
                    if (arg1 == "SLIDE" || arg1 == "SLIDEJIGSAW") {
                        DefaultGame = GameType.SlideJigsaw;
                        continue;
                    }
                }
            }
            if (IsSoundEnabled) {
                UserTempFilePath = Environment.GetEnvironmentVariable("TEMP");
                InitializeResources();
            }
            new MainGameWindow().Show();
        }
        private void InitializeResources() {
            foreach (string soundName in SoundResources) {
                string fileFullPath = $@"{UserTempFilePath}\{soundName}";
                CopyInsertedFileToPath($"Resources.Sound.{soundName}", fileFullPath);
            }
        }
    }
}
