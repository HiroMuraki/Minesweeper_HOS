using System.Collections.ObjectModel;

namespace GridGameHOS.Common {
    /// <summary>
    /// 用于游戏窗口的Game项
    /// </summary>
    public interface IGridGame {
        /// <summary>
        /// 游戏关联的游戏窗口
        /// </summary>
        MainGameWindow GameWindow { get; set; }
        /// <summary>
        /// 用于标注当前游戏行数
        /// </summary>
        int RowSize { get; }
        /// <summary>
        /// 用于标注当前游戏列数
        /// </summary>
        int ColumnSize { get; }
        /// <summary>
        /// 用于在游戏窗口上显示方块序列
        /// </summary>
        ObservableCollection<IBlocks> BlocksArray { get; }
        /// <summary>
        /// 游戏类型
        /// </summary>
        GameType Type { get; }
        /// <summary>
        /// 游戏大小状态
        /// </summary>
        string GameSizeStatus { get; }
        /// <summary>
        /// 游戏进度状态
        /// </summary>
        string ProcessStatus { get; }
        /// <summary>
        /// 开始游戏的方法
        /// </summary>
        void StartGame();
        /// <summary>
        /// 卸载游戏的方法，当切换游戏时调用
        /// </summary>
        void UnloadGame();
        /// <summary>
        /// 快速游戏，传入level标记
        /// </summary>
        /// <param name="level"></param>
        void QuickGame(int level);
    }
}
