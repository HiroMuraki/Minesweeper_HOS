using System.Collections.ObjectModel;

namespace Common {
    /// <summary>
    /// 用于游戏窗口的Game项
    /// </summary>
    public interface IGridGame {
        MainGameWindow GameWindow { get; set; }
        ObservableCollection<IBlocks> BlocksArray { get; }
        GameType Type { get; }
        string GameSizeStatus { get; }
        string ProcessStatus { get; }
        void StartGame();
        void UnloadGame();
        void QuickGame(int level);
    }
}
