using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Kutori.Common
{
    /// <summary>
    /// Returned on a successful reply creation.
    /// </summary>
    public class ReplyDisplayModel : PostDisplayModel
    {
        /// <summary>
        /// The ID of the Post this reply is for.
        /// </summary>
        /// <example>1</example>
        [JsonPropertyName("id")]
        [Required]
        public int PostId { get; set; }
    }
}
