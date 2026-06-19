using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using HomeLedger.API.Models;

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
    public async Task<ActionResult<IEnumerable<Family>>> GetFamilies()
    {
        var fams = await _service.GetAllAsync();
        var famsList = fams as IList<Family> ?? [.. fams];

        return famsList.Count == 0 ? NoContent() : Ok(famsList);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Family?>> GetFamily(int id)
    {
        var fam = await _service.GetByIdAsync(id);
        if (fam == null)
            return NotFound(new { Message = $"Family with ID {id} not found." });

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        return Ok(fam);
    }

    [HttpPost]
    public async Task<ActionResult<Family>> PostFamily([FromBody] Family createFam)
    {
        if (createFam == null)
            return BadRequest("Request body is missing or invalid");

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var newFam = await _service.AddAsync(createFam);
        return CreatedAtAction(nameof(GetFamily), new { id = newFam.FamilyId }, newFam);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<Family>> PutFamily(
        [FromRoute] int id,
        [FromBody] Family updateFam
    )
    {
        if (updateFam == null)
            return BadRequest("Request body is missing or invalid");

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var updatedFam = await _service.UpdateAsync(id, updateFam);
        if (updatedFam == null)
            return NotFound(new { Message = $"Family with ID {id} not found." });

        return Ok(updatedFam);
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> DeleteFamily(int id)
    {
        var result = await _service.DeleteAsync(id);
        if (!result)
            return NotFound(new { Message = $"Family with ID {id} not found." });

        return NoContent();
    }
}