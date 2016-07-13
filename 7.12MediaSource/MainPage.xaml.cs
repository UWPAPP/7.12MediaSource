using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;


using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.Storage;
using Windows.Storage.Pickers;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace _7._12MediaSource
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        MediaSource mediaSource;
        MediaPlaybackItem mediaPlaybackItem;

        public MainPage()
        {
            this.InitializeComponent();
        }

        //private async void pickFileButton_Click(object sender, RoutedEventArgs e)
        //{
        //    var filePicker = new Windows.Storage.Pickers.FileOpenPicker();

        //    //Add filetype filters.  In this case wmv and mp4.
        //    filePicker.FileTypeFilter.Add(".wmv");
        //    filePicker.FileTypeFilter.Add(".mp4");
        //    filePicker.FileTypeFilter.Add(".mkv");

        //    //Set picker start location to the video library
        //    filePicker.SuggestedStartLocation = PickerLocationId.VideosLibrary;

        //    //Retrieve file from picker
        //    StorageFile file = await filePicker.PickSingleFileAsync();

        //    if (file != null)
        //    {
        //        mediaSource = MediaSource.CreateFromStorageFile(file);
        //        mediaElement.SetPlaybackSource(mediaSource);
        //    }
        //}

        private async void pickFileButton_Click(object sender, RoutedEventArgs e)
        {
            var filePicker = new Windows.Storage.Pickers.FileOpenPicker();

            //Add filetype filters.  In this case wmv and mp4.
            filePicker.FileTypeFilter.Add(".wmv");
            filePicker.FileTypeFilter.Add(".mp4");
            filePicker.FileTypeFilter.Add(".mkv");

            //Set picker start location to the video library
            filePicker.SuggestedStartLocation = PickerLocationId.VideosLibrary;

            //Retrieve file from picker
            StorageFile file = await filePicker.PickSingleFileAsync();

            mediaSource = MediaSource.CreateFromStorageFile(file);
            mediaPlaybackItem = new MediaPlaybackItem(mediaSource);

            //mediaPlaybackItem.AudioTracksChanged += PlaybackItem_AudioTracksChanged;
            mediaPlaybackItem.VideoTracksChanged += MediaPlaybackItem_VideoTracksChanged;
            //mediaPlaybackItem.TimedMetadataTracksChanged += MediaPlaybackItem_TimedMetadataTracksChanged;

            mediaElement.SetPlaybackSource(mediaPlaybackItem);
        }

        private async void MediaPlaybackItem_VideoTracksChanged(MediaPlaybackItem sender, IVectorChangedEventArgs args)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                videoTracksComboBox.Items.Clear();
                for (int index = 0; index < sender.VideoTracks.Count; index++)
                {
                    var videoTrack = sender.VideoTracks[index];
                    ComboBoxItem item = new ComboBoxItem();
                    item.Content = String.IsNullOrEmpty(videoTrack.Label) ? "Track " + index : videoTrack.Label;
                    item.Tag = index;
                    videoTracksComboBox.Items.Add(item);
                }
            });
        }

        private void videoTracksComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int trackIndex = (int)((ComboBoxItem)((ComboBox)sender).SelectedItem).Tag;
            mediaPlaybackItem.VideoTracks.SelectedIndex = trackIndex;
        }



        private void audioTracksComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int trackIndex = (int)((ComboBoxItem)((ComboBox)sender).SelectedItem).Tag;
            mediaPlaybackItem.AudioTracks.SelectedIndex = trackIndex;
        }

        private async void PlaybackItem_AudioTracksChanged(MediaPlaybackItem sender, IVectorChangedEventArgs args)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                audioTracksComboBox.Items.Clear();
                for (int index = 0; index < sender.AudioTracks.Count; index++)
                {
                    var audioTrack = sender.AudioTracks[index];
                    ComboBoxItem item = new ComboBoxItem();
                    item.Content = String.IsNullOrEmpty(audioTrack.Label) ? "Track " + index : audioTrack.Label;
                    item.Tag = index;
                    audioTracksComboBox.Items.Add(item);
                }
            });
        }

        private async void MediaPlaybackItem_TimedMetadataTracksChanged(MediaPlaybackItem sender, IVectorChangedEventArgs args)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                for (int index = 0; index < sender.TimedMetadataTracks.Count; index++)
                {
                    var timedMetadataTrack = sender.TimedMetadataTracks[index];

                    ToggleButton toggle = new ToggleButton()
                    {
                        Content = String.IsNullOrEmpty(timedMetadataTrack.Label) ? "Track " + index : timedMetadataTrack.Label,
                        Tag = (uint)index
                    };
                    toggle.Checked += Toggle_Checked;
                    toggle.Unchecked += Toggle_Unchecked;

                    MetadataButtonPanel.Children.Add(toggle);
                }
            });
        }

        private void Toggle_Checked(object sender, RoutedEventArgs e)
        {
            mediaPlaybackItem.TimedMetadataTracks.SetPresentationMode((uint)((ToggleButton)sender).Tag,
                TimedMetadataTrackPresentationMode.PlatformPresented);
        }

        private void Toggle_Unchecked(object sender, RoutedEventArgs e)
        {
            mediaPlaybackItem.TimedMetadataTracks.SetPresentationMode((uint)((ToggleButton)sender).Tag,
                TimedMetadataTrackPresentationMode.Disabled);
        }
    }
}
