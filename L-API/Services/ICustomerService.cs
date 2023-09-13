using L_API.Models;
using L_API.Modal;
using L_API.Helper;
namespace L_API.Services
{
	public interface ICustomerService
	{
		Task<List<CustomerModal>> GetAllCustomer();
		Task<CustomerModal> GetCustomerByCode(string code);
		Task<APIResponse> RemoveCustomer(string code);
		Task<APIResponse> CreateCustomer(CustomerModal data);
		Task<APIResponse> UpdateCustomer(CustomerModal data, string code);
    }
}

