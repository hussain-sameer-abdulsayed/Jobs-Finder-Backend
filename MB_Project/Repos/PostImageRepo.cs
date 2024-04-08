using MB_Project.Data;
using MB_Project.IRepos;
using MB_Project.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SkiaSharp;
using System.IO;

namespace MB_Project.Repos
{
    public class PostImageRepo : IPostImageRepo
    {
        private readonly MB_ProjectContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public PostImageRepo(MB_ProjectContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<string> SecndarySaveUploadedFile(IFormFile file)
        {
            // Check if a file was uploaded
            if (file != null && file.Length > 0)
            {
                const int maxFileSizeInBytes = 5 * 1024 * 1024; // 5 MB
                if (file.Length > maxFileSizeInBytes)
                {
                    return "size";
                }
                // Generate a unique file name
                string uniqueFileName = Guid.NewGuid().ToString()+ ".png";

                // Construct the file path
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images/postSecondaryImages");
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                //// Save the file to the file system
                //using (var fileStream = new FileStream(filePath, FileMode.Create))
                //{
                //    await file.CopyToAsync(fileStream);
                //}

                int maxWidth=200, maxHeight = 200;



                // Resize the image
                using (var inputStream = file.OpenReadStream())
                {
                    using (var originalBitmap = SKBitmap.Decode(inputStream))
                    {
                        int newWidth, newHeight;
                        if (originalBitmap.Width > originalBitmap.Height)
                        {
                            newWidth = maxWidth;
                            newHeight = (int)((float)originalBitmap.Height / originalBitmap.Width * maxWidth);
                        }
                        else
                        {
                            newHeight = maxHeight;
                            newWidth = (int)((float)originalBitmap.Width / originalBitmap.Height * maxHeight);
                        }

                        using (var resizedBitmap = originalBitmap.Resize(new SKImageInfo(newWidth, newHeight), SKFilterQuality.High))
                        {
                            // Save the resized image to the file system
                            using (var outputStream = File.Create(filePath))
                            {
                                resizedBitmap.Encode(SKEncodedImageFormat.Png, 100).SaveTo(outputStream);
                            }
                        }
                    }
                }



                // Return the unique file name
                return uniqueFileName;
            }
            else
            {
                // Return null if no file was uploaded
                return null;
            }
        }




        public Task<bool> DeletePostImages(int postId)
        {
            throw new NotImplementedException();
        }
        public Task<IEnumerable<PostImage>> GetPostImages(int postId)
        {
            throw new NotImplementedException();
        }
        public Task<bool> UpdatePostImages(int postId, IFormFile File)
        {
            throw new NotImplementedException();
        }




        public async Task<bool> Create(PostImage postImage)
        {
            try
            {
                await _context.PostsImages.AddAsync(postImage);
                _context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public async Task<bool> Delete(int id)
        {
            try
            {
                var obj = await _context.PostsImages.FindAsync(id);
                if (obj == null)
                {
                    return false;
                }
                _context.Attach(obj);
                _context.PostsImages.Remove(obj);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public async Task<List<string>> GetPostSecondaryImagesUniqueFileNames(int PostId)
        {
            try
            {
                var SecondaryImagesUrl = await _context.PostsImages
                    .Where(x => x.PostId == PostId)
                    .Select(x => x.ImageUrl)
                    .ToListAsync();
                /*
                var list = new List<PostImage>();
                var listPostImages = new List<PostImage>();
                const string _baseImageUrl = "https://localhost:7181/images/postSecondaryImages/";
                foreach (var image in SecondaryImagesUrl)
                {
                    var post = new PostImage
                    {
                        ImageUrl = $"{_baseImageUrl}{image.ImageUrl}",
                        PostId = PostId
                    };
                    listPostImages.Add(post);
                }
                return listPostImages;
                */
                return SecondaryImagesUrl;
            }
            catch
            {
                return null;
            }
        }
        public Task<List<string>> GetPostSecondaryImages(List<string> uniqueFileName)
        {
            try
            {
                if (uniqueFileName.Count() == 0)
                {
                    return null;
                }
                var imageUrl = new List<string>();
                const string _baseImageUrl = "https://localhost:7181/images/postSecondaryImages/";
                foreach (var image in uniqueFileName)
                {
                    imageUrl.Add($"{_baseImageUrl}{image}");
                }

                return Task.FromResult(imageUrl);
            }
            catch
            {
                return null;
            }
        }






        /*
        public async Task<PostImage> GetPostImage(int id)
        {
            try
            {
                var obj = await _context.PostsImages.FindAsync(id);
                return obj;
            }
            catch
            {
                return null;
            }
        }
        */
    }
}
