using ChatProject.Models;

namespace ChatProject.Services;

public interface IRefreshTokenService
{
    Task<RefreshToken?> GetRefreshTokenAsync(string refreshToken);
    Task<List<RefreshToken>> GetRefreshTokensAsync();
    Task<RefreshToken> AddTokenAsync(RefreshToken token);
    Task DeleteUserTokensAsync(string userId);
    Task RevokeTokenAsync(string refreshToken);
}