using MediaToolkit;
using MediaToolkit.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using VideoLibrary;
using YoutubeDownloader.Helper;

namespace YoutubeDownloader.MVVM.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        public MainViewModel()
        {
            TypeFormatList = new List<string>() { ".mp3", ".mp4" };
            YoutubeURL = "";
            StatusInfo = "";
            ProgressBar = "0";
        }
        private string _youtubeURL;
        private string _statusInfo;
        private string _progressBar;

        public List<string> TypeFormatList { get; set; }
        public string YoutubeURL 
        {
            get 
            { 
                return _youtubeURL; 
            }
            set {
                _youtubeURL = value; 
                OnPropertyChanged(); 
            }
        }
        public string StatusInfo
        {
            get
            {
                return _statusInfo;
            }
            set
            {
                _statusInfo = value;
                OnPropertyChanged();
            }
        }
        public string ProgressBar
        {
            get
            {
                return _progressBar;
            }
            set
            {
                _progressBar = value;
                OnPropertyChanged();
            }
        }
        public ICommand StartDownloadCommand { get { return new RelayCommand(this.StartYoutubeLinkDownloadAsync, e => true); } }
        public ICommand OpenMP3FolderCommand { get { return new RelayCommand(this.OpenMp3Folder, e => true) ; } }
        public ICommand OpenMP4FolderCommand { get { return new RelayCommand(this.OpenMp4Folder, e => true); } }
        private async void StartYoutubeLinkDownloadAsync(object obj)
        {
            try
            {
                ComboBox formatBox = (ComboBox)obj;
                this.StatusInfo = $"Starting {formatBox.SelectedItem} download...";
                switch (formatBox.SelectedItem)
                {
                    case ".mp3":
                        await StartMp3Download();
                        break;
                    case ".mp4":
                        await StartMp4Download();
                        break;
                    default:
                        this.StatusInfo = "Format type could not be identified.";
                        break;
                }
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show("Lak, gib einen vernünftigen Youtubelink ein.\n Mach keine Faxxen hier.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Herzlichen Glückwunsch! Du hast ein Fehler entdeckt. Bitte die gleichfolgende Meldung an Martin weiterreichen. Dange.");
                MessageBox.Show(ex.ToString());
            }

        }
        private async Task StartMp3Download()
        {            
            this.ProgressBar = "40";
            string mp3Dir = Directory.GetCurrentDirectory() + "\\mp3\\";
            if (!Directory.Exists(mp3Dir))
                Directory.CreateDirectory(mp3Dir);
            var source = mp3Dir;
            var youtube = YouTube.Default;
            this.StatusInfo = "Downloading mp4 file";
            var vid = await youtube.GetVideoAsync(this.YoutubeURL);
            this.ProgressBar = "60";
            this.StatusInfo = "Creating mp4 file";
            File.WriteAllBytes(source + vid.FullName, vid.GetBytes());
            this.ProgressBar = "80";
            var inputFile = new MediaFile { Filename = source + vid.FullName };
            var outputFile = new MediaFile { Filename = $"{source + vid.FullName}.mp3" };
            this.StatusInfo = "Converting and creating mp3 file";
            this.ProgressBar = "90";
            using (var engine = new Engine())
            {
                engine.GetMetadata(inputFile);
                engine.Convert(inputFile, outputFile);
            }
            this.ProgressBar = "100";
            this.StatusInfo = "Done";
        }
        private async Task StartMp4Download()
        {
            string mp4Dir = Directory.GetCurrentDirectory() + "\\mp4\\";
            if (!Directory.Exists(mp4Dir))
                Directory.CreateDirectory(mp4Dir);
            var source = mp4Dir;
            var youtube = YouTube.Default;
            this.StatusInfo = "Downloading mp4 file";
            this.ProgressBar = "40";
            var vid = await youtube.GetVideoAsync(this.YoutubeURL);
            this.ProgressBar = "80";
            this.StatusInfo = "Creating mp4 file";
            File.WriteAllBytes(source + vid.FullName, vid.GetBytes());
            this.ProgressBar = "100";
            this.StatusInfo = "Done";
        }
        private void OpenMp3Folder(object obj)
        {
            string mp3Dir = Directory.GetCurrentDirectory() + "\\mp3\\";
            if (!Directory.Exists(mp3Dir))
                Directory.CreateDirectory(mp3Dir);
            Process.Start(mp3Dir);
        }
        private void OpenMp4Folder(object obj)
        {
            string mp4Dir = Directory.GetCurrentDirectory() + "\\mp4\\";
            if (!Directory.Exists(mp4Dir))
                Directory.CreateDirectory(mp4Dir);
            Process.Start(mp4Dir);
        }
    }
}
