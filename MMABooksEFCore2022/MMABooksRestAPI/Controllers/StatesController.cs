using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MMABooksEFClasses.Models;

namespace MMABooksRestAPI.Controllers
{
    // Defines the base URL structure for API
    // endpoints in this controller.
    // The ROUTE is the URL https://localhost:7082/api/states
    [Route("api/[controller]")]
    // Marks this class as an API controller,
    // enabling special behaviors tailored for
    // building RESTful APIs, such as automatic
    // model validation and consistent error responses.
    // This controller uses the States model.
    [ApiController]
    // This is controller used to manage CRUD operations'
    // create, read, update, and delete for States record.
    public class StatesController : ControllerBase
    {
        // A private read-only field that holds a
        // reference to the MMABooksContext 
        // database context. It is used to connect
        // to the database and perform CRUD operations.
        private readonly MMABooksContext _context;

        // Constructor that initializes the StatesController
        // with an MMABooksContext instance. This allows the
        // controller to interact with the database using the
        // provided context.
        public StatesController(MMABooksContext context)
        {
            _context = context;
        }

        // GET: api/States
        // This method handles an HTTP GET request to retrieve a
        // list of State objects from the database.
        [HttpGet]
        public async Task<ActionResult<IEnumerable<State>>> GetStates()
        {
            // Checks if the States DbSet is null. If it is,
            // it returns a 404 NotFound error.
            if (_context.States == null)
            {
                return NotFound();
            }
            // Asynchronously fetches and returns the list of
            // states from the database. The operation will
            // complete once the data is successfully retrieved
            // and verified to be non-null.
            return await _context.States.ToListAsync();
        }

        // GET: api/States/5
        // This method handles an HTTP GET request to retrieve
        // a specific State object from the database, based
        // on the provided primary key in the URL.
        [HttpGet("{id}")]
        public async Task<ActionResult<State>> GetState(string id)
        {
            if (_context.States == null)
            {
                return NotFound();
            }
            // Asynchronously finds the state with the
            // specified StateCode in the database. This operation
            // will complete once the state is retrieved
            // or not found.
            var state = await _context.States.FindAsync(id);
            // If the state is not found, null, it
            // returns a 404 NotFound error.
            if (state == null)
            {
                return NotFound();
            }

            return state;
        }

        // PUT: api/States/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        // This method handles an HTTP PUT request to update a
        // specific state record in the database. The update
        // is based on the primary key provided in the URL
        // and the data sent in the request body.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutState(string id, State state)
        {
            // Checks if the StateCode in the URL matches the StateCode
            // in the request body. If they don't match, it
            // returns a 400 BadRequest error.
            if (id != state.StateCode)
            {
                return BadRequest();
            }

            // Marks the state entity as modified so the changes
            // will be tracked and saved to the database.
            _context.Entry(state).State = EntityState.Modified;

            try
            {
                // Attempts to save the changes asynchronously to the database.
                // If successful, the changes are committed.
                await _context.SaveChangesAsync();
            }
            // Catches any concurrency exceptions that
            // occur during the update process. If the
            // state doesn't exist, it returns a
            // 404 NotFound error. Otherwise, it
            // rethrows the exception.
            catch (DbUpdateConcurrencyException)
            {
                if (!StateExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            // Returns a 204 NoContent status code, indicating
            // the update was successful and there is
            // no content to return.
            return NoContent();
        }

        // POST: api/States
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        // This method handles an HTTP PUT request to update a
        // specific state record in the database. The update
        // is based on the primary key provided in the URL
        // and the data sent in the request body.
        [HttpPost]
        public async Task<ActionResult<State>> PostState(State state)
        {
            // If the States DbSet is null, it returns
            // a ProblemDetails response indicating that
            // the "States" entity set in the database
            // context is null.
            if (_context.States == null)
            {
                return Problem("Entity set 'MMABooksContext.States'  is null.");
            }
            // Adds the new State object to the database context.
            _context.States.Add(state);
            try
            {
                // Asynchronously saves the changes to the database,
                // committing the new State object to the database.
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (StateExists(state.StateCode))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }
            // This generates a 201 Created HTTP response,
            // using the GetState action to generate the
            // URL with the created StateCode,
            // and returns the created state object to
            // confirm it was successfully added to the database.
            return CreatedAtAction("GetState", new { id = state.StateCode }, state);
        }

        // DELETE: api/States/5
        // This method handles an HTTP DELETE request
        // to delete a state record from the database,
        // identified by the StateCode provided in the URL.
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteState(string id)
        {
            if (_context.States == null)
            {
                return NotFound();
            }
            // Asynchronously retrieves the state record
            // with the specified StateCode. If the state is not
            // found, it returns a 404 NotFound response.
            var state = await _context.States.FindAsync(id);
            if (state == null)
            {
                return NotFound();
            }
            // Marks the state entity for removal
            // from the database context.
            _context.States.Remove(state);
            // Asynchronously saves the change to the
            // database, deleting the state record.
            await _context.SaveChangesAsync();
            // Returns a 204 NoContent response,
            // indicating the deletion was successful.
            return NoContent();
        }

        // Helper method to check if a state exists in the database.
        // Returns true if a state with the specified StateCode exists,
        // otherwise returns false.
        private bool StateExists(string id)
        {
            return (_context.States?.Any(e => e.StateCode == id)).GetValueOrDefault();
        }
    }
}
