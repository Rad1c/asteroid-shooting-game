using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO.Packaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Media;

using System.Windows.Threading;

namespace RocketGame
{
    public partial class MainWindow : Window
    {
        DispatcherTimer gameTimer = new DispatcherTimer();
        List<Rectangle> itemRemover = new List<Rectangle>();
        SoundPlayer meteorHitSound = new SoundPlayer("sounds/hit.wav");
        SoundPlayer laserSound = new SoundPlayer("sounds/laser_shoot.wav");
        Random random = new Random();
        bool leftPlayer1, rightPlayer1, leftPlayer2, rightPlayer2;
        bool twoPlayers = false;
        bool gameStart = false;
        int oldScore = 0;
        bool timerState = true;
        int meteorCounter = 50;
        int meteorSpriteCounter = 0;
        int limit = 10;
        int scorePlayer1 = 0;
        int scorePlayer2 = 0;
        int healthPlayer1 = 3;
        int healthPlayer2 = 3;
        int meteorSpeed = 8;
        int playerSpeed = 10;

        Rect playerHitBox;
        Rect playerHitBox1;

        public MainWindow()
        {
            InitializeComponent();
            setMainCanvasBackground();
            this.lblHiScore.Foreground = Brushes.White;
            setPlayer2();
            this.lblHiScore.Content = Properties.Settings.Default.hiScore.ToString();

            gameTimer.Interval = TimeSpan.FromMilliseconds(20);
            gameTimer.Tick += game;
           
            mainCanvas.Focus();

            ImageBrush playerImage = new ImageBrush();
            playerImage.ImageSource = new BitmapImage(new Uri("Resources/rocket.png", UriKind.Relative));

            rctgPlayer1.Fill = playerImage;
        }

        private void startGame()
        {
            lblStartGame.Visibility = System.Windows.Visibility.Collapsed;
            label1.Visibility = System.Windows.Visibility.Collapsed;
            label2.Visibility = System.Windows.Visibility.Collapsed;
            label3.Visibility = System.Windows.Visibility.Collapsed;
            label4.Visibility = System.Windows.Visibility.Collapsed;
            gameTimer.Start();
            gameStart = true;
        }

        private void setMainCanvasBackground()
        {
            ImageBrush bg = new ImageBrush();
            bg.TileMode = TileMode.Tile;
            bg.Viewport = new Rect(0, 0, 0.15, 0.15);
            bg.ViewportUnits = BrushMappingMode.RelativeToBoundingBox;
            int rand = random.Next(1, 3);
            string bgPath = "Resources/Space_Stars" + rand + ".png";
            bg.ImageSource = new BitmapImage(new Uri(bgPath, UriKind.Relative));
            mainCanvas.Background = bg;
        }

        private void setPlayer2()
        {
            this.lblScore1.Visibility = Visibility.Hidden;
            this.player2Hart1.Visibility = Visibility.Hidden;
            this.player2Hart2.Visibility = Visibility.Hidden;
            this.player2Hart3.Visibility = Visibility.Hidden;

            ImageBrush playerImage = new ImageBrush();
            playerImage.ImageSource = new BitmapImage(new Uri("Resources/rocket.png", UriKind.Relative));
            rctgPlayer2.Fill = playerImage;
            rctgPlayer2.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void showPlayer2()
        {
            this.lblScore1.Visibility = Visibility.Visible;
            this.player2Hart1.Visibility = Visibility.Visible;
            this.player2Hart2.Visibility = Visibility.Visible;
            this.player2Hart3.Visibility = Visibility.Visible;

            rctgPlayer2.Visibility = Visibility.Visible;
        }

        private void mainCanvas_KeyDown(object sender, KeyEventArgs e)
        {
            ImageBrush playerLaser = new ImageBrush();
            playerLaser.ImageSource = new BitmapImage(new Uri("Resources/player_laser.png", UriKind.Relative));

            if (e.Key == Key.Left)
            {
                this.leftPlayer1 = true;
            }

            if (e.Key == Key.A)
            {
                this.leftPlayer2 = true;
            }

            if (e.Key == Key.D)
            {
                this.rightPlayer2 = true;
            }

            if (e.Key == Key.Right)
            {
                this.rightPlayer1 = true;
            }

            if (e.Key == Key.Space && healthPlayer1 > 0 && timerState)
            {
                if (!gameStart)
                    startGame();

                Rectangle newBullet = new Rectangle
                {
                    Tag = "bulletPlayer1",
                    Height = 15,
                    Width = 4,
                };

                Canvas.SetLeft(newBullet, Canvas.GetLeft(rctgPlayer1) + rctgPlayer1.Width / 2 - 2);
                Canvas.SetTop(newBullet, Canvas.GetTop(rctgPlayer1) - newBullet.Height);
                newBullet.Fill = playerLaser;
                mainCanvas.Children.Add(newBullet);
                laserSound.Play();
            }

            if (e.Key == Key.S && healthPlayer2 > 0 && gameStart && timerState)
            {
                if (!twoPlayers)
                {
                    twoPlayers = true;
                    showPlayer2();
                }

                Rectangle newBullet = new Rectangle
                {
                    Tag = "bulletPlayer2",
                    Height = 15,
                    Width = 4,
                };

                Canvas.SetLeft(newBullet, Canvas.GetLeft(rctgPlayer2) + rctgPlayer1.Width / 2 - 2);
                Canvas.SetTop(newBullet, Canvas.GetTop(rctgPlayer2) - newBullet.Height);
                newBullet.Fill = playerLaser;
                mainCanvas.Children.Add(newBullet);
                laserSound.Play();
            }
        }

        private void mainCanvas_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Left)
            {
                this.leftPlayer1 = false;
            }

            if (e.Key == Key.Right)
            {
                this.rightPlayer1 = false;
            }

            if (e.Key == Key.A)
            {
                this.leftPlayer2 = false;
            }

            if (e.Key == Key.D)
            {
                this.rightPlayer2 = false;
            }

            if (e.Key == Key.Escape)
            {
                timerState = (timerState == true) ? false : true;

                if (!timerState)
                    gameTimer.Stop();
                else
                    gameTimer.Start();
            }
        }

        private void loadMeteors()
        {
            ImageBrush meteorBrush = new ImageBrush();
            meteorSpriteCounter = random.Next(1, 5);

            string path = "Resources/rock" + meteorSpriteCounter + ".png";
            BitmapImage bmp = new BitmapImage(new Uri(path, UriKind.Relative));
            meteorBrush.ImageSource = bmp;

            Rectangle newMeteor = new Rectangle();
            newMeteor.Tag = "meteor";
            newMeteor.Height = (int)bmp.Height + (int)(0.30f * bmp.Height);
            newMeteor.Width = (int)bmp.Width + (int)(0.30f * bmp.Width);
            newMeteor.Fill = meteorBrush;

            Canvas.SetTop(newMeteor, -(int)bmp.Height);
            Canvas.SetLeft(newMeteor, random.Next(0, (int)mainCanvas.ActualWidth - (int)bmp.Width));
            mainCanvas.Children.Add(newMeteor);
        }

        private void game(object sender, EventArgs e)
        {
            playerHitBox = new Rect(Canvas.GetLeft(rctgPlayer1), Canvas.GetTop(rctgPlayer1), rctgPlayer1.Width, rctgPlayer1.Height);
            playerHitBox1 = new Rect(Canvas.GetLeft(rctgPlayer2), Canvas.GetTop(rctgPlayer2), rctgPlayer2.Width, rctgPlayer2.Height);
            meteorCounter--;

            lblScore.Content = scorePlayer1.ToString();
            lblScore1.Content = scorePlayer2.ToString();
            this.lblHiScore.Content = Properties.Settings.Default.hiScore.ToString();

            if (meteorCounter < 0)
            {
                loadMeteors();
                meteorCounter = limit;
            }

            if (Math.Max(scorePlayer1, scorePlayer2) % 20 == 0 && Math.Max(scorePlayer1, scorePlayer2) != oldScore)
            {
                oldScore = Math.Max(scorePlayer1, scorePlayer2);

                if(limit > 1)
                    limit--;
                meteorSpeed++;
            }

            if (leftPlayer1 == true && Canvas.GetLeft(rctgPlayer1) > 5)
            {
                Canvas.SetLeft(rctgPlayer1, Canvas.GetLeft(rctgPlayer1) - playerSpeed);
            }

            if (rightPlayer1 == true && Canvas.GetLeft(rctgPlayer1) + 55 < Application.Current.MainWindow.Width)
            {
                Canvas.SetLeft(rctgPlayer1, Canvas.GetLeft(rctgPlayer1) + playerSpeed);
            }

            if (leftPlayer2 == true && Canvas.GetLeft(rctgPlayer2) > 5 && twoPlayers)
            {
                Canvas.SetLeft(rctgPlayer2, Canvas.GetLeft(rctgPlayer2) - playerSpeed);
            }

            if (rightPlayer2 == true && Canvas.GetLeft(rctgPlayer2) + 55 < Application.Current.MainWindow.Width && twoPlayers)
            {
                Canvas.SetLeft(rctgPlayer2, Canvas.GetLeft(rctgPlayer2) + playerSpeed);
            }

            foreach (var item in mainCanvas.Children.OfType<Rectangle>())
            {
                if (item is Rectangle && ((string)item.Tag == "bulletPlayer1" || (string)item.Tag == "bulletPlayer2"))
                {
                    Canvas.SetTop(item, Canvas.GetTop(item) - 15);

                    Rect bulletHitBox = new Rect(Canvas.GetLeft(item), Canvas.GetTop(item), item.Width, item.Height);

                    if (Canvas.GetTop(item) < 10)
                    {
                        itemRemover.Add(item);
                    }

                    foreach (var item1 in mainCanvas.Children.OfType<Rectangle>())
                    {
                        if (item1 is Rectangle && (string)item1.Tag == "meteor")
                        {
                            Rect meteorHit = new Rect(Canvas.GetLeft(item1), Canvas.GetTop(item1), item1.Width, item1.Height);

                            if (bulletHitBox.IntersectsWith(meteorHit))
                            {
                                itemRemover.Add(item);
                                itemRemover.Add(item1);

                                if ((string)item.Tag == "bulletPlayer1")
                                    scorePlayer1++;
                                else
                                    scorePlayer2++;
                            }
                        }
                    }
                }

                if (item is Rectangle && (string)item.Tag == "meteor")
                {
                    Canvas.SetTop(item, Canvas.GetTop(item) + meteorSpeed);

                    if (Canvas.GetTop(item) > (int)mainCanvas.ActualHeight)
                    {
                        itemRemover.Add(item);
                    }

                    Rect meteorHitBox = new Rect(Canvas.GetLeft(item), Canvas.GetTop(item), item.Width, item.Height);

                    if (playerHitBox.IntersectsWith(meteorHitBox))
                    {
                        itemRemover.Add(item);
                        meteorHitSound.Play();
                        healthPlayer1--;
                        if (healthPlayer1 == 2)
                            hart1.Visibility = System.Windows.Visibility.Collapsed;
                        else if (healthPlayer1 == 1)
                            hart2.Visibility = System.Windows.Visibility.Collapsed;
                        else
                            hart3.Visibility = System.Windows.Visibility.Collapsed;

                        if (healthPlayer1 == 0 && twoPlayers)
                            rctgPlayer1.Visibility = System.Windows.Visibility.Collapsed;
                    }

                    if (playerHitBox1.IntersectsWith(meteorHitBox) && twoPlayers)
                    {
                        itemRemover.Add(item);
                        meteorHitSound.Play();
                        healthPlayer2--;

                        if (healthPlayer2 == 2)
                        {
                            player2Hart3.Visibility = System.Windows.Visibility.Collapsed;
                        }
                        else if (healthPlayer2 == 1)
                        {
                            player2Hart2.Visibility = System.Windows.Visibility.Collapsed;
                        }
                        else
                        {
                            player2Hart1.Visibility = System.Windows.Visibility.Collapsed;
                            rctgPlayer2.Visibility = System.Windows.Visibility.Collapsed;
                        }
                    }
                }
            }

            foreach (var item in itemRemover)
            {
                mainCanvas.Children.Remove(item);
            }

            if ((healthPlayer1 < 1 && !twoPlayers) || (twoPlayers && healthPlayer1 < 1 && healthPlayer2 < 1))
            {
                if (scorePlayer1 > Properties.Settings.Default.hiScore || scorePlayer2 > Properties.Settings.Default.hiScore)
                    Properties.Settings.Default.hiScore = Math.Max(scorePlayer2, scorePlayer1);

                Properties.Settings.Default.Save();
                this.lblHiScore.Content = Properties.Settings.Default.hiScore.ToString();
                gameTimer.Stop();

                if (!twoPlayers)
                {
                    GameOver gameOver = new GameOver(scorePlayer1);
                    var response = gameOver.ShowDialog();

                    if (object.Equals("True", response.ToString()))
                    {
                        System.Diagnostics.Process.Start(Application.ResourceAssembly.Location);
                        Application.Current.Shutdown();
                    }
                    else
                    {
                        Application.Current.Shutdown();
                    }
                }
                else
                {
                    twoPlayers = false;
                    GameOver gameOver = new GameOver(scorePlayer1, scorePlayer2);
                    var response = gameOver.ShowDialog();

                    if (object.Equals("True", response.ToString()))
                    {
                        System.Diagnostics.Process.Start(Application.ResourceAssembly.Location);
                        Application.Current.Shutdown();
                    }
                    else
                    {
                        Application.Current.Shutdown();
                    }
                }
            }
        }
    }
}
