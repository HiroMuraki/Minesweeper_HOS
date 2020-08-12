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

namespace MinesweepGameLite {
    /// <summary>
    /// LabeledData.xaml 的交互逻辑
    /// </summary>
    public partial class LabeledData : UserControl {
        //标头
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
        //数据
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
        //构造函数
        public LabeledData() {
            InitializeComponent();
        }
    }
}
