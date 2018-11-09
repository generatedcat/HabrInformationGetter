using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Net;
using System.Xml.Linq;
using System.Xml;

namespace HabrInformationGetter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Channel channel = new Channel();
        List<Item> itemCollectionToDisplay = new List<Item>();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void GetHabrInfoButton_Click(object sender, RoutedEventArgs e)
        {
           
            WebClient client = new WebClient();
            client.Encoding = System.Text.Encoding.UTF8;
            string xmlStr = client.DownloadString("https://habrahabr.ru/rss/interesting/");
            XElement str = XElement.Parse(xmlStr);

            IEnumerable<XElement> items = str.Elements("channel").FirstOrDefault().Elements("item");
            itemCollectionToDisplay = (from itm in items
                                       //where itm.Element("title") != null
                                       select new Item()
                        {
                            Title = itm.Element("title").Value,
                            Link = itm.Element("link").Value,
                            Description = itm.Element("description").Value,
                            PubDate = Convert.ToDateTime(itm.Element("pubDate").Value),
                        }).ToList<Item>();
            displayHabrItem(0, itemCollectionToDisplay.Count());

            
        }

        private void SaveFile_Click(object sender, RoutedEventArgs e)
        {
            string path = PathBox.Text;
            if (!Directory.Exists(path))
            {
                MessageBox.Show("Укажите существующий путь");
                return;
            }
            else
            {
                if (itemCollectionToDisplay.Count == 0)
                {
                    MessageBox.Show("Сначала получите информацию");
                    return;
                }
                SaveFile.IsEnabled = false;
                FileStream fs = new FileStream(path+@"\test.xml", FileMode.OpenOrCreate);
                System.Xml.Serialization.XmlSerializer s = new System.Xml.Serialization.XmlSerializer(typeof(Channel));
                s.Serialize(fs, channel);
                MessageBox.Show("Файл успешно создан");
            }
        }

        private void displayHabrItem(int index, int count)
        {
            currentPage.Content = (index+1).ToString();
            totalPages.Content = count.ToString();

            Item displayedItem = itemCollectionToDisplay[index];
            labelTitle.Content = displayedItem.Title;
            labeldescription.Content = displayedItem.Description;
            labelManagingEditor.Content = "Сюда непонятно что заносить";
            labelGenerator.Content = displayedItem.Link;
            labelPubDate.Content = displayedItem.PubDate.ToShortDateString();
        }

        private void previous_Click(object sender, RoutedEventArgs e)
        {
            if (itemCollectionToDisplay.Count == 0)
            {
                MessageBox.Show("Сначала получите информацию");
                return;
            }
            int currentIndex = Convert.ToInt32(currentPage.Content);
            if (currentIndex <= 1)
            {
                MessageBox.Show("Нет предыдущих страниц");
                return;
            }
            currentIndex--;
            currentPage.Content = currentIndex;
            displayHabrItem(currentIndex-1, itemCollectionToDisplay.Count());

        }

        private void next_Click(object sender, RoutedEventArgs e)
        {
            if (itemCollectionToDisplay.Count == 0)
            {
                MessageBox.Show("Сначала получите информацию");
                return;
            }
            int currentIndex = Convert.ToInt32(currentPage.Content);
            int totalPagesDisplayed = Convert.ToInt32(totalPages.Content);
            if (currentIndex+1 > totalPagesDisplayed)
            {
                MessageBox.Show("Нет следующих страниц");
                return;
            }
            currentIndex++;
            currentPage.Content = currentIndex;
            displayHabrItem(currentIndex - 1, itemCollectionToDisplay.Count());
        }
    }
}
