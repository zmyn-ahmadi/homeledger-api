using HomeLedger.API.DTOs.FamilyDTO;
using HomeLedger.API.Models;
using HomeLedger.API.Services.Family;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HomeLedger.API.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
[Produces("application/json")]
[Consumes("application/json")]
public class FamilyController(IFamilyService familyService) : ControllerBase
{
    private readonly IFamilyService _service = familyService;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<FamilyDTO>>> GetFamilies()
    {
        var fams = await _service.GetAllAsync();
        var famsList = fams as IList<FamilyDTO> ?? [.. fams];

        return famsList.Count == 0 ? NoContent() : Ok(famsList);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<FamilyDTO?>> GetFamily(int id)
    {
        var fam = await _service.GetByIdAsync(id);
        return fam != null
            ? Ok(fam)
            : NotFound(new { Message = $"Family with ID {id} not found." });
    }

    [HttpPost]
    public async Task<ActionResult<FamilyDTO>> PostFamily([FromBody] CreateFamilyDTO createFam)
    {
        if (createFam == null)
            return BadRequest("Request body is missing or invalid");

        var newFam = await _service.AddAsync(createFam);
        return CreatedAtAction(nameof(GetFamily), new { id = newFam.FamilyId }, newFam);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<FamilyDTO>> PutFamily(
        [FromRoute] int id,
        [FromBody] UpdateFamilyDTO updateFam
    )
    {
        if (updateFam == null)
            return BadRequest("Request body is missing or invalid");

        var updatedFam = await _service.UpdateAsync(id, updateFam);
        return updatedFam == null
            ? NotFound(new { Message = $"Family with ID {id} not found." })
            : Ok(updatedFam);
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> DeleteFamily(int id)
    {
        var result = await _service.DeleteAsync(id);
        return result ? NoContent() : NotFound(new { Message = $"Family with ID {id} not found" });
    }

    [HttpPost("/family/invitations")]
    public async Task<ActionResult<string>> CreateInvitation()
    {
        var link = await _service.InviteAsync();
        return Ok(link);
    }

    [HttpPost("/family/invitations/{code}/accept")]
    public async Task<IActionResult> AcceptInvitation(string code)
    {
        await _service.AcceptInvitationAsync(code);
        return NoContent();
    }
}
