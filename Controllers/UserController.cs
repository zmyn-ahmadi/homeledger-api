using HomeLedger.API.Dtos.User;
using HomeLedger.API.Models;
using HomeLedger.API.Services.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HomeLedger.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Consumes("application/json")]
public class UserController(IUserServices userServices) : ControllerBase
{
  private readonly IUserServices _userServices = userServices;

  [Authorize]
  [HttpGet("search")]
  public ActionResult<IEnumerable<User>> SearchUser([FromQuery] string name)
  {
    var query = _userServices.GetUserQueryAble();
    if (!string.IsNullOrEmpty(name))
    {
      // this is 'Where' is composed onto the intial query
      query = query.Where(u => u.FirstName.Contains(name));
    }

    return Ok(query.ToList());
  }

  /// <summary>
  /// Retrieves list of Users
  /// </summary>
  /// <return>Users</return>
  // [Authorize(Policy = "SuperUserOnly")]
  [Authorize(Policy = "SuperUserOnly")]
  [HttpGet()]
  public async Task<ActionResult<IEnumerable<User>>> GetUsers()
  {
    var users = await _userServices.GetUsersAsync();
    return Ok(users);
  }

  [Authorize(Policy = "SuperUserOnly")]
  [HttpGet("{id:int}")]
  public async Task<ActionResult<User>> GetUser([FromRoute] int id)
  {
    var user = await _userServices.GetUserAsync(id);
    if (user == null)
      return NotFound(new { Message = $"User with ID {id} not found." });

    if (!ModelState.IsValid)
      return BadRequest(ModelState);

    return Ok(user);
  }

  [Authorize(Policy = "SuperUserOnly")]
  [HttpPost]
  public async Task<ActionResult<User>> PostUser([FromBody] CreateRequestDto createUser)
  {
    if (createUser == null)
      return BadRequest("Request body is missing or invalid.");

    if (!ModelState.IsValid)
      return BadRequest(ModelState);

    var createdUser = await _userServices.CreateUserAsync(createUser);

    return CreatedAtAction(nameof(GetUser), new { id = createdUser.UserId }, createdUser);
  }

  [Authorize(Policy = "SuperUserOnly")]
  [HttpPut("{id:int}")]
  public async Task<IActionResult> PutUser(
      [FromRoute] int id,
      [FromBody] UpdateRequestDto updateUser
  )
  {
    if (!ModelState.IsValid)
      return BadRequest(ModelState);

    var updated = await _userServices.UpdateUserAsync(id, updateUser);
    if (!updated)
      return NotFound(new { Message = $"User with ID: {id} not found." });

    return NoContent();
  }

  [Authorize(Policy = "SuperUserOnly")]
  [HttpDelete("{id:int}")]
  public async Task<IActionResult> DeleteUser([FromRoute] int id)
  {
    if (!ModelState.IsValid)
      return BadRequest(ModelState);

    var deleted = await _userServices.DeleteUserAsync(id);
    if (!deleted)
      return NotFound(new { Message = $"User with ID: {id} not found." });

    return NoContent();
  }

  [HttpPost("register")]
  public async Task<ActionResult<UserResponseDto>> RegisterUser(
      [FromBody] CreateRequestDto createUser
  )
  {
    if (createUser == null)
      return BadRequest("Request body is missing or invalid.");

    if (!ModelState.IsValid)
      return BadRequest(ModelState);

    var createdUser = await _userServices.CreateUserAsync(createUser);

    return CreatedAtAction(nameof(GetUser), new { id = createdUser.UserId }, createdUser);
  }
}
