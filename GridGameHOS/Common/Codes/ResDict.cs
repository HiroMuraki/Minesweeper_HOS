using System;
using System.Windows;

namespace GridGameHOS.Common {
    public static class ResDict {
        public static ResourceDictionary PreSetting = new ResourceDictionary() {
            Source = new Uri("/GridGameHOS;component/Styles/PreSetting.xaml", UriKind.Relative)
        };
    }
}
