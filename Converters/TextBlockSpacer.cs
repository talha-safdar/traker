using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace Traker.Converters
{
    public static class TextBlockSpacer
    {
        public static readonly DependencyProperty LetterSpacingProperty =
        DependencyProperty.RegisterAttached(
            "LetterSpacing",
            typeof(double),
            typeof(TextBlockSpacer),
            new PropertyMetadata(0.0, OnLetterSpacingChanged));

        public static double GetLetterSpacing(DependencyObject obj)
            => (double)obj.GetValue(LetterSpacingProperty);

        public static void SetLetterSpacing(DependencyObject obj, double value)
            => obj.SetValue(LetterSpacingProperty, value);

        private static void OnLetterSpacingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TextBlock tb)
            {
                var text = tb.Text;
                tb.Inlines.Clear();

                double spacing = (double)e.NewValue;

                foreach (char c in text)
                {
                    var innerText = new TextBlock
                    {
                        Text = c.ToString(),
                        Margin = new Thickness(0, 0, spacing, 0)
                    };

                    tb.Inlines.Add(new InlineUIContainer(innerText));
                }
            }
        }
    }
}
