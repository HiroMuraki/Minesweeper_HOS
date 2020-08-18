﻿using System;
using System.Windows;
using static Common.GeneralAction;

namespace Common {
    /// <summary>
    /// 应用载入操作，包括载入资源，读取启动参数
    /// </summary>
    public partial class App : Application {
        //是否启用音频
        public static bool IsSoundEnabled = true;
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
                }
            }
            if (IsSoundEnabled) {
                UserTempFilePath = Environment.GetEnvironmentVariable("TEMP");
                InitializeResources();
            }
            new MainGameWindow().Show();
        }
        /// <summary>
        /// 初始化资源，用于将内嵌资源复制到指定目录
        /// </summary>
        private void InitializeResources() {
            foreach (string soundName in SoundResources) {
                string fileFullPath = $@"{UserTempFilePath}\{soundName}";
                CopyInsertedFileToPath($"Resources.Sound.{soundName}", fileFullPath);
            }
        }
    }
}
