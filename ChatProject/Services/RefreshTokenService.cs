using ChatProject.Models;
using ChatProject.Repositories;

namespace ChatProject.Services;

public class RefreshTokenService : IRefreshTokenService
{
    private readonly IRefreshTokenRepository _repository;
    public RefreshTokenService(IRefreshTokenRepository repository)
    {
        _repository = repository;
    }
    public async Task<RefreshToken?> GetRefreshTokenAsync(string refreshToken)
    {
        return await _repository.GetRefreshTokenAsync(refreshToken);
    }
    public async Task<List<RefreshToken>> GetRefreshTokensAsync()
    {
        return await _repository.GetRefreshTokensAsync();
    }
    public async Task<RefreshToken> AddTokenAsync(RefreshToken token)
    {
        return await _repository.AddTokenAsync(token);
    }
    public async Task DeleteUserTokensAsync(string userId)
    {
        await _repository.DeleteUserTokensAsync(userId);
    }
    public async Task RevokeTokenAsync(string refreshToken)
    {
        await _repository.RevokeTokenAsync(refreshToken);
    }
}