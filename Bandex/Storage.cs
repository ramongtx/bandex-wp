using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;

namespace Bandex
{
    internal class Storage
    {
        public static void cleanDateList()
        {
            if (!IsolatedStorageSettings.ApplicationSettings.Contains("dateList")) return;

            DateTime today = Convert.ToDateTime(DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd"));

            List<string> dateList = (List<string>)IsolatedStorageSettings.ApplicationSettings["dateList"];
            foreach (string date in dateList)
            {
                if (Convert.ToDateTime(date) < today)
                {
                    if (IsolatedStorageSettings.ApplicationSettings.Contains(date)) IsolatedStorageSettings.ApplicationSettings.Remove(date);
                }
            }
        }

        public static string loadDate(string date)
        {
            if (!IsolatedStorageSettings.ApplicationSettings.Contains(date)) return null;
            return IsolatedStorageSettings.ApplicationSettings[date].ToString();
        }

        public static void removeDate(string date)
        {
            if (IsolatedStorageSettings.ApplicationSettings.Contains(date)) IsolatedStorageSettings.ApplicationSettings.Remove(date);
            if (IsolatedStorageSettings.ApplicationSettings.Contains("dateList"))
            {
                List<string> dateList = (List<string>)IsolatedStorageSettings.ApplicationSettings["dateList"];
                if (dateList.Contains(date))
                {
                    dateList.Remove(date);
                    IsolatedStorageSettings.ApplicationSettings.Remove("dateList");
                    IsolatedStorageSettings.ApplicationSettings.Add("dateList", dateList);
                }
            }
        }

        public static void saveDate(string date, string page)
        {
            if (IsolatedStorageSettings.ApplicationSettings.Contains(date)) IsolatedStorageSettings.ApplicationSettings.Remove(date);
            IsolatedStorageSettings.ApplicationSettings.Add(date, page);
            if (!IsolatedStorageSettings.ApplicationSettings.Contains("dateList"))
            {
                List<string> dateList = new List<string>();
                dateList.Add(date);
                IsolatedStorageSettings.ApplicationSettings.Add("dateList", dateList);
            }
            else
            {
                List<string> dateList = (List<string>)IsolatedStorageSettings.ApplicationSettings["dateList"];
                dateList.Add(date);
                IsolatedStorageSettings.ApplicationSettings.Remove("dateList");
                IsolatedStorageSettings.ApplicationSettings.Add("dateList", dateList);
            }
        }
    }
}