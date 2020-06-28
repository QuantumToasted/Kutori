using System.Net;

namespace Kutori.Models
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public class Post
    {
        public int Id { get; set; }

        public int? PostId { get; set; }

        public string Text { get; set; }

        // seconds since UTC epoch
        public long Timestamp { get; set; }

        public IPAddress IP { get; set; }

        // data:image/(png/jpeg/gif);base64,BASE64_ENCODED_IMAGE
        public string ImageData { get; set; } 
    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
