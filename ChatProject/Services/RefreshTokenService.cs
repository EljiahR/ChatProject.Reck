using ChatProject.Models;

namespace ChatProject.Services;

public class RefreshTokenService
{
    private readonly IRefreshTokenService _repository;
    public RefreshTokenService(IRefreshTokenService repository)
    {
        _repository = repository;
    }
    public async Task<List<RefreshToken>> GetRefreshTokens()
    {
        return await _repository.GetRefreshTokens();
    }
    public async Task<RefreshToken> AddToken(RefreshToken token)
    {
        return await _repository.AddToken(token);
    }
    public async Task DeleteUserTokens(string userId)
    {
        await _repository.DeleteUserTokens(userId);
    }
    public async Task RevokeToken(string refreshToken)
    {
        await _repository.RevokeToken(refreshToken);
    }
}