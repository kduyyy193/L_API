using L_API.Modal;
using L_API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace L_API.Controllers
{
    [Authorize]
    [EnableCors]
    [EnableRateLimiting("fixed")]
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : Controller
    {
        private readonly ICustomerService customerService;

        public CustomerController(ICustomerService customerService)
        {
            this.customerService = customerService;
        }

        [HttpGet("GetAllCustomer")]
        public async Task<IActionResult> GetAllCustomer()
        {
            var data = await this.customerService.GetAllCustomer();
            if(data == null)
            {
                return NotFound();
            }
            return Ok(data);
        }

        [HttpGet("GetCustomerByCode")]
        public async Task<IActionResult> GetCustomerByCode(string code)
        {
            var data = await this.customerService.GetCustomerByCode(code);
            if (data == null)
            {
                return NotFound();
            }
            return Ok(data);
        }

        [HttpPost("CreateCustomer")]
        public async Task<IActionResult> CreateCustomer(CustomerModal customer)
        {
            var data = await this.customerService.CreateCustomer(customer);
            return Ok(data);
        }

        [HttpPut("UpdateCustomer")]
        public async Task<IActionResult> UpdateCustomer(CustomerModal customer, string code)
        {
            var data = await this.customerService.UpdateCustomer(customer, code);
            return Ok(data);
        }

        [HttpDelete("RemoveCustomer")]
        public async Task<IActionResult> RemoveCustomer(string code)
        {
            var data = await this.customerService.RemoveCustomer(code);
            if (data == null)
            {
                return NotFound();
            }
            return Ok(data);
        }
    }
}
