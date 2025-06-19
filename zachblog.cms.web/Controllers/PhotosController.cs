using Microsoft.AspNetCore.Mvc;
using OrchardCore.FileStorage;
using OrchardCore.Media;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace zachblog.cms.web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PhotosController : ControllerBase
    {
        private readonly IMediaFileStore _mediaFileStore;

        public PhotosController(IMediaFileStore mediaFileStore)
        {
            _mediaFileStore = mediaFileStore;
        }

        [HttpGet("featured/{year}/{month}")]
        public async Task<IActionResult> GetFeaturedPhotos(int year, int month)
        {
            try
            {
                var directoryPath = $"Featured-Photos/{year}/{month}";
                
                // Check if directory exists
                var directoryInfo = await _mediaFileStore.GetDirectoryInfoAsync(directoryPath);
                if (directoryInfo == null)
                {
                    return Ok(new { photos = new List<object>(), message = "Directory not found" });
                }                // Get all files in the directory
                var files = new List<IFileStoreEntry>();
                await foreach (var file in _mediaFileStore.GetDirectoryContentAsync(directoryPath))
                {
                    files.Add(file);
                }
                
                // Filter for image files
                var imageExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp", ".bmp", ".tiff" };
                var photos = new List<object>();

                foreach (var file in files.Where(f => !f.IsDirectory))
                {
                    var extension = Path.GetExtension(file.Name).ToLowerInvariant();
                    if (imageExtensions.Contains(extension))
                    {
                        photos.Add(new
                        {
                            filename = file.Name,
                            url = $"/media/{directoryPath}/{file.Name}",
                            size = file.Length,
                            lastModified = file.LastModifiedUtc,
                            caption = Path.GetFileNameWithoutExtension(file.Name)
                        });
                    }
                }

                // Sort by filename or last modified date
                photos = photos.OrderBy(p => ((dynamic)p).filename).ToList();

                return Ok(new { photos, count = photos.Count });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet("featured/latest")]
        public async Task<IActionResult> GetLatestFeaturedPhotos()
        {
            var currentYear = DateTime.Now.Year;
            var currentMonth = DateTime.Now.Month;

            // Try current month first, then previous months
            var monthsToTry = new List<(int year, int month)>
            {
                (currentYear, currentMonth)
            };

            // Add previous months
            for (int i = 1; i <= 3; i++)
            {
                var date = new DateTime(currentYear, currentMonth, 1).AddMonths(-i);
                monthsToTry.Add((date.Year, date.Month));
            }

            foreach (var (year, month) in monthsToTry)
            {
                var result = await GetFeaturedPhotos(year, month);
                if (result is OkObjectResult okResult)
                {
                    var data = okResult.Value as dynamic;
                    if (data?.photos != null && ((IEnumerable<object>)data.photos).Any())
                    {
                        return Ok(new { 
                            photos = data.photos, 
                            year, 
                            month, 
                            count = ((IEnumerable<object>)data.photos).Count() 
                        });
                    }
                }
            }

            return Ok(new { photos = new List<object>(), message = "No photos found in recent months" });
        }
    }
}
