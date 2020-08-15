using System;
using System.IO;
using System.Reflection;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Common {
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
        /// 用于播放音频
        /// </summary>
        /// <param name="soundName">传入音频文件文件名（无扩展名），但扩展名为.wav</param>
        public static readonly string[] SoundResources = new string[] {
            "BlockClickSound.wav",
            "BlockFlagSound.wav",
            "MenuMouseHoverSound.wav",
            "MenuButtonClickSound.wav"
        };
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
    }
}
