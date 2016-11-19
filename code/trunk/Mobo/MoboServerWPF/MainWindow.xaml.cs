/**
* Author: Christopher Cola
* Created on 07/03/2016
*/

using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace MoboServerWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            textBox.Clear();
            new Program(this);
        }

        public void RefreshListBox()
        {
            ObservableCollection<string> players = new ObservableCollection<string>();

            foreach(Player player in Player.players.Values)
            {
                players.Add(string.Format("{0} ({1}) HP: {2} HOST: {3} SCORE: {4}", player.name, player.uid, player.health, player.host, player.score));
            }

            Dispatcher.Invoke(() =>
            {
                listBox.ItemsSource = players;
            });
        }

        public void Append(string text)
        {
            Dispatcher.Invoke(() =>
            {
                textBox.AppendText(text + "\n");
                textBox.Focus();
                textBox.CaretIndex = textBox.Text.Length;
                textBox.ScrollToEnd();
            });
        }
    }
}
