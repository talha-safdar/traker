using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Traker.Behaviours
{
    /// <summary>
    /// Behaviours for text filter.
    /// </summary>
    public enum TextFilterMode
    {
        AlphaNumeric,
        DigitsOnly,
        DigitsAndDotsOnly,
        Currency,               // Strict: 123.45 format
        LettersOnly,
        LettersAndSpacesOnly,
        Time24,
        DateDDMMYYYY
    }

    public static class TextboxFilter
    {
        private static readonly Regex AlphaNumericRegex = new Regex(@"^[A-Za-z0-9_]+$");
        private static readonly Regex DigitsOnlyRegex = new Regex(@"^\d+$");
        private static readonly Regex DigitsAndDotsRegex = new Regex(@"^[0-9.]+$");
        private static readonly Regex LettersOnlyRegex = new Regex(@"^[A-Za-z]+$");
        private static readonly Regex LettersAndSpacesRegex = new Regex(@"^[A-Za-z\s]+$");

        // Strict Currency: Must start with digit, max one dot, max 2 decimals, no trailing dot
        private static readonly Regex CurrencyFinalRegex = new Regex(@"^[0-9]+(\.[0-9]{1,2})?$");
        // Partial Currency: Allows typing (e.g., allows a trailing dot while user is mid-type)
        private static readonly Regex CurrencyPartialRegex = new Regex(@"^[0-9]*\.?[0-9]{0,2}$");

        private static readonly Regex Time24Partial = new Regex(@"^([0-1]?\d|2[0-3])?(:([0-5]?\d)?)?$");
        private static readonly Regex Time24Final = new Regex(@"^(?:[01]\d|2[0-3]):[0-5]\d$");
        private static readonly Regex DatePartial = new Regex(@"^(0?\d|[12]\d|3[01])?(/(0?\d|1[0-2])?)?(/(\d{0,4})?)?$");
        private static readonly Regex DateFinal = new Regex(@"^(0[1-9]|[12]\d|3[01])/(0[1-9]|1[0-2])/\d{4}$");

        #region Attached Properties

        public static bool GetEnableFilter(DependencyObject obj) => (bool)obj.GetValue(EnableFilterProperty);
        public static void SetEnableFilter(DependencyObject obj, bool value) => obj.SetValue(EnableFilterProperty, value);
        public static readonly DependencyProperty EnableFilterProperty = DependencyProperty.RegisterAttached("EnableFilter", typeof(bool), typeof(TextboxFilter), new PropertyMetadata(false, OnChanged));

        public static TextFilterMode GetFilterMode(DependencyObject obj) => (TextFilterMode)obj.GetValue(FilterModeProperty);
        public static void SetFilterMode(DependencyObject obj, TextFilterMode value) => obj.SetValue(FilterModeProperty, value);
        public static readonly DependencyProperty FilterModeProperty = DependencyProperty.RegisterAttached("FilterMode", typeof(TextFilterMode), typeof(TextboxFilter), new PropertyMetadata(TextFilterMode.AlphaNumeric));

        public static int GetMaxLength(DependencyObject obj) => (int)obj.GetValue(MaxLengthProperty);
        public static void SetMaxLength(DependencyObject obj, int value) => obj.SetValue(MaxLengthProperty, value);
        public static readonly DependencyProperty MaxLengthProperty = DependencyProperty.RegisterAttached("MaxLength", typeof(int), typeof(TextboxFilter), new PropertyMetadata(int.MaxValue));

        #endregion

        private static void OnChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TextBox tb)
            {
                if ((bool)e.NewValue)
                {
                    tb.PreviewTextInput += OnPreviewText;
                    tb.PreviewKeyDown += OnPreviewKey;
                    tb.LostFocus += OnLostFocus; // Added to clean up trailing dots
                    DataObject.AddPastingHandler(tb, OnPaste);
                }
                else
                {
                    tb.PreviewTextInput -= OnPreviewText;
                    tb.PreviewKeyDown -= OnPreviewKey;
                    tb.LostFocus -= OnLostFocus;
                    DataObject.RemovePastingHandler(tb, OnPaste);
                }
            }
        }

        private static void OnLostFocus(object sender, RoutedEventArgs e)
        {
            var tb = (TextBox)sender;
            if (GetFilterMode(tb) == TextFilterMode.Currency)
            {
                // If user leaves "123.", remove the trailing dot
                if (tb.Text.EndsWith(".")) tb.Text = tb.Text.TrimEnd('.');
                // If user left it empty or just a dot, handle as needed
            }
        }

        private static void OnPreviewKey(object sender, KeyEventArgs e)
        {
            var mode = GetFilterMode((DependencyObject)sender);
            if (e.Key == Key.Space && mode != TextFilterMode.LettersAndSpacesOnly) e.Handled = true;
        }

        private static void OnPreviewText(object sender, TextCompositionEventArgs e)
        {
            var tb = (TextBox)sender;
            var mode = GetFilterMode(tb);
            string raw = tb.Text.Insert(tb.CaretIndex, e.Text);

            switch (mode)
            {
                case TextFilterMode.AlphaNumeric: e.Handled = !AlphaNumericRegex.IsMatch(e.Text); return;
                case TextFilterMode.DigitsOnly: e.Handled = !DigitsOnlyRegex.IsMatch(e.Text); return;
                case TextFilterMode.DigitsAndDotsOnly: e.Handled = !DigitsAndDotsRegex.IsMatch(e.Text); return;
                case TextFilterMode.LettersOnly: e.Handled = !LettersOnlyRegex.IsMatch(e.Text); return;
                case TextFilterMode.LettersAndSpacesOnly: e.Handled = !LettersAndSpacesRegex.IsMatch(e.Text); return;

                case TextFilterMode.Currency:
                    // Prevent leading dot
                    if (raw.StartsWith(".")) { e.Handled = true; return; }
                    // Allow digits and one dot only
                    if (!CurrencyPartialRegex.IsMatch(raw)) { e.Handled = true; return; }
                    break;

                case TextFilterMode.Time24:
                    if (!DigitsOnlyRegex.IsMatch(e.Text)) { e.Handled = true; return; }
                    if (raw.Length == 2) { tb.Text = raw + ":"; tb.CaretIndex = tb.Text.Length; e.Handled = true; }
                    else if (!Time24Partial.IsMatch(raw)) e.Handled = true;
                    break;

                case TextFilterMode.DateDDMMYYYY:
                    if (!DigitsOnlyRegex.IsMatch(e.Text)) { e.Handled = true; return; }
                    if (raw.Length == 2 || raw.Length == 5) { tb.Text = raw + "/"; tb.CaretIndex = tb.Text.Length; e.Handled = true; }
                    else if (!DatePartial.IsMatch(raw)) e.Handled = true;
                    break;
            }
        }

        private static void OnPaste(object sender, DataObjectPastingEventArgs e)
        {
            var tb = (TextBox)sender;
            var mode = GetFilterMode(tb);
            if (!e.DataObject.GetDataPresent(DataFormats.Text)) { e.CancelCommand(); return; }

            string text = (string)e.DataObject.GetData(DataFormats.Text);
            bool valid = mode switch
            {
                TextFilterMode.AlphaNumeric => AlphaNumericRegex.IsMatch(text),
                TextFilterMode.DigitsOnly => DigitsOnlyRegex.IsMatch(text),
                TextFilterMode.DigitsAndDotsOnly => DigitsAndDotsRegex.IsMatch(text),
                TextFilterMode.LettersOnly => LettersOnlyRegex.IsMatch(text),
                TextFilterMode.LettersAndSpacesOnly => LettersAndSpacesRegex.IsMatch(text),
                TextFilterMode.Currency => CurrencyFinalRegex.IsMatch(text),
                TextFilterMode.Time24 => Time24Final.IsMatch(text),
                TextFilterMode.DateDDMMYYYY => DateFinal.IsMatch(text),
                _ => true
            };

            if (!valid) e.CancelCommand();
        }
    }
}