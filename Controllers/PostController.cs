using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Kutori.Common;
using Kutori.Database;
using Kutori.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Kutori.Controllers
{
    [Route("api/posts")]
    [ApiController]
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public class PostController : ControllerBase
    {
        private const int MAXIMUM_POST_LENGTH = 1000;

        private readonly DatabaseContext _context;

        public PostController(DatabaseContext context)
        {
            _context = context;
        }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        /// <summary>
        /// Gets all existing posts.
        /// </summary>
        /// <param name="last" example="0">Return only posts newer than this ID.</param>
        /// <param name="images" example="false">If set to true, image data is included in the post model. This is disabled by default as image data is a large amount of the response.</param>
        /// <returns>A dictionary of posts with their IDs as the key.</returns>
        /// <response code="200">Posts successfully retrieved.</response>
        [HttpGet]
        [ProducesResponseType(typeof(IDictionary<string, PostListModel>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IDictionary<string, PostListModel>>> GetPostsAsync(int last = 0, bool images = false)
        {
            var posts = await _context.Posts.Where(x => !x.PostId.HasValue && x.Id > last)
                .OrderBy(x => x.Id).ToListAsync();

            return posts.ToDictionary(x => x.Id.ToString(), x => new PostListModel
            {
                Text = x.Text,
                Timestamp = x.Timestamp,
                Replies = _context.Posts.Count(y => y.PostId == x.Id),
                Image = images ? x.ImageData : null
            });
        }

        /// <summary>
        /// Gets all replies for an existing post.
        /// </summary>
        /// <param name="id" example="1">The ID of the post.</param>
        /// <param name="last" example="0">Return only replies newer than this ID.</param>
        /// <param name="images" example="false">If set to true, image data is included in the reply models. This is disabled by default as image data is a large amount of the response.</param>
        /// <returns>An object detailing the post and its replies.</returns>
        /// <response code="404">A post does not exist with that ID or was deleted.</response>
        /// <response code="200">Replies successfully retrieved.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(IDictionary<string, ReplyListModel>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IDictionary<string, ReplyListModel>>> GetRepliesAsync(int id, int last = 0, bool images = false)
        {
            var post = await _context.Posts.FindAsync(id);

            if (post is null)
            {
                return NotFound(new ErrorModel
                {
                    Error = $"No post exists with the ID {id}.",
                    StatusCode = (int) HttpStatusCode.NotFound
                });
            }
                

            var replies = await _context.Posts.Where(x => x.PostId == id && x.Id > last)
                .OrderBy(x => x.Id)
                .ToListAsync();

            return replies.ToDictionary(x => x.Id.ToString(), x => new ReplyListModel
            {
                Text = x.Text,
                PostId = post.Id,
                Image = images ? x.ImageData : null,
                Timestamp = x.Timestamp
            });
        }

        /// <summary>
        /// Creates a new post.
        /// </summary>
        /// <param name="post">An object detailing the content for the new post.</param>
        /// <returns>The newly created post with its unique ID.</returns>
        /// <response code="400">The text you provided was too long, or the image provided was invalid.</response>
        /// <response code="200">Post was successfully created.</response>
        [HttpPost]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(PostDisplayModel), StatusCodes.Status200OK)]
        public async Task<ActionResult<PostDisplayModel>> CreatePostAsync(PostCreateModel post)
        {
            if (post.Text.Length > MAXIMUM_POST_LENGTH)
            {
                return BadRequest(new ErrorModel
                {
                    Error = $"Your post must be {MAXIMUM_POST_LENGTH} characters in length or shorter.",
                    StatusCode = (int) HttpStatusCode.BadRequest
                });
            }

            if (!post.HasValidImage(out var error))
            {
                return BadRequest(new ErrorModel
                {
                    Error = error,
                    StatusCode = (int) HttpStatusCode.BadRequest
                });
            }

            var entity = _context.Posts.Add(new Post
            {
                Text = post.Text,
                Timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                IP = HttpContext.Features.Get<IHttpConnectionFeature>().RemoteIpAddress,
                ImageData = post.Image
            }).Entity;

            await _context.SaveChangesAsync();

            return new PostDisplayModel
            {
                Id = entity.Id,
                Text = entity.Text,
                Timestamp = entity.Timestamp,
                Image = post.Image
            };
        }

        /// <summary>
        /// Creates a reply for an existing post.
        /// </summary>
        /// <param name="reply">An object detailing the content of the new reply.</param>
        /// <param name="id">The ID of the post you are replying to.</param>
        /// <returns>The newly created reply with its unique ID.</returns>
        /// <response code="404">A post does not exist with that ID or was deleted.</response>
        /// <response code="400">The text you provided was too long, or the image provided was invalid.</response>
        /// <response code="200">Reply was successfully created.</response>
        [HttpPost("{id}")]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ReplyDisplayModel), StatusCodes.Status200OK)]
        public async Task<ActionResult<ReplyDisplayModel>> CreateReplyAsync(int id, PostCreateModel reply)
        {
            if (reply.Text.Length > MAXIMUM_POST_LENGTH)
            {
                return BadRequest(new ErrorModel
                {
                    Error = $"Your reply must be {MAXIMUM_POST_LENGTH} characters in length or shorter.",
                    StatusCode = (int)HttpStatusCode.BadRequest
                });
            }

            var post = await _context.Posts.FindAsync(id);

            if (post is null)
            {
                return NotFound(new ErrorModel
                {
                    Error = $"No post exists with the ID {id}.",
                    StatusCode = (int) HttpStatusCode.NotFound
                });
            }

            if (!reply.HasValidImage(out var error))
            {
                return BadRequest(new ErrorModel
                {
                    Error = error,
                    StatusCode = (int) HttpStatusCode.BadRequest
                });
            }

            var entity = _context.Posts.Add(new Post
            {
                Text = reply.Text,
                Timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                IP = HttpContext.Features.Get<IHttpConnectionFeature>().RemoteIpAddress,
                PostId = id,
                ImageData = reply.Image
            }).Entity;

            await _context.SaveChangesAsync();

            return new ReplyDisplayModel
            {
                Id = entity.Id,
                Text = entity.Text,
                Timestamp = entity.Timestamp,
                Image = reply.Image,
                PostId = post.Id
            };
        }

        /// <summary>
        /// Deletes one of your posts or replies.
        /// </summary>
        /// <remarks>
        /// <para>Because this endpoint exists, consumers wishing to maintain proper state will need to be prepared for replies to suddenly disappear from <code>GET /api/posts/{id}</code>.</para>
        /// <para>If a post is deleted, all replies are also deleted. Keep this in mind for proper state as well.</para>
        /// </remarks>
        /// <param name="id">The ID of the post or reply you wish to delete.</param>
        /// <response code="404">A post or reply does not exist with that ID or was deleted.</response>
        /// <response code="403">You do not have permission to delete another user's post or reply.</response>
        /// <response code="204">Post or reply successfully deleted.</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DeletePostAsync(int id)
        {
            var post = await _context.Posts.FindAsync(id);

            if (post is null)
            {
                return NotFound(new ErrorModel
                {
                    Error = $"No post exists with the ID {id}.",
                    StatusCode = (int) HttpStatusCode.NotFound
                });
            }

            if (!post.IP.Equals(HttpContext.Features.Get<IHttpConnectionFeature>().RemoteIpAddress))
            {
                HttpContext.Response.StatusCode = (int) HttpStatusCode.Forbidden;
                return new JsonResult(new ErrorModel
                {
                    Error = $"You do not have permission to remove the post with ID {id}.",
                    StatusCode = (int) HttpStatusCode.Forbidden
                });
            }

            _context.Posts.Remove(post);

            if (!post.PostId.HasValue)
                _context.Posts.RemoveRange(await _context.Posts.Where(x => x.PostId == post.Id).ToListAsync());

            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
