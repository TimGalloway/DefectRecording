using Android.App;
using Android.Widget;
using Android.OS;
using Android.Graphics;
using System;
using Android.Content;
using Android.Provider;
using System.Collections.Generic;
using Android.Content.PM;
using SQLite;
using MimeKit;
using System.IO;
using DefectRecording.Helpers;

namespace DefectRecording
{
    [Activity(Label = "DefectRecording", MainLauncher = true, Icon = "@drawable/dr")]
    public class MainActivity : Activity
    {
        ImageView _imageView;
        SQLiteConnection db;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Main);

            if (IsThereAnAppToTakePictures())
            {
                CreateDirectoryForPictures("/sdcard/Android/data/DefectRecording/", "pics");

                Button button = FindViewById<Button>(Resource.Id.myButton);
                _imageView = FindViewById<ImageView>(Resource.Id.imageView1);
                button.Click += TakeAPicture;

                Button button1 = FindViewById<Button>(Resource.Id.button1);
                button1.Click += sendEmail;

            }
            var docFolder1 = "/sdcard/Android/data/DefectRecording/";
            var docFolder2 = "files/";
            var docsFolder = docFolder1 + docFolder2;
            CreateDirectoryForDatabase(docFolder1, docFolder2);

            var pathToDatabase = System.IO.Path.Combine(docsFolder, "DefectRecording.db");
            db = new SQLiteConnection(pathToDatabase);
            db.CreateTable<Defect>();

        }

        private void SavetoDB(object sender, EventArgs e)
        {
            EditText editText1 = FindViewById<EditText>(Resource.Id.editText1);
            var newDefect = new Defect();
            newDefect.ImgName = App._file.ToString();
            newDefect.Description = editText1.Text;
            db.Insert(newDefect);
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
            App.bitmap = App._file.Path.LoadAndResizeBitmap(width, height);
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
        }

        private void CreateDirectoryForPictures(String file1, String file2)
        {
            //App._dir = new Java.IO.File(
            //    Android.OS.Environment.GetExternalStoragePublicDirectory(
            //        Android.OS.Environment.DirectoryPictures), "CameraAppDemo");
            App._dir = new Java.IO.File(file1, file2);
            if (!App._dir.Exists())
            {
                App._dir.Mkdirs();
            }
        }

        private void CreateDirectoryForDatabase(String file1, String file2)
        {
            Java.IO.File ldir = new Java.IO.File(file1, file2);
            if (!ldir.Exists())
            {
                ldir.Mkdirs();
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
            App._file = new Java.IO.File(App._dir, String.Format("myPhoto_{0}.jpg", Guid.NewGuid()));
            intent.PutExtra(MediaStore.ExtraOutput, Android.Net.Uri.FromFile(App._file));
            StartActivityForResult(intent, 0);
        }

        private void sendEmail(object sender, EventArgs ea)
        {
            try
            {
                EditText editText1 = FindViewById<EditText>(Resource.Id.editText1);
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("Tim - Android", "galloway@iinet.net.au"));
                message.To.Add(new MailboxAddress("Tim", "galloway@iinet.net.au"));
                message.Subject = "How you doin?";

                var body = new TextPart("plain")
                {
                    Text = editText1.Text
                };

                // create an image attachment for the file located at path
                var attachment = new MimePart("image", "gif")
                {
                    ContentObject = new ContentObject(File.OpenRead(App._file.ToString()), ContentEncoding.Default),
                    ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
                    ContentTransferEncoding = ContentEncoding.Base64,
                    FileName = System.IO.Path.GetFileName(App._file.ToString())
                };

                // now create the multipart/mixed container to hold the message text and the
                // image attachment
                var multipart = new Multipart("mixed");
                multipart.Add(body);
                multipart.Add(attachment);

                // now set the multipart/mixed as the message body
                message.Body = multipart;

                using (var client = new MailKit.Net.Smtp.SmtpClient())
                {
                    // For demo-purposes, accept all SSL certificates (in case the server supports STARTTLS)
                    client.ServerCertificateValidationCallback = (s, c, h, e) => true;

                    client.Connect("mail.iinet.net.au", 25, false);

                    // Note: since we don't have an OAuth2 token, disable
                    // the XOAUTH2 authentication mechanism.
                    client.AuthenticationMechanisms.Remove("XOAUTH2");

                    // Note: only needed if the SMTP server requires authentication
                    client.Authenticate("galloway", "SuzTL1000");

                    client.Send(message);
                    client.Disconnect(true);
                }
                editText1.Text = "";
            }
            catch (Exception ex)
            {

            }
        }
    }
}

