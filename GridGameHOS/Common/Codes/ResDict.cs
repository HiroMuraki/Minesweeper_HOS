using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Common {
    public static class ResDict {
        public static ResourceDictionary PreSetting = new ResourceDictionary() {
            Source = new Uri("/GridGameHOS;component/Styles/PreSetting.xaml", UriKind.Relative)
        };
    }
}
