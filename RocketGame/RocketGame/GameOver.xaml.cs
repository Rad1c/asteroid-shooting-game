using System;
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
using System.Windows.Shapes;

namespace RocketGame
{
    public partial class GameOver : Window
    {
        int hiScore = Properties.Settings.Default.hiScore;

        public GameOver()
        {
            InitializeComponent();
        }

        public GameOver(int score)
        {
            InitializeComponent();
            lbl1.Content = "Score: ";
            lblValue1.Content = score.ToString();
            lbl2.Content = "Highscore: ";
            lblValue2.Content = hiScore.ToString();
            lbl3.Visibility = System.Windows.Visibility.Collapsed;
            lblValue3.Visibility = System.Windows.Visibility.Collapsed;
        }

        public GameOver(int player1, int player2)
        {
            InitializeComponent();
            lbl1.Content = "Player 1: ";
            lblValue1.Content = player1.ToString();
            lbl2.Content = "Player 2: ";
            lblValue2.Content = player2.ToString();
            lbl3.Content = "Highscore: ";
            lblValue3.Content = hiScore.ToString();
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void btnRetry_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void label1_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
    }
}
