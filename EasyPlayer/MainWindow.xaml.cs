﻿using System;
using System.Collections.Generic;
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
using System.Windows.Forms;
using System.IO;

namespace EasyPlayer
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MediaPlayer player;
        private List<string> playList;
        private int numberSong = 0;
        private bool isPlaying = false;
        private Timer timer;

        public MainWindow()
        {
            InitializeComponent();
            player = new MediaPlayer();
            playList = new List<string>();
            StreamReader filePlayList;
            player.MediaEnded += Player_MediaEnded;
            timer = new Timer();
            timer.Tick += Timer_Tick;
            timer.Interval = 1000;

            try
            {
                filePlayList = new StreamReader("playlist.txt");
            }
            catch (FileNotFoundException e)
            {
                System.Windows.MessageBox.Show("Папка для воспроизведения не выбрана");
                return;
            }
            
            string songPath;
            while((songPath = filePlayList.ReadLine()) != null)
            {
                playList.Add(songPath);
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
            TimeSpan curTime = player.Position;
            lblTimeLeft.Content = curTime.Seconds;
            
        }

        private void Player_MediaEnded(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
            if (numberSong < playList.Count)
            {
                numberSong++;
                player.Open(new Uri(playList[numberSong]));
            }
            else
            {
                player.Stop();
                numberSong = 0;
                isPlaying = false;
            }
        }

        private void BtnOpen_Click(object sender, RoutedEventArgs e)
        {
            var FBD = new FolderBrowserDialog();
            if(FBD.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Directories.SearchFiles(FBD.SelectedPath);
                var filePlayList = new StreamReader("playlist.txt");
                string songPath;
                while ((songPath = filePlayList.ReadLine()) != null)
                {
                    playList.Add(songPath);
                }
            }
        }

        private void BtnPlay_Click(object sender, RoutedEventArgs e)
        {
            if (!isPlaying)
            {
                isPlaying = true;
                player.Open(new Uri(playList[numberSong]));
            }
            player.Play();
            timer.Start();
        }

        private void BtnPause_Click(object sender, RoutedEventArgs e)
        {
            if(isPlaying)
                player.Pause();
            timer.Stop();
        }

        private void BtnStop_Click(object sender, RoutedEventArgs e)
        {
            if (isPlaying)
            {
                player.Stop();
                isPlaying = false;
                timer.Stop();
            }
        }
    }
}