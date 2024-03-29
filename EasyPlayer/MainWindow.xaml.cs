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
            player.MediaOpened += Player_MediaOpened;
            timer = new Timer();
            timer.Tick += Timer_Tick;
            timer.Interval = 1000;

            try
            {
                filePlayList = new StreamReader("playlist.txt");
            }
            catch (FileNotFoundException)
            {
                System.Windows.MessageBox.Show("Папка для воспроизведения не выбрана");
                return;
            }
            
            string songPath;
            while((songPath = filePlayList.ReadLine()) != null)
            {
                playList.Add(songPath);
            }
            filePlayList.Dispose();

            numberSong = Properties.Settings.Default.numberSong;
            isPlaying = Properties.Settings.Default.isPlaying;
            slrVolume.Value = Properties.Settings.Default.Volume;
            slrBalance.Value = Properties.Settings.Default.Balance;

            if (isPlaying)
            {
                player.Open(new Uri(playList[numberSong]));
                player.Play();
                timer.Start();
            }
        }

        private void Player_MediaOpened(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
            lblDuration.Content = player.NaturalDuration.TimeSpan.ToString(@"hh\:mm\:ss");
            pgb.Maximum = player.NaturalDuration.TimeSpan.TotalSeconds;
            string nameFile = player.Source.ToString();
            lblNameFile.Content = nameFile.Substring(nameFile.LastIndexOf('/') + 1) + " [" + (numberSong + 1).ToString() + '/' + playList.Count.ToString() + ']';
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
            TimeSpan curTime = player.Position;
            lblTimeLeft.Content = curTime.ToString(@"hh\:mm\:ss");
            pgb.Value = curTime.TotalSeconds;
        }

        private void Player_MediaEnded(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
            if (numberSong < playList.Count)
            {
                numberSong++;
                if (numberSong == playList.Count)
                    numberSong = 0;
                pgb.Value = 0;
                player.Open(new Uri(playList[numberSong]));
                player.Play();
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
            playList.Clear();
            if (FBD.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Directories.SearchFiles(FBD.SelectedPath);
                var filePlayList = new StreamReader("playlist.txt");
                string songPath;
                while ((songPath = filePlayList.ReadLine()) != null)
                {
                    playList.Add(songPath);
                }
                numberSong = 0;
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

        private void btnLeft_Click(object sender, RoutedEventArgs e)
        {
            player.Stop();
            numberSong--;
            if ((uint)numberSong == uint.MaxValue)
                numberSong = playList.Count - 1;
            player.Open(new Uri(playList[numberSong]));
            player.Play();
        }

        private void btnRight_Click(object sender, RoutedEventArgs e)
        {
            player.Stop();
            numberSong++;
            if (numberSong == playList.Count)
                numberSong = 0;
            player.Open(new Uri(playList[numberSong]));
            player.Play();
        }

        private void pgb_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            player.Position = TimeSpan.FromSeconds(pgb.Value);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Properties.Settings.Default.numberSong = numberSong;
            Properties.Settings.Default.isPlaying = isPlaying;
            Properties.Settings.Default.curPosition = player.NaturalDuration.TimeSpan;
            Properties.Settings.Default.Save();
        }

        private void BtnMute_Click(object sender, RoutedEventArgs e)
        {
            player.IsMuted = player.IsMuted == false;
        }

        private void SrlVolume_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            player.Volume = slrVolume.Value / 100;
            Properties.Settings.Default.Volume = slrVolume.Value;
            Properties.Settings.Default.Save();
        }

        private void SrlBalance_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            player.Balance = slrBalance.Value / 100;
            Properties.Settings.Default.Balance = slrBalance.Value;
            Properties.Settings.Default.Save();
        }

        private void SlrBalance_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            player.Balance = 0;
            slrBalance.Value = 0;
            Properties.Settings.Default.Balance = slrBalance.Value;
            Properties.Settings.Default.Save();
        }
    }
}
