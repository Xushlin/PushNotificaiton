using System;
using System.Linq;
using Android.App;
using Android.Content;
using Android.Util;
using Android.Widget;
using Firebase.Messaging;

namespace XamarinAndroidFCM
{
    [Service]
    [IntentFilter(new[] {"com.google.firebase.MESSAGING_EVENT"})]
    public class MyFirebaseMessagingService : FirebaseMessagingService
    {
        private const string Tag = "MyFirebaseMsgService";
        public override void OnMessageReceived(RemoteMessage message)
        {
            Log.Debug(Tag, "From: " + message.From);
            if (message.GetNotification() != null)
            {
                //These is how most messages will be received
                Log.Debug(Tag, "Notification Message Body: " + message.GetNotification().Body);
                SendNotification(message.GetNotification().Body);
            }
            else
            {
                //Only used for debugging payloads sent from the Azure portal
                CreateNotification("Test FCM", message.Data.Values.First(), "15:30");
            }
        }

        void SendNotification(string messageBody)
        {
            var intent = new Intent(this, typeof(MainActivity));
            intent.AddFlags(ActivityFlags.ClearTop);
            var pendingIntent = PendingIntent.GetActivity(this, 0, intent, PendingIntentFlags.OneShot);

            var notificationBuilder = new Notification.Builder(this)
                .SetContentTitle("FCM Message")
                .SetSmallIcon(Resource.Drawable.ic_launcher)
                .SetContentText(messageBody)
                .SetAutoCancel(true)
                .SetContentIntent(pendingIntent);

            var notificationManager = NotificationManager.FromContext(this);
            notificationManager.Notify(0, notificationBuilder.Build());
        }

        void CreateNotification(string title, string desc, string time)
        {
            var notificationManager = GetSystemService(Context.NotificationService) as NotificationManager;
            var uiIntent = new Intent(this, typeof(MainActivity));
            var notification = new Notification(Resource.Mipmap.ic_launcher, title);
            notification.Flags = NotificationFlags.AutoCancel;
            notification.Defaults = NotificationDefaults.All;
            notification.Vibrate = new long[] { 0, 100, 200, 300 };
            if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.JellyBean)
            {
                var contentView = new RemoteViews(PackageName, Resource.Layout.Custom_Notification);
                contentView.SetTextViewText(Resource.Id.txtTitle, title);
                contentView.SetTextViewText(Resource.Id.txtTime, time);
                contentView.SetTextViewText(Resource.Id.txtContent, desc);
                notification.BigContentView = contentView;
            }
            notification.SetLatestEventInfo(this, title, desc, PendingIntent.GetActivity(this, 0, uiIntent, PendingIntentFlags.UpdateCurrent));

            var rnd = new Random();
            var notificationId = rnd.Next(10000, 99999);

            notificationManager.Notify(notificationId, notification);
        }
    }
}