using MB_Project.Data;
using MB_Project.IRepos;
using MB_Project.Models;
using MB_Project.Models.DTOS.PostDto;
using MB_Project.Models.DTOS.UserDto;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using SkiaSharp;

namespace MB_Project.Repos
{
    public class PostRepo : IPostRepo
    {
        private readonly MB_ProjectContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public PostRepo(MB_ProjectContext context, IWebHostEnvironment webHostEnvironment)
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
                string uniqueFileName = Guid.NewGuid().ToString();

                // Construct the file path
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images/postSecondaryImages");
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                // Save the file to the file system
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
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
        public async Task<string> SaveUploadedFile(IFormFile file)
        {
            // Check if a file was uploaded
            if (file != null && file.Length > 0)
            {
                // Generate a unique file name
                string uniqueFileName = Guid.NewGuid().ToString() + ".png";

                // Construct the file path
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images/post");
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                // Save the file to the file system
                //using (var fileStream = new FileStream(filePath, FileMode.Create))
                //{
                //    await file.CopyToAsync(fileStream);
                //}

                int maxWidth = 200, maxHeight = 200;



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






        public async Task<Post> CreatePost(Post post)
        {
            try
            {
                await _context.Posts.AddAsync(post);
                _context.SaveChanges();
                return post;
            }
            catch
            {
                return null;
            }
        }
        public async Task<bool> CreatePostMainImage(int PostId,string uniqueFileName)
        {
            try
            {
                var post = await _context.Posts.FindAsync(PostId);
                if (post == null)
                {
                    return false;
                }
                post.MainImage = uniqueFileName;
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public async Task<bool> DeletePost(int postId)
        {
            try
            {
                var post = await _context.Posts
                        .Include(p => p.Categories)
                        .Include(p => p.SecondaryImages)
                        .Include(p => p.PostFeatures)
                        .Include(p => p.Reviews)
                        .FirstOrDefaultAsync(p => p.Id == postId);
                if (post == null)
                {
                    return false;
                }
                var uniqueFileName = await GetUniqueFileNameForPostMainImage(postId);
                if(uniqueFileName == "error")
                {
                    return false;
                }
                var chk = await DeletePostMainImage(postId);
                if (!chk)
                {
                    return false;
                }
                _context.Attach(post);
                _context.Posts.Remove(post);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }   
        public async Task<bool> DeletePostMainImage(int PostId)
        {
            try
            {
                string Directory = Path.Combine(_webHostEnvironment.WebRootPath, "images/post");
                string uniqueFileName = await GetUniqueFileNameForPostMainImage(PostId);
                if (uniqueFileName == "error")
                {
                    return false;
                }
                // Construct the full file path based on the unique file name and the images directory
                string filePath = Path.Combine(Directory, uniqueFileName);

                // Check if the file exists before attempting to delete it
                if (File.Exists(filePath))
                {
                    // Delete the file
                    File.Delete(filePath);
                    var post = await _context.Posts.FindAsync(PostId);
                    post.MainImage = null;
                    _context.Attach(post);
                    await _context.SaveChangesAsync();
                    return true; // Return true if the file was successfully deleted
                }
                else
                {
                    return false; 
                }
            }
            catch
            {
                return false; 
            }
        }
        public void DeleteFile(string uniqueFileName)
        {
            string Directory = Path.Combine(_webHostEnvironment.WebRootPath, "images/post");
            string filePath = Path.Combine(Directory, uniqueFileName);

            // Check if the file exists before attempting to delete it
            if (File.Exists(filePath))
            {
                // Delete the file
                File.Delete(filePath);
            }
        }
        public Task<string> GetPostMainImage(string uniqueFileName)
        {
            try
            {
                const string _baseImageUrl = "https://localhost:7181/images/post/";
                if (string.IsNullOrEmpty(uniqueFileName))
                {
                    return Task.FromResult<string>(null);
                }
                string imageUrl = $"{_baseImageUrl}{uniqueFileName}";
                return Task.FromResult(imageUrl);
            }
            catch
            {
                return Task.FromResult<string>(null);
            }
        }
        public async Task<string> GetUniqueFileNameForPostMainImage(int postId)
        {
            try
            {
                var uniqueFileName = await _context.Posts
                    .Where(x => x.Id == postId)
                    .Select(x => x.MainImage)
                    .FirstOrDefaultAsync();
                if (uniqueFileName == null)
                {
                    return null;
                }
                return uniqueFileName;
            }
            catch
            {
                return null;
            }
        }
        public async Task<IEnumerable<Post>> GetPosts()
        {
            try
            {
                var posts = await _context.Posts
                                          .Select(p => new Post
                                          {
                                              Id = p.Id,
                                              Title = p.Title,
                                              BasePrice = p.BasePrice,
                                              MainImage = p.MainImage,
                                              FreelancerId = p.FreelancerId,
                                              Description = p.Description,
                                              /*
                                              //Description = p.Description,
                                              //Categories = p.Categories.Select(c => new Category
                                              //{
                                              //    Id = c.Id,
                                              //    Name = c.Name
                                              //}).ToList(),

                                              //Reviews = p.Reviews.Select(c => new Review 
                                              //{ 
                                              //    Id = c.Id, 
                                              //    PostId = c.PostId,
                                              //    UserId = c.UserId,
                                              //    Content = c.Content,
                                              //    Rating = c.Rating
                                              //}).ToList(),
                                              //SecondaryImages = p.SecondaryImages.Select(c => new PostImage 
                                              //{ 
                                              //    Id = c.Id,
                                              //    ImageUrl = c.ImageUrl,
                                              //    PostId = c.PostId
                                              //}).ToList(),
                                              //PostFeatures = p.PostFeatures.Select(c=> new PostFeature
                                              //{
                                              //    Id = c.Id,
                                              //    PostId= c.PostId,
                                              //    Title= c.Title,
                                              //    Price = c.Price
                                              //}).ToList(),
                                              */
                                          }).ToListAsync();
                foreach (var post in posts)
                {
                    var uniqueFileName = await GetUniqueFileNameForPostMainImage(post.Id);
                    //var uniqueFileNames = await GetPostSecondaryImagesUniqueFileNames(post.Id);
                    post.MainImage = await GetPostMainImage(uniqueFileName);
                    //post.images = await GetPostSecondaryImages(uniqueFileNames);
                }

                return posts;
            }
            catch
            {
                return Enumerable.Empty<Post>();
            }
        }
        public async Task<Post> GetPostsById(int workId)
        {
            try
            {
                var uniqueFileName = await GetUniqueFileNameForPostMainImage(workId);
                var MainImage = await GetPostMainImage(uniqueFileName);
                var SecondaryImagesUniqueFileNames = await GetPostSecondaryImagesUniqueFileNames(workId);
                var SecondaryImages = await GetPostSecondaryImages(SecondaryImagesUniqueFileNames);

                var post = await _context.Posts
                    .Where(x => x.Id == workId)
                    .Select(p => new Post
                {
                    Id = p.Id,
                    Title = p.Title,
                    Description = p.Description,
                    BasePrice = p.BasePrice,
                    MainImage = MainImage,
                    FreelancerId = p.FreelancerId,
                    images = p.images,

                    Categories = p.Categories.Select(c => new Category
                    {
                        Id = c.Id,
                        Name = c.Name
                    }).ToList(),

                    Reviews = p.Reviews.Select(c => new Review
                    {
                        Id = c.Id,
                        PostId = c.PostId,
                        UserId = c.UserId,
                        Content = c.Content,
                        Rating = c.Rating
                    }).ToList(),
                    /*
                    SecondaryImages = p.SecondaryImages.Select(c => new PostImage
                    {
                        Id = c.Id,
                        ImageUrl = c.ImageUrl,
                        PostId = c.PostId
                    }).ToList(),
                    */
                    PostFeatures = p.PostFeatures.Select(c => new PostFeature
                    {
                        Id = c.Id,
                        PostId = c.PostId,
                        Title = c.Title,
                        Price = c.Price
                    }).ToList(),
                })
                    .FirstOrDefaultAsync();
                if (post == null)
                {
                    return null;
                }

                post.images = SecondaryImages;// string to string

                return post;
            }
            catch
            {
                return null;
            }
        }
        public async Task<IEnumerable<Post>> SearchPosts(string name)
        {
            try
            {
                var posts = await _context.Posts
                    .Where(c => EF.Functions.Like(c.Title,$"%{name}%")||EF.Functions.Like(c.Description, $"%{name}%"))
                    .Select(p => new Post
                    {
                        Id = p.Id,
                        Title = p.Title,
                        Description = p.Description,
                        BasePrice = p.BasePrice,
                        MainImage = p.MainImage,
                        FreelancerId = p.FreelancerId,
                        /*
                        Categories = p.Categories.Select(c => new Category
                        {
                            Id = c.Id,
                            Name = c.Name
                        }).ToList(),

                        Reviews = p.Reviews.Select(c => new Review
                        {
                            Id = c.Id,
                            PostId = c.PostId,
                            UserId = c.UserId,
                            Content = c.Content,
                            Rating = c.Rating
                        }).ToList(),
                        SecondaryImages = p.SecondaryImages.Select(c => new PostImage
                        {
                            Id = c.Id,
                            ImageUrl = c.ImageUrl,
                            PostId = c.PostId
                        }).ToList(),
                        PostFeatures = p.PostFeatures.Select(c => new PostFeature
                        {
                            Id = c.Id,
                            PostId = c.PostId,
                            Title = c.Title,
                            Price = c.Price
                        }).ToList(),
                        */
                    })
            .ToListAsync(); ;
                foreach (var post in posts)
                {
                    var uniqueFileName = await GetUniqueFileNameForPostMainImage(post.Id);
                    var MainImage = await GetPostMainImage(uniqueFileName);
                    //var SecondaryImagesUniqueFileNames = await GetPostSecondaryImagesUniqueFileNames(post.Id);
                    //var SecondaryImages = await GetPostSecondaryImages(SecondaryImagesUniqueFileNames);
                    post.MainImage = MainImage;
                    //post.images = SecondaryImages.ToList();
                }
                return posts;
            }
            catch 
            { 
                return Enumerable.Empty<Post>(); 
            }

        }
        public async Task<IEnumerable<Post>> GetUserPosts(string userId)
        {
            try
            {
                var posts = await _context.Posts
                    .Where(x => x.FreelancerId == userId)
                    .Select(p => new Post
                    {
                        Id = p.Id,
                        Title = p.Title,
                        Description = p.Description,
                        BasePrice = p.BasePrice,
                        MainImage = p.MainImage,
                        FreelancerId = p.FreelancerId,
                        /*
                        Categories = p.Categories.Select(c => new Category
                        {
                            Id = c.Id,
                            Name = c.Name
                        }).ToList(),

                        Reviews = p.Reviews.Select(c => new Review
                        {
                            Id = c.Id,
                            PostId = c.PostId,
                            UserId = c.UserId,
                            Content = c.Content,
                            Rating = c.Rating
                        }).ToList(),
                        SecondaryImages = p.SecondaryImages.Select(c => new PostImage
                        {
                            Id = c.Id,
                            ImageUrl = c.ImageUrl,
                            PostId = c.PostId
                        }).ToList(),
                        PostFeatures = p.PostFeatures.Select(c => new PostFeature
                        {
                            Id = c.Id,
                            PostId = c.PostId,
                            Title = c.Title,
                            Price = c.Price
                        }).ToList(),
                        */
                    })
                    .ToListAsync();
                foreach (var post in posts)
                {
                    var uniqueFileName = await GetUniqueFileNameForPostMainImage(post.Id);
                    var MainImage = await GetPostMainImage(uniqueFileName);
                    //var SecondaryImagesUniqueFileNames = await GetPostSecondaryImagesUniqueFileNames(post.Id);
                    //var SecondaryImages = await GetPostSecondaryImages(SecondaryImagesUniqueFileNames);
                    post.MainImage = MainImage;
                }
                return posts;
            }
            catch
            {
                return Enumerable.Empty<Post>();
            }
        }
        public async Task<bool> UpdatePost(int postId, UpdatePostDto postDto)
        {
            try
            {
                var obj = await _context.Posts
                        .Include(p => p.Categories)
                        .Include(p => p.SecondaryImages)
                        .FirstOrDefaultAsync(p => p.Id == postId);
                if (obj == null)
                {
                    return false;
                }
                _context.Attach(obj);
                if(postDto.BasePrice is not null)
                {
                    obj.BasePrice = postDto.BasePrice;
                }
                if (postDto.MainImage is not null)
                {
                    string uniqueFileName = await UpdatePostMainImage(postId, postDto.MainImage);
                    if (uniqueFileName == "error")
                    {
                        return false;
                    }
                    obj.MainImage = uniqueFileName;
                }
                if(postDto.Title is not null)
                {
                    obj.Title = postDto.Title;
                }
                if(postDto.Description is not null)
                {
                    obj.Description = postDto.Description;
                }
                
                if(postDto.CategoriesIds is not null)
                {
                    var categories = new List<Category>();
                    foreach (var category in postDto.CategoriesIds)
                    {
                        categories.Add(await _context.Categories.FindAsync(category));
                    }
                    obj.Categories = categories;
                }

                await _context.SaveChangesAsync();
                return true;
            }
            catch 
            { 
                return false; 
            }

        }
        public async Task<List<Post>> GetPostsByCategoryId(int CategoryId)
        {
            var category = await _context.Categories.FindAsync(CategoryId);
            if (category == null)
            {
                return null;
            }
            var postsIds = await _context.PostCategories
                                         .Where(x=>x.CategoryId == CategoryId)
                                         .Select(x=>x.PostId)
                                         .ToListAsync();
            var posts = new List<Post>();
            foreach (var postid in postsIds)
            {
                posts.Add(await _context.Posts
                    .Where(x => x.Id == postid)
                    .Select(p => new Post
                    {
                        Id = p.Id,
                        Title = p.Title,
                        Description = p.Description,
                        BasePrice = p.BasePrice,
                        MainImage = p.MainImage,
                        FreelancerId = p.FreelancerId,
                        /*
                        Categories = p.Categories.Select(c => new Category
                        {
                            Id = c.Id,
                            Name = c.Name
                        }).ToList(),

                        Reviews = p.Reviews.Select(c => new Review
                        {
                            Id = c.Id,
                            PostId = c.PostId,
                            UserId = c.UserId,
                            Content = c.Content,
                            Rating = c.Rating
                        }).ToList(),
                        SecondaryImages = p.SecondaryImages.Select(c => new PostImage
                        {
                            Id = c.Id,
                            ImageUrl = c.ImageUrl,
                            PostId = c.PostId
                        }).ToList(),
                        PostFeatures = p.PostFeatures.Select(c => new PostFeature
                        {
                            Id = c.Id,
                            PostId = c.PostId,
                            Title = c.Title,
                            Price = c.Price
                        }).ToList(),
                        */
                    })
                    .FirstOrDefaultAsync());
            }
            foreach (var post in posts)
            {
                var uniqueFileName = await GetUniqueFileNameForPostMainImage(post.Id);
                var MainImage = await GetPostMainImage(uniqueFileName);
                //var SecondaryImagesUniqueFileNames = await GetPostSecondaryImagesUniqueFileNames(post.Id);
                //var SecondaryImages = await GetPostSecondaryImages(SecondaryImagesUniqueFileNames);
                post.MainImage = MainImage;
            }
            return posts;
        }
        public async Task<string> UpdatePostMainImage(int postId , IFormFile file)
        {
            try
            {
                string Directory = Path.Combine(_webHostEnvironment.WebRootPath, "images/post");
                var oldUniqueFileName = await GetUniqueFileNameForPostMainImage(postId);
                if (oldUniqueFileName == null)
                {
                    return "error";
                }
                var chkDelete = await DeletePostMainImage(postId);
                if (chkDelete == false)
                {
                    return "error";
                }
                var NewUniqueFileName = await SaveUploadedFile(file);
                if (NewUniqueFileName == "size")
                {
                    return "error";
                }
                var chk = await CreatePostMainImage(postId, NewUniqueFileName);
                if (chk == false)
                {
                    return "error";
                }
                return NewUniqueFileName;
            }
            catch
            {
                return "error";
            }
            
        }
        public async Task<List<PostImage>> GetPostImagesObj(int postId)
        {
            var post = await _context.PostsImages
                                         .Where(x => x.PostId == postId)
                                         .ToListAsync();
            
            const string _baseImageUrl = "https://localhost:7181/images/postSecondaryImages/";
            var postWithUrlImg = post;
            foreach (var item in postWithUrlImg)
            {
                item.ImageUrl = $"{_baseImageUrl}{item.ImageUrl}";
            }
            return postWithUrlImg;
        }

        
        // postimages functions
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
        public async Task<bool> Update(int id, PostImage postImage)
        {
            try
            {
                var obj = await _context.PostsImages.FindAsync(id);
                if (obj == null)
                {
                    return false;
                }
                _context.Attach(obj);
                obj.ImageUrl = postImage.ImageUrl;
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public async Task<Post> GetPostBySellerId(string userId)
        {
            var post = await _context.Posts.FirstOrDefaultAsync(x => x.FreelancerId == userId);
            return post;
        }

        public async Task<bool> AddPostCategory(List<PostCategory> postCategories)
        {
            try
            {
                await _context.PostCategories.AddRangeAsync(postCategories);
                _context.SaveChanges();
                return true;
            }
            catch 
            {
                return false;
            }
        }
    }
}
