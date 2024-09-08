
using E_Commerce.DataAccess.Models;

namespace E_Commerce.Core.IServices
{
	public interface ITokenService
    {
        string GetToken(AppUser user);
    }
}
