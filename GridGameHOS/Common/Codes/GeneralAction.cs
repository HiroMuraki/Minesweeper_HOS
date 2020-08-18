using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Common {
    /// <summary>
    /// 常见且具有通用性的操作
    /// </summary>
    public static class GeneralAction {
        /// <summary>
        /// 初始化资源
        /// </summary>
        #region 自定义光标与音频
        public static readonly MediaPlayer BlockClickSound = new MediaPlayer();
        public static readonly MediaPlayer BlockFlagSound = new MediaPlayer();
        public static readonly MediaPlayer MenuMouseHoverSound = new MediaPlayer();
        public static readonly MediaPlayer MenuButtonClickSound = new MediaPlayer();
        public static readonly Cursor NormalCursor = new Cursor(new MemoryStream(GridGameHOS.Properties.Resources.CursorStatic));
        public static readonly Cursor ClickedCursor = new Cursor(new MemoryStream(GridGameHOS.Properties.Resources.CursorClicked));
        public static readonly Cursor LoadingGameCursor = new Cursor(new MemoryStream(GridGameHOS.Properties.Resources.LoadingGame));
        public static readonly Cursor DetectorAimerCursor = new Cursor(new MemoryStream(GridGameHOS.Properties.Resources.DetectorAimer));
        public static readonly DoubleAnimation AnimationForBlurEffect = new DoubleAnimation {
            From = 0,
            To = 15,
            AccelerationRatio = 0.2,
            DecelerationRatio = 0.8,
            Duration = TimeSpan.FromMilliseconds(200)
        };
        #endregion
        /// <summary>
        /// 可用游戏列表，用于在可用游戏枚举和对应文字描述之间映射
        /// </summary>
        public static Dictionary<GameType, string> GameDictionary = new Dictionary<GameType, string> {
            {GameType.Minesweeper,"扫雷" },
            {GameType.SlideJigsaw,"滑块拼图" },
            {GameType.TwoZeroFourEight,"2048H" }
        };
        /// <summary>
        /// 用于将嵌入资源复制到指定目录
        /// </summary>
        /// <param name="resourceName">传递嵌入资源路径</param>
        /// <param name="targetPath">传递复制目标路径路径</param>
        public static void CopyInsertedFileToPath(string resourceName, string targetPath) {
            Assembly assembly = Assembly.GetExecutingAssembly();
            string assemblyName = Assembly.GetExecutingAssembly().GetName().Name;
            Stream insertedResource = assembly.GetManifestResourceStream($@"{assemblyName}.{resourceName}");
            BufferedStream fileReader = new BufferedStream(insertedResource);
            FileStream fileWritter = File.OpenWrite($"{targetPath}");

            byte[] buffer = new byte[128];
            int length;
            do {
                length = fileReader.Read(buffer, 0, buffer.Length);
                fileWritter.Write(buffer, 0, length);
            } while (length > 0);

            insertedResource.Close();
            fileReader.Close();
            fileWritter.Close();
        }
        /// <summary>
        ///  记录音频列表
        /// </summary>
        public static readonly string[] SoundResources = new string[] {
            "BlockClickSound.wav",
            "BlockFlagSound.wav",
            "MenuMouseHoverSound.wav",
            "MenuButtonClickSound.wav"
        };
        /// <summary>
        /// 用于播放音频
        /// </summary>
        /// <param name="soundName">传入音频文件文件名（无扩展名），但扩展名为.wav</param>
        public static void PlayFXSound(string soundName) {
            if (!App.IsSoundEnabled) {
                return;
            }
            Uri path = new Uri($@"{App.UserTempFilePath}\{soundName}.wav", UriKind.Absolute);
            switch (soundName) {
                case nameof(BlockClickSound):
                    BlockClickSound.Open(path);
                    BlockClickSound.Play();
                    break;
                case nameof(BlockFlagSound):
                    BlockFlagSound.Open(path);
                    BlockFlagSound.Play();
                    break;
                case nameof(MenuMouseHoverSound):
                    MenuMouseHoverSound.Open(path);
                    MenuMouseHoverSound.Play();
                    break;
                case nameof(MenuButtonClickSound):
                    MenuButtonClickSound.Open(path);
                    MenuButtonClickSound.Play();
                    break;
            }
        }
        /// <summary>
        /// 播放缩放动画
        /// </summary>
        /// <param name="element">要缩放的控件</param>
        /// <param name="scaleFrom">缩放的起始值</param>
        /// <param name="scaleTo">缩放的目标值</param>
        /// <param name="duration">动画持续时间，单位为毫秒</param>
        public static void PlayScaleTransform(UIElement element, double scaleFrom, double scaleTo, int duration) {
            ScaleTransform scale = new ScaleTransform();
            DoubleAnimation animation = new DoubleAnimation() {
                From = scaleFrom,
                To = scaleTo,
                AccelerationRatio = 0.2,
                DecelerationRatio = 0.5,
                Duration = TimeSpan.FromMilliseconds(duration)
            };
            element.RenderTransform = scale;
            element.RenderTransformOrigin = new Point(0.5, 0.5);
            scale.BeginAnimation(ScaleTransform.ScaleXProperty, animation);
            scale.BeginAnimation(ScaleTransform.ScaleYProperty, animation);
        }
    }
}
