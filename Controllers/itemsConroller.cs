using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;

namespace Ex3.API.Controllers
{
    [Route("api/items")]
    [ApiController]
    public class itemsController : ControllerBase
    {
        [HttpGet("getAllProducts")]
        public ActionResult<List<Product>> getAllItems([FromServices] List<Product> productsList) //notice- according DI, service will be injected
        {
            return productsList;
        }

        [HttpGet("getProduct/{id}")]
        public ActionResult<string> getName(int id, [FromServices] List<Product> productsList)
        {
            foreach (Product product in productsList)
            {
                if (product.Id == id) { return Ok(product.Name); }
            }
            return NotFound();
        }

        [HttpPost("addProduct/{name}/{id}")]
        public ActionResult<Product> addItem(int id, string name, [FromServices] List<Product> productsList)
        {
            foreach (Product product in productsList)
            {
                if (product.Id == id)
                {
                    return BadRequest("Product already exists");
                }
            }
            Product newProduct = new Product(id, name);
            productsList.Add(newProduct);
            return Ok(newProduct);
        }
    }
}
