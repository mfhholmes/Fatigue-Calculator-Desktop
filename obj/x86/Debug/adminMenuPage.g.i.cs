﻿#pragma checksum "..\..\..\adminMenuPage.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "6207FCC79B880C3DA8AC8DCA004ED208"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.269
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

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


namespace Fatigue_Calculator_Desktop {
    
    
    /// <summary>
    /// adminMenuPage
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
    public partial class adminMenuPage : System.Windows.Controls.Page, System.Windows.Markup.IComponentConnector {
        
        
        #line 40 "..\..\..\adminMenuPage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Border bdrSettings;
        
        #line default
        #line hidden
        
        
        #line 75 "..\..\..\adminMenuPage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock GraphLabel;
        
        #line default
        #line hidden
        
        
        #line 84 "..\..\..\adminMenuPage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnUserList;
        
        #line default
        #line hidden
        
        
        #line 85 "..\..\..\adminMenuPage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnGraph;
        
        #line default
        #line hidden
        
        
        #line 86 "..\..\..\adminMenuPage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnSettings;
        
        #line default
        #line hidden
        
        
        #line 87 "..\..\..\adminMenuPage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnOptions;
        
        #line default
        #line hidden
        
        
        #line 90 "..\..\..\adminMenuPage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnExit;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/Fatigue Calculator Desktop;component/adminmenupage.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\adminMenuPage.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.bdrSettings = ((System.Windows.Controls.Border)(target));
            return;
            case 2:
            this.GraphLabel = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 3:
            this.btnUserList = ((System.Windows.Controls.Button)(target));
            
            #line 84 "..\..\..\adminMenuPage.xaml"
            this.btnUserList.Click += new System.Windows.RoutedEventHandler(this.btnUserList_Click);
            
            #line default
            #line hidden
            return;
            case 4:
            this.btnGraph = ((System.Windows.Controls.Button)(target));
            
            #line 85 "..\..\..\adminMenuPage.xaml"
            this.btnGraph.Click += new System.Windows.RoutedEventHandler(this.btnGraph_Click);
            
            #line default
            #line hidden
            return;
            case 5:
            this.btnSettings = ((System.Windows.Controls.Button)(target));
            
            #line 86 "..\..\..\adminMenuPage.xaml"
            this.btnSettings.Click += new System.Windows.RoutedEventHandler(this.btnSettings_Click);
            
            #line default
            #line hidden
            return;
            case 6:
            this.btnOptions = ((System.Windows.Controls.Button)(target));
            
            #line 87 "..\..\..\adminMenuPage.xaml"
            this.btnOptions.Click += new System.Windows.RoutedEventHandler(this.btnOptions_Click);
            
            #line default
            #line hidden
            return;
            case 7:
            this.btnExit = ((System.Windows.Controls.Button)(target));
            
            #line 90 "..\..\..\adminMenuPage.xaml"
            this.btnExit.Click += new System.Windows.RoutedEventHandler(this.btnExit_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

