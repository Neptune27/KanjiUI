﻿using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

using Microsoft.UI;           // Needed for WindowId
using Microsoft.UI.Windowing; // Needed for AppWindow
using WinRT.Interop;          // Needed for XAML/HWND interop

using KanjiUI.Views;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace KanjiUI
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Shell : Window
    {
        private AppWindow m_AppWindow;

        public Shell()
        {
            Title = "Kanji UI";
            this.InitializeComponent();


            // Check to see if customization is supported.
            // Currently only supported on Windows 11.
            m_AppWindow = GetAppWindowForCurrentWindow();
            if (AppWindowTitleBar.IsCustomizationSupported())
            {
                var titleBar = m_AppWindow.TitleBar;
                // Hide default title bar.
                titleBar.ExtendsContentIntoTitleBar = true;
            }
            else
            {
                // Title bar customization using these APIs is currently
                // supported only on Windows 11. In other cases, hide
                // the custom title bar element.
                AppTitleBar.Visibility = Visibility.Collapsed;
            }
        }

        private AppWindow GetAppWindowForCurrentWindow()
        {
            IntPtr hWnd = WindowNative.GetWindowHandle(this);
            WindowId wndId = Win32Interop.GetWindowIdFromWindow(hWnd);
            return AppWindow.GetFromWindowId(wndId);
        }

        private void NavigationView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            SetContentFrame(args.SelectedItemContainer as NavigationViewItem);

            //Debug.WriteLine(Type.GetType(item.Tag.ToString()));
        }


        private void SetContentFrame(NavigationViewItem item)
        {
            if (item == null)
            {
                return;
            }

            if (item.Tag == null)
            {
                return;
            }


            if (item.Tag.ToString() == "Settings")
            {
                ContentFrame.Navigate(typeof(Settings));
            }
            else
            {
                ContentFrame.Navigate(Type.GetType(item.Tag.ToString()), item.Content);
            }

            Debug.WriteLine($"Set Content to: {item.Tag}");
            Debug.WriteLine($"NavView: {NavView.SelectedItem}");

        }


        private void NavigationView_Loaded(object sender, RoutedEventArgs e)
        {
            SetCurrentNavigationViewItem(GetNavigationViewItems(typeof(HomePage)).First());
            var settings = NavView.SettingsItem as NavigationViewItem;
            settings.Tag = "Settings";
        }

        public void SetCurrentNavigationViewItem(NavigationViewItem item)
        {
            if (item == null)
            {
                return;
            }

            if (item.Tag == null)
            {
                return;
            }
            //var tmp = Type.GetType(item.Tag.ToString());
            //var tmp2 = item.Content;

            ContentFrame.Navigate(Type.GetType(item.Tag.ToString()), item.Content);
            NavView.SelectedItem = item;
        }


        public List<NavigationViewItem> GetNavigationViewItems(Type type)
        {
            return GetNavigationViewItems().Where(i => i.Tag.ToString() == type.FullName).ToList();
        }

        public List<NavigationViewItem> GetNavigationViewItems()
        {
            List<NavigationViewItem> result = new();
            var items = NavView.MenuItems.Select(i => (NavigationViewItem)i).ToList();
            items.AddRange(NavView.FooterMenuItems.Select(i => (NavigationViewItem)i));
            result.AddRange(items);

            foreach (NavigationViewItem mainItem in items)
            {
                result.AddRange(mainItem.MenuItems.Select(i => (NavigationViewItem)i));
            }

            return result;
        }
    }
}
