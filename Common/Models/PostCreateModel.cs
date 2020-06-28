using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace Kutori.Common
{
    /// <summary>
    /// Used for creating a new post or reply.
    /// </summary>
    public class PostCreateModel
    {
        private static readonly byte[] Gif87aHeader = { 0x47, 0x49, 0x46, 0x38, 0x37, 0x61 };
        private static readonly byte[] Gif89aHeader = { 0x47, 0x49, 0x46, 0x38, 0x39, 0x61 };
        private static readonly byte[] PngHeader = { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A };
        private static readonly byte[] JpegHeader1 = { 0xFF, 0xD8, 0xFF, 0xDB };
        private static readonly byte[] JpegHeader2 = { 0xFF, 0xD8, 0xFF, 0xE0, 0x00, 0x10, 0x4A, 0x46, 0x49, 0x46, 0x00, 0x01 };
        private static readonly byte[] JpegHeader3 = { 0xFF, 0xD8, 0xFF, 0xEE };

        private static readonly Regex ImageDataUriRegex = new Regex(@"data:image\/(png|jpeg|gif);base64,[A-Za-z0-9+\/]*(={0,2})$", RegexOptions.Compiled);
        private const int MAX_IMAGE_SIZE_BYTES = 2_097_152;

        /// <summary>
        /// The text of the post you are creating.
        /// </summary>
        /// <example>Kutori a BEST</example>
        [JsonPropertyName("text")]
        [Required]
        public string Text { get; set; }

        /// <summary>
        /// An RFC 2397 data URI representing the image you are attaching to the post.
        /// </summary>
        /// <example>data:image/png;base64,RAW_BASE64_DATA_OF_KUTORI</example>
        [JsonPropertyName("image")]
        public string Image { get; set; }

        internal bool HasValidImage(out string error)
        {
            error = null;
            if (Image == null) // only allow null, empty string means it's defined and should be an error
            {
                return true;
            }

            var match = ImageDataUriRegex.Match(Image);

            if (!match.Success)
            {
                error = "Image data was not properly formatted. It must be a proper base-64 encoded data URI with the media type as image/png, image/jpg, or image/gif.";
                return false;
            }

            var count = 0;
            if (match.Groups.Count > 2)
            {
                count = match.Groups[2].Length;
            }

            /* FOR STORING IMAGES, USE THIS TO GET INPUT LENGTH
                x = (n * (3/4)) - y

                1. x is the size of a file in bytes

                2. n is the length of the Base64 String

                3. y will be 2 if Base64 ends with '==' and 1 if Base64 ends with '='.
            */
            var data = match.Groups[1].Value;
            var size = data.Length * 3 / 4 - count;
            if (size > MAX_IMAGE_SIZE_BYTES)
            {
                error = $"Base-64 encoded image must be smaller than {MAX_IMAGE_SIZE_BYTES} bytes in size.";
                return false;
            }

            try
            {
                var bytes = new Span<byte>(Convert.FromBase64String(data));
                switch (match.Groups[0].Value)
                {
                    case "gif" when !bytes.StartsWith(new ReadOnlySpan<byte>(Gif87aHeader)) &&
                                    !bytes.StartsWith(new ReadOnlySpan<byte>(Gif89aHeader)):
                        error = "The GIF image header was malformed or missing.";
                        return false;
                    case "png" when !bytes.StartsWith(new ReadOnlySpan<byte>(PngHeader)):
                        error = "The PNG image header was malformed or missing.";
                        return false;
                    case "jpeg" when !bytes.StartsWith(new ReadOnlySpan<byte>(JpegHeader1)) &&
                                     !bytes.StartsWith(new ReadOnlySpan<byte>(JpegHeader2)) &&
                                     !bytes.StartsWith(new ReadOnlySpan<byte>(JpegHeader3)):
                        error = "The JPEG image header was malformed or missing.";
                        return false;
                }
            }
            catch (FormatException)
            {
                error = "The supplied base64 data string was improperly formatted.";
                return false;
            }
            

            return true;
        }
    }
}
