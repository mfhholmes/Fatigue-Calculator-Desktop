﻿#pragma checksum "..\..\graphPage.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "3F039FF7666C3722BED59D973744078C"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.1
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
    /// graphPage
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
    public partial class graphPage : System.Windows.Controls.Page, System.Windows.Markup.IComponentConnector {
        
        
        #line 41 "..\..\graphPage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Canvas context;
        
        #line default
        #line hidden
        
        
        #line 42 "..\..\graphPage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnBack;
        
        #line default
        #line hidden
        
        
        #line 43 "..\..\graphPage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnRecommend;
        
        #line default
        #line hidden
        
        
        #line 44 "..\..\graphPage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnGraph24;
        
        #line default
        #line hidden
        
        
        #line 45 "..\..\graphPage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnGraph48;
        
        #line default
        #line hidden
        
        
        #line 46 "..\..\graphPage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnGraph72;
        
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
            System.Uri resourceLocater = new System.Uri("/Fatigue Calculator Desktop;component/graphpage.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\graphPage.xaml"
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
            
            #line 9 "..\..\graphPage.xaml"
            ((Fatigue_Calculator_Desktop.graphPage)(target)).Loaded += new System.Windows.RoutedEventHandler(this.Page_Loaded);
            
            #line default
            #line hidden
            return;
            case 2:
            this.context = ((System.Windows.Controls.Canvas)(target));
            return;
            case 3:
            this.btnBack = ((System.Windows.Controls.Button)(target));
            
            #line 42 "..\..\graphPage.xaml"
            this.btnBack.Click += new System.Windows.RoutedEventHandler(this.btnBack_Click);
            
            #line default
            #line hidden
            return;
            case 4:
            this.btnRecommend = ((System.Windows.Controls.Button)(target));
            
            #line 43 "..\..\graphPage.xaml"
            this.btnRecommend.Click += new System.Windows.RoutedEventHandler(this.btnRecommend_Click);
            
            #line default
            #line hidden
            return;
            case 5:
            this.btnGraph24 = ((System.Windows.Controls.Button)(target));
            
            #line 44 "..\..\graphPage.xaml"
            this.btnGraph24.Click += new System.Windows.RoutedEventHandler(this.btnGraph24_Click);
            
            #line default
            #line hidden
            return;
            case 6:
            this.btnGraph48 = ((System.Windows.Controls.Button)(target));
            
            #line 45 "..\..\graphPage.xaml"
            this.btnGraph48.Click += new System.Windows.RoutedEventHandler(this.btnGraph48_Click);
            
            #line default
            #line hidden
            return;
            case 7:
            this.btnGraph72 = ((System.Windows.Controls.Button)(target));
            
            #line 46 "..\..\graphPage.xaml"
            this.btnGraph72.Click += new System.Windows.RoutedEventHandler(this.btnGraph72_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

