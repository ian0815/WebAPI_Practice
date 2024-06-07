using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPI_practice.Models;

namespace WebAPI_practice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InformationController : ControllerBase
    {
        private readonly MemberSystemContext _context;

        public InformationController(MemberSystemContext context)
        {
            _context = context;
        }

        // GET: api/Information
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Information>>> GetInformation()
        {
            return Ok(await _context.Information.ToListAsync());
        }

        // GET: api/Information/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Information>> GetInformation(int id)
        {
            // If the database is empty, return NotFound.
            if (_context.Information == null)
            {
                return NotFound("No data in database.");
            }

            // Keep the data of the input id.
            Information information = await _context.Information.FindAsync(id);

            if (information == null)
            {
                return NotFound("The id is not exist.");
            }

            return Ok(information);
        }

        // PUT: api/Information/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutInformation(int id, ReviseInformation reviseInfo)
        {
            // Find the information of input id.
            Information originalInfo = await _context.Information.FindAsync(id);

            // If the id after revising and the input id is not the same, return BadRequest.
            // The information sent to the function is the data after revising.
            if (originalInfo == null)
            {
                return BadRequest("The ID is not exist.");
            }

            // Check whether the info after revising is blank.
            if (reviseInfo.Name == "" || reviseInfo.Password == "" || reviseInfo.Phone == "")
            {
                return BadRequest("The information cannot be blank.");
            }

            originalInfo.Name = reviseInfo.Name;
            originalInfo.Password = reviseInfo.Password;
            originalInfo.Phone = reviseInfo.Phone;

            // Set the state of this data to the Modified.
            _context.Entry(originalInfo).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!InformationExists(id))
                {
                    return NotFound("The id is not exist.");
                }
                else
                {
                    throw;
                }
            }

            return Ok(originalInfo);
        }

        // POST: api/Information
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Route("register")]
        public async Task<ActionResult<Information>> PostInformation(Register registerInfo)
        {
            if (_context.Information.Any(u => u.Email == registerInfo.Email))
            {
                return Content("The account is already exist.");
            }

            Information newInformation = new Information();

            if (registerInfo != null)
            {
                newInformation.Name = registerInfo.Name;
                newInformation.Password = registerInfo.Password;
                newInformation.Email = registerInfo.Email;
                newInformation.Phone = registerInfo.Phone;
            }
            else
            {
                newInformation = null;
                return BadRequest();
            }

            await _context.Information.AddAsync(newInformation);
            await _context.SaveChangesAsync();

            return Ok(newInformation);
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> login(Login loginInfo)
        {
            Information information = await _context.Information.SingleOrDefaultAsync(u => u.Email == loginInfo.Email);

            if (information == null)
            {
                return NotFound("The account is not exist.");
            }

            if (information.Password == loginInfo.Password)
            {
                information.Password = null;
                return Ok(information);
            }
            else
            {
                return BadRequest("The password is wrong.");
            }
        }

        // DELETE: api/Information/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteInformation(int id)
        {
            var information = await _context.Information.FindAsync(id);
            if (information == null)
            {
                return NotFound();
            }

            _context.Information.Remove(information);
            await _context.SaveChangesAsync();

            return Content("The id: " + id + " is deleted.");
        }

        private bool InformationExists(int id)
        {
            return (_context.Information?.Any(e => e.MemberId == id)).GetValueOrDefault();
        }
    }
}
