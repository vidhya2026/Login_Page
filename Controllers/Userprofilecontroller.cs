using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserLogin.Data;           
using UserLogin.Models;
using UserLogin.Models.ViewModels;

namespace UserLogin.Controllers
{
  
    [Authorize]
    public class UserProfileController : Controller
    {
       
        private readonly AppDbContext _context;

      
        private readonly IWebHostEnvironment _env;

     
        public UserProfileController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

       
        // Fetches every row from the UserProfiles table and passes the list to the view.
        public IActionResult Index()
        {
      
            var users = _context.UserProfiles.ToList();
            return View(users);
        }

       
        public IActionResult Details(int id)
        {
            
            var user = _context.UserProfiles.Find(id);

            if (user == null) return RedirectToAction("Index");

            return View(user);
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View(new UserProfileViewModel());
        }


        
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(UserProfileViewModel model)
        {
    
            ValidateImage(model.ImageFile);

            if (!ModelState.IsValid)
            {   
                return View(model);
            }

         
            var imagePath = await SaveImage(model.ImageFile);

            // Map the ViewModel to the domain model that EF Core will persist.
       
            var user = new Userprofiles
            {
                Name = model.Name,
                Age = model.Age,
                Gender = model.Gender,
                Email = model.Email,
                Phone = model.Phone,
                ImagePath = imagePath
            };

        
            _context.UserProfiles.Add(user);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

  
        public IActionResult Edit(int id)
        {
            var user = _context.UserProfiles.Find(id);
            if (user == null) return RedirectToAction("Index");

            // Populate the ViewModel with existing data so the form shows current values.
            var model = new UserProfileViewModel
            {
                Id = user.Id,
                Name = user.Name,
                Age = user.Age,
                Gender = user.Gender,
                Email = user.Email,
                Phone = user.Phone,
                ExistingImagePath = user.ImagePath   
            };

            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UserProfileViewModel model)
        {
            // Only validate the image if the user actually selected a new file.
            if (model.ImageFile != null)
                ValidateImage(model.ImageFile);

            if (!ModelState.IsValid)
                return View(model);

            var user = _context.UserProfiles.Find(model.Id);
            if (user == null) return RedirectToAction("Index");

            // Update scalar fields from the posted ViewModel.
            user.Name = model.Name;
            user.Age = model.Age;
            user.Gender = model.Gender;
            user.Email = model.Email;
            user.Phone = model.Phone;

            // Only replace the image if a new file was uploaded; otherwise keep the old path.
            if (model.ImageFile != null)
                user.ImagePath = await SaveImage(model.ImageFile);

            // EF Core's change tracker already knows about this entity (we called Find()),
            // so we only need SaveChangesAsync() — no explicit Update() call is needed.
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        // ─── DELETE ───────────────────────────────────────────────────────────────
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var user = _context.UserProfiles.Find(id);
            if (user == null) return RedirectToAction("Index");

            // Optionally delete the physical file from wwwroot/uploads as well.
            DeleteImageFile(user.ImagePath);

            // Remove() marks the entity as EntityState.Deleted.
            // SaveChangesAsync() issues the DELETE SQL statement.
            _context.UserProfiles.Remove(user);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        // ─── PRIVATE HELPERS ─────────────────────────────────────────────────────


        private void ValidateImage(IFormFile? file)
        {
            if (file == null) return;  

            // 3 MB expressed in bytes: 3 * 1024 * 1024 = 3,145,728.
           
            const long maxBytes = 3L * 1024 * 1024;
            if (file.Length > maxBytes)
                ModelState.AddModelError("ImageFile", "Image must be 3 MB or smaller");

         
            var allowedTypes = new[] { "image/jpeg", "image/jpg", "image/png", "image/webp" };
            if (file.ContentType == "image/gif")
                ModelState.AddModelError("ImageFile", "GIF images are not allowed");
            else if (!allowedTypes.Contains(file.ContentType.ToLower()))
                ModelState.AddModelError("ImageFile", "Only JPEG, PNG, and WebP images are allowed");

            // Double-check with file extension because some browsers send incorrect MIME types.
            var ext = Path.GetExtension(file.FileName).ToLower();
            var allowedExts = new[] { ".jpg", ".jpeg", ".png", ".webp" };
            if (ext == ".gif")
                ModelState.AddModelError("ImageFile", "GIF files are not allowed");
            else if (!allowedExts.Contains(ext))
                ModelState.AddModelError("ImageFile", "File extension must be .jpg, .jpeg, .png, or .webp");
        }

        // SaveImage writes the file to wwwroot/uploads/ and returns the relative URL.
        // async/await is used because CopyToAsync does non-blocking I/O.
        private async Task<string?> SaveImage(IFormFile? file)
        {
            if (file == null || file.Length == 0) return null;

        
            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";

        
            var uploadDir = Path.Combine(_env.WebRootPath, "uploads");

            
            Directory.CreateDirectory(uploadDir);

            var filePath = Path.Combine(uploadDir, fileName);

           
            await using var stream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(stream);

            // Return a root-relative URL that <img src="..."> can use directly.
            return $"/uploads/{fileName}";
        }

        // DeleteImageFile removes the physical file from disk when a record is deleted.
        private void DeleteImageFile(string? imagePath)
        {
            if (string.IsNullOrEmpty(imagePath)) return;

            // Convert the URL-relative path (/uploads/xyz.jpg) to an absolute disk path.
            var fullPath = Path.Combine(_env.WebRootPath, imagePath.TrimStart('/'));

            // System.IO.File.Exists prevents a FileNotFoundException if the file was
            // already removed manually or by a previous failed request.
            if (System.IO.File.Exists(fullPath))
                System.IO.File.Delete(fullPath);
        }
    }
}