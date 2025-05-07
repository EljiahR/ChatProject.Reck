using ChatProject.Models;

namespace ChatProject.Repositories;

public interface IRefreshTokenRepository
{
    Task<List<RefreshToken>> GetRefreshTokens();
    Task<RefreshToken> AddToken(RefreshToken token);
    Task DeleteUserTokens(string userId);
    Task RevokeToken(string refreshToken);
}