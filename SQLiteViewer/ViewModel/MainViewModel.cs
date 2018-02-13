using System.Threading.Tasks;
using BeyondCompareSQLitePlugin.Model;
using GalaSoft.MvvmLight;

namespace SQLiteViewer.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            if (IsInDesignMode)
            {
                Content = new[] {"Text Data"};
            }
            else
            {
                Content = new[] { "Just drop sqlite file!" };
            }
        }

        private string[] content;

        public string[] Content
        {
            get { return content;}
            set { base.Set(ref content, value); }
        }
    

        public async Task Drop(string path)
        {
            var lines = await Task.Run<string[]>(() =>
            {
                var databaseContent = DbContext.GetTableContent(path);
                var text = Report.CreateTextReport(databaseContent);
                return text;
            });

            Content = lines;
        }
    }
}