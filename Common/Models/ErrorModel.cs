using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Kutori.Common
{
    /// <summary>
    /// Returned by the API when a handled error occurs during processing.
    /// </summary>
    public sealed class ErrorModel
    {
        /// <summary>
        /// The status code returned in the response.
        /// </summary>
        /// <example>404</example>
        [JsonPropertyName("status")]
        [Required]
        public int StatusCode { get; set; }

        /// <summary>
        /// The error message detailing why there was an error in the first place.
        /// </summary>
        /// <example>No existing post exists with the ID 1.</example>
        [JsonPropertyName("error")]
        [Required]
        public string Error { get; set; }
    }
}
