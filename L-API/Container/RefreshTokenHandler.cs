using System.Security.Cryptography;
using L_API.Models;
using L_API.Repos;
using L_API.Services;
using Microsoft.EntityFrameworkCore;

namespace L_API.Container
{
	public class RefreshTokenHandler: IRefreshTokenHandler
	{
        private readonly DbL2Context context;

        public RefreshTokenHandler(DbL2Context context)
        {
            this.context = context;
        }

        public async Task<string> GenerateToken(string username)
        {
            var randomNumber = new byte[32];
            using (var randomNumberGenerator = RandomNumberGenerator.Create())
            {
              randomNumberGenerator.GetBytes(randomNumber);
                string refreshToken = Convert.ToBase64String(randomNumber);
                var ExistToken = await this.context.Refreshtokens.FirstOrDefaultAsync(item => item.Userid == username);
                if(ExistToken != null)
                {
                    ExistToken.RefreshToken = refreshToken;
                } else
                {
                    await this.context.Refreshtokens.AddAsync(new Refreshtoken
                    {
                        Userid = username,
                        Tokenid = new Random().Next().ToString(),
                        RefreshToken = refreshToken,
                    });;
                }
                await this.context.SaveChangesAsync();
                return refreshToken;
            }
                

        }
	}
}

