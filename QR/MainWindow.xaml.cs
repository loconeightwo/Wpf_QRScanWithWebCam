using System;
using System.Windows;

namespace QR
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.Closing += (s, e) => (this.DataContext as IDisposable).Dispose();
        }
    }
}
