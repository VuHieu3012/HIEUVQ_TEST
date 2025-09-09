using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using AuthModule.Models;
using AuthModule.DTOs;
using AuthModule.Services;
using AuthModule.Data;

namespace AuthModule.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IAuthService _authService;
    private readonly ApplicationDbContext _context;

    public HomeController(ILogger<HomeController> logger, IAuthService authService, ApplicationDbContext context)
    {
        _logger = logger;
        _authService = authService;
        _context = context;
    }

    public IActionResult Index()
    {
        return View();
    }

    [Route("Data")]
    public async Task<IActionResult> Data()
    {
        var users = await _context.Users.ToListAsync();
        return View(users);
    }

    [HttpGet("api/data/users")]
    public async Task<IActionResult> GetUsers()
    {
        try
        {
            var authHeader = Request.Headers["Authorization"].FirstOrDefault();
            if (authHeader?.StartsWith("Bearer ") != true)
                return Json(new { success = false, error = "Access denied" });

            var token = authHeader.Substring("Bearer ".Length).Trim();
            if (!_authService.IsAdmin(token))
                return Json(new { success = false, error = "Access denied" });

            var users = await _context.Users
                .Select(u => new { u.Id, u.Username, u.Email, u.UserType, u.CreatedAt, u.UpdatedAt })
                .ToListAsync();

            return Json(new { success = true, data = users });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, error = ex.Message });
        }
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
