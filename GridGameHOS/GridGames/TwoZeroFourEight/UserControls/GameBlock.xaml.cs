using System.Windows;
using System.Windows.Controls;

namespace GridGameHOS.TwoZeroFourEightLite {
    /// <summary>
    /// GameBlock.xaml 的交互逻辑
    /// </summary>
    public partial class GameBlock : UserControl, IGameBlock {
        public int Number {
            get {
                return (int)GetValue(NumberProperty);
            }
            set {
                SetValue(NumberProperty, value);
            }
        }
        public static readonly DependencyProperty NumberProperty =
            DependencyProperty.Register("Number", typeof(int), typeof(GameBlock), new PropertyMetadata(0));

        public GameBlock() {
            InitializeComponent();
        }
    }
}
