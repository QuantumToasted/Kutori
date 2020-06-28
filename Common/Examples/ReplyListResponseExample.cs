using System.Collections.Generic;
using Swashbuckle.AspNetCore.Filters;

namespace Kutori.Common
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public sealed class ReplyListResponseExample : IExamplesProvider<IDictionary<string, ReplyListModel>>
    {
        public IDictionary<string, ReplyListModel> GetExamples()
        {
            return new Dictionary<string, ReplyListModel>
            {
                ["2"] = new ReplyListModel()
                {
                    Image = "data:image/png;base64,RAW_BASE64_DATA_OF_ITHEA",
                    Text = "Ithea is better, moron.",
                    PostId = 1,
                    Timestamp = 1593284912
                }
            };
        }
    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
