namespace NotificationService.Data
{
    public class Notifications
    {
        private static readonly Models.Notification[] NotificationsData = new[] // mock notification data
        {
            new Models.Notification()
            {
                Id = 1,
                MobileType = (int)(PushNotification.MobileType.Android),
                JsonPayload = @"{ ""to"": ""/token/(android device id goes here)"", ""priority"": ""high"", ""topic"": """", ""notification"": { ""title"": ""NotificationService"", ""body"": ""This is test #1"" } }",
                DeviceToken = "(android device id goes here)",
                IsSent = true,
                SentOn = new DateTime(2019, 12, 31),
                Errors = null
            },
            new Models.Notification()
            {
                Id = 2,
                MobileType = (int)(PushNotification.MobileType.iOS),
                JsonPayload = @"{ ""aps"": { ""alert"": { ""title"": ""NotificationService"", ""body"": ""This is test #2"" }, ""sound"": ""default"" } }",
                DeviceToken = "(ios device id goes here)",
                IsSent = true,
                SentOn = null,
                Errors = null
            },
            new Models.Notification()
            {
                Id = 3,
                MobileType = (int)(PushNotification.MobileType.Android),
                JsonPayload = @"{ ""to"": ""/token/(android device id goes here)"", ""priority"": ""high"", ""topic"": """", ""notification"": { ""title"": ""NotificationService"", ""body"": ""This is test #3"" } }",
                DeviceToken = "(android device id goes here)",
                IsSent = false,
                SentOn = null,
                Errors = null
            },
        };

        /// <summary>
        /// get notifications
        /// </summary>
        /// <param name="isSent"></param>
        /// <returns></returns>
        public static IEnumerable<Models.Notification>? GetNotifications(bool isSent)
        {
            return NotificationsData.AsEnumerable().Where(m => m.IsSent == isSent);
        }
    }
}
