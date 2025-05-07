using ChatProject.Models;

namespace ChatProject.Services;

public interface IRefreshTokenService
{
    Task<List<RefreshToken>> GetRefreshTokens();
    Task<RefreshToken> AddToken(RefreshToken token);
    Task DeleteUserTokens(string userId);
    Task RevokeToken(string refreshToken);
}