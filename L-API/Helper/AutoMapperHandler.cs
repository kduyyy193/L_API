using AutoMapper;
using L_API.Models;
using L_API.Modal;
namespace L_API.Helper
{
	public class AutoMapperHandler: Profile
	{
		public AutoMapperHandler()
		{
			CreateMap<Customer, CustomerModal>().ForMember(item => item.StatusName, otp => otp.MapFrom(
				item => item.IsActive? "Active": "In active")).ReverseMap();

		}
	}
}

