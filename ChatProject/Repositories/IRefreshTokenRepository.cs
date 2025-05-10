using ChatProject.Models;

namespace ChatProject.Repositories;

public interface IRefreshTokenRepository
{
    Task<RefreshToken?> GetRefreshTokenAsync(string refreshToken);
    Task<List<RefreshToken>> GetRefreshTokensAsync();
    Task<RefreshToken> AddTokenAsync(RefreshToken token);
    Task DeleteUserTokensAsync(string userId);
    Task RevokeTokenAsync(string refreshToken);
}