using Microsoft.EntityFrameworkCore;
using CrmSystem.Api.Data;
using CrmSystem.Api.Models;

namespace CrmSystem.Api.Repositories;

/// <summary>
/// 互动记录数据访问实现
/// </summary>
public class InteractionRepository : IInteractionRepository
{
    private readonly CrmDbContext _context;

    public InteractionRepository(CrmDbContext context)
    {
        _context = context;
    }

    public async Task<Interaction?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Interactions
            .AsNoTracking()
            .FirstOrDefaultAsync(i => i.Id == id, cancellationToken);
    }

    public async Task<List<Interaction>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default)
    {
        return await _context.Interactions
            .Where(i => i.CustomerId == customerId)
            .OrderByDescending(i => i.HappenedAt)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<Interaction?> GetLatestByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default)
    {
        return await _context.Interactions
            .Where(i => i.CustomerId == customerId)
            .OrderByDescending(i => i.HappenedAt)
            .AsNoTracking()
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<Interaction> CreateAsync(Interaction interaction, CancellationToken cancellationToken = default)
    {
        _context.Interactions.Add(interaction);
        await _context.SaveChangesAsync(cancellationToken);
        return interaction;
    }

    public async Task<Interaction> UpdateAsync(Interaction interaction, CancellationToken cancellationToken = default)
    {
        _context.Interactions.Update(interaction);
        await _context.SaveChangesAsync(cancellationToken);
        return interaction;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var interaction = await _context.Interactions
            .FirstOrDefaultAsync(i => i.Id == id, cancellationToken);

        if (interaction == null)
        {
            return false;
        }

        _context.Interactions.Remove(interaction);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
