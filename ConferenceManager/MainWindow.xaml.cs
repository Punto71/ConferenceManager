using ConferenceManager.Service;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;

namespace ConferenceManager {
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {

        private readonly NotifyIcon MyNotifyIcon;

        public MainWindow() {
            InitializeComponent();
            MyNotifyIcon = new NotifyIcon();
            MyNotifyIcon.Icon = System.Drawing.Icon.ExtractAssociatedIcon(System.Windows.Forms.Application.ExecutablePath);
            MyNotifyIcon.MouseDoubleClick += MyNotifyIcon_MouseDoubleClick;
            MessageList.ItemsSource = Logger.MessageList;
            Logger.MessageListChanged += Logger_MessageListChanged;
            Server.start(13500, "*");
        }

        void Logger_MessageListChanged(object sender, EventArgs e) {
        }

        void MyNotifyIcon_MouseDoubleClick(object sender, System.Windows.Forms.MouseEventArgs e) {
            MyNotifyIcon.Visible = false;
            this.ShowInTaskbar = true;
            this.WindowState = WindowState.Normal;
            this.Show();
            this.Activate();
        }

        private void Window_StateChanged(object sender, EventArgs e) {
            if (this.WindowState == System.Windows.WindowState.Maximized)
                this.WindowState = System.Windows.WindowState.Normal;
        }

        private void Window_Closing(object sender, CancelEventArgs e) {
            MyNotifyIcon.Visible = false;
        }

        private void ToTray_Click(object sender, RoutedEventArgs e) {
            this.WindowState = System.Windows.WindowState.Minimized;
            this.ShowInTaskbar = false;
            MyNotifyIcon.Visible = true;
        }

        private void Close_Click(object sender, RoutedEventArgs e) {
            this.Close();
        }

        private void Minimize_Click(object sender, RoutedEventArgs e) {
            this.WindowState = System.Windows.WindowState.Minimized;
        }

        private void TopButton_Click(object sender, RoutedEventArgs e) {
            if (LockIcon.IsVisible) {
                LockIcon.Visibility = System.Windows.Visibility.Hidden;
                UnlockIcon.Visibility = System.Windows.Visibility.Visible;
                this.Topmost = true;
                this.ShowInTaskbar = false;
            } else {
                LockIcon.Visibility = System.Windows.Visibility.Visible;
                UnlockIcon.Visibility = System.Windows.Visibility.Hidden;
                this.Topmost = false;
                this.ShowInTaskbar = true;
            }
        }

        private void Border_MouseLeftButtonDown_1(object sender, MouseButtonEventArgs e) {
            this.DragMove();
            WindowSticking(this);
        }

        public void WindowSticking(Window win) {
            int screenH = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Height;
            int screenW = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Width;
            if (win.Top <= 0 || win.Left <= 0 || win.Top + win.Height >= screenH || win.Left + win.Width >= screenW) {
                if (win.Left + win.Width > screenW)
                    win.Left = screenW - win.Width;
                if (win.Top + win.Height > screenH)
                    win.Top = screenH - win.Height;
                if (win.Left < 0)
                    this.Left = 0;
                if (win.Top < 0)
                    win.Top = 0;

            }
        }
    }
}
