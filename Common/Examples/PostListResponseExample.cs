using System.Collections.Generic;
using Swashbuckle.AspNetCore.Filters;

namespace Kutori.Common
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public sealed class PostListResponseExample : IExamplesProvider<IDictionary<string, PostListModel>>
    {
        public IDictionary<string, PostListModel> GetExamples()
        {
            return new Dictionary<string, PostListModel>
            {
                ["1"] = new PostListModel
                {
                    Image = "data:image/png;base64,RAW_BASE64_DATA_OF_KUTORI",
                    Replies = 15,
                    Text = "Kutori a BEST",
                    Timestamp = 1593284912
                }
            };
        }
    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
