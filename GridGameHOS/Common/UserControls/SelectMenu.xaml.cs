using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace GridGameHOS.Common {
    /// <summary>
    /// UserControl1.xaml 的交互逻辑
    /// </summary>
    public partial class SelectMenu : UserControl {
        /// <summary>
        /// 用于保存菜单的可切换标签列表
        /// </summary>
        public List<string> AllowedLabels { get; set; }
        /// <summary>
        /// 用于跟踪当前标签的下标位置
        /// </summary>
        private int currentLabelIndex = 0;
        /// <summary>
        /// 用于表示当前选择的标签，是依赖属性
        /// </summary>
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
        /// <summary>
        /// 切换标签时的事件，由控件调用方使用
        /// </summary>
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
        /// <summary>
        /// 切换标签时的事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LArrow_Click(object sender, MouseButtonEventArgs e) {
            --currentLabelIndex;
            if (currentLabelIndex < 0) {
                currentLabelIndex = AllowedLabels.Count - 1;
            }
            CurrentLabel = AllowedLabels[currentLabelIndex];
            RoutedEventArgs args = new RoutedEventArgs(LabelSwitchedEvent, this);
            RaiseEvent(args);
        }
        private void RArrow_Click(object sender, MouseButtonEventArgs e) {
            ++currentLabelIndex;
            if (currentLabelIndex >= AllowedLabels.Count) {
                currentLabelIndex = 0;
            }
            CurrentLabel = AllowedLabels[currentLabelIndex];
            RoutedEventArgs args = new RoutedEventArgs(LabelSwitchedEvent, this);
            RaiseEvent(args);
        }
    }
}
