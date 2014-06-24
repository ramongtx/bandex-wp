using System;
using System.ComponentModel;
using System.Net;
using System.Windows;

namespace Bandex
{
    internal class WebReader
    {
        public delegate void PageHandler(string page, int index);

        private PageHandler getPage_pageHandler;
        private int getPage_index;

        public void getPage(string url, PageHandler pageHandler, int index)
        {
            getPage_pageHandler = pageHandler;
            getPage_index = index;

            IsLoading = true;

            WebClient webClient = new WebClient();
            webClient.Encoding = System.Text.Encoding.GetEncoding("iso-8859-1");
            System.Uri uri = new System.Uri(url);
            webClient.DownloadStringCompleted += new DownloadStringCompletedEventHandler(getPage_downloadCompletedHandler);
            webClient.DownloadStringAsync(uri);
        }

        private void getPage_downloadCompletedHandler(object sender, DownloadStringCompletedEventArgs e)
        {
            try
            {
                if (e == null || e.Result == null || e.Error != null)
                {
                    MessageBox.Show("Erro baixando cardápio.");
                    getPage_pageHandler("", getPage_index);
                    return;
                }
                getPage_pageHandler(e.Result, getPage_index);
            }
            catch
            {
                MessageBox.Show("Erro baixando cardápio.");
                getPage_pageHandler("", getPage_index);
                return;
            }
        }

        private bool _isLoading = false;

        public bool IsLoading
        {
            get
            {
                return _isLoading;
            }
            set
            {
                _isLoading = value;
                NotifyPropertyChanged("IsLoading");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}