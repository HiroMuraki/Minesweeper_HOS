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

namespace Common {
    /// <summary>
    /// UserControl1.xaml 的交互逻辑
    /// </summary>
    public partial class SelectMenu : UserControl {
        public List<string> AllowedLabels { get; set; }
        private int currentLabelIndex = 0;
        public string CurrentLabel {
            get {
                return (string)GetValue(CurrentLabelProperty);
            }
            set {
                SetValue(CurrentLabelProperty, value);
            }
        }
        public static readonly DependencyProperty CurrentLabelProperty =
            DependencyProperty.Register("CurrentLabel", typeof(string), typeof(SelectMenu), new PropertyMetadata(""));

        public event RoutedEventHandler LabelSwitched {
            add {
                AddHandler(LabelSwitchedEvent, value);
            }
            remove {
                RemoveHandler(LabelSwitchedEvent, value);
            }
        }
        public static readonly RoutedEvent LabelSwitchedEvent = EventManager.RegisterRoutedEvent(
            "LabelSwitched", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(SelectMenu));

        public SelectMenu() {
            InitializeComponent();
        }
        private void LArrow_Click(object sender, MouseButtonEventArgs e) {
            --currentLabelIndex;
            if (currentLabelIndex < 0) {
                currentLabelIndex = AllowedLabels.Count - 1;
            }
            this.CurrentLabel = AllowedLabels[currentLabelIndex];
            RoutedEventArgs args = new RoutedEventArgs(LabelSwitchedEvent, this);
            RaiseEvent(args);
        }
        private void RArrow_Click(object sender, MouseButtonEventArgs e) {
            ++currentLabelIndex;
            if (currentLabelIndex >= this.AllowedLabels.Count) {
                currentLabelIndex = 0;
            }
            this.CurrentLabel = AllowedLabels[currentLabelIndex];
            RoutedEventArgs args = new RoutedEventArgs(LabelSwitchedEvent, this);
            RaiseEvent(args);
        }
    }
}
