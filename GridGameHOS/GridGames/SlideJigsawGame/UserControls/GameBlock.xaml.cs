using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SlideJigsawGameLite {
    /// <summary>
    /// GameBlock.xaml 的交互逻辑
    /// </summary>
    public partial class GameBlock : UserControl {
        public int BlockID {
            get {
                return (int)GetValue(BlockIDProperty);
            }
            set {
                SetValue(BlockIDProperty, value);
            }
        }
        public static readonly DependencyProperty BlockIDProperty =
            DependencyProperty.Register("BlockID", typeof(int), typeof(GameBlock), new PropertyMetadata(0));

        public event RoutedEventHandler ButtonClick {
            add {
                AddHandler(ButtonClickEvent, value);
            }
            remove {
                RemoveHandler(ButtonClickEvent, value);
            }
        }
        public static readonly RoutedEvent ButtonClickEvent = EventManager.RegisterRoutedEvent(
            "ButtonClick", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(GameBlock));
        private void OnButtonClick(object sender, MouseButtonEventArgs e) {
            RoutedEventArgs args = new RoutedEventArgs(ButtonClickEvent, this);
            RaiseEvent(args);
        }

        public GameBlock() {
            InitializeComponent();
        }


    }
}
