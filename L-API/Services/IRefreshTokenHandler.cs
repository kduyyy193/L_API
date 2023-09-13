namespace L_API.Services
{
	public interface IRefreshTokenHandler
	{
		Task<string> GenerateToken(string username);
    }
}

