using L_API.Models;
using L_API.Services;
using L_API.Repos;
using L_API.Modal;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace L_API.Container
{
	public class CustomerService: ICustomerService
    {
        private readonly DbL2Context context;
        private readonly IMapper mapper;
        private readonly ILogger<CustomerService> logger;


        public CustomerService(DbL2Context context, IMapper mapper, ILogger<CustomerService> logger)
        {
            this.context = context;
            this.mapper = mapper;
            this.logger = logger;
        }

        public async Task<APIResponse> CreateCustomer(CustomerModal data)
        {
            try
            {
                this.logger.LogInformation("Create Begins");
                Customer customer = this.mapper.Map<CustomerModal, Customer>(data);
                await this.context.Customers.AddAsync(customer);
                await this.context.SaveChangesAsync();

                return new APIResponse
                {
                    ResponseCode = 201,
                    Result = data.Code
                };
            }
            catch (Exception err)
            {
                this.logger.LogError(err.Message,err);
                return new APIResponse
                {
                    ResponseCode = 400,
                    ErrorMessage = err.Message
                };
            }
        }


        public async Task<List<CustomerModal>> GetAllCustomer()
        {
            var _data = await this.context.Customers.ToListAsync()
                        ?? throw Exception("Something went wrong!");
            var _response = this.mapper.Map<List<Customer>, List<CustomerModal>>(_data);
            return _response;
        }

        public async Task<CustomerModal> GetCustomerByCode(string code)
        {
            var customer = await this.context.Customers.FindAsync(code) ?? throw new Exception("Customer not found!");
            var _response = this.mapper.Map<Customer, CustomerModal>(customer);
            return _response;
        }


        public async Task<APIResponse> RemoveCustomer(string code)
        {
            try
            {
                var customer = await this.context.Customers.FindAsync(code);
                if (customer != null)
                {
                    this.context.Customers.Remove(customer);
                    await this.context.SaveChangesAsync();
                    return new APIResponse { ResponseCode = 200, Result = "Success!" };
                }
                return new APIResponse { ResponseCode = 404, ErrorMessage = "Data not found!" };
            }
            catch (Exception err)
            {
                return new APIResponse { ResponseCode = 400, ErrorMessage = err.Message };
            }
        }

        public async Task<APIResponse> UpdateCustomer(CustomerModal data, string code)
        {
            try
            {
                var customer = await this.context.Customers.FindAsync(code);
                if (customer == null)
                {
                    return new APIResponse { ResponseCode = 404, ErrorMessage = "Data not found!" };
                }

                if (!string.IsNullOrEmpty(data.Name) && data.Name != customer.Name)
                {
                    customer.Name = data.Name;
                }
                if (!string.IsNullOrEmpty(data.Email) && data.Email != customer.Email)
                {
                    customer.Email = data.Email;
                }
                if (!string.IsNullOrEmpty(data.Phone) && data.Phone != customer.Phone)
                {
                    customer.Phone = data.Phone;
                }
                if (data.IsActive && data.IsActive != customer.IsActive)
                {
                    customer.IsActive = data.IsActive;
                }
                if (data.Creditlimit.HasValue && data.Creditlimit.Value != customer.Creditlimit)
                {
                    customer.Creditlimit = data.Creditlimit.Value;
                }

                await this.context.SaveChangesAsync();

                return new APIResponse { ResponseCode = 200, Result = customer.Code };
            }
            catch (Exception err)
            {
                return new APIResponse { ResponseCode = 400, ErrorMessage = err.Message };
            }
        }

        private Exception Exception(string v)
        {
            throw new NotImplementedException();
        }
    }
}

