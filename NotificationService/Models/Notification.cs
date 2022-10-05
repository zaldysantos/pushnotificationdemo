namespace NotificationService.Models
{
    public class Notification
    {
        public int Id { get; set; }

        public int MobileType { get; set; }

        public string JsonPayload { get; set; }

        public string DeviceToken { get; set; }

        public bool IsSent { get; set; }

        public DateTime? SentOn { get; set; }

        public string? Errors { get; set; }
    }
}
