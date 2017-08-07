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
using EntityFramework;
using Microsoft.EntityFrameworkCore;
using EntityFramework.Models;

namespace DefectRecording
{
    [Activity(Label = "DefectRecording", MainLauncher = true, Icon = "@drawable/dr")]
    public class MainActivity : Activity
    {
        ImageView _imageView;
        //SQLiteConnection db;
        DefectContext db;

        protected override async void OnCreate(Bundle bundle)
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
                button1.Click += SavetoDB;

            }
            var docFolder1 = "/sdcard/Android/data/DefectRecording/";
            var docFolder2 = "files/";
            var docsFolder = docFolder1 + docFolder2;
            CreateDirectoryForDatabase(docFolder1, docFolder2);

            var pathToDatabase = System.IO.Path.Combine(docsFolder, "db_adonet.db");

            try
            {
                db = new DefectContext(pathToDatabase);
                await db.Database.MigrateAsync(); //We need to ensure the latest Migration was added. This is different than EnsureDatabaseCreated.
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
        }

        private void SavetoDB(object sender, EventArgs e)
        {
            EditText editText1 = FindViewById<EditText>(Resource.Id.editText1);
            Defect newDefect = new Defect();
            newDefect.ImgName = App._file.ToString();
            newDefect.Description = editText1.Text;
            db.Defects.Add(newDefect);
            db.SaveChanges();
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
            App._dir = new Java.IO.File(file1,file2);
            if (!App._dir.Exists())
            {
                App._dir.Mkdirs();
            }
        }

        private void CreateDirectoryForDatabase(String file1, String file2)
        {
            Java.IO.File ldir = new Java.IO.File(file1,file2);
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
    }

  
}


