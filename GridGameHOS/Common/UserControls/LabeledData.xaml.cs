using System.Windows;
using System.Windows.Controls;

namespace GridGameHOS.Common {
    /// <summary>
    /// LabeledData.xaml 的交互逻辑
    /// </summary>
    public partial class LabeledData : UserControl {
        /// <summary>
        /// 数据标签
        /// </summary>
        public string DataHeader {
            get {
                return (string)GetValue(DataHeaderProperty);
            }
            set {
                SetValue(DataHeaderProperty, value);
            }
        }
        public static readonly DependencyProperty DataHeaderProperty =
            DependencyProperty.Register("DataHeader", typeof(string), typeof(LabeledData), new PropertyMetadata(""));
        /// <summary>
        /// 数据内容
        /// </summary>
        public string DataDetails {
            get {
                return (string)GetValue(DataDetailsProperty);
            }
            set {
                SetValue(DataDetailsProperty, value);
            }
        }
        public static readonly DependencyProperty DataDetailsProperty =
            DependencyProperty.Register("DataDetails", typeof(string), typeof(LabeledData), new PropertyMetadata(""));
        /// <summary>
        /// 默认构造函数
        /// </summary>
        public LabeledData() {
            InitializeComponent();
        }
    }
}
