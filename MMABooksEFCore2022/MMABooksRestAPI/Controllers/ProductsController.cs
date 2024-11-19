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
    // The ROUTE is the URL https://localhost:7082/api/products
    [Route("api/[controller]")]
    // Marks this class as an API controller,
    // enabling special behaviors tailored for
    // building RESTful APIs, such as automatic
    // model validation and consistent error responses.
    // This controller uses the Product model.
    [ApiController]
    // This is controller used to manage CRUD operations'
    // create, read, update, and delete for Product record.
    public class ProductsController : ControllerBase
    {
        // A private read-only field that holds a
        // reference to the MMABooksContext 
        // database context. It is used to connect
        // to the database and perform CRUD operations.
        private readonly MMABooksContext _context;

        // Constructor that initializes the ProductsController
        // with an MMABooksContext instance. This allows the
        // controller to interact with the database using the
        // provided context.
        public ProductsController(MMABooksContext context)
        {
            _context = context;
        }

        // GET: api/Products
        // This method handles an HTTP GET request to retrieve a
        // list of Product objects from the database.
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
            // Checks if the Products DbSet is null. If it is,
            // it returns a 404 NotFound error.
            if (_context.Products == null)
            {
                return NotFound();
            }
            // Asynchronously fetches and returns the list of
            // products from the database. The operation will
            // complete once the data is successfully retrieved
            // and verified to be non-null.
            return await _context.Products.ToListAsync();
        }

        // GET: api/Products/5
        // This method handles an HTTP GET request to retrieve
        // a specific Product object from the database, based
        // on the provided primary key in the URL.
        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProduct(string id)
        {
            if (_context.Products == null)
            {
                return NotFound();
            }
            // Asynchronously finds the product with the
            // specified ProductCode in the database. This operation
            // will complete once the product is retrieved
            // or not found.
            var product = await _context.Products.FindAsync(id);
            // If the product is not found, null, it
            // returns a 404 NotFound error.
            if (product == null)
            {
                return NotFound();
            }

            return product;
        }

        // PUT: api/Products/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        // This method handles an HTTP PUT request to update a
        // specific product record in the database. The update
        // is based on the primary key provided in the URL
        // and the data sent in the request body.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProduct(string id, Product product)
        {
            // Checks if the ProductCode in the URL matches the ProductCode
            // in the request body. If they don't match, it
            // returns a 400 BadRequest error.
            if (id != product.ProductCode)
            {
                return BadRequest();
            }

            // Marks the product entity as modified so the changes
            // will be tracked and saved to the database.
            _context.Entry(product).State = EntityState.Modified;

            try
            {
                // Attempts to save the changes asynchronously to the database.
                // If successful, the changes are committed.
                await _context.SaveChangesAsync();
            }
            // Catches any concurrency exceptions that
            // occur during the update process. If the
            // product doesn't exist, it returns a
            // 404 NotFound error. Otherwise, it
            // rethrows the exception.
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(id))
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

        // POST: api/Products
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        // This method handles an HTTP PUT request to update a
        // specific product record in the database. The update
        // is based on the primary key provided in the URL
        // and the data sent in the request body.
        [HttpPost]
        public async Task<ActionResult<Product>> PostProduct(Product product)
        {
            // If the Products DbSet is null, it returns
            // a ProblemDetails response indicating that
            // the "Products" entity set in the database
            // context is null.
            if (_context.Products == null)
            {
                return Problem("Entity set 'MMABooksContext.Products'  is null.");
            }
            // Adds the new Product object to the database context.
            _context.Products.Add(product);
            try
            {
                // Asynchronously saves the changes to the database,
                // committing the new Product object to the database.
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (ProductExists(product.ProductCode))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }
            // This generates a 201 Created HTTP response,
            // using the GetProduct action to generate the
            // URL with the created ProductCode,
            // and returns the created product object to
            // confirm it was successfully added to the database.
            return CreatedAtAction("GetProduct", new { id = product.ProductCode }, product);
        }

        // DELETE: api/Products/5
        // This method handles an HTTP DELETE request
        // to delete a product record from the database,
        // identified by the ProductCode provided in the URL.
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(string id)
        {
            if (_context.Products == null)
            {
                return NotFound();
            }
            // Asynchronously retrieves the product record
            // with the specified ProductCode. If the product is not
            // found, it returns a 404 NotFound response.
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            // Marks the product entity for removal
            // from the database context.
            _context.Products.Remove(product);
            // Asynchronously saves the change to the
            // database, deleting the product record.
            await _context.SaveChangesAsync();
            // Returns a 204 NoContent response,
            // indicating the deletion was successful.
            return NoContent();
        }

        // Helper method to check if a product exists in the database.
        // Returns true if a product with the specified ProductCode exists,
        // otherwise returns false.
        private bool ProductExists(string id)
        {
            return (_context.Products?.Any(e => e.ProductCode == id)).GetValueOrDefault();
        }
    }
}
