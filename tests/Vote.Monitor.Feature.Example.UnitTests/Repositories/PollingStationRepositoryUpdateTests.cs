﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;
using Vote.Monitor.Core.Exceptions;
using Vote.Monitor.Domain.DataContext;
using Vote.Monitor.Domain.Models;
using Vote.Monitor.Feature.PollingStation.Repositories;
using Xunit;

namespace Vote.Monitor.Feature.PollingStation.UnitTests.Repositories;
public class PollingStationRepositoryUpdateTests
{

    private void Init(string dbname, out AppDbContext context, out PollingStationRepository repository)
    {
        //,out  DbContextOptionsBuilder<AppDbContext> optionsBuilder ,
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseInMemoryDatabase(dbname);
         context = new AppDbContext(optionsBuilder.Options);
        repository = new PollingStationRepository(context);

    }
    [Fact]
    public async Task UpdateAsync_ShouldUpdatePollingStation()
    {
        Init("TestDb6", out AppDbContext context, out PollingStationRepository repository);

        // Arrange
        var id = 1;
        var entity = new PollingStationModel
        {
            Id = id,
            DisplayOrder = 2,
            Address = "456 Main St",
            Tags = new List<TagModel>
                {
                    new TagModel {Key = "key3", Value = "value3"},
                    new TagModel {Key = "key4", Value = "value4"}
                }
        };
        var pollingStation = new PollingStationModel
        {
            Id = id,
            DisplayOrder = 1,
            Address = "123 Main St",
            Tags = new List<TagModel>
                {
                    new TagModel {Key = "key1", Value = "value1"},
                    new TagModel {Key = "key2", Value = "value2"}
                }
        };
        await repository.AddAsync(pollingStation);


        // Act
        var result = await repository.UpdateAsync(id, entity);

        // Assert

        Assert.Equal(entity.DisplayOrder, result.DisplayOrder);
        Assert.Equal(entity.Address, result.Address);
        Assert.True(result.Tags.Any(t=>t.Key == entity.Tags[0].Key && t.Value == entity.Tags[0].Value), "tags not found");
        Assert.True(result.Tags.Any(t => t.Key == entity.Tags[1].Key && t.Value == entity.Tags[1].Value), "tags not found");
        //todo -check the tags count
        //Assert.True(context.Tags.Count() == 2, "tags count failed");    
    }

    [Fact]
    public async Task UpdateAsync_ShouldThrowNotFoundException()
    {
        // Arrange
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseInMemoryDatabase("TestDb7");
        var context = new AppDbContext(optionsBuilder.Options);
        var repository = new PollingStationRepository(context);
        var id = 1;
        var entity = new PollingStationModel { Id = id, Address = "addr1" };


        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException<PollingStationModel>>(() => repository.UpdateAsync(id, entity));
    }
}