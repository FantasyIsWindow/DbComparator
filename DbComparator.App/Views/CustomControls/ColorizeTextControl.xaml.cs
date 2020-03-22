using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace DbComparator.App.Views.CustomControls
{
    public partial class ColorizeTextControl : UserControl
    {
        public static DependencyProperty MainTextProperty = DependencyProperty.Register
            (
                "MainText",
                typeof(string),
                typeof(ColorizeTextControl),
                new FrameworkPropertyMetadata
                (
                    "",
                    new PropertyChangedCallback(EnterMainText)
                )
            );

        public static DependencyProperty CompareTextProperty = DependencyProperty.Register
            (
                "CompareText",
                typeof(string),
                typeof(ColorizeTextControl),
                new FrameworkPropertyMetadata("")
            );

        public string MainText
        {
            get => (string)GetValue(MainTextProperty);
            set => SetValue(MainTextProperty, value);
        }

        public string CompareText
        {
            get => (string)GetValue(CompareTextProperty);
            set => SetValue(CompareTextProperty, value);
        }

        private static void EnterMainText(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ColorizeTextControl)d).SetTextToRichBox();
        }

        private void SetTextToRichBox()
        {
            rtb.Document.Blocks.Clear();

            if (CompareText == " ")
            {
                TextColorize(MainText, "Black");
            }
            else if (!string.IsNullOrEmpty(MainText) && !string.IsNullOrEmpty(CompareText))
            {
                var mainText = MainText.Split(new char[] { ' ', ',' });

                for (int i = 0; i < mainText.Length; i++)
                {
                    if (!IsWord(mainText[i]))
                    {
                        TextColorize(mainText[i], "Red");
                    }
                    else
                    {
                        TextColorize(mainText[i], "Black");
                    }
                }
            }
        }

        private void TextColorize(string word, string color)
        {
            Paragraph paragraph = rtb.CaretPosition.Paragraph;
            if (paragraph == null)
            {
                paragraph = new Paragraph();
            }
            Run run = new Run(word + " ");
            run.Foreground = new BrushConverter().ConvertFromString(color) as SolidColorBrush;
            paragraph.Inlines.Add(run);
            rtb.Document.Blocks.Add(paragraph);
        }

        private bool IsWord(string word)
        {
            var compareText = CompareText.Split(new char[] { ' ', ',' }).ToList();
            if (compareText.Contains(word))
            {
                compareText.Remove(word);
                return true;
            }
            return false;
        }


        public ColorizeTextControl()
        {
            InitializeComponent();
        }
    }
}
