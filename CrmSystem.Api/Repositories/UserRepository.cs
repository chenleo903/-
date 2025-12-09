using Microsoft.EntityFrameworkCore;
using CrmSystem.Api.Data;
using CrmSystem.Api.Models;

namespace CrmSystem.Api.Repositories;

/// <summary>
/// 用户数据访问实现
/// </summary>
public class UserRepository : IUserRepository
{
    private readonly CrmDbContext _context;

    public UserRepository(CrmDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.UserName == username, cancellationToken);
    }

    public async Task<bool> AnyAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Users.AnyAsync(cancellationToken);
    }

    public async Task<User> CreateAsync(User user, CancellationToken cancellationToken = default)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync(cancellationToken);
        return user;
    }

    public async Task UpdateLastLoginAsync(Guid userId, DateTimeOffset lastLoginAt, CancellationToken cancellationToken = default)
    {
        var user = await _context.Users.FindAsync(new object[] { userId }, cancellationToken);
        
        if (user != null)
        {
            user.LastLoginAt = lastLoginAt;
            user.UpdatedAt = DateTimeOffset.UtcNow;
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
