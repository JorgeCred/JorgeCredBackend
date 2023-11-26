namespace JorgeCred.Application.Dtos.Request
{
    public class PushNotificationRequest
    {
        public string Endpoint { get; set; }
        public KeysRequest Keys { get; set; }
    }

    public class KeysRequest
    {
        public string Auth { get; set; }
        public string? ExpirationTime { get; set; }
        public string p256dh { get; set; }
    }

}
