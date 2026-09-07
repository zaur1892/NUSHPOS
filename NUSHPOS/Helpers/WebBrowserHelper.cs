using System.Windows;
using System.Windows.Controls;

namespace NUSHPOS.Helpers
{
    public static class WebBrowserHelper
    {
        public static readonly DependencyProperty HtmlProperty = DependencyProperty.RegisterAttached(
            "Html", typeof(string), typeof(WebBrowserHelper), new FrameworkPropertyMetadata(OnHtmlChanged));

        [AttachedPropertyBrowsableForType(typeof(WebBrowser))]
        public static string GetHtml(WebBrowser d)
        {
            return (string)d.GetValue(HtmlProperty);
        }

        public static void SetHtml(WebBrowser d, string value)
        {
            d.SetValue(HtmlProperty, value);
        }

        static void OnHtmlChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is WebBrowser browser)
            {
                if (browser.DataContext is ViewModels.ReportTabModel tabModel)
                {
                    tabModel.ActiveWebBrowser = browser;
                }

                browser.DataContextChanged -= Browser_DataContextChanged;
                browser.DataContextChanged += Browser_DataContextChanged;

                var html = e.NewValue as string;
                if (!string.IsNullOrWhiteSpace(html))
                {
                    browser.NavigateToString(html);
                }
            }
        }

        private static void Browser_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is WebBrowser browser && browser.DataContext is ViewModels.ReportTabModel tabModel)
            {
                tabModel.ActiveWebBrowser = browser;
            }
        }
    }
}
