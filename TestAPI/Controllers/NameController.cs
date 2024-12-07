using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TestAPI.Data;
using TestAPI.Model;

namespace test2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NameController : ControllerBase
    {
        private readonly AppDbContext _dbContext;

        // Constructor to inject the DbContext
        public NameController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // Get all names with their ages and cities from the database
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var names = await _dbContext.Names.ToListAsync(); // Fetch all names from the database
            return Ok(names); // Return the names as a response
        }

        // Add a new name with age and city
        [HttpPost]
        public async Task<IActionResult> AddName([FromBody] NameModel name)
        {
            if (name == null || string.IsNullOrWhiteSpace(name.Name) || name.Age <= 0)
                return BadRequest("Invalid name or age.");

            // Add the new name to the database
            _dbContext.Names.Add(name);
            await _dbContext.SaveChangesAsync();  // Save the changes to the database

            return CreatedAtAction(nameof(GetById), new { id = name.Id }, name);  // Return the created name with the location
        }

        // Get a specific name by ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var name = await _dbContext.Names.FindAsync(id); // Fetch the name by ID from the database
            if (name == null)
                return NotFound("Name not found."); // Return 404 if the name is not found

            return Ok(name); // Return the name as a response
        }

        // Update name, age, or city by ID
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateName(int id, [FromBody] NameModel updatedName)
        {
            var name = await _dbContext.Names.FindAsync(id); // Find the existing name by ID
            if (name == null)
                return NotFound("Name not found."); // Return 404 if the name is not found

            // Update Name if it is provided and not a placeholder string (like "string")
            if (!string.IsNullOrWhiteSpace(updatedName.Name) && updatedName.Name != "string")
            {
                name.Name = updatedName.Name;
            }

            // Update Age if it is a valid positive number
            if (updatedName.Age > 0)
            {
                name.Age = updatedName.Age;
            }

            // Update City if it is provided and not a placeholder string (like "string")
            if (!string.IsNullOrWhiteSpace(updatedName.City) && updatedName.City != "string")
            {
                name.City = updatedName.City;
            }

            // Save the updated name to the database
            await _dbContext.SaveChangesAsync();

            return Ok(name); // Return the updated name
        }

        // Delete a name by ID
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteName(int id)
        {
            var name = await _dbContext.Names.FindAsync(id); // Find the name by ID
            if (name == null)
                return NotFound("Name not found."); // Return 404 if the name is not found

            _dbContext.Names.Remove(name); // Remove the name from the database
            await _dbContext.SaveChangesAsync();  // Save the changes to the database

            return NoContent(); // Return 204 No Content after successful deletion
        }
    }
}