using Microsoft.EntityFrameworkCore;
using CrmSystem.Api.Data;
using CrmSystem.Api.DTOs;
using CrmSystem.Api.Exceptions;
using CrmSystem.Api.Models;
using CrmSystem.Api.Repositories;

namespace CrmSystem.Api.Services;

/// <summary>
/// 客户业务逻辑实现
/// </summary>
public class CustomerService : ICustomerService
{
    private readonly ICustomerRepository _customerRepository;
    private readonly CrmDbContext _context;
    private readonly ILogger<CustomerService> _logger;

    public CustomerService(
        ICustomerRepository customerRepository,
        CrmDbContext context,
        ILogger<CustomerService> logger)
    {
        _customerRepository = customerRepository;
        _context = context;
        _logger = logger;
    }

    public async Task<Customer> CreateCustomerAsync(CreateCustomerRequest request, CancellationToken cancellationToken = default)
    {
        // 检查唯一性约束
        var exists = await _customerRepository.ExistsAsync(
            request.CompanyName,
            request.ContactName,
            null,
            cancellationToken);

        if (exists)
        {
            throw new ConflictException("A customer with the same company name and contact name already exists");
        }

        var customer = new Customer
        {
            Id = Guid.NewGuid(),
            CompanyName = request.CompanyName,
            ContactName = request.ContactName,
            Wechat = request.Wechat,
            Phone = request.Phone,
            Email = request.Email,
            Industry = request.Industry,
            Source = request.Source,
            Status = request.Status,
            Tags = request.Tags,
            Score = request.Score,
            LastInteractionAt = null,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow,
            IsDeleted = false
        };

        try
        {
            return await _customerRepository.CreateAsync(customer, cancellationToken);
        }
        catch (DbUpdateException ex) when (ex.InnerException?.Message.Contains("uq_customer_company_contact") == true)
        {
            // 处理数据库级别的唯一性约束冲突
            throw new ConflictException("A customer with the same company name and contact name already exists");
        }
    }

    public async Task<Customer> GetCustomerByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var customer = await _customerRepository.GetByIdAsync(id, cancellationToken);

        if (customer == null)
        {
            throw new NotFoundException("Customer not found");
        }

        return customer;
    }

    public async Task<Customer> UpdateCustomerAsync(
        Guid id,
        UpdateCustomerRequest request,
        DateTimeOffset? originalUpdatedAt = null,
        CancellationToken cancellationToken = default)
    {
        // 获取现有客户（使用跟踪查询以便更新）
        var customer = await _context.Customers
            .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted, cancellationToken);

        if (customer == null)
        {
            throw new NotFoundException("Customer not found");
        }

        // 验证版本（如果提供了 If-Match）
        if (originalUpdatedAt.HasValue)
        {
            // 使用毫秒精度比较，避免微秒级差异
            var currentMillis = customer.UpdatedAt.ToUnixTimeMilliseconds();
            var providedMillis = originalUpdatedAt.Value.ToUnixTimeMilliseconds();

            if (currentMillis != providedMillis)
            {
                throw new ConcurrencyException("Customer has been modified by another user", customer.UpdatedAt);
            }
        }
        else
        {
            _logger.LogWarning("Update request without If-Match header for customer {CustomerId}", id);
        }

        // 检查唯一性约束（排除当前客户）
        var exists = await _customerRepository.ExistsAsync(
            request.CompanyName,
            request.ContactName,
            id,
            cancellationToken);

        if (exists)
        {
            throw new ConflictException("A customer with the same company name and contact name already exists");
        }

        // 更新字段
        customer.CompanyName = request.CompanyName;
        customer.ContactName = request.ContactName;
        customer.Wechat = request.Wechat;
        customer.Phone = request.Phone;
        customer.Email = request.Email;
        customer.Industry = request.Industry;
        customer.Source = request.Source;
        customer.Status = request.Status;
        customer.Tags = request.Tags;
        customer.Score = request.Score;
        customer.UpdatedAt = DateTimeOffset.UtcNow;

        try
        {
            await _context.SaveChangesAsync(cancellationToken);
            return customer;
        }
        catch (DbUpdateConcurrencyException)
        {
            // EF Core 检测到并发冲突
            var current = await _context.Customers
                .AsNoTracking()
                .FirstAsync(c => c.Id == id, cancellationToken);
            throw new ConcurrencyException("Customer has been modified by another user", current.UpdatedAt);
        }
        catch (DbUpdateException ex) when (ex.InnerException?.Message.Contains("uq_customer_company_contact") == true)
        {
            throw new ConflictException("A customer with the same company name and contact name already exists");
        }
    }

    public async Task DeleteCustomerAsync(
        Guid id,
        DateTimeOffset? originalUpdatedAt = null,
        CancellationToken cancellationToken = default)
    {
        // 获取现有客户（使用跟踪查询以便更新）
        var customer = await _context.Customers
            .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted, cancellationToken);

        if (customer == null)
        {
            throw new NotFoundException("Customer not found");
        }

        // 验证版本（如果提供了 If-Match）
        if (originalUpdatedAt.HasValue)
        {
            var currentMillis = customer.UpdatedAt.ToUnixTimeMilliseconds();
            var providedMillis = originalUpdatedAt.Value.ToUnixTimeMilliseconds();

            if (currentMillis != providedMillis)
            {
                throw new ConcurrencyException("Customer has been modified by another user", customer.UpdatedAt);
            }
        }
        else
        {
            _logger.LogWarning("Delete request without If-Match header for customer {CustomerId}", id);
        }

        // 软删除
        customer.IsDeleted = true;
        customer.UpdatedAt = DateTimeOffset.UtcNow;

        try
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            var current = await _context.Customers
                .AsNoTracking()
                .FirstAsync(c => c.Id == id, cancellationToken);
            throw new ConcurrencyException("Customer has been modified by another user", current.UpdatedAt);
        }
    }

    public async Task<PagedResponse<Customer>> SearchCustomersAsync(
        CustomerSearchRequest request,
        CancellationToken cancellationToken = default)
    {
        return await _customerRepository.SearchAsync(request, cancellationToken);
    }
}
