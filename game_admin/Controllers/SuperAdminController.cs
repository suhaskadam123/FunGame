using DataAccessLayer.Data;
using DataAccessLayer.Models;
using game_admin.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace game_admin.Controllers
{
    [Authorize(Roles = "superadmin")]
    public class SuperAdminController : Controller
    {
        private readonly AuthService _authService;
        private readonly ApplicationDbContext _context;

        public SuperAdminController(AuthService authService,ApplicationDbContext context)
        {
            _authService = authService;
            this._context = context;
        }
        //method for get superadmin dashboard page
        public async Task<IActionResult> Dashboard()
        {
            var users = await _authService.GetAllUsers();
          //  ViewBag.TotalUsers = users.Count;
            ViewBag.TotalSuperDistributors = await _context.Users.CountAsync(u => u.Role == "superDistributor" && u.DeleteStatus == 1);
            ViewBag.TotalDistributors = await _context.Users.CountAsync(u => u.Role == "Distributer" && u.DeleteStatus == 1);
            ViewBag.TotalRetailers = await _context.Users.CountAsync(u => u.Role == "Retailer" && u.DeleteStatus == 1);
            ViewBag.TotalUsers = await _context.Users.CountAsync(u => u.Role == "User" && u.DeleteStatus == 1);
            return View("~/Views/SuperAdmin/Dashboard.cshtml", users);
        }
        //method for get AllSuperDistributer page

        [HttpGet]
        public async Task<IActionResult> AllSuperDistributer()
        {
            var superDistributors = await _authService.GetAllSuperDistributors();
            return View("~/Views/SuperAdmin/AllSuperDistributer.cshtml", superDistributors);
        }
        [HttpGet]
        public IActionResult CreateSuperDistributer()
        {
            return View("~/Views/SuperAdmin/CreateSuperDistributer.cshtml");
        }
        [HttpPost]
        public async Task<IActionResult> CreateSuperDistributer(User model)
        {
            //if (!ModelState.IsValid)
            //{
            //    return View("~/Views/SuperAdmin/CreateSuperDistributer.cshtml", model);
            //}

            // Await the Task<bool> to get the actual boolean value
            var currentUser = await _authService.GetUserByUsernameAsync(User.Identity.Name);
            var exists = await _authService.UsernameExists(model.Username);
            if (exists)
            {
                ModelState.AddModelError("Username", "Username already exists.");
                return View("~/Views/SuperAdmin/CreateSuperDistributer.cshtml", model);
            }

            // Validate commission
            if (model.Percentage < 0 || model.Percentage > 100)
            {
                ModelState.AddModelError("Percentage", "Commission should be between 0 and 100.");
                return View("~/Views/SuperAdmin/CreateSuperDistributer.cshtml", model);
            }
           
            // Generate unique 6-digit ID
            model.UniqueId = await _authService.GenerateUniqueUserIdAsync();

            model.Role = "superDistributor";
            model.ReferName = "superuser";
            model.DateTime = DateTime.Now;
            model.SuperDistributerId = 0;
            model.DistributerId = 0;
            model.RetailerId = 0;
            model.DeleteStatus = 1;
            model.Balance = 0;
            model.ReferId = currentUser.UserId;            // 👈 Store who created the user

            await _authService.CreateUserAsync(model);

            TempData["Success"] = "Super Distributer added successfully.";
            return RedirectToAction("AllSuperDistributer");
        }
       
        // GET: Show Edit Form
        [HttpGet]
        public async Task<IActionResult> EditSuperDistributer(int id)
        {
            var user = await _authService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return View("~/Views/SuperAdmin/EditSuperDistributer.cshtml", user);
        }

        [HttpPost]
        public async Task<IActionResult> EditSuperDistributer(User model)
        {
            //if (!ModelState.IsValid)
            //{
            //    return View("~/Views/SuperAdmin/EditSuperDistributer.cshtml", model);
            //}

            var existingUser = await _authService.GetUserByIdAsync(model.UserId);
            if (existingUser == null)
            {
                return NotFound();
            }

            // Update only necessary fields
            existingUser.Username = model.Username;
            existingUser.Password = model.Password;
            existingUser.Percentage = model.Percentage;
            existingUser.Note = model.Note;
            existingUser.Status = model.Status;
            //existingUser.ReferName = model.ReferName;

            await _authService.UpdateUserAsync(existingUser);

            TempData["Success"] = "Super Distributer updated successfully.";
            return RedirectToAction("AllSuperDistributer");
        }
        [HttpGet]
        public async Task<IActionResult> TransferCreditToSuperDistributer(int id)
        {
            var user = await _authService.GetUserByIdAsync(id);
            if(user == null)
            {
                return NotFound();
            }
            return View("~/Views/SuperAdmin/TransferCreditToSuperDistributer.cshtml", user);
        }

        [HttpPost]
        public async Task<IActionResult> CreditToSuperDistributer(int TargetUserId, int Amount, string Password)
        {
            var superadminUsername = User.Identity?.Name;
            var superadmin = await _authService.GetUserByUsernameAsync(superadminUsername);

            if (superadmin == null || superadmin.Role != "superadmin")
                return Unauthorized();

            if (superadmin.Password != Password)
            {
                TempData["Error"] = "Incorrect password.";
                return RedirectToAction("TransferCreditToSuperDistributer", new { id = TargetUserId });
            }

            var targetUser = await _authService.GetUserByIdAsync(TargetUserId);
            if (targetUser == null || targetUser.Role != "superDistributor")
            {
                TempData["Error"] = "SuperDistributer not found.";
                return RedirectToAction("TransferCreditToSuperDistributer");
            }

            if (Amount <= 0 || superadmin.Balance < Amount)
            {
                TempData["Error"] = "Insufficient superadmin balance or invalid amount.";
                return RedirectToAction("TransferCreditToSuperDistributer", new { id = TargetUserId });
            }

            superadmin.Balance -= Amount;
            targetUser.Balance += Amount;

            await _authService.UpdateUserAsync(superadmin);
            await _authService.UpdateUserAsync(targetUser);

            TempData["Success"] = $"₹{Amount} credited to {targetUser.Username}.";
            return RedirectToAction("AllSuperDistributer", new { id = TargetUserId });
        }

        [HttpGet]
        public async Task<IActionResult> WithdrawCreditFromSuperDistributer(int id)
        {
            var user = await _authService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return View("~/Views/SuperAdmin/WithdrawCreditFromSuperDistributer.cshtml", user);
        }
        [HttpPost]
        public async Task<IActionResult> WithdrawFromSuperDistributer(int TargetUserId, int Amount, string Password)
        {
            var superadminUsername = User.Identity?.Name;
            var superadmin = await _authService.GetUserByUsernameAsync(superadminUsername);

            if (superadmin == null || superadmin.Role != "superadmin")
                return Unauthorized();

            if (superadmin.Password != Password)
            {
                TempData["Error"] = "Incorrect password.";
                return RedirectToAction("WithdrawCreditFromSuperDistributer", new { id = TargetUserId });
            }

            var targetUser = await _authService.GetUserByIdAsync(TargetUserId);
            if (targetUser == null || targetUser.Role != "superDistributor")
            {
                TempData["Error"] = "SuperDistributer not found.";
                return RedirectToAction("WithdrawCreditFromSuperDistributer");
            }

            if (Amount <= 0 || targetUser.Balance < Amount)
            {
                TempData["Error"] = "Insufficient distributer balance or invalid amount.";
                return RedirectToAction("WithdrawCreditFromSuperDistributer", new { id = TargetUserId });
            }

            targetUser.Balance -= Amount;
            superadmin.Balance += Amount;

            await _authService.UpdateUserAsync(superadmin);
            await _authService.UpdateUserAsync(targetUser);

            TempData["Success"] = $"₹{Amount} withdrawn from {targetUser.Username}.";
            return RedirectToAction("AllSuperDistributer", new { id = TargetUserId });
        }

        [HttpPost]
        public async Task<IActionResult> BanSuperDistributer(int id)
        {
            var user = await _authService.GetUserByIdAsync(id);
            if (user == null)
                return NotFound();

            user.Status = 0; // Or any logic you want to define "banned"
            await _authService.UpdateUserAsync(user);

            TempData["Success"] = "User banned successfully.";
            return RedirectToAction("AllSuperDistributer");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteSuperDistributer(int id)
        {
            var user = await _authService.GetUserByIdAsync(id);
            if (user == null)
                return NotFound();

            await _authService.DeleteUserAsync(user);
            TempData["Success"] = "User permanently deleted.";
            return RedirectToAction("AllSuperDistributer");
        }



        //end super distributer 

        //start distributer

        [HttpGet]
        public async Task<IActionResult> AllDistributer()
        {
            var Distributors = await _authService.GetAllDistributors();
            return View("~/Views/SuperAdmin/AllDistributer.cshtml", Distributors);

        }
        [HttpGet]
        public IActionResult CreateDistributer()
        {
            return View("~/Views/SuperAdmin/CreateDistributer.cshtml");
        }
        [HttpPost]
        public async Task<IActionResult> CreateDistributer(User model)
        {
            //if (!ModelState.IsValid)
            //{
            //    return View("~/Views/SuperAdmin/CreateSuperDistributer.cshtml", model);
            //}

            // Await the Task<bool> to get the actual boolean value
            var currentUser = await _authService.GetUserByUsernameAsync(User.Identity.Name);
            var exists = await _authService.UsernameExists(model.Username);
            if (exists)
            {
                ModelState.AddModelError("Username", "Username already exists.");
                return View("~/Views/SuperAdmin/CreateDistributer.cshtml", model);
            }

            // Validate commission
            if (model.Percentage < 0 || model.Percentage > 100)
            {
                ModelState.AddModelError("Percentage", "Commission should be between 0 and 100.");
                return View("~/Views/SuperAdmin/CreateDistributer.cshtml", model);
            }

            // Generate unique 6-digit ID
            model.UniqueId = await _authService.GenerateUniqueUserIdAsync();

            model.Role = "Distributer";
            model.ReferName = "superuser";
            model.DateTime = DateTime.Now;
            model.SuperDistributerId = 0;
            model.DistributerId = 0;
            model.RetailerId = 0;
            model.DeleteStatus = 1;
            model.Balance = 0;
            model.ReferId = currentUser.UserId;

            await _authService.CreateUserAsync(model);

            TempData["Success"] = " Distributer added successfully.";
            return RedirectToAction("AllDistributer");
        }

        // GET: Show Edit Form
        [HttpGet]
        public async Task<IActionResult> EditDistributer(int id)
        {
            var user = await _authService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return View("~/Views/SuperAdmin/EditDistributer.cshtml", user);
        }
        [HttpPost]
        public async Task<IActionResult> EditDistributer(User model)
        {
            //if (!ModelState.IsValid)
            //{
            //    return View("~/Views/SuperAdmin/EditSuperDistributer.cshtml", model);
            //}

            var existingUser = await _authService.GetUserByIdAsync(model.UserId);
            if (existingUser == null)
            {
                return NotFound();
            }

            // Update only necessary fields
            existingUser.Username = model.Username;
            existingUser.Password = model.Password;
            existingUser.Percentage = model.Percentage;
            existingUser.Note = model.Note;
            existingUser.Status = model.Status;
            //existingUser.ReferName = model.ReferName;

            await _authService.UpdateUserAsync(existingUser);

            TempData["Success"] = " Distributer updated successfully.";
            return RedirectToAction("AllDistributer");
        }
        [HttpGet]
        public async Task<IActionResult> TransferCreditToDistributer(int id)
        {
            var user = await _authService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return View("~/Views/SuperAdmin/TransferCreditToDistributer.cshtml", user);
        }
        [HttpPost]
        public async Task<IActionResult> TransferCreditToDistributer(int TargetUserId, int Amount, string Password)
        {
            var superadminUsername = User.Identity?.Name;
            var superadmin = await _authService.GetUserByUsernameAsync(superadminUsername);

            if (superadmin == null || superadmin.Role != "superadmin")
                return Unauthorized();

            if (superadmin.Password != Password)
            {
                TempData["Error"] = "Incorrect password.";
                return RedirectToAction("TransferCreditToDistributer", new { id = TargetUserId });
            }

            var targetUser = await _authService.GetUserByIdAsync(TargetUserId);
            if (targetUser == null || targetUser.Role != "Distributer")
            {
                TempData["Error"] = "Distributer not found.";
                return RedirectToAction("TransferCreditToDistributer");
            }

            if (Amount <= 0 || superadmin.Balance < Amount)
            {
                TempData["Error"] = "Insufficient superadmin balance or invalid amount.";
                return RedirectToAction("TransferCreditToDistributer", new { id = TargetUserId });
            }

            superadmin.Balance -= Amount;
            targetUser.Balance += Amount;

            await _authService.UpdateUserAsync(superadmin);
            await _authService.UpdateUserAsync(targetUser);

            TempData["Success"] = $"₹{Amount} credited to {targetUser.Username}.";
            return RedirectToAction("AllDistributer", new { id = TargetUserId });
        }
        [HttpGet]
        public async Task<IActionResult> WithdrawCreditFromDistributer(int id)
        {
            var user = await _authService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return View("~/Views/SuperAdmin/WithdrawCreditFromDistributer.cshtml", user);
        }
        [HttpPost]
        public async Task<IActionResult> WithdrawCreditFromDistributer(int TargetUserId, int Amount, string Password)
        {
            var superadminUsername = User.Identity?.Name;
            var superadmin = await _authService.GetUserByUsernameAsync(superadminUsername);

            if (superadmin == null || superadmin.Role != "superadmin")
                return Unauthorized();

            if (superadmin.Password != Password)
            {
                TempData["Error"] = "Incorrect password.";
                return RedirectToAction("WithdrawCreditFromDistributer", new { id = TargetUserId });
            }

            var targetUser = await _authService.GetUserByIdAsync(TargetUserId);
            if (targetUser == null || targetUser.Role != "Distributer")
            {
                TempData["Error"] = "SuperDistributer not found.";
                return RedirectToAction("WithdrawCreditFromDistributer");
            }

            if (Amount <= 0 || targetUser.Balance < Amount)
            {
                TempData["Error"] = "Insufficient distributer balance or invalid amount.";
                return RedirectToAction("WithdrawCreditFromDistributer", new { id = TargetUserId });
            }

            targetUser.Balance -= Amount;
            superadmin.Balance += Amount;

            await _authService.UpdateUserAsync(superadmin);
            await _authService.UpdateUserAsync(targetUser);

            TempData["Success"] = $"₹{Amount} withdrawn from {targetUser.Username}.";
            return RedirectToAction("AllDistributer", new { id = TargetUserId });
        }

        [HttpPost]
        public async Task<IActionResult> BanDistributer(int id)
        {
            var user = await _authService.GetUserByIdAsync(id);
            if (user == null)
                return NotFound();

            user.Status = 0; // Or any logic you want to define "banned"
            await _authService.UpdateUserAsync(user);

            TempData["Success"] = "User banned successfully.";
            return RedirectToAction("AllSuperDistributer");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteDistributer(int id)
        {
            var user = await _authService.GetUserByIdAsync(id);
            if (user == null)
                return NotFound();

            await _authService.DeleteUserAsync(user);
            TempData["Success"] = "User permanently deleted.";
            return RedirectToAction("AllSuperDistributer");
        }
    



    //end  distributer 

    //start retailer


     [HttpGet]
        public async Task<IActionResult> AllRetailer()
        {
            var Retailer = await _authService.GetAllRetailer();
            return View("~/Views/SuperAdmin/AllRetailer.cshtml", Retailer);

        }
        [HttpGet]
        public IActionResult CreateRetailer()
        {
            return View("~/Views/SuperAdmin/CreateRetailer.cshtml");
        }
        [HttpPost]
        public async Task<IActionResult> CreateRetailer(User model)
        {
            //if (!ModelState.IsValid)
            //{
            //    return View("~/Views/SuperAdmin/CreateSuperDistributer.cshtml", model);
            //}

            // Await the Task<bool> to get the actual boolean value
            var currentUser = await _authService.GetUserByUsernameAsync(User.Identity.Name);
            var exists = await _authService.UsernameExists(model.Username);
            if (exists)
            {
                ModelState.AddModelError("Username", "Username already exists.");
                return View("~/Views/SuperAdmin/CreateRetailer.cshtml", model);
            }

            // Validate commission
            if (model.Percentage < 0 || model.Percentage > 100)
            {
                ModelState.AddModelError("Percentage", "Commission should be between 0 and 100.");
                return View("~/Views/SuperAdmin/CreateRetailer.cshtml", model);
            }

            // Generate unique 6-digit ID
            model.UniqueId = await _authService.GenerateUniqueUserIdAsync();

            model.Role = "Retailer";
            model.ReferName = "superuser";
            model.DateTime = DateTime.Now;
            model.SuperDistributerId = 0;
            model.DistributerId = 0;
            model.RetailerId = 0;
            model.DeleteStatus = 1;
            model.Balance = 0;
            model.ReferId = currentUser.UserId;

            await _authService.CreateUserAsync(model);

            TempData["Success"] = " Distributer added successfully.";
            return RedirectToAction("AllRetailer");
        }

        // GET: Show Edit Form
        [HttpGet]
        public async Task<IActionResult> EditRetailer(int id)
        {
            var user = await _authService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return View("~/Views/SuperAdmin/EditRetailer.cshtml", user);
        }
        [HttpPost]
        public async Task<IActionResult> EditRetailer(User model)
        {
            //if (!ModelState.IsValid)
            //{
            //    return View("~/Views/SuperAdmin/EditSuperDistributer.cshtml", model);
            //}

            var existingUser = await _authService.GetUserByIdAsync(model.UserId);
            if (existingUser == null)
            {
                return NotFound();
            }

            // Update only necessary fields
            existingUser.Username = model.Username;
            existingUser.Password = model.Password;
            existingUser.Percentage = model.Percentage;
            existingUser.Note = model.Note;
            existingUser.Status = model.Status;
            //existingUser.ReferName = model.ReferName;

            await _authService.UpdateUserAsync(existingUser);

            TempData["Success"] = " Retailer updated successfully.";
            return RedirectToAction("AllRetailer");
        }
        [HttpGet]
        public async Task<IActionResult> TransferCreditToRetailer(int id)
        {
            var user = await _authService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return View("~/Views/SuperAdmin/TransferCreditToRetailer.cshtml", user);
        }
        [HttpPost]
        public async Task<IActionResult> TransferCreditToRetailer(int TargetUserId, int Amount, string Password)
        {
            var superadminUsername = User.Identity?.Name;
            var superadmin = await _authService.GetUserByUsernameAsync(superadminUsername);

            if (superadmin == null || superadmin.Role != "superadmin")
                return Unauthorized();

            if (superadmin.Password != Password)
            {
                TempData["Error"] = "Incorrect password.";
                return RedirectToAction("TransferCreditToRetailer", new { id = TargetUserId });
            }

            var targetUser = await _authService.GetUserByIdAsync(TargetUserId);
            if (targetUser == null || targetUser.Role != "Retailer")
            {
                TempData["Error"] = "Distributer not found.";
                return RedirectToAction("TransferCreditToRetailer");
            }

            if (Amount <= 0 || superadmin.Balance < Amount)
            {
                TempData["Error"] = "Insufficient superadmin balance or invalid amount.";
                return RedirectToAction("TransferCreditToRetailer", new { id = TargetUserId });
            }

            superadmin.Balance -= Amount;
            targetUser.Balance += Amount;

            await _authService.UpdateUserAsync(superadmin);
            await _authService.UpdateUserAsync(targetUser);

            TempData["Success"] = $"₹{Amount} credited to {targetUser.Username}.";
            return RedirectToAction("AllRetailer", new { id = TargetUserId });
        }
        [HttpGet]
        public async Task<IActionResult> WithdrawCreditFromRetailer(int id)
        {
            var user = await _authService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return View("~/Views/SuperAdmin/WithdrawCreditFromRetailer.cshtml", user);
        }
        [HttpPost]
        public async Task<IActionResult> WithdrawCreditFromRetailer(int TargetUserId, int Amount, string Password)
        {
            var superadminUsername = User.Identity?.Name;
            var superadmin = await _authService.GetUserByUsernameAsync(superadminUsername);

            if (superadmin == null || superadmin.Role != "superadmin")
                return Unauthorized();

            if (superadmin.Password != Password)
            {
                TempData["Error"] = "Incorrect password.";
                return RedirectToAction("WithdrawCreditFromRetailer", new { id = TargetUserId });
            }

            var targetUser = await _authService.GetUserByIdAsync(TargetUserId);
            if (targetUser == null || targetUser.Role != "Retailer")
            {
                TempData["Error"] = "SuperDistributer not found.";
                return RedirectToAction("WithdrawCreditFromRetailer");
            }

            if (Amount <= 0 || targetUser.Balance < Amount)
            {
                TempData["Error"] = "Insufficient distributer balance or invalid amount.";
                return RedirectToAction("WithdrawCreditFromRetailer", new { id = TargetUserId });
            }

            targetUser.Balance -= Amount;
            superadmin.Balance += Amount;

            await _authService.UpdateUserAsync(superadmin);
            await _authService.UpdateUserAsync(targetUser);

            TempData["Success"] = $"₹{Amount} withdrawn from {targetUser.Username}.";
            return RedirectToAction("AllRetailer", new { id = TargetUserId });
        }

        [HttpPost]
        public async Task<IActionResult> BanRetailer(int id)
        {
            var user = await _authService.GetUserByIdAsync(id);
            if (user == null)
                return NotFound();

            user.Status = 0; // Or any logic you want to define "banned"
            await _authService.UpdateUserAsync(user);

            TempData["Success"] = "User banned successfully.";
            return RedirectToAction("AllRetailer");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteRetailer(int id)
        {
            var user = await _authService.GetUserByIdAsync(id);
            if (user == null)
                return NotFound();

            await _authService.DeleteUserAsync(user);
            TempData["Success"] = "User permanently deleted.";
            return RedirectToAction("AllRetailer");
        }


        //end  retailer 

        //start User perations


        [HttpGet]
        public async Task<IActionResult> AllUser()
        {
            var Retailer = await _authService.GetAllUser();
            return View("~/Views/SuperAdmin/AllUser.cshtml", Retailer);

        }
        [HttpGet]
        public IActionResult CreateUser()
        {
            return View("~/Views/SuperAdmin/CreateUser.cshtml");
        }
        [HttpPost]
        public async Task<IActionResult> CreateUser(User model)
        {
            //if (!ModelState.IsValid)
            //{
            //    return View("~/Views/SuperAdmin/CreateSuperDistributer.cshtml", model);
            //}

            // Await the Task<bool> to get the actual boolean value
            var currentUser = await _authService.GetUserByUsernameAsync(User.Identity.Name);
            var exists = await _authService.UsernameExists(model.Username);
            if (exists)
            {
                ModelState.AddModelError("Username", "Username already exists.");
                return View("~/Views/SuperAdmin/CreateUser.cshtml", model);
            }

            // Validate commission
            if (model.Percentage < 0 || model.Percentage > 100)
            {
                ModelState.AddModelError("Percentage", "Commission should be between 0 and 100.");
                return View("~/Views/SuperAdmin/CreateUser.cshtml", model);
            }

            // Generate unique 6-digit ID
            model.UniqueId = await _authService.GenerateUniqueUserIdAsync();

            model.Role = "User";
            model.ReferName = "superuser";
            model.DateTime = DateTime.Now;
            model.SuperDistributerId = 0;
            model.DistributerId = 0;
            model.RetailerId = 0;
            model.DeleteStatus = 1;
            model.Balance = 0;
            model.ReferId = currentUser.UserId;

            await _authService.CreateUserAsync(model);

            TempData["Success"] = " User added successfully.";
            return RedirectToAction("AllUser");
        }

        // GET: Show Edit Form
        [HttpGet]
        public async Task<IActionResult> EditUser(int id)
        {
            var user = await _authService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return View("~/Views/SuperAdmin/EditUser.cshtml", user);
        }
        [HttpPost]
        public async Task<IActionResult> EditUser(User model)
        {
            //if (!ModelState.IsValid)
            //{
            //    return View("~/Views/SuperAdmin/EditSuperDistributer.cshtml", model);
            //}

            var existingUser = await _authService.GetUserByIdAsync(model.UserId);
            if (existingUser == null)
            {
                return NotFound();
            }

            // Update only necessary fields
            existingUser.Username = model.Username;
            existingUser.Password = model.Password;
            existingUser.Percentage = model.Percentage;
            existingUser.Note = model.Note;
            existingUser.Status = model.Status;
            //existingUser.ReferName = model.ReferName;

            await _authService.UpdateUserAsync(existingUser);

            TempData["Success"] = " User updated successfully.";
            return RedirectToAction("AllUser");
        }
        [HttpGet]
        public async Task<IActionResult> TransferCreditToUser(int id)
        {
            var user = await _authService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return View("~/Views/SuperAdmin/TransferCreditToUser.cshtml", user);
        }
        [HttpPost]
        public async Task<IActionResult> TransferCreditToUser(int TargetUserId, int Amount, string Password)
        {
            var superadminUsername = User.Identity?.Name;
            var superadmin = await _authService.GetUserByUsernameAsync(superadminUsername);

            if (superadmin == null || superadmin.Role != "superadmin")
                return Unauthorized();

            if (superadmin.Password != Password)
            {
                TempData["Error"] = "Incorrect password.";
                return RedirectToAction("TransferCreditToUser", new { id = TargetUserId });
            }

            var targetUser = await _authService.GetUserByIdAsync(TargetUserId);
            if (targetUser == null || targetUser.Role != "User")
            {
                TempData["Error"] = "Distributer not found.";
                return RedirectToAction("TransferCreditToUser");
            }

            if (Amount <= 0 || superadmin.Balance < Amount)
            {
                TempData["Error"] = "Insufficient superadmin balance or invalid amount.";
                return RedirectToAction("TransferCreditToUser", new { id = TargetUserId });
            }

            superadmin.Balance -= Amount;
            targetUser.Balance += Amount;

            await _authService.UpdateUserAsync(superadmin);
            await _authService.UpdateUserAsync(targetUser);

            TempData["Success"] = $"₹{Amount} credited to {targetUser.Username}.";
            return RedirectToAction("AllUser", new { id = TargetUserId });
        }
        [HttpGet]
        public async Task<IActionResult> WithdrawCreditFromUser(int id)
        {
            var user = await _authService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return View("~/Views/SuperAdmin/WithdrawCreditFromUser.cshtml", user);
        }
        [HttpPost]
        public async Task<IActionResult> WithdrawCreditFromUser(int TargetUserId, int Amount, string Password)
        {
            var superadminUsername = User.Identity?.Name;
            var superadmin = await _authService.GetUserByUsernameAsync(superadminUsername);

            if (superadmin == null || superadmin.Role != "superadmin")
                return Unauthorized();

            if (superadmin.Password != Password)
            {
                TempData["Error"] = "Incorrect password.";
                return RedirectToAction("WithdrawCreditFromUser", new { id = TargetUserId });
            }

            var targetUser = await _authService.GetUserByIdAsync(TargetUserId);
            if (targetUser == null || targetUser.Role != "User")
            {
                TempData["Error"] = "SuperDistributer not found.";
                return RedirectToAction("WithdrawCreditFromUser");
            }

            if (Amount <= 0 || targetUser.Balance < Amount)
            {
                TempData["Error"] = "Insufficient distributer balance or invalid amount.";
                return RedirectToAction("WithdrawCreditFromUser", new { id = TargetUserId });
            }

            targetUser.Balance -= Amount;
            superadmin.Balance += Amount;

            await _authService.UpdateUserAsync(superadmin);
            await _authService.UpdateUserAsync(targetUser);

            TempData["Success"] = $"₹{Amount} withdrawn from {targetUser.Username}.";
            return RedirectToAction("AllUser", new { id = TargetUserId });
        }

        [HttpPost]
        public async Task<IActionResult> BanUser(int id)
        {
            var user = await _authService.GetUserByIdAsync(id);
            if (user == null)
                return NotFound();

            user.Status = 0; // Or any logic you want to define "banned"
            await _authService.UpdateUserAsync(user);

            TempData["Success"] = "User banned successfully.";
            return RedirectToAction("AllUser");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _authService.GetUserByIdAsync(id);
            if (user == null)
                return NotFound();

            await _authService.DeleteUserAsync(user);
            TempData["Success"] = "User permanently deleted.";
            return RedirectToAction("AllUser");
        }
    }


}
