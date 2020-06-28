using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Kutori.Common
{
    /// <summary>
    /// Returned on a successful reply creation.
    /// </summary>
    public class ReplyDisplayModel
    {
        /// <summary>
        /// The ID of the new post.
        /// </summary>
        /// <example>1</example>
        [JsonPropertyName("id")]
        [Required]
        public int Id { get; set; }

        /// <summary>
        /// The text of the new post.
        /// </summary>
        /// <example>Kutori a BEST</example>
        [JsonPropertyName("text")]
        [Required]
        public string Text { get; set; }

        /// <summary>
        /// The timestamp of the post, expressed as seconds since the Unix epoch.
        /// </summary>
        /// <example>1593284912</example>
        [JsonPropertyName("timestamp")]
        [Required]
        public long Timestamp { get; set; }

        /// <summary>
        /// An RFC 2397 data URI representing the image attached to the post.
        /// </summary>
        /// <example>data:image/png;base64,RAW_BASE64_DATA_OF_KUTORI</example>
        [JsonPropertyName("image")]
        public string Image { get; set; }

        /// <summary>
        /// The ID of the Post this reply is for.
        /// </summary>
        /// <example>1</example>
        [JsonPropertyName("id")]
        [Required]
        public int PostId { get; set; }
    }
}
