﻿#pragma checksum "..\..\..\graphUtilPage.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "3002D38F78BCD5F22D7AD99ABDCA161E"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.239
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
    /// graphUtilPage
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
    public partial class graphUtilPage : System.Windows.Controls.Page, System.Windows.Markup.IComponentConnector {
        
        
        #line 38 "..\..\..\graphUtilPage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock lblTitle;
        
        #line default
        #line hidden
        
        
        #line 39 "..\..\..\graphUtilPage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnBack;
        
        #line default
        #line hidden
        
        
        #line 40 "..\..\..\graphUtilPage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnSwitch;
        
        #line default
        #line hidden
        
        
        #line 44 "..\..\..\graphUtilPage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Grid grdResults;
        
        #line default
        #line hidden
        
        
        #line 76 "..\..\..\graphUtilPage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock typeHeader;
        
        #line default
        #line hidden
        
        
        #line 87 "..\..\..\graphUtilPage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Grid grdByDate;
        
        #line default
        #line hidden
        
        
        #line 93 "..\..\..\graphUtilPage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.DatePicker dtpDate;
        
        #line default
        #line hidden
        
        
        #line 95 "..\..\..\graphUtilPage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Grid grdByID;
        
        #line default
        #line hidden
        
        
        #line 108 "..\..\..\graphUtilPage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox cboIds;
        
        #line default
        #line hidden
        
        
        #line 109 "..\..\..\graphUtilPage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.DatePicker dtpFirst;
        
        #line default
        #line hidden
        
        
        #line 110 "..\..\..\graphUtilPage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.DatePicker dtpLast;
        
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
            System.Uri resourceLocater = new System.Uri("/Fatigue Calculator Desktop;component/graphutilpage.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\graphUtilPage.xaml"
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
            this.lblTitle = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 2:
            this.btnBack = ((System.Windows.Controls.Button)(target));
            
            #line 39 "..\..\..\graphUtilPage.xaml"
            this.btnBack.Click += new System.Windows.RoutedEventHandler(this.btnBack_Click);
            
            #line default
            #line hidden
            return;
            case 3:
            this.btnSwitch = ((System.Windows.Controls.Button)(target));
            
            #line 40 "..\..\..\graphUtilPage.xaml"
            this.btnSwitch.Click += new System.Windows.RoutedEventHandler(this.btnSwitch_Click);
            
            #line default
            #line hidden
            return;
            case 4:
            this.grdResults = ((System.Windows.Controls.Grid)(target));
            return;
            case 5:
            this.typeHeader = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 6:
            this.grdByDate = ((System.Windows.Controls.Grid)(target));
            return;
            case 7:
            this.dtpDate = ((System.Windows.Controls.DatePicker)(target));
            
            #line 93 "..\..\..\graphUtilPage.xaml"
            this.dtpDate.SelectedDateChanged += new System.EventHandler<System.Windows.Controls.SelectionChangedEventArgs>(this.dtpDate_SelectedDateChanged);
            
            #line default
            #line hidden
            return;
            case 8:
            this.grdByID = ((System.Windows.Controls.Grid)(target));
            return;
            case 9:
            this.cboIds = ((System.Windows.Controls.ComboBox)(target));
            
            #line 108 "..\..\..\graphUtilPage.xaml"
            this.cboIds.SelectionChanged += new System.Windows.Controls.SelectionChangedEventHandler(this.cboIds_SelectionChanged);
            
            #line default
            #line hidden
            return;
            case 10:
            this.dtpFirst = ((System.Windows.Controls.DatePicker)(target));
            
            #line 109 "..\..\..\graphUtilPage.xaml"
            this.dtpFirst.SelectedDateChanged += new System.EventHandler<System.Windows.Controls.SelectionChangedEventArgs>(this.dtp_SelectedDateChanged);
            
            #line default
            #line hidden
            return;
            case 11:
            this.dtpLast = ((System.Windows.Controls.DatePicker)(target));
            
            #line 110 "..\..\..\graphUtilPage.xaml"
            this.dtpLast.SelectedDateChanged += new System.EventHandler<System.Windows.Controls.SelectionChangedEventArgs>(this.dtp_SelectedDateChanged);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

