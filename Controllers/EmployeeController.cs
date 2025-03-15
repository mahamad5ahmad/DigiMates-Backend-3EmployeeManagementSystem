using EmployeeManagementAPI.Data;
using EmployeeManagementAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using EmployeeManagement.Models;
namespace EmployeeManagementAPI.Controllers
{ 
     // Require authentication
    //[Authorize]
    [Route("api/employees")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly AppDbContext _context;

        public EmployeeController(AppDbContext context)
        {
            _context = context;
        }

        // ✅ POST: api/employees → Create an employee
        
        [HttpPost]
        [Authorize(Policy = "AdminPolicy")]
        public async Task<IActionResult> CreateEmployee([FromBody] Employee employee)
        {
            if (employee == null)
                return BadRequest("Invalid employee data.");

            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetEmployeeById), new { id = employee.Id }, employee);
        }

        // ✅ GET: api/employees → Get all employees
       
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Employee>>> GetAllEmployees(
            [FromQuery] int page = 1, 
            [FromQuery] int size = 10, 
            [FromQuery] string? sort = "id,asc", 
            [FromQuery] string? department = null)
        {
            if (page < 1 || size < 1) return BadRequest("Page and size must be positive numbers.");

            var query = _context.Employees.AsQueryable();

            // ✅ Filtering by Department
            if (!string.IsNullOrEmpty(department))
            {
                query = query.Where(e => e.Department == department);
            }

            // ✅ Sorting Logic
            string[] sortParams = sort.Split(',');
            string sortField = sortParams[0].ToLower();
            bool isDescending = sortParams.Length > 1 && sortParams[1].ToLower() == "desc";

            query = sortField switch
            {
                "name" => isDescending ? query.OrderByDescending(e => e.Name) : query.OrderBy(e => e.Name),
                "email" => isDescending ? query.OrderByDescending(e => e.Email) : query.OrderBy(e => e.Email),
                "department" => isDescending ? query.OrderByDescending(e => e.Department) : query.OrderBy(e => e.Department),
                "salary" => isDescending ? query.OrderByDescending(e => e.Salary) : query.OrderBy(e => e.Salary),
                _ => isDescending ? query.OrderByDescending(e => e.Id) : query.OrderBy(e => e.Id),
            };

            // ✅ Pagination Logic
            var totalEmployees = await query.CountAsync();
            var employees = await query.Skip((page - 1) * size).Take(size).ToListAsync();

            // ✅ Return Response with Pagination Info
            return Ok(new
            {
                totalEmployees,
                totalPages = (int)Math.Ceiling((double)totalEmployees / size),
                currentPage = page,
                employees
            });
        }

        // ✅ GET: api/employees/{id} → Get an employee by ID
       
        [HttpGet("{id}")]
        public async Task<ActionResult<Employee>> GetEmployeeById(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
                return NotFound($"Employee with ID {id} not found.");

            return employee;
        }

        // ✅ PUT: api/employees/{id} → Update an employee
     
        [HttpPut("{id}")]
        [Authorize(Policy = "AdminPolicy")]
        public async Task<IActionResult> UpdateEmployee(int id, [FromBody] Employee employee)
        {
            if (id != employee.Id)
                return BadRequest("Employee ID mismatch.");

            var existingEmployee = await _context.Employees.FindAsync(id);
            if (existingEmployee == null)
                return NotFound($"Employee with ID {id} not found.");

            existingEmployee.Name = employee.Name;
            existingEmployee.Email = employee.Email;
            existingEmployee.Department = employee.Department;
            existingEmployee.Salary = employee.Salary;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // ✅ DELETE: api/employees/{id} → Delete an employee
        [HttpDelete("{id}")]
        [Authorize(Policy = "AdminPolicy")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
                return NotFound($"Employee with ID {id} not found.");

            _context.Employees.Remove(employee);
            await _context.SaveChangesAsync();
            return NoContent();
        }

   
        [HttpGet("debug-token")]
        //[Authorize(Policy = "UserPolicy")]
        public IActionResult DebugToken()
        {
        var authorizationHeader = Request.Headers["Authorization"].FirstOrDefault();
        if (string.IsNullOrEmpty(authorizationHeader))
            return Unauthorized("No token provided");

        var token = authorizationHeader.Replace("Bearer ", "");
        var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();

        try
        {
            var jsonToken = handler.ReadJwtToken(token);
            return Ok(new
            {
            Issuer = jsonToken.Issuer,
            Audience = jsonToken.Audiences,
            Expiration = jsonToken.ValidTo,
            Claims = jsonToken.Claims.Select(c => new { c.Type, c.Value })
            });
        }
        catch (Exception ex)
        {
        return BadRequest($"Token validation failed: {ex.Message}");
        }
        } 

    }
}
