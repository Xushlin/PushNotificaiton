using System.Collections.Generic;
using Android.App;
using Android.Util;
using WindowsAzure.Messaging;
using Firebase.Iid;

namespace XamarinAndroidFCM
{
    [Service]
    [IntentFilter(new[] {"com.google.firebase.INSTANCE_ID_EVENT"})]
    public class MyFirebaseIidService : FirebaseInstanceIdService
    {
        private const string Tag = "MyFirebaseIIDService";
        NotificationHub _hub;

        public override void OnTokenRefresh()
        {
            var refreshedToken = FirebaseInstanceId.Instance.Token;
            Log.Debug(Tag, "FCM token: " + refreshedToken);
            SendRegistrationToServer(refreshedToken);
        }

        void SendRegistrationToServer(string token)
        {
            // Register with Notification Hubs
            _hub = new NotificationHub(Constants.NotificationHubName,Constants.ListenConnectionString, this);

            var tags = new List<string>() { "1" };
            var regID = _hub.Register(token, tags.ToArray()).RegistrationId;
            Log.Debug(Tag, $"Successful registration of ID {regID}");
        }
    }
}