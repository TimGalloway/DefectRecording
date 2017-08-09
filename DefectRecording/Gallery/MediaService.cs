using System;

using Android.App;
using Android.Content;
using Android.Widget;
using System.Threading.Tasks;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using Android.Views;

namespace DefectRecording.Gallery
{
    public class MediaService : Java.Lang.Object, IMediaService
        {
            public async Task OpenGallery()
            {
                try
                {
                    var status = await CrossPermissions.Current.CheckPermissionStatusAsync(Plugin.Permissions.Abstractions.Permission.Storage);
                    if (status != PermissionStatus.Granted)
                    {
                        if (await CrossPermissions.Current.ShouldShowRequestPermissionRationaleAsync(Plugin.Permissions.Abstractions.Permission.Storage))
                        {
                            Toast.MakeText(Application.Context, "Need Storage permission to access to your photos.", ToastLength.Long).Show();
                        }

                        var results = await CrossPermissions.Current.RequestPermissionsAsync(new[] { Plugin.Permissions.Abstractions.Permission.Storage });
                        status = results[Plugin.Permissions.Abstractions.Permission.Storage];
                    }

                    if (status == PermissionStatus.Granted)
                    {
                        Toast.MakeText(Application.Context, "Select max 20 images", ToastLength.Long).Show();
                        var imageIntent = new Intent(
                            Intent.ActionPick);
                        imageIntent.SetType("image/*");
                        imageIntent.PutExtra(Intent.ExtraAllowMultiple, true);
                        imageIntent.SetAction(Intent.ActionGetContent);
                        ((Activity)Forms.Context).StartActivityForResult(
                            Intent.CreateChooser(imageIntent, "Select photo"), MainActivity.OPENGALLERYCODE);

                    }
                    else if (status != PermissionStatus.Unknown)
                    {
                        Toast.MakeText(Application.Context, "Permission Denied. Can not continue, try again.", ToastLength.Long).Show();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    Toast.MakeText(Application.Context, "Error. Can not continue, try again.", ToastLength.Long).Show();
                }
            }


        }
    }
}