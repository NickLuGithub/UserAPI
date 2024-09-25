using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using UserAPI.Models;
using UserAPI.ViewModel;

namespace UserAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : Controller
    {
        // db
        private readonly TestContext _context;

        public UserController(TestContext context)
        {
            _context = context;
        }

        // 建立使用者資料
        [HttpPost(Name = "UserCreate")]
        public async Task<IActionResult> CreateUser([FromBody] UserViewModel user)
        {
            if (user == null)
            {
                return BadRequest("User is null.");
            }

            // 在這裡可以添加檢查 Email 的邏輯，例如防止重複註冊
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == user.Email);
            if (existingUser != null)
            {
                return Conflict("Email already in use.");
            }

            // 檢查 Email 格式是否正確
            if (!Regex.IsMatch(user.Email, @"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$"))
            {
                return BadRequest("Invalid email format.");
            }

            // 長度不少於6，數字和英文字母混合，至少有一個數字和一個英文字母
            if (!Regex.IsMatch(user.Password, @"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d]{6,}$"))
            {
                return BadRequest("Password must contain at least one letter and one number.");
            }


            // 確認沒有問題，建立 User 物件
            User db_user = new User
            {
                Email = user.Email,
                Password =user.Password,   
                Name = user.Name,
                Age = user.Age,
                Sex = user.sex,
                Area = user.Area
    };


            _context.Users.Add(db_user);
            await _context.SaveChangesAsync();

            return View();
        }

        [HttpGet("Search")]
        public async Task<IActionResult> SearchUser(
            string? name = null,
            int? minAge = null,
            int? maxAge = null,
            string? sex = null,
            int pageNumber = 1,
            int pageSize = 10
        )
        {
            var query = _context.Users.AsQueryable();

            if ( !string.IsNullOrEmpty(name)) {
                query = query.Where(u => u.Name == name);
            }

            if ( minAge.HasValue && maxAge.HasValue) {
                query = query.Where(u => u.Age >= minAge && u.Age <= maxAge);
            } else if ( minAge.HasValue) {
                query = query.Where(u => u.Age >= minAge);
            } else if ( maxAge.HasValue) {
                query = query.Where(u => u.Age <= maxAge);
            }

            // 查詢年齡範圍
            if (minAge.HasValue)
            {
                query = query.Where(u => u.Age >= minAge.Value);
            }
            if (maxAge.HasValue)
            {
                query = query.Where(u => u.Age <= maxAge.Value);
            }

            // 性別篩選
            if (!string.IsNullOrEmpty(sex))
            {
                query = query.Where(u => u.Sex == sex);
            }

            // 計算總記錄數
            var totalRecords = await query.CountAsync();

            // 分頁處理
            var users = await query
                .OrderBy(u => u.Id)  // 可以根據具體需求進行排序
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return Ok(new
            {
                TotalRecords = totalRecords,
                PageNumber = pageNumber,
                PageSize = pageSize,
                Users = users
            });
        }
        // GET: api/Users/Statistics
        [HttpGet("Statistics")]
        public async Task<IActionResult> GetUserStatistics()
        {
            // 匯總查詢城市及性別統計
            var statistics = await _context.Users
                .GroupBy(u => new { u.Area, u.Sex })
                .Select(g => new
                {
                    City = g.Key.Area,
                    Gender = g.Key.Sex,
                    UserCount = g.Count()
                })
                .OrderBy(result => result.City)  // 可按城市排序
                .ThenBy(result => result.Gender)  // 可按性別排序
                .ToListAsync();

            return Ok(statistics);
        }
    }


}
