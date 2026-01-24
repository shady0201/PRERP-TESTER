using Microsoft.Web.WebView2.Wpf;
using PRERP_TESTER.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRERP_TESTER.Services
{
    public sealed class TabRuntime
    {
        public TabWebItem State { get; }
        public WebView2? WebView { get; private set; }

        public bool IsWebViewCreated => WebView is not null;

        public TabRuntime(TabWebItem state) => State = state;

        public void AttachWebView(WebView2 wv) => WebView = wv;

        public void DisposeWebView()
        {
            try
            {
                if (WebView is not null)
                {
                    WebView.CoreWebView2?.Stop();
                    WebView.Dispose();
                }
            }
            catch { /* ignore */ }
            finally
            {
                WebView = null;
            }
        }
    }
}
