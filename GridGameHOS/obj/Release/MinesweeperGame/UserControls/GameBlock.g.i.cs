﻿#pragma checksum "..\..\..\..\MinesweeperGame\UserControls\GameBlock.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "9897803E3A37F2D15C676F5699A7D612E3CA2DF42DC0E578CCAC9C6BEEA9F602"
//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:4.0.30319.42000
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------

using MinesweepGameLite;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;


namespace MinesweepGameLite {
    
    
    /// <summary>
    /// GameBlock
    /// </summary>
    public partial class GameBlock : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector {
        
        
        #line 8 "..\..\..\..\MinesweeperGame\UserControls\GameBlock.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal MinesweepGameLite.GameBlock UserControl;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/Common;component/minesweepergame/usercontrols/gameblock.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\MinesweeperGame\UserControls\GameBlock.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.UserControl = ((MinesweepGameLite.GameBlock)(target));
            return;
            case 2:
            
            #line 109 "..\..\..\..\MinesweeperGame\UserControls\GameBlock.xaml"
            ((System.Windows.Controls.Label)(target)).MouseDoubleClick += new System.Windows.Input.MouseButtonEventHandler(this.OnDoubleOpenBlock);
            
            #line default
            #line hidden
            return;
            case 3:
            
            #line 115 "..\..\..\..\MinesweeperGame\UserControls\GameBlock.xaml"
            ((System.Windows.Controls.Label)(target)).MouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(this.OnOpenBlock);
            
            #line default
            #line hidden
            
            #line 116 "..\..\..\..\MinesweeperGame\UserControls\GameBlock.xaml"
            ((System.Windows.Controls.Label)(target)).MouseRightButtonDown += new System.Windows.Input.MouseButtonEventHandler(this.OnFlagBlock);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}
