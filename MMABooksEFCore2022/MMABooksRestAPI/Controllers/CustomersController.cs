using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MMABooksEFClasses.Models;
using static System.Net.WebRequestMethods;

namespace MMABooksRestAPI.Controllers
{
    // Defines the base URL structure for API
    // endpoints in this controller.
    // The ROUTE is the URL https://localhost:7082/api/customers
    [Route("api/[controller]")]
    // Marks this class as an API controller,
    // enabling special behaviors tailored for
    // building RESTful APIs, such as automatic
    // model validation and consistent error responses.
    // This controller uses the Customer model.
    [ApiController]
    // This is controller used to manage CRUD operations'
    // create, read, update, and delete for Customer record.
    public class CustomersController : ControllerBase
    {
        // A private read-only field that holds a
        // reference to the MMABooksContext 
        // database context. It is used to connect
        // to the database and perform CRUD operations.
        private readonly MMABooksContext _context;

        // Constructor that initializes the CustomersController
        // with an MMABooksContext instance. This allows the
        // controller to interact with the database using the
        // provided context.
        public CustomersController(MMABooksContext context)
        {
            _context = context;
        }

        // GET: api/Customers
        // This method handles an HTTP GET request to retrieve a
        // list of Customer objects from the database.
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Customer>>> GetCustomers()
        {
            // Checks if the Customers DbSet is null. If it is,
            // it returns a 404 NotFound error.
            if (_context.Customers == null)
            {
                return NotFound();
            }
            // Asynchronously fetches and returns the list of
            // customers from the database. The operation will
            // complete once the data is successfully retrieved
            // and verified to be non-null.
            return await _context.Customers.ToListAsync();
        }

        // GET: api/Customers/5
        // This method handles an HTTP GET request to retrieve
        // a specific Customer object from the database, based
        // on the provided primary key in the URL.
        [HttpGet("{id}")]
        public async Task<ActionResult<Customer>> GetCustomer(int id)
        {
            if (_context.Customers == null)
            {
                return NotFound();
            }
            // Asynchronously finds the customer with the
            // specified id in the database. This operation
            // will complete once the customer is retrieved
            // or not found.
            var customer = await _context.Customers.FindAsync(id);
            // If the customer is not found, null, it
            // returns a 404 NotFound error.
            if (customer == null)
            {
                return NotFound();
            }
            return customer;
        }

        // PUT: api/Customers/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        // This method handles an HTTP PUT request to update a
        // specific customer record in the database. The update
        // is based on the primary key provided in the URL
        // and the data sent in the request body.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCustomer(int id, Customer customer)
        {
            // Checks if the id in the URL matches the customer
            // ID in the request body. If they don't match, it
            // returns a 400 BadRequest error.
            if (id != customer.CustomerId)
            {
                return BadRequest();
            }

            // Marks the customer entity as modified so the changes
            // will be tracked and saved to the database.
            _context.Entry(customer).State = EntityState.Modified;

            try
            {
                // Attempts to save the changes asynchronously to the database.
                // If successful, the changes are committed.
                await _context.SaveChangesAsync();
            }
            // Catches any concurrency exceptions that
            // occur during the update process. If the
            // customer doesn't exist, it returns a
            // 404 NotFound error. Otherwise, it
            // rethrows the exception.
            catch (DbUpdateConcurrencyException)
            {
                if (!CustomerExists(id))
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

        // POST: api/Customers
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        // This method handles an HTTP PUT request to update a
        // specific customer record in the database. The update
        // is based on the primary key provided in the URL
        // and the data sent in the request body.
        [HttpPost]
        public async Task<ActionResult<Customer>> PostCustomer(Customer customer)
        {
            // If the Customers DbSet is null, it returns
            // a ProblemDetails response indicating that
            // the "Customers" entity set in the database
            // context is null.
            if (_context.Customers == null)
            {
                return Problem("Entity set 'MMABooksContext.Customers'  is null.");
            }
            // Adds the new Customer object to the database context.
            _context.Customers.Add(customer);
            // Asynchronously saves the changes to the database,
            // committing the new Customer object to the database.
            await _context.SaveChangesAsync();
            // This generates a 201 Created HTTP response,
            // using the GetCustomer action to generate the
            // URL with the automatically created CustomerId,
            // and returns the created customer object to
            // confirm it was successfully added to the database.
            return CreatedAtAction("GetCustomer", new { id = customer.CustomerId }, customer);
        }

        // DELETE: api/Customers/5
        // This method handles an HTTP DELETE request
        // to delete a customer record from the database,
        // identified by the customerId provided in the URL.
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            if (_context.Customers == null)
            {
                return NotFound();
            }
            // Asynchronously retrieves the customer record
            // with the specified id. If the customer is not
            // found, it returns a 404 NotFound response.
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
            {
                return NotFound();
            }
            // Marks the customer entity for removal
            // from the database context.
            _context.Customers.Remove(customer);
            // Asynchronously saves the change to the
            // database, deleting the customer record.
            await _context.SaveChangesAsync();
            // Returns a 204 NoContent response,
            // indicating the deletion was successful.
            return NoContent();
        }

        // Helper method to check if a customer exists in the database.
        // Returns true if a customer with the specified id exists,
        // otherwise returns false.
        private bool CustomerExists(int id)
        {
            return (_context.Customers?.Any(e => e.CustomerId == id)).GetValueOrDefault();
        }
    }
}
