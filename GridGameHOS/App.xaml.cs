using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using static Common.GeneralAction;

namespace Common {
    /// <summary>
    /// 应用载入操作，包括载入资源，读取启动参数
    /// </summary>
    public partial class App : Application {
        //用于指示音频是否可用
        public static bool IsSoundAvaliable = false;
        //是否启用重置启动
        public static bool IsResetStart = false;
        //是否启用音频
        public static bool IsSoundEnabled = true;
        //是否以精简模式启动
        public static bool IsLiteMode = false;
        //游戏类型
        public static GameType DefaultGame = GameType.Minesweeper;
        //用户系统的临时文件目录，用于存放临时资源文件
        public static string UserTempFilePath { get; private set; }
        /// <summary>
        /// 程序载入时的操作，包括读取启动参数，载入资源
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Application_Startup(object sender, StartupEventArgs e) {
            foreach (string arg in e.Args) {
                if (arg.StartsWith("-")) {
                    string arg1 = arg.Trim('-', '+').ToLower();
                    if (arg1 == "nosound" || arg1 == "silent") {
                        IsSoundEnabled = false;
                        continue;
                    }
                    if (arg1 == "slide" || arg1 == "slidejigsaw") {
                        DefaultGame = GameType.SlideJigsaw;
                        continue;
                    }
                    if (arg1 == "2048" || arg1 == "2048H") {
                        DefaultGame = GameType.TwoZeroFourEight;
                        continue;
                    }
                    if (arg1 == "sound" || arg1 == "reset") {
                        IsResetStart = true;
                    }
                    if (arg1 == "litemode" || arg1 == "lite") {
                        IsLiteMode = true;
                    }
                }
            }
            MainGameWindow gameWindow = new MainGameWindow();
            if (IsSoundEnabled) {
                UserTempFilePath = Environment.GetEnvironmentVariable("TEMP");
                InitializeResources();
            }
            if (IsLiteMode) {
                gameWindow.SettingMenu.Visibility = Visibility.Collapsed;
            }
            gameWindow.Show();
        }
        /// <summary>
        /// 初始化资源，用于将内嵌资源复制到指定目录
        /// </summary>
        private void InitializeResources() {
            //音频资源
            foreach (string soundName in SoundResources) {
                string fileFullPath = $@"{UserTempFilePath}\{soundName}";
                if (File.Exists(fileFullPath) && !IsResetStart) {
                    continue;
                }
                CopyInsertedFileToPath($"Resources.Sound.{soundName}", fileFullPath);
            }
            IsSoundAvaliable = true;
        }
        /// <summary>
        /// 关闭应用时清理释放的临时资源
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Application_Exit(object sender, ExitEventArgs e) {
            ClearResourcesAsync();
        }
        /// <summary>
        /// 清理释放的资源文件
        /// </summary>
        private async void ClearResourcesAsync() {
            //音频资源
            await Task.Run(() => {
                foreach (string soundName in SoundResources) {
                    string fileFullPath = $@"{UserTempFilePath}\{soundName}";
                    if (File.Exists(fileFullPath)) {
                        File.Delete(fileFullPath);
                    }
                }
            });
        }
    }
}
