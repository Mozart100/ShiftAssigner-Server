using Microsoft.EntityFrameworkCore;
using ShiftAssignerServer.Data;
using ShiftAssignerServer.Models;

namespace ShiftAssignerServer.Services;

/// <summary>
/// Service for managing the main schema registry.
/// Handles tracking of tenant schemas in the central Schema table.
/// </summary>
public interface IMainSchemaService
{
    /// <summary>
    /// Adds a new schema entry to track a tenant's schema in the main database.
    /// </summary>
    /// <param name="companyName">The company name/tenant identifier</param>
    /// <returns>True if the schema was added successfully</returns>
    Task<bool> AddTenantSchemaAsync(string companyName);
    
    /// <summary>
    /// Checks if a tenant schema is already registered.
    /// </summary>
    /// <param name="companyName">The company name/tenant identifier</param>
    /// <returns>True if the schema exists in the registry</returns>
    Task<bool> SchemaExistsAsync(string companyName);
}

/// <summary>
/// Implementation of MainSchemaService that manages tenant schema registry
/// in the main database context.
/// </summary>
public class MainSchemaService : IMainSchemaService
{
    private readonly MainSchemaDbContext _mainContext;

    public MainSchemaService(MainSchemaDbContext mainContext)
    {
        _mainContext = mainContext;
    }

    /// <summary>
    /// Adds a new schema entry to track a tenant's schema in the main database.
    /// This creates a record that tracks which tenant schemas exist in the system.
    /// </summary>
    /// <param name="companyName">The company name/tenant identifier</param>
    /// <returns>True if the schema was added successfully</returns>
    public async Task<bool> AddTenantSchemaAsync(string companyName)
    {
        if (string.IsNullOrWhiteSpace(companyName))
        {
            throw new ArgumentException("Company name cannot be null or empty", nameof(companyName));
        }

        try
        {
            // Check if schema already exists
            var existingSchema = await _mainContext.Schemas
                .FirstOrDefaultAsync(s => s.CompanyName == companyName);

            if (existingSchema != null)
            {
                // Schema already exists, ensure it's active
                if (!existingSchema.IsActive)
                {
                    existingSchema.IsActive = true;
                    await _mainContext.SaveChangesAsync();
                }
                return true;
            }

            // Create new schema entry
            var schemaEntry = new Schema
            {
                CompanyName = companyName,
                IsActive = true
            };

            _mainContext.Schemas.Add(schemaEntry);
            await _mainContext.SaveChangesAsync();

            return true;
        }
        catch (Exception)
        {
            // Log the exception in a real application
            return false;
        }
    }

    /// <summary>
    /// Checks if a tenant schema is already registered in the main database.
    /// </summary>
    /// <param name="companyName">The company name/tenant identifier</param>
    /// <returns>True if the schema exists and is active</returns>
    public async Task<bool> SchemaExistsAsync(string companyName)
    {
        if (string.IsNullOrWhiteSpace(companyName))
        {
            return false;
        }

        try
        {
            return await _mainContext.Schemas
                .AnyAsync(s => s.CompanyName == companyName && s.IsActive);
        }
        catch (Exception)
        {
            // Log the exception in a real application
            return false;
        }
    }
}