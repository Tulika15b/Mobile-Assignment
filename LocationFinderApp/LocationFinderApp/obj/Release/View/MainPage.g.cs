﻿#pragma checksum "C:\Users\gur32587\Downloads\Mobile-Assignment-master (1)\Mobile-Assignment-master\LocationFinderApp\LocationFinderApp\View\MainPage.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "9AA8D8B8C0F552B23F8B3A54BC175AF8"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.34014
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Microsoft.Phone.Controls;
using System;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Resources;
using System.Windows.Shapes;
using System.Windows.Threading;


namespace LocationFinderApp {
    
    
    public partial class MainPage : Microsoft.Phone.Controls.PhoneApplicationPage {
        
        internal System.Windows.Controls.Grid LayoutRoot;
        
        internal System.Windows.Controls.StackPanel TitlePanel;
        
        internal System.Windows.Controls.Grid ContentPanel;
        
        internal System.Windows.Controls.TextBox LocationUserName;
        
        internal System.Windows.Controls.TextBlock Lat;
        
        internal System.Windows.Controls.TextBlock Long;
        
        internal System.Windows.Controls.Button Submit_Button;
        
        internal System.Windows.Controls.TextBlock lastSubmitted_txt_blk;
        
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
            System.Windows.Application.LoadComponent(this, new System.Uri("/LocationFinderApp;component/View/MainPage.xaml", System.UriKind.Relative));
            this.LayoutRoot = ((System.Windows.Controls.Grid)(this.FindName("LayoutRoot")));
            this.TitlePanel = ((System.Windows.Controls.StackPanel)(this.FindName("TitlePanel")));
            this.ContentPanel = ((System.Windows.Controls.Grid)(this.FindName("ContentPanel")));
            this.LocationUserName = ((System.Windows.Controls.TextBox)(this.FindName("LocationUserName")));
            this.Lat = ((System.Windows.Controls.TextBlock)(this.FindName("Lat")));
            this.Long = ((System.Windows.Controls.TextBlock)(this.FindName("Long")));
            this.Submit_Button = ((System.Windows.Controls.Button)(this.FindName("Submit_Button")));
            this.lastSubmitted_txt_blk = ((System.Windows.Controls.TextBlock)(this.FindName("lastSubmitted_txt_blk")));
        }
    }
}

