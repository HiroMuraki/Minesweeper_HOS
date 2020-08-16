using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Common {
    /// <summary>
    /// StatusButton.xaml 的交互逻辑
    /// </summary>
    public partial class StatusButton : UserControl {
        /// <summary>
        /// 用于表示按钮是否处于活动状态，null标识未知，false标识非激活，true标识激活
        /// </summary>
        public bool? IsOn {
            get {
                return (bool?)GetValue(IsOnProperty);
            }
            set {
                SetValue(IsOnProperty, value);
            }
        }
        public static readonly DependencyProperty IsOnProperty =
            DependencyProperty.Register("IsOn", typeof(bool?), typeof(StatusButton), new PropertyMetadata(null));
        /// <summary>
        /// 鼠标左键点击时调用的方法
        /// </summary>
        public event RoutedEventHandler ButtonClick {
            add {
                AddHandler(ButtonClickEvent, value);
            }
            remove {
                RemoveHandler(ButtonClickEvent, value);
            }
        }
        public static readonly RoutedEvent ButtonClickEvent = EventManager.RegisterRoutedEvent(
            "ButtonClick", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(StatusButton));
        private void OnButtonClick(object sender, MouseButtonEventArgs e) {
            RoutedEventArgs args = new RoutedEventArgs(ButtonClickEvent, this);
            RaiseEvent(args);
        }
        /// <summary>
        /// 鼠标右键点击时调用的方法
        /// </summary>
        public event RoutedEventHandler ButtonRightClick {
            add {
                AddHandler(ButtonRightClickEvent, value);
            }
            remove {
                RemoveHandler(ButtonRightClickEvent, value);
            }
        }
        public static readonly RoutedEvent ButtonRightClickEvent = EventManager.RegisterRoutedEvent(
            "ButtonRightClick", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(StatusButton));
        private void OnButtonRightClick(object sender, MouseButtonEventArgs e) {
            RoutedEventArgs args = new RoutedEventArgs(ButtonRightClickEvent, this);
            RaiseEvent(args);
        }
        /// <summary>
        /// 默认构造函数
        /// </summary>
        public StatusButton() {
            InitializeComponent();
        }
    }
}
