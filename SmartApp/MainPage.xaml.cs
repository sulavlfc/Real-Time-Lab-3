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

using Windows.Media.Capture;
using Windows.Storage;
using Windows.Graphics.Imaging;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Storage.Streams;

using Microsoft.ProjectOxford.Emotion;
using Microsoft.ProjectOxford.Emotion.Contract;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace SmartApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        CameraCaptureUI captureUI = new CameraCaptureUI();
        StorageFile photo;
        IRandomAccessStream imageStream;

        const string APIKEY = "API Key";   //Insert API Key to Microsoft Cognitive Services for Emotion
        EmotionServiceClient emotionServiceClient = new EmotionServiceClient(APIKEY);
        Emotion[] emotionResult;

        public MainPage()
        {
            this.InitializeComponent();
            captureUI.PhotoSettings.Format = CameraCaptureUIPhotoFormat.Jpeg;	//captured image format id JPEG
            captureUI.PhotoSettings.CroppedSizeInPixels = new Size(200, 200);  //crop image of size 200 x 200
        }

        private async void btnTakePic_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                photo = await captureUI.CaptureFileAsync(CameraCaptureUIMode.Photo); //specify capture mode as photo but not video

                if (photo == null)
                {
                    //User cancelled photo
                    return;
                }
                else
                {
                    imageStream = await photo.OpenAsync(FileAccessMode.Read);
                    BitmapDecoder decoder = await BitmapDecoder.CreateAsync(imageStream);
                    SoftwareBitmap softwareBitmap = await decoder.GetSoftwareBitmapAsync();

                    SoftwareBitmap softwareBitmapBGR8 = SoftwareBitmap.Convert(softwareBitmap, BitmapPixelFormat.Bgra8, BitmapAlphaMode.Premultiplied);
                    SoftwareBitmapSource bitmapSource = new SoftwareBitmapSource();
                    await bitmapSource.SetBitmapAsync(softwareBitmapBGR8);

                    image.Source = bitmapSource;

                }
            }
            catch
            {
                txtOut.Text = "Error Taking Photo";
            }

        }

        private async void btnGetEmotion_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                emotionResult = await emotionServiceClient.RecognizeAsync(imageStream.AsStream());

                if (emotionResult != null)
                {
                    Scores score = emotionResult[0].Scores;
                    txtOut.Text = "Your emotions are :\n \n" + "Happiness :" + score.Happiness + "\n" + 
                                                            "Sadness  :"  + score.Sadness   + "\n" +
                                                            "Surprise :" + score.Surprise  + "\n" +
                                                            "Fear     :"       + score.Fear      + "\n" +
                                                            "Anger    :"      + score.Anger     + "\n" +
                                                            "Contempt :"   + score.Contempt  + "\n" +
                                                            "Disgust  :"    + score.Disgust   + "\n" +
                                                            "Neutral  :"    + score.Neutral   + "\n" ; 
                }
            }
            catch
            {
                txtOut.Text = "Error returning the Emotion";
            }
        }
    }
}
