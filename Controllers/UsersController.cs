

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuizApplicationMVC.Data;
using QuizApplicationMVC.Models;
using Microsoft.AspNetCore.Http;
using QuizApplicationMVC.Services;


namespace QuizApplicationMVC.Controllers
{
    public class UsersController : Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly IQuizRepository _quizRepository;
        private readonly ApplicationDBContext _context;

        public UsersController(IUserRepository userRepository, IQuizRepository quizRepository, ApplicationDBContext context)
        {
            _userRepository = userRepository;
            _quizRepository = quizRepository;
            _context = context;
        }


        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home", null);
        }

        // GET: Users
        public async Task<IActionResult> Index()
        {
            if (HttpContext.Session.GetInt32("Id") == null)
            {
                return RedirectToAction("Login", "Users");
            }

            if (TempData.ContainsKey("QuizEvaluated") && (bool)TempData["QuizEvaluated"])
            {
                TempData.Remove("QuizEvaluated"); // Clear TempData
            }
            try
            {
                var users = await _userRepository.GetAllUsersAsync();
                return users != null ? View(users) : Problem("Entity set 'ApplicationDBContext.Users' is null.");
            }
            catch (Exception ex)
            {
                // Log the exception (ex) if needed
                return Problem("An error occurred while fetching users: " + ex.Message);
            }
        }

        // GET: Users/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _userRepository.GetUserByIdAsync((int)id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }




        // GET: Users/Create
        public IActionResult Create()
        {
            return View();
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login([Bind("Email,Password")] Users users)
        {
            Console.WriteLine("Successfully Logged In");

            var userData = await _userRepository.AuthenticateUserAsync(users.Email, users.Password);

            if (userData == null)
            {
                ModelState.AddModelError("", "User not found. Please check your email and password.");
                return View();
            }

            HttpContext.Session.SetInt32("Id", userData.Id);
            Console.WriteLine(HttpContext.Session.GetInt32("Id"));

            return RedirectToAction("Index", "Home", null);
        }




        // POST: Users/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Email,Password")] Users users)
        {
            if (ModelState.IsValid)
            {
                await _userRepository.AddUserAsync(users);
                return RedirectToAction("Login", "Users");
            }
            return View(users);
        }



        public async Task<IActionResult> QuizHistory()
        {
            if (HttpContext.Session.GetInt32("Id") == null)
            {
                return RedirectToAction("Login", "Users");
            }

            var userId = HttpContext.Session.GetInt32("Id");

            var quizHistory = await _quizRepository.GetQuizHistoryByUserIdAsync(userId.Value);

            quizHistory.Reverse(); // Reverse the list

            return View(quizHistory);
        }

        // GET: Users/Edit/5

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var users = await _userRepository.GetUserByIdAsync(id.Value);
            if (users == null)
            {
                return NotFound();
            }

            return View(users);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Email,Password")] Users users)
        {
            if (id != users.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _userRepository.UpdateUserAsync(users);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_userRepository.UserExists(users.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                // Corrected RedirectToAction syntax
                return RedirectToAction(nameof(Details), new { id = users.Id });
            }
            return View(users);
        }


        // GET: Users/Delete/5

        public async Task<IActionResult> Delete(int id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var users = await _userRepository.GetUserByIdAsync(id);
            if (users == null)
            {
                return NotFound();
            }

            return View(users);
        }




        // POST: Users/Delete/5
        
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // Retrieve the user and their related quizzes
            var user = await _userRepository.GetUserByIdAsync(id);
            if (user == null)
            {
                return Problem("Entity set 'ApplicationDBContext.Users' is null.");
            }

            // Delete related quizzes along with their questions and user history
            var quizzes = await _quizRepository.GetQuizzesByUserIdAsync(id);
            foreach (var quiz in quizzes)
            {
                // Delete the quiz along with its questions and user history
                await _quizRepository.DeleteQuizWithHistoryAsync(quiz.Id);
            }

            // Delete user history entries associated with the user
            var userHistories = await _context.QuizUserHistory
                         .Where(uh => uh.UserId == id)
                         .ToListAsync();
            if (userHistories != null)
            {
                _context.QuizUserHistory.RemoveRange(userHistories);
                await _context.SaveChangesAsync();
            }

            // Delete the user
            await _userRepository.DeleteUserAsync(user.Id);

            HttpContext.Session.Clear(); 

            // Redirect to the login page after deletion
            return RedirectToAction("Login", "Users"); 
        }


    }
}