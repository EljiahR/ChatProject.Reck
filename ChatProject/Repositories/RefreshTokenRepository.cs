using ChatProject.Data;
using ChatProject.Models;
using Microsoft.EntityFrameworkCore;

namespace ChatProject.Repositories;

public class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly ChatDbContext _context;
    private readonly DbSet<RefreshToken> _dbSet;

    public RefreshTokenRepository(ChatDbContext context) 
    {
        _context = context;
        _dbSet = context.Set<RefreshToken>();
    }

    public async Task<List<RefreshToken>> GetRefreshTokens() 
    {
        return await _dbSet.ToListAsync();
    }
    public async Task<RefreshToken> AddToken(RefreshToken token)
    {
        var existingTokens = await _dbSet.Where(t => t.UserId == token.UserId).ToListAsync();
        if (existingTokens.Count > 0) {
            _dbSet.RemoveRange(existingTokens);
        }

        await _dbSet.AddAsync(token);
        await _context.SaveChangesAsync();

        return token;
    }
    public async Task DeleteUserTokens(string userId)
    {
        var existingTokens = await _dbSet.Where(t => t.UserId == userId).ToListAsync();
        if (existingTokens.Count > 0) {
            _dbSet.RemoveRange(existingTokens);
            await _context.SaveChangesAsync();
        }
    }
    public async Task RevokeToken(string refreshToken)
    {
        var existingToken = await _dbSet.FirstOrDefaultAsync(t => t.Token == refreshToken);
        if (existingToken != null) 
        {
            existingToken.IsRevoked = true;
            await _context.SaveChangesAsync();
        }
    }
}