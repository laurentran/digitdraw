using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;

using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;
using System.Runtime.Serialization.Json;
using Windows.Data.Json;
using DigitDraw.Request;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace DigitDraw
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        private Point Origin
        {
            get;
            set;
        }

        PointerPoint point;

        private void onPointerPressed(object sender, PointerRoutedEventArgs e)
        {
            point = e.GetCurrentPoint(NumbercCanvas);
        }

        private void onPointerMoved(object sender, PointerRoutedEventArgs e)
        {
            Line line = new Line();
            line.Stroke = new SolidColorBrush(Colors.Black);
            line.StrokeThickness = 9;
            line.X1 = point.Position.X;
            line.Y1 = point.Position.Y;
            line.X2 = e.GetCurrentPoint(NumbercCanvas).Position.X;
            line.Y2 = e.GetCurrentPoint(NumbercCanvas).Position.Y;
            point = e.GetCurrentPoint(NumbercCanvas);
            this.NumbercCanvas.Children.Add(line);
        }

        private async void onSendClick(object sender, RoutedEventArgs e)
        {
            try
            {
                Progress.IsActive = true;
                // Save canvas.  
                RenderTargetBitmap bitmap = await this.CanvasToBMP();
                //IBuffer buffer = await getPixelBuffer(bitmap);
                var buffer = await bitmap.GetPixelsAsync();
                int[] featureVector = CreateFeatureVector(bitmap, buffer);
                await InvokeRequestResponseService(featureVector);

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }  
        
        }

        public async Task InvokeRequestResponseService(int[] featureVector)
        {

            using (var client = new HttpClient())
            {
                ScoreRequest scoreRequest = ScoreRequestData.GetScoreRequestObject(featureVector, string.Format("score00{0}", 1));

                const string apiKey = "qgw8I24RAtzRfAzNhsKMFLg2PiF9ONqRNbUFAVImEtw5jEcTd91+KBEisgx1yvrBrqEFvra1ZqSX6EmU1XBbcQ=="; // Replace this with the API key for the web service
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

                client.BaseAddress = new Uri("https://ussouthcentral.services.azureml.net/workspaces/66aefb9a5ae3400abd250a7e96f6ef58/services/100455b48f094073914370dfe4476d35/score");
                HttpResponseMessage response = await client.PostAsJsonAsync("", scoreRequest);
                if (response.IsSuccessStatusCode)
                {
                    Progress.IsActive = false;
                    string result = await response.Content.ReadAsStringAsync();
                    Debug.WriteLine("Result: {0}", result);
                    Debug.WriteLine(result[2]);
                    Result.Text = result[2].ToString();

                }
                else
                {
                    Debug.WriteLine("Failed with status code: {0}", response.StatusCode);
                }
            }
        }

        private async Task<RenderTargetBitmap> CanvasToBMP()
        {
            // Initialization  
            RenderTargetBitmap bitmap = null;
            try
            {
                // Convert canvas to bmp
                var bmp = new RenderTargetBitmap();
                await bmp.RenderAsync(NumbercCanvas);
                bitmap = bmp as RenderTargetBitmap;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
            return bitmap;
        }

        //private async void onPointerExited(object sender, PointerRoutedEventArgs e)
        //{
        //    try
        //    {
        //        // Save canvas.  
        //        RenderTargetBitmap bitmap = await this.CanvasToBMP();
        //        //IBuffer buffer = await getPixelBuffer(bitmap);
        //        var buffer = await bitmap.GetPixelsAsync();
        //        int[] featureVector = CreateFeatureVector(bitmap, buffer);
        //    }
        //    catch (Exception ex)
        //    {
        //        Debug.WriteLine(ex.ToString());
        //    }  
        //}

        private void onPointerExited(object sender, PointerRoutedEventArgs e)
        {
            //TODO
        }



        public int[] CreateFeatureVector(RenderTargetBitmap bitmap, IBuffer buffer)
        {
            int targetPixels = 28*28; 
            int[] featureVector = new int[targetPixels];

            int canvasWidth = (int)NumbercCanvas.RenderSize.Width;
            int canvasHeight = (int)NumbercCanvas.RenderSize.Height;
            int widthAspectRatio = canvasWidth / 28;
            int heightAspectRatio = canvasHeight / 28;
            int compressedPixels = widthAspectRatio * heightAspectRatio;
            int totalPixels = canvasWidth * canvasHeight;

            int num4 = canvasWidth * heightAspectRatio; 

            int bitmapPixels = bitmap.PixelWidth * bitmap.PixelHeight;

            if (bitmapPixels == totalPixels)
            {
                List<List<int>> list = new List<List<int>>();
                for (int i = 0; i < bitmapPixels; i++)
                {
                    int num7 = i % canvasWidth / widthAspectRatio % 28;
                    int num8 = i / num4 * 28 + num7;
                    if (list.Count <= num8)
                    {
                        list.Add(new List<int>());
                    }
                    list[num8].Add(buffer.GetByte((uint)(4*i)));
                }

                if (list.Count * 70 == totalPixels)
                {
                    for (int j = 0; j < targetPixels; j++)
                    {
                        if (list[j].Count == compressedPixels)
                        {
                            int blackPixels = 0;
                            foreach (int item in list[j])
                            {
                                if (item == 255)
                                {
                                    //Debug.WriteLine("White");
                                    continue;
                                }
                                blackPixels++;
                                //Debug.WriteLine("Black");
                            }
                            double blackPixelRatio = (double)blackPixels / (double)compressedPixels;
                            Debug.WriteLine(blackPixelRatio);
                            featureVector[j] = (int)(blackPixelRatio * 255);
                        }
                    }
                }

            }
            return featureVector;
        }

        private void onClearClick(object sender, RoutedEventArgs e)
        {
            Result.Text = "";
            NumbercCanvas.Children.Clear();
        }




    }
}
