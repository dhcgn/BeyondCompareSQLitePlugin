using System;
using System.Collections.Generic;
using System.Linq;
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
using BeyondCompareSQLitePlugin.Model;
using SQLiteViewer.ViewModel;


namespace SQLiteViewer
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void UIElement_OnDrop(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(DataFormats.FileDrop))
                return;
            
            var dataString = (string[])e.Data.GetData(DataFormats.FileDrop);

            if (dataString == null || !dataString.Any())
                return;

            await new ViewModelLocator().Main.Drop(dataString[0]);
        }
    }
}
