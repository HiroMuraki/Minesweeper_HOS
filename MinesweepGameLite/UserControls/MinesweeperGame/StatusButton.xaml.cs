using System;
using System.CodeDom;
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

namespace MinesweepGameLite {
    /// <summary>
    /// StatusButton.xaml 的交互逻辑
    /// </summary>
    public partial class StatusButton : UserControl {
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

        

        public StatusButton() {
            InitializeComponent();
        }
    }
}
