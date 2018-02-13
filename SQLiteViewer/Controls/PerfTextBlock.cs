using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SQLiteViewer.Controls
{
    public class PerfTextBlock : Control
    {
        private FormattedText _formattedText;

        static PerfTextBlock()
        {
            //DefaultStyleKeyProperty.OverrideMetadata(typeof(MyTextBlockTest), new FrameworkPropertyMetadata(typeof(MyTextBlockTest)));
        }

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register(
                "Text",
                typeof(string),
                typeof(PerfTextBlock),
                new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.AffectsMeasure,
                    (o, e) => ((PerfTextBlock)o).TextPropertyChanged((string)e.NewValue)));

        private void TextPropertyChanged(string text)
        {
            var typeface = new Typeface(
                new FontFamily("Consolas"),
                FontStyles.Normal, FontWeights.Normal, FontStretches.Normal);

            _formattedText = new FormattedText(
                text, CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight, typeface, 12, Brushes.Black);
        }


        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            if (_formattedText != null)
            {
                drawingContext.DrawText(_formattedText, new Point());
            }
        }

        protected override Size MeasureOverride(Size constraint)
        {
            //return base.MeasureOverride(constraint);

            return _formattedText != null
                ? new Size(_formattedText.Width, _formattedText.Height)
                : new Size();
        }
    }
}