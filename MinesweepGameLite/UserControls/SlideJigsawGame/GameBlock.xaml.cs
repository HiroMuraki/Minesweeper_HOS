using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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
