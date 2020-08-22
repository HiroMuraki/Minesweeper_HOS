using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MinesweeperGameLite {
    /// <summary>
    /// MineBlock.xaml 的交互逻辑
    /// </summary>
    public partial class GameBlock : UserControl {
        #region 属性
        //是否打开旗帜标记
        public bool IsFlaged {
            get {
                return (bool)GetValue(IsFlagedProperty);
            }
            set {
                SetValue(IsFlagedProperty, value);
            }
        }
        public static readonly DependencyProperty IsFlagedProperty =
            DependencyProperty.Register("IsFlaged", typeof(bool), typeof(GameBlock), new PropertyMetadata(false));
        //是否打开
        public bool IsOpen {
            get {
                return (bool)GetValue(IsOpenProperty);
            }
            set {
                SetValue(IsOpenProperty, value);
            }
        }
        public static readonly DependencyProperty IsOpenProperty =
            DependencyProperty.Register("IsOpen", typeof(bool), typeof(GameBlock), new PropertyMetadata(false));
        //周围雷数
        public int NearMinesCount {
            get {
                return (int)GetValue(NearMinesCountProperty);
            }
            set {
                SetValue(NearMinesCountProperty, value);
            }
        }
        public static readonly DependencyProperty NearMinesCountProperty =
            DependencyProperty.Register("NearMinesCount", typeof(int), typeof(GameBlock), new PropertyMetadata(0));
        //是否为雷块
        public bool IsMineBlock {
            get {
                return (bool)GetValue(IsMineBlockProperty);
            }
            set {
                SetValue(IsMineBlockProperty, value);
            }
        }
        public static readonly DependencyProperty IsMineBlockProperty =
            DependencyProperty.Register("IsMineBlock", typeof(bool), typeof(GameBlock), new PropertyMetadata(false));
        #endregion

        #region 事件
        //打开事件
        public event RoutedEventHandler OpenBlock {
            add {
                AddHandler(OpenBlockEvent, value);
            }
            remove {
                RemoveHandler(OpenBlockEvent, value);
            }
        }
        public static readonly RoutedEvent OpenBlockEvent = EventManager.RegisterRoutedEvent(
            "OpenBlock", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(GameBlock));
        private void OnOpenBlock(object sender, MouseButtonEventArgs e) {
            RoutedEventArgs args = new RoutedEventArgs(OpenBlockEvent, this);
            RaiseEvent(args);
        }
        //标记事件
        public event RoutedEventHandler FlagBlock {
            add {
                AddHandler(FlagBlockEvent, value);
            }
            remove {
                RemoveHandler(FlagBlockEvent, value);
            }
        }
        public static readonly RoutedEvent FlagBlockEvent = EventManager.RegisterRoutedEvent(
            "FlagBlock", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(GameBlock));
        private void OnFlagBlock(object sender, MouseButtonEventArgs e) {
            RoutedEventArgs args = new RoutedEventArgs(FlagBlockEvent, this);
            RaiseEvent(args);
        }
        //快开事件
        public event RoutedEventHandler DoubleOpenBlock {
            add {
                AddHandler(DoubleOpenBlockEvent, value);
            }
            remove {
                RemoveHandler(DoubleOpenBlockEvent, value);
            }
        }
        public static readonly RoutedEvent DoubleOpenBlockEvent = EventManager.RegisterRoutedEvent(
            "DoubleOpenBlock", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(GameBlock));
        private void OnDoubleOpenBlock(object sender, MouseButtonEventArgs e) {
            RoutedEventArgs args = new RoutedEventArgs(DoubleOpenBlockEvent, this);
            RaiseEvent(args);

        }
        #endregion

        //构造函数
        public GameBlock() {
            InitializeComponent();
        }
    }
}
