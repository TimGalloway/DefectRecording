using Android.App;
using Android.Widget;
using Android.OS;
using Android.Graphics;
using System;
using Android.Content;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Net.Http.Headers;
using RestSharp;
using Android.Provider;
using System.Collections.Generic;
using Android.Content.PM;

namespace DefectRecording
{
    [Activity(Label = "DefectRecording", MainLauncher = true, Icon = "@drawable/dr")]
    public class MainActivity : Activity
    {
        ImageView _imageView;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Main);

            if (IsThereAnAppToTakePictures())
            {
                CreateDirectoryForPictures("/sdcard/Android/data/DefectRecording/", "pics");

                Button button = FindViewById<Button>(Resource.Id.btnOpenCamera);
                _imageView = FindViewById<ImageView>(Resource.Id.imageView1);
                button.Click += TakeAPicture;
            }
            Button button1 = FindViewById<Button>(Resource.Id.btnSave);
            button1.Click += SendToServer;
        }

        private void SendToServer(object sender, EventArgs e)
        {
            EditText Location = FindViewById<EditText>(Resource.Id.Location);
            EditText Description = FindViewById<EditText>(Resource.Id.Description);
            Button button1 = FindViewById<Button>(Resource.Id.btnSave);

            button1.Text = "Saving";

            byte[] b = System.IO.File.ReadAllBytes(App._file.ToString());
            String b64String = Convert.ToBase64String(b);

            // Do the post to server
            Defect newDefect = new Defect();
            newDefect.Location = Location.Text;
            newDefect.Description = Description.Text;
            newDefect.ImageName = App.filename;
            newDefect.ImageBase64 = b64String;

            ////var client = new RestClient("http://ec2-52-34-120-128.us-west-2.compute.amazonaws.com");
            ////var request = new RestRequest("api/Defects", Method.POST);
            ////var client = new RestClient("http://defectrecording.herokuapp.com/");
            //var client = new RestClient("http://gallowayconsulting.no-ip.org/Defects/");
            //var request = new RestRequest("defects", Method.POST);
            //request.AddObject(newDefect);

            //IRestResponse response = client.Execute(request);

            //var content = response.Content; // raw content as string

            //if (response.ErrorException == null)
            //{
            //    Location.Text = "";
            //    Description.Text = "";
            //    _imageView.SetImageDrawable(null);
            //    button1.Text = "Save";
            //}

            SendToServerAsync(newDefect);

        }

        private async void SendToServerAsync(Defect newDefect)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("http://gallowayconsulting.no-ip.org/Defects");
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("applicaion/json"));
                    var uri = new Uri("http://gallowayconsulting.no-ip.org/Defects/");
                    string serializedObject = JsonConvert.SerializeObject(newDefect);
                    HttpContent contentPost = new StringContent(serializedObject, System.Text.Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await client.PostAsync(uri, contentPost);
                    if (response.IsSuccessStatusCode)
                    {
                        var data = await response.Content.ReadAsStringAsync();
                        Defect retDefect = JsonConvert.DeserializeObject<Defect>(data);
                        //return rettrafficEvent;
                    }

                }
            }
            catch (Exception)
            {
                //return null
            }
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            // Make it available in the gallery

            Intent mediaScanIntent = new Intent(Intent.ActionMediaScannerScanFile);
            Android.Net.Uri contentUri = Android.Net.Uri.FromFile(App._file);
            mediaScanIntent.SetData(contentUri);
            SendBroadcast(mediaScanIntent);

            // Display in ImageView. We will resize the bitmap to fit the display.
            // Loading the full sized image will consume to much memory
            // and cause the application to crash.

            int height = Resources.DisplayMetrics.HeightPixels;
            int width = _imageView.Height;
            App.bitmap = App._file.Path.LoadAndResizeBitmap(width / 3, height / 3);
            if (App.bitmap != null)
            {
                _imageView.SetImageBitmap(App.bitmap);
                App.bitmap = null;
            }

            // Dispose of the Java side bitmap.
            GC.Collect();
        }

        public static class App
        {
            public static Java.IO.File _file;
            public static Java.IO.File _dir;
            public static Bitmap bitmap;
            public static String filename;
        }

        private void CreateDirectoryForPictures(String file1, String file2)
        {
            App._dir = new Java.IO.File(file1, file2);
            if (!App._dir.Exists())
            {
                App._dir.Mkdirs();
            }
        }

        private bool IsThereAnAppToTakePictures()
        {
            Intent intent = new Intent(MediaStore.ActionImageCapture);
            IList<ResolveInfo> availableActivities =
                PackageManager.QueryIntentActivities(intent, PackageInfoFlags.MatchDefaultOnly);
            return availableActivities != null && availableActivities.Count > 0;
        }

        private void TakeAPicture(object sender, EventArgs eventArgs)
        {
            Intent intent = new Intent(MediaStore.ActionImageCapture);
            var guid = Guid.NewGuid();
            App.filename = String.Format("myPhoto_{0}.jpg", guid);
            App._file = new Java.IO.File(App._dir, String.Format("myPhoto_{0}.jpg", guid));
            intent.PutExtra(MediaStore.ExtraOutput, Android.Net.Uri.FromFile(App._file));
            StartActivityForResult(intent, 0);
        }
    }
}
