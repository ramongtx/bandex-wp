using HtmlAgilityPack;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Bandex
{
    public partial class MainPage : PhoneApplicationPage
    {
        private List<int> initializedList = new List<int>();
        private bool[] isLoading = new bool[3];
        private List<LongListSelector> llsList = new List<LongListSelector>();
        private List<PanoramaItem> panoramaItemList = new List<PanoramaItem>();
        private ProgressIndicator progressIndicatorRef;

        public MainPage()
        {
            InitializeComponent();

            initializeLists();

            checkDayOfWeek(DateTime.Now.DayOfWeek);

            Storage.cleanDateList();
        }

        public void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            var progressIndicator = SystemTray.ProgressIndicator;
            if (progressIndicator != null)
            {
                return;
            }

            progressIndicatorRef = new ProgressIndicator();

            SystemTray.SetProgressIndicator(this, progressIndicatorRef);
            progressIndicatorRef.Text = "Carregando...";

            updatePanoramaPage(0);
        }

        private void checkDayOfWeek(DayOfWeek day)
        {
            if (DateTime.Now.DayOfWeek >= DayOfWeek.Saturday)
            {
                initializedList.AddRange(Enumerable.Range(0, 2));
                hojeItem.Header = "Indisponível";

                ObservableCollection<MealModel> cardapios = new ObservableCollection<MealModel>();
                llsList[0].ItemsSource = cardapios;
                cardapios.Add(new MealModel("Final de semana ;D", ""));
            }
            if (DateTime.Now.DayOfWeek >= DayOfWeek.Thursday) depoisItem.Visibility = Visibility.Collapsed;
            if (DateTime.Now.DayOfWeek >= DayOfWeek.Friday) amanhaItem.Visibility = Visibility.Collapsed;
        }

        private void finishedLoading(int index)
        {
            isLoading[index] = false;
            llsList[index].Visibility = Visibility.Visible;
            if (isLoading[0] || isLoading[1] || isLoading[2]) return;
            progressIndicatorRef.IsVisible = false;
            progressIndicatorRef.IsIndeterminate = false;
        }

        private void initializeLists()
        {
            llsList.Add(longList0);
            llsList.Add(longList1);
            llsList.Add(longList2);

            panoramaItemList.Add(hojeItem);
            panoramaItemList.Add(amanhaItem);
            panoramaItemList.Add(depoisItem);

            isLoading[0] = false;
            isLoading[1] = false;
            isLoading[2] = false;
        }

        private void Panorama_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            updatePanoramaPage(mainPanorama.SelectedIndex);
        }

        private void panoramaItem_DoubleTap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            int index = mainPanorama.SelectedIndex;
            if (index < 0) index = 0;
            if (initializedList.Contains(index)) initializedList.Remove(index);
            string date = DateTime.Now.AddDays(index).ToString("yyyy-MM-dd");
            Storage.removeDate(date);
            updatePanoramaPage(index);
        }

        private void readAndSave(string page, int index)
        {
            readToLLS(page, index);
            string date = DateTime.Now.AddDays(index).ToString("yyyy-MM-dd");
            Storage.saveDate(date, page);
            finishedLoading(index);
        }

        private void readToLLS(string page, int index)
        {
            ObservableCollection<MealModel> cardapios = new ObservableCollection<MealModel>();
            llsList[index].ItemsSource = cardapios;

            if (page.Count() == 0)
            {
                cardapios.Add(new MealModel("Indisponível :(", "Toque duplo para tentar novamente."));
                return;
            }

            HtmlDocument htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(page);

            int refeicao = 0;
            foreach (HtmlNode node in htmlDocument.DocumentNode.SelectNodes("//table[@class='fundo_cardapio']"))
            {
                string incoming = node.InnerText.Trim();
                string final = "";
                string[] cleanList = incoming.Split(new Char[] { '\n' });
                for (int i = 0; i < cleanList.Count(); i++)
                {
                    string partial = cleanList[i].Trim();
                    if (partial.Count() > 0)
                    {
                        final += partial + "\n";
                    }
                }
                string nomeRefeicao = "";
                switch (refeicao)
                {
                    case 0:
                        nomeRefeicao = "Almoço";
                        break;

                    case 1:
                        nomeRefeicao = "Almoço Vegetariano";
                        break;

                    default:
                        nomeRefeicao = "Jantar";
                        break;
                }
                cardapios.Add(new MealModel(nomeRefeicao, final));
                refeicao++;
            }
        }

        private void updatePanoramaPage(int index)
        {
            if (initializedList.Contains(index)) return;
            if (isLoading[index]) return;

            initializedList.Add(index);
            string dayName;
            if (index == 0) dayName = "hoje";
            else if (index == 1) dayName = "amanhã";
            else dayName = "depois";
            panoramaItemList[index].Header = dayName += " (" + DateTime.Now.AddDays(index).ToString("dd/MM") + ")";

            string date = DateTime.Now.AddDays(index).ToString("yyyy-MM-dd");
            string page = (Storage.loadDate(date));
            if (page != null) readToLLS(page, index);
            else
            {
                isLoading[index] = true;
                progressIndicatorRef.IsVisible = true;
                progressIndicatorRef.IsIndeterminate = true;
                llsList[index].Visibility = Visibility.Collapsed;

                WebReader cl = new WebReader();
                cl.getPage("http://engenheiros.prefeitura.unicamp.br/cardapio.php?d=" + date, readAndSave, index);
            }
        }
    }
}