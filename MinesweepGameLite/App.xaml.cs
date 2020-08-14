using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using static MinesweepGameLite.MainCodes.GeneralAction;

namespace MinesweepGameLite {
    /// <summary>
    /// 应用载入操作，包括载入资源，读取启动参数
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
        #region 自定义光标与音频
        public static readonly MediaPlayer BlockClickSound = new MediaPlayer();
        public static readonly MediaPlayer BlockFlagSound = new MediaPlayer();
        public static readonly MediaPlayer MenuMouseHoverSound = new MediaPlayer();
        public static readonly MediaPlayer MenuButtonClickSound = new MediaPlayer();
        public static readonly Cursor NormalCursor = new Cursor(new MemoryStream(MinesweepGameLite.Properties.Resources.CursorStatic));
        public static readonly Cursor ClickedCursor = new Cursor(new MemoryStream(MinesweepGameLite.Properties.Resources.CursorClicked));
        public static readonly Cursor LoadingGameCursor = new Cursor(new MemoryStream(MinesweepGameLite.Properties.Resources.LoadingGame));
        public static readonly Cursor DetectorAimerCursor = new Cursor(new MemoryStream(MinesweepGameLite.Properties.Resources.DetectorAimer));
        public static readonly DoubleAnimation AnimationForBlurEffect = new DoubleAnimation {
            From = 0,
            To = 20,
            AccelerationRatio = 0.2,
            DecelerationRatio = 0.8,
            Duration = TimeSpan.FromMilliseconds(200)
        };
        #endregion
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
        public static void PlayFXSound(string soundName) {
            if (!App.IsSoundEnabled) {
                return;
            }
            Uri path = new Uri($@"{App.UserTempFilePath}\{soundName}.wav", UriKind.Absolute);
            switch (soundName) {
                case nameof(App.BlockClickSound):
                    App.BlockClickSound.Open(path);
                    App.BlockClickSound.Play();
                    break;
                case nameof(App.BlockFlagSound):
                    App.BlockFlagSound.Open(path);
                    App.BlockFlagSound.Play();
                    break;
                case nameof(App.MenuMouseHoverSound):
                    App.MenuMouseHoverSound.Open(path);
                    App.MenuMouseHoverSound.Play();
                    break;
                case nameof(App.MenuButtonClickSound):
                    App.MenuButtonClickSound.Open(path);
                    App.MenuButtonClickSound.Play();
                    break;
            }

        }
    }
}
