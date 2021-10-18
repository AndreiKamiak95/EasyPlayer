using System;
using System.IO;

namespace EasyPlayer
{
    public static class Directories
    {
        private static StreamWriter playList;
        public static void SearchFiles(string startDirectory)
        {
            playList = new StreamWriter("playlist.txt", false);
            string[] folders = Directory.GetDirectories(startDirectory, "*", SearchOption.AllDirectories);
            if (folders.Length == 0)
            {
                string[] files = Directory.GetFiles(startDirectory);
                foreach (var element in files)
                {
                    if (element.IndexOf(".mp3") >= 0)
                    {
                        playList.WriteLine(element);
                    }
                }
            }
            else
            {
                for (int i = 0; i < folders.Length; i++)
                {
                    string[] files = Directory.GetFiles(folders[i]);
                    foreach (var element in files)
                    {
                        if (element.IndexOf(".mp3") >= 0)
                        {
                            playList.WriteLine(element);
                        }
                    }
                }
            }

            playList.Close();
        }
    }
}
