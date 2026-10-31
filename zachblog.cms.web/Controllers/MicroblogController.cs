using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OrchardCore.ContentFields.Fields;
using OrchardCore.ContentManagement;
using OrchardCore.ContentManagement.Records;
using OrchardCore.Media;
using OrchardCore.Media.Fields;
using ISession = YesSql.ISession;
using YesSql;
using zachblog.cms.web.Microblog;

namespace zachblog.cms.web.Controllers
{
    [Route("tweets")]
    public class MicroblogController : Controller
    {
        private static readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".webp" };

        private readonly IContentManager _contentManager;
        private readonly ISession _session;
        private readonly IMediaFileStore _mediaFileStore;
        private readonly IAuthorizationService _authorizationService;

        public MicroblogController(
            IContentManager contentManager,
            ISession session,
            IMediaFileStore mediaFileStore,
            IAuthorizationService authorizationService)
        {
            _contentManager = contentManager;
            _session = session;
            _mediaFileStore = mediaFileStore;
            _authorizationService = authorizationService;
        }

        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            var model = new MicroblogIndexViewModel
            {
                Posts = await GetLatestAsync(),
                CanPublish = await CanPublishAsync()
            };
            return View(model);
        }

        [HttpPost("")]
        [ValidateAntiForgeryToken]
        [RequestSizeLimit(MicroblogConstants.MaxImageBytes * 5)]
        public async Task<IActionResult> Create(string body, List<IFormFile> images)
        {
            if (!await CanPublishAsync())
            {
                return User.Identity?.IsAuthenticated == true ? Forbid() : Challenge();
            }

            body = body?.Trim();
            images = (images ?? new()).Where(f => f != null && f.Length > 0).ToList();

            if (string.IsNullOrEmpty(body) && images.Count == 0)
            {
                TempData["MicroblogError"] = "Write something or attach an image.";
                return RedirectToAction(nameof(Index));
            }

            foreach (var image in images)
            {
                if (image.Length > MicroblogConstants.MaxImageBytes ||
                    !AllowedExtensions.Contains(Path.GetExtension(image.FileName).ToLowerInvariant()))
                {
                    TempData["MicroblogError"] = "Images must be jpg, png, gif or webp and under 10 MB.";
                    return RedirectToAction(nameof(Index));
                }
            }

            var now = DateTime.UtcNow;
            var paths = new List<string>();
            foreach (var image in images)
            {
                var path = $"microblog/{now:yyyy}/{now:MM}/{Guid.NewGuid():N}{Path.GetExtension(image.FileName).ToLowerInvariant()}";
                await using var stream = image.OpenReadStream();
                paths.Add(await _mediaFileStore.CreateFileFromStreamAsync(path, stream, false));
            }

            var item = await _contentManager.NewAsync(MicroblogConstants.ContentType);
            item.Owner = User.Identity?.Name;
            item.Author = User.Identity?.Name;
            item.CreatedUtc = now;
            item.ModifiedUtc = now;
            item.Alter<ContentPart>(MicroblogConstants.ContentType, part =>
            {
                part.Alter<TextField>(MicroblogConstants.BodyField, f => f.Text = body ?? string.Empty);
                part.Alter<MediaField>(MicroblogConstants.ImagesField, f => f.Paths = paths.ToArray());
            });
            await _contentManager.CreateAsync(item, VersionOptions.Published);

            return RedirectToAction(nameof(Index));
        }

        [HttpGet("feed.xml")]
        public async Task<IActionResult> Feed()
        {
            var posts = await GetLatestAsync();
            var baseUrl = $"{Request.Scheme}://{Request.Host}";
            var channel = new XElement("channel",
                new XElement("title", "Zach's Tweets"),
                new XElement("link", baseUrl + "/tweets"),
                new XElement("description", "Microblog posts"));

            foreach (var post in posts)
            {
                var text = post.Body ?? string.Empty;
                var title = text.Length > 80 ? text[..80] + "…" : text;
                var description = WebUtility.HtmlEncode(text).Replace("\n", "<br/>");
                if (post.FirstImageUrl != null)
                {
                    var imageUrl = post.FirstImageUrl.StartsWith("/") ? baseUrl + post.FirstImageUrl : post.FirstImageUrl;
                    description += $"<p><img src=\"{WebUtility.HtmlEncode(imageUrl)}\" /></p>";
                }
                channel.Add(new XElement("item",
                    new XElement("title", string.IsNullOrEmpty(title) ? "Photo" : title),
                    new XElement("link", baseUrl + "/tweets#" + post.ContentItemId),
                    new XElement("guid", new XAttribute("isPermaLink", "false"), post.ContentItemId),
                    new XElement("pubDate", post.CreatedUtc.ToString("R")),
                    new XElement("description", description)));
            }

            var doc = new XDocument(new XDeclaration("1.0", "utf-8", null),
                new XElement("rss", new XAttribute("version", "2.0"), channel));
            return Content(doc.Declaration + Environment.NewLine + doc.Root, "application/rss+xml", Encoding.UTF8);
        }

        private Task<bool> CanPublishAsync() =>
            _authorizationService.AuthorizeAsync(User, MicroblogPermissions.PublishMicroblog)
                .ContinueWith(t => t.Result);

        private async Task<List<MicroblogPostViewModel>> GetLatestAsync()
        {
            var items = await _session.Query<ContentItem, ContentItemIndex>(x =>
                    x.ContentType == MicroblogConstants.ContentType && x.Published)
                .OrderByDescending(x => x.CreatedUtc)
                .Take(MicroblogConstants.PageSize)
                .ListAsync();

            return items.Select(item =>
            {
                var part = item.Get<ContentPart>(MicroblogConstants.ContentType);
                var paths = part?.Get<MediaField>(MicroblogConstants.ImagesField)?.Paths ?? Array.Empty<string>();
                return new MicroblogPostViewModel
                {
                    ContentItemId = item.ContentItemId,
                    Body = part?.Get<TextField>(MicroblogConstants.BodyField)?.Text,
                    CreatedUtc = item.CreatedUtc ?? DateTime.UtcNow,
                    ImageUrls = paths.Select(p => _mediaFileStore.MapPathToPublicUrl(p)).ToList()
                };
            }).ToList();
        }
    }
}
