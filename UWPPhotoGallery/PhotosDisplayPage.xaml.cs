using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using UWPPhotoGallery.Model;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace UWPPhotoGallery
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PhotosDisplayPage : Page
    {
        public ObservableCollection<Photo> Photos;

        //public static List<Photo> selectedPhotos { get; set; } = new List<Photo>();


        public PhotosDisplayPage()
        {
            this.InitializeComponent();
            Photos = new ObservableCollection<Photo>();
            DeleteBtn.Visibility = Visibility.Collapsed;
            //this.Loaded += PhotosDisplayPage_Loaded;
        }

        /// <summary>
        /// This method will actually query for all the pictures present in the my pictures folder and display it to the user
        /// We have subscribed for this because the loading of pics is async and cannot be done in the constructor of the page.
        /// If there are no pictures it will display a pop up box asking the user to add pics to the folder.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void PhotosDisplayPage_Loaded(object sender, RoutedEventArgs e)
        {
           
            LoadingRing.IsActive = true;
            //now if there is no context - ( i.e all photos should be loaded)

            await PhotoManager.GetAllPhotos(Photos);
            LoadingRing.IsActive = false;



        }
        async protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            
            //now do whatever that needs to be done
            LoadingRing.IsActive = true;
            //now if there is no context - ( i.e all photos should be loaded)

            bool loaded = await PhotoManager.GetAllPhotos(Photos);
            //if retval is false and photos collection is empty - tell the user something is wrong
            if(loaded && Photos.Count == 0)
            {
                //there are no pictures to display 
                //prompt the user to use the menu option to add pictures
                var messageDialog = new MessageDialog("No Photos found in PicturesLibrary. Use Add Option of AllPhotos menu to add pictures to the collection");
                messageDialog.Commands.Add(new UICommand("OK"));
                await messageDialog.ShowAsync();

            } else if (!loaded){
                //something went wrong - tell the user to try again
                var messageDialog = new MessageDialog("Something went wrong, Contact App Support");
                messageDialog.Commands.Add(new UICommand("OK"));
                await messageDialog.ShowAsync();

            }
            LoadingRing.IsActive = false;

        }

        private void PhotoImage_Tapped(object sender, TappedRoutedEventArgs e)
        {
            //on tapping this image, load the flipview control with the cureent image as the default one
            Photo selectedphoto = PhotoGrid.SelectedItem as Photo;
            PhotoFlipView.SelectedItem = selectedphoto;
            //enable flipview
            PhotoFlipView.Visibility = Visibility.Visible;
            //disable the listtypetextbox
            DisplayTypeTextBlock.Visibility = Visibility.Collapsed;
        }

        private void SelectBtn_Click(object sender, RoutedEventArgs e)
        {
            //make photos selectable for the user to choose
            DeleteBtn.Visibility = Visibility.Visible;
            SelectBtn.Visibility = Visibility.Collapsed;
            PhotoGrid.SelectionMode = ListViewSelectionMode.Multiple;

            //Flipview still pops up when photo is clicked? Unable to find control to disable.
            PhotoFlipView.Visibility = Visibility.Collapsed;

            DisplayTypeTextBlock.Text = "Select Photos";

        }

        private void DeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            DeleteBtn.Visibility = Visibility.Collapsed;
            SelectBtn.Visibility = Visibility.Visible;
            this.Frame.Navigate(typeof(PhotosDisplayPage));
        }
    }
}
