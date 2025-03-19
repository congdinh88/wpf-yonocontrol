using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
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

namespace YonoControl.ControlApp
{
    /// <summary>
    /// Interaction logic for Titlebar.xaml
    /// </summary>
    public partial class Titlebar : UserControl
    {
        public Titlebar()
        {
            InitializeComponent();
            this.MouseDown += TitleBar_MouseDown;
            Loaded += CustomTitleBar_Loaded;
        }

        private bool isSystemMenuOpen = false;
        private void TitleBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Window window = Window.GetWindow(this);
            if (e.ClickCount == 2)
            {
                MaximizeRestoreButton_Click(null, null);
            }

            if (isSystemMenuOpen)
            {
                isSystemMenuOpen = false;
                return;
            }

            if (window != null && e.ChangedButton == MouseButton.Left && !IsMouseOverControlButton(e))
            {
                window.DragMove();
            }
        }
        private bool IsMouseOverControlButton(MouseButtonEventArgs e)
        {
            // Kiểm tra xem chuột có đang trên các nút điều khiển không
            return MinimizeButton.IsMouseOver || MaximizeRestoreButton.IsMouseOver || CloseButton.IsMouseOver;
        }
        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            Window window = Window.GetWindow(this);
            if (window != null)
            {
                window.WindowState = WindowState.Minimized;
            }
        }
        private void MaximizeRestoreButton_Click(object sender, RoutedEventArgs e)
        {
            Window window = Window.GetWindow(this);
            if (window != null)
            {
                if (window.WindowState == WindowState.Maximized)
                {
                    window.WindowState = WindowState.Normal;
                    MaximizeRestoreButton.Content = "\uE922";
                }
                else
                {
                    //Vừa màn hình hiện tại của máy tính không bị che Task bar của window
                    window.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;
                    window.MaxWidth = SystemParameters.MaximizedPrimaryScreenWidth;

                    window.WindowState = WindowState.Maximized;
                    MaximizeRestoreButton.Content = "\uE923";
                }
            }
        }
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Window window = Window.GetWindow(this);
            if (window != null)
            {
                window.Close();
            }
        }

        // Begin icon click
        [DllImport("user32.dll")]
        private static extern bool GetCursorPos(out POINT lpPoint);

        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);

        private const int WM_SYSCOMMAND = 0x112;
        private const int SC_MOUSEMENU = 0xF090;

        [StructLayout(LayoutKind.Sequential)]
        private struct POINT
        {
            public int X;
            public int Y;
        }
        private void Icon_Click(object sender, MouseButtonEventArgs e)
        {
            isSystemMenuOpen = true;
            Window window = Window.GetWindow(this);
            if (GetCursorPos(out POINT point))
            {
                IntPtr lParam = new IntPtr((point.Y << 16) | (point.X & 0xFFFF));
                SendMessage(new System.Windows.Interop.WindowInteropHelper(Application.Current.MainWindow).Handle, WM_SYSCOMMAND, new IntPtr(SC_MOUSEMENU), lParam);
            }


        }

        private void CustomTitleBar_Loaded(object sender, RoutedEventArgs e)
        {
            Window window = Window.GetWindow(this); // Lấy window chứa TitleBar
            if (window != null)
            {
                window.StateChanged += Window_StateChanged;
                UpdateRestoreButton(); // Cập nhật icon khi khởi động
            }
        }

        private void Window_StateChanged(object sender, EventArgs e)
        {
            UpdateRestoreButton();
        }

        private void UpdateRestoreButton()
        {
            Window window = Window.GetWindow(this);
            if (window == null) return;

            if (window.WindowState == WindowState.Maximized)
                MaximizeRestoreButton.Content = "\uE923";
            else
                MaximizeRestoreButton.Content = "\uE922";
        }
        // End icon click

    }
}
