using Microsoft.EntityFrameworkCore;
using Vote.Monitor.Core.Exceptions;
using Vote.Monitor.Domain.DataContext;
using Vote.Monitor.Domain.Models;
using EFCore.BulkExtensions;
using Microsoft.AspNetCore.Mvc.Formatters;

namespace Vote.Monitor.Feature.PollingStation.Repositories;
internal class PollingStationRepository : IPollingStationRepository
{
    private readonly AppDbContext _context;

    //temp 
    public PollingStationRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<PollingStationModel> AddAsync(PollingStationModel entity)

    {
        if (entity.Tags == null || entity.Tags.Count == 0) throw new ArgumentException("At least 1 tag is required!");

        var duplicateTag = entity.Tags.GroupBy(x => x.Key).Where(g => g.Count() > 1).Select(y => y.Key).FirstOrDefault();
        if (duplicateTag != null) throw new ArgumentException($"Duplicate tag key: {duplicateTag}");

        List<TagModel> tags = new List<TagModel>();
        foreach (var tag in entity.Tags)
        {
            var efTag = _context.Tags.FirstOrDefault(x => x.Key == tag.Key && x.Value == tag.Value);
            if (efTag != null)
            {
                tags.Add(efTag);
            }
            else tags.Add(tag);
        }
        entity.Tags = tags;
        await _context.PollingStations.AddAsync(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<PollingStationModel> GetByIdAsync(Guid id)
    {
        var pollingStation = await _context.PollingStations
            .FirstOrDefaultAsync(ps => ps.Id == id) ??
            throw new NotFoundException<PollingStationModel>($"Polling Station not found for ID: {id}");

        return pollingStation;
    }

    public async Task<IEnumerable<PollingStationModel>> GetAllAsync(int pageSize = 0, int page = 1)
    {
        if (pageSize < 0) throw new ArgumentOutOfRangeException(nameof(pageSize));
        if (pageSize > 0 && page < 1) throw new ArgumentOutOfRangeException(nameof(page));

        if (pageSize == 0) return await _context.PollingStations.OrderBy(st => st.DisplayOrder).ToListAsync();

        return await _context.PollingStations.OrderBy(st => st.DisplayOrder)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

    }

    public async Task<PollingStationModel> UpdateAsync(Guid id, PollingStationModel entity)
    {
        var pollingStation = await _context.PollingStations
           .FirstOrDefaultAsync(ps => ps.Id == id) ??
            throw new NotFoundException<PollingStationModel>($"Polling Station not found for ID: {id}");

        pollingStation.DisplayOrder = entity.DisplayOrder;
        pollingStation.Address = entity.Address;

        //delete orphaned tags that are not in the new list of tags
        var tagsToDelete = pollingStation.Tags.Where(t => !entity.Tags.Any(et => et.Key == t.Key && et.Value == t.Value)).ToList();
        if (tagsToDelete != null)
            foreach (var tag in tagsToDelete) pollingStation.Tags.Remove(tag);

        DeleteOrphanedTags(tagsToDelete);

        foreach (var tag in entity.Tags)
        {
            var tagToUpdate = pollingStation.Tags.FirstOrDefault(t => t.Key == tag.Key && t.Value == tag.Value);
            if (tagToUpdate != null) continue;
            var tagToAdd = await _context.Tags.FirstOrDefaultAsync(t => t.Key == tag.Key && t.Value == tag.Value);

            if (tagToAdd == null)
            {
                tagToAdd = new TagModel
                {
                    Key = tag.Key,
                    Value = tag.Value
                };

                _context.Tags.Add(tagToAdd);

            }

            pollingStation.Tags.Add(tagToAdd);
        }
        await _context.SaveChangesAsync();
        return pollingStation;
    }

    public async Task DeleteAsync(Guid id)
    {
        var pollingStation = await _context.PollingStations.FirstOrDefaultAsync(ps => ps.Id == id) ??
            throw new NotFoundException<PollingStationModel>($"Polling Station not found for ID: {id}");

        DeleteOrphanedTags(pollingStation.Tags);
        _context.PollingStations.Remove(pollingStation);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAllAsync()
    {

        _context.Tags.RemoveRange(_context.Tags);
        _context.PollingStations.RemoveRange(_context.PollingStations);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<PollingStationModel>> GetAllAsync(List<TagModel>? filterCriteria, int pageSize = 0, int page = 1)
    {
        if (pageSize < 0) throw new ArgumentOutOfRangeException(nameof(pageSize));
        if (pageSize > 0 && page < 1) throw new ArgumentOutOfRangeException(nameof(page));

        if (filterCriteria == null || filterCriteria.Count == 0) return await GetAllAsync(pageSize, page);

        if (pageSize == 0) return _context.PollingStations.AsEnumerable().Where(
            station => filterCriteria.Count(filter => filterCriteria.All
                (tag => station.Tags.Any(t => t.Key == tag.Key && t.Value == tag.Value))) == filterCriteria.Count
              ).OrderBy(st => st.DisplayOrder);

        return _context.PollingStations.AsEnumerable().Where(
            station => filterCriteria.Count(filter => filterCriteria.All(tag => station.Tags.Any(t => t.Key == tag.Key && t.Value == tag.Value))) == filterCriteria.Count
              ).OrderBy(st => st.DisplayOrder)
            .Skip((page - 1) * pageSize)
            .Take(pageSize);
    }

    public async Task<int> CountAsync(List<TagModel>? filterCriteria)
    {
        if (filterCriteria == null || filterCriteria.Count == 0) return await _context.PollingStations.CountAsync();

        return _context.PollingStations.AsEnumerable().Where(
            station => filterCriteria.Count(filter => filterCriteria.All(tag => station.Tags.Any(t => t.Key == tag.Key && t.Value == tag.Value))) == filterCriteria.Count
              ).Count();
    }

    public async Task<int> BulkInsertAsync(List<PollingStationModel> records)
    {
        await _context.BulkInsertAsync(records);

        return records.Count;
    }

    private void DeleteOrphanedTags(IEnumerable<TagModel>? tagsToDelete)
    {
        if (tagsToDelete == null) return;
        // return all the from context that have ids that are in tagsToDelete and that have only one polling station
        foreach (var tag in tagsToDelete)
        {
            var orphanedTag = _context.Tags.Include(t => t.PollingStationTags).FirstOrDefault(t => t.Id == tag.Id && t.PollingStationTags.Count == 1);
            if (orphanedTag != null) _context.Tags.Remove(orphanedTag);
        }
    }

}
