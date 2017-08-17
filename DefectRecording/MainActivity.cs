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
using RestSharp;

namespace DefectRecording
{
    [Activity(Label = "DefectRecording", MainLauncher = true, Icon = "@drawable/dr")]
    public class MainActivity : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Main);

            Button button1 = FindViewById<Button>(Resource.Id.button1);
            button1.Click += SendToServer;
        }

        private void SendToServer(object sender, EventArgs e)
        {
            EditText Location = FindViewById<EditText>(Resource.Id.Location);
            EditText Description = FindViewById<EditText>(Resource.Id.Description);

            // Do the post to server
            Defect newDefect = new Defect();
            newDefect.location = Location.Text;
            newDefect.description = Description.Text;

            var client = new RestClient("http://localhost:58385");

            var request = new RestRequest("api/Defects", Method.POST);
            request.AddParameter("Location", newDefect.location);
            request.AddParameter("Description", newDefect.description);

            IRestResponse response = client.Execute(request);

            //HttpClient client;

            //client = new HttpClient();
            //client.MaxResponseContentBufferSize = 256000;

            //String restURL = "http://gallowayconsulting.no-ip.org:3000/defects/";
            //Uri uri = new Uri(string.Format(restURL, string.Empty));

            //var json = JsonConvert.SerializeObject(newDefect);
            //StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
            //content.Headers.Allow.Add("application/json");
            //content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

            //HttpResponseMessage response = null;
            //response = await client.PostAsync(uri, content);

            //response.EnsureSuccessStatusCode();

            //if (response.IsSuccessStatusCode)
            //{
            //    Location.Text = "";
            //    Description.Text = "";
            //}
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
        }

        public static class App
        {
            public static Java.IO.File _file;
            public static Java.IO.File _dir;
            public static Bitmap bitmap;
        }

    }
}