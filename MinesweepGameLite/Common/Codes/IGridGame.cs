namespace Common {
    /// <summary>
    /// 用于游戏窗口的Game项
    /// </summary>
    public interface IGridGame {
        MainGameWindow GameWindow { get; set; }
        GameType Type { get; }
        string GameSizeStatus { get; }
        string ProcessStatus { get; }
        void StartGame();
        void UnloadGame();
        void QuickStartGame(int level);
    }
}
