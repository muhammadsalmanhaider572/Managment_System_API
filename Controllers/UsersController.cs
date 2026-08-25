using Managment_System_API_Application.Interfaces;
using Managment_System_API_Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Managment_System_API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }


    // =====================================================
    // GET ALL USERS
    // GET: api/Users?pageNumber=1&pageSize=10&search=
    // =====================================================

    [HttpGet]
    public async Task<IActionResult> GetUsers(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string search = "")
    {
        var result =
            await _userService.GetUsersAsync(
                pageNumber,
                pageSize,
                search);

        return Ok(new
        {
            users = result.Users,
            totalRecords = result.TotalRecords,
            pageNumber = pageNumber,
            pageSize = pageSize,
            search = search
        });
    }


    // =====================================================
    // GET USER BY ID
    // GET: api/Users/5
    // =====================================================

    [Authorize]
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetUser(int id)
    {
        var user =
            await _userService.GetUserByIdAsync(id);

        if (user == null)
        {
            return NotFound(new
            {
                message = "User not found."
            });
        }

        return Ok(user);
    }


    // =====================================================
    // REGISTER USER
    // POST: api/Users
    // =====================================================

    [HttpPost]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> RegisterUser(
        [FromForm] User user,
        IFormFile? profileImage)
    {
        var result =
            await _userService.RegisterUserAsync(
                user,
                profileImage);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }


    // =====================================================
    // UPDATE USER
    // PUT: api/Users/5
    // =====================================================

    [HttpPut("{id:int}")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> UpdateUser(
        int id,
        [FromForm] User user,
        IFormFile? profileImage)
    {
        user.Id = id;

        var result =
            await _userService.UpdateUserAsync(
                user,
                profileImage);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }


    // =====================================================
    // DELETE USER
    // DELETE: api/Users/5
    // =====================================================

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        var result =
            await _userService.DeleteUserAsync(id);

        if (!result.Success)
        {
            return NotFound(result);
        }

        return Ok(result);
    }


    // =====================================================
    // EXPORT USERS
    // GET: api/Users/export
    // =====================================================

    [HttpGet("export")]
    public async Task<IActionResult> ExportUsers()
    {
        var file =
            await _userService.ExportUsersAsync();

        return File(
            file,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            "Users.xlsx");
    }
}