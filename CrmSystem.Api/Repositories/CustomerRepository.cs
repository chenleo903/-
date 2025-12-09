using Microsoft.EntityFrameworkCore;
using CrmSystem.Api.Data;
using CrmSystem.Api.Models;
using CrmSystem.Api.DTOs;

namespace CrmSystem.Api.Repositories;

/// <summary>
/// 客户数据访问实现
/// </summary>
public class CustomerRepository : ICustomerRepository
{
    private readonly CrmDbContext _context;

    public CustomerRepository(CrmDbContext context)
    {
        _context = context;
    }

    public async Task<Customer?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Customers
            .Where(c => c.Id == id && !c.IsDeleted)
            .AsNoTracking()
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<Customer?> GetByIdIncludingDeletedAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Customers
            .Where(c => c.Id == id)
            .AsNoTracking()
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(string companyName, string contactName, Guid? excludeId = null, CancellationToken cancellationToken = default)
    {
        var query = _context.Customers
            .Where(c => !c.IsDeleted && c.CompanyName == companyName && c.ContactName == contactName);

        if (excludeId.HasValue)
        {
            query = query.Where(c => c.Id != excludeId.Value);
        }

        return await query.AnyAsync(cancellationToken);
    }

    public async Task<PagedResponse<Customer>> SearchAsync(CustomerSearchRequest request, CancellationToken cancellationToken = default)
    {
        var query = _context.Customers.Where(c => !c.IsDeleted);

        // 关键词搜索（大小写不敏感，搜索公司名和联系人名）
        // 使用 EF.Functions.ILike 以利用 PostgreSQL 索引
        if (!string.IsNullOrWhiteSpace(request.Keyword))
        {
            var keyword = $"%{request.Keyword}%";
            query = query.Where(c =>
                EF.Functions.ILike(c.CompanyName, keyword) ||
                EF.Functions.ILike(c.ContactName, keyword));
        }

        // 状态筛选
        if (request.Status.HasValue)
        {
            query = query.Where(c => c.Status == request.Status.Value);
        }

        // 行业筛选
        if (!string.IsNullOrWhiteSpace(request.Industry))
        {
            query = query.Where(c => c.Industry == request.Industry);
        }

        // 来源筛选
        if (request.Source.HasValue)
        {
            query = query.Where(c => c.Source == request.Source.Value);
        }

        // 排序
        query = ApplySorting(query, request.SortBy, request.SortOrder);

        // 计算总数
        var total = await query.CountAsync(cancellationToken);

        // 分页参数验证和应用
        var page = Math.Max(1, request.Page ?? 1);
        var pageSize = Math.Min(100, Math.Max(1, request.PageSize ?? 20));

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return new PagedResponse<Customer>
        {
            Items = items,
            Total = total
        };
    }

    public async Task<Customer> CreateAsync(Customer customer, CancellationToken cancellationToken = default)
    {
        _context.Customers.Add(customer);
        await _context.SaveChangesAsync(cancellationToken);
        return customer;
    }

    public async Task<Customer> UpdateAsync(Customer customer, CancellationToken cancellationToken = default)
    {
        _context.Customers.Update(customer);
        await _context.SaveChangesAsync(cancellationToken);
        return customer;
    }

    public async Task<bool> SoftDeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var customer = await _context.Customers
            .Where(c => c.Id == id && !c.IsDeleted)
            .FirstOrDefaultAsync(cancellationToken);

        if (customer == null)
        {
            return false;
        }

        customer.IsDeleted = true;
        customer.UpdatedAt = DateTimeOffset.UtcNow;
        
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    /// <summary>
    /// 应用排序逻辑
    /// </summary>
    private IQueryable<Customer> ApplySorting(IQueryable<Customer> query, string? sortBy, string? sortOrder)
    {
        var isAscending = string.Equals(sortOrder, "asc", StringComparison.OrdinalIgnoreCase);

        return sortBy?.ToLowerInvariant() switch
        {
            "createdat" => isAscending
                ? query.OrderBy(c => c.CreatedAt)
                : query.OrderByDescending(c => c.CreatedAt),
            
            "updatedat" => isAscending
                ? query.OrderBy(c => c.UpdatedAt)
                : query.OrderByDescending(c => c.UpdatedAt),
            
            // 默认按 LastInteractionAt 降序排列（null 值排在最后）
            _ => query.OrderByDescending(c => c.LastInteractionAt ?? DateTimeOffset.MinValue)
        };
    }
}
