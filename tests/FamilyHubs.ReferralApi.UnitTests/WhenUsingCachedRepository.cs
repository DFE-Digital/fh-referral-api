using Ardalis.Specification;
using FamilyHubs.ReferralApi.Infrastructure.Persistence.Repository;
using FamilyHubs.SharedKernel.Interfaces;
using FluentAssertions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;
using System.Linq.Expressions;

namespace FamilyHubs.ReferralApi.UnitTests;

public class TestEntity : IAggregateRoot
{
    public int Id { get; set; } = default!;
    public string Name { get; set; } = default!;
}

public class TestResult : IAggregateRoot
{
    
}

public class TestSpecificationWithResult : ISpecification<TestEntity, TestResult>
{
    private bool IsCacheEnabled = false;
    public ISpecificationBuilder<TestEntity, TestResult> Query => throw new NotImplementedException();

    public Expression<Func<TestEntity, TestResult>>? Selector => throw new NotImplementedException();

    public Func<IEnumerable<TestResult>, IEnumerable<TestResult>>? PostProcessingAction => throw new NotImplementedException();

    public IDictionary<string, object> Items { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public IEnumerable<WhereExpressionInfo<TestEntity>> WhereExpressions => throw new NotImplementedException();

    public IEnumerable<OrderExpressionInfo<TestEntity>> OrderExpressions => throw new NotImplementedException();

    public IEnumerable<IncludeExpressionInfo> IncludeExpressions => throw new NotImplementedException();

    public IEnumerable<string> IncludeStrings => throw new NotImplementedException();

    public IEnumerable<SearchExpressionInfo<TestEntity>> SearchCriterias => throw new NotImplementedException();

    public int? Take => throw new NotImplementedException();

    public int? Skip => throw new NotImplementedException();

    public bool CacheEnabled { get { return IsCacheEnabled; } }

    public void SetCacheEnabled(bool isCacheEnabled)
    {
        IsCacheEnabled = isCacheEnabled;
    }

    public string? CacheKey
    {
        get { return "Test"; }
    }

    public bool AsNoTracking => throw new NotImplementedException();

    public bool AsSplitQuery => throw new NotImplementedException();

    public bool AsNoTrackingWithIdentityResolution => throw new NotImplementedException();

    public bool IgnoreQueryFilters => throw new NotImplementedException();

    ISpecificationBuilder<TestEntity> ISpecification<TestEntity>.Query => throw new NotImplementedException();

    Func<IEnumerable<TestEntity>, IEnumerable<TestEntity>>? ISpecification<TestEntity>.PostProcessingAction => throw new NotImplementedException();

    public IEnumerable<TestResult> Evaluate(IEnumerable<TestEntity> entities)
    {
        throw new NotImplementedException();
    }

    public bool IsSatisfiedBy(TestEntity entity)
    {
        throw new NotImplementedException();
    }

    IEnumerable<TestEntity> ISpecification<TestEntity>.Evaluate(IEnumerable<TestEntity> entities)
    {
        throw new NotImplementedException();
    }
}

public class TestSpecification : ISpecification<TestEntity>
{
    private bool IsCacheEnabled = false;
    public ISpecificationBuilder<TestEntity> Query => throw new NotImplementedException();

    public IDictionary<string, object> Items { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public IEnumerable<WhereExpressionInfo<TestEntity>> WhereExpressions => throw new NotImplementedException();

    public IEnumerable<OrderExpressionInfo<TestEntity>> OrderExpressions => throw new NotImplementedException();

    public IEnumerable<IncludeExpressionInfo> IncludeExpressions => throw new NotImplementedException();

    public IEnumerable<string> IncludeStrings => throw new NotImplementedException();

    public IEnumerable<SearchExpressionInfo<TestEntity>> SearchCriterias => throw new NotImplementedException();

    public int? Take => throw new NotImplementedException();

    public int? Skip => throw new NotImplementedException();

    public Func<IEnumerable<TestEntity>, IEnumerable<TestEntity>>? PostProcessingAction => throw new NotImplementedException();

    public bool CacheEnabled { get { return IsCacheEnabled; } }

    public void SetCacheEnabled(bool isCacheEnabled)
    {
        IsCacheEnabled = isCacheEnabled;
    }

    public string? CacheKey
    {
        get { return "Test"; }
    }
    
    public bool AsNoTracking => throw new NotImplementedException();

    public bool AsSplitQuery => throw new NotImplementedException();

    public bool AsNoTrackingWithIdentityResolution => throw new NotImplementedException();

    public bool IgnoreQueryFilters => throw new NotImplementedException();

    public IEnumerable<TestEntity> Evaluate(IEnumerable<TestEntity> entities)
    {
        throw new NotImplementedException();
    }

    public bool IsSatisfiedBy(TestEntity entity)
    {
        throw new NotImplementedException();
    }
}

public class WhenUsingCachedRepository : BaseCreateDbUnitTest
{

    [Fact]
    public async Task ThenGetByIdAsync_ShouldReturnCorrectEntityFromCache()
    {
        // Arrange
        var entity = new TestEntity { Id = 1, Name = "Test" };
        MemoryCache myCache = new MemoryCache(new MemoryCacheOptions());
        myCache.CreateEntry(entity);
        var mockLogger = new Mock<ILogger<CachedRepository<TestEntity>>>();
        ApplicationDbContext appDbContext = GetApplicationDbContext();
        var mockSourceRepository = new Mock<EfRepository<TestEntity>>(appDbContext);

        
        mockSourceRepository.Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);

        var repository = new CachedRepository<TestEntity>(myCache, mockLogger.Object, mockSourceRepository.Object);

        // Act
        var result = await repository.GetByIdAsync(1);

        // Assert
        mockSourceRepository.Verify(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()), Times.Once);
        result.Should().BeEquivalentTo(entity);
        myCache.Dispose();
    }

    [Fact]
    public async Task ThenListAsync_ShouldReturnCorrectListFromCache()
    {
        // Arrange
        var entity = new TestEntity { Id = 1, Name = "Test" };
        MemoryCache myCache = new MemoryCache(new MemoryCacheOptions());
        myCache.CreateEntry(entity);
        var mockLogger = new Mock<ILogger<CachedRepository<TestEntity>>>();
        ApplicationDbContext appDbContext = GetApplicationDbContext();
        var mockSourceRepository = new Mock<EfRepository<TestEntity>>(appDbContext);

        var list = new List<TestEntity> { new TestEntity { Id = 1, Name = "Test1" }, new TestEntity { Id = 2, Name = "Test2" } };
        mockSourceRepository.Setup(x => x.ListAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(list);

        var repository = new CachedRepository<TestEntity>(myCache, mockLogger.Object, mockSourceRepository.Object);

        // Act
        var result = await repository.ListAsync();

        // Assert
        mockSourceRepository.Verify(x => x.ListAsync(It.IsAny<CancellationToken>()), Times.Once);
        myCache.Dispose();
    }

    [Fact]
    public async Task ThenListAsyncWithSpecificationWithCacheKeyEnabled_ShouldReturnCorrectListFromCache()
    {
        // Arrange
        var entity = new TestEntity { Id = 1, Name = "Test" };
        MemoryCache myCache = new MemoryCache(new MemoryCacheOptions());
        myCache.CreateEntry(entity);
        var mockLogger = new Mock<ILogger<CachedRepository<TestEntity>>>();
        ApplicationDbContext appDbContext = GetApplicationDbContext();
        var mockSourceRepository = new Mock<EfRepository<TestEntity>>(appDbContext);

        var list = new List<TestEntity> { new TestEntity { Id = 1, Name = "Test1" }, new TestEntity { Id = 2, Name = "Test2" } };
        mockSourceRepository.Setup(x => x.ListAsync(It.IsAny<ISpecification<TestEntity>>() , It.IsAny<CancellationToken>()))
            .ReturnsAsync(list);

        var repository = new CachedRepository<TestEntity>(myCache, mockLogger.Object, mockSourceRepository.Object);
        var spec = new TestSpecification();
        spec.SetCacheEnabled(true);
        ISpecification<TestEntity> specification = spec;

        // Act
        var result = await repository.ListAsync(specification);

        // Assert
        mockSourceRepository.Verify(x => x.ListAsync(It.IsAny<ISpecification<TestEntity>>(), It.IsAny<CancellationToken>()), Times.Once);
        myCache.Dispose();
    }

    [Fact]
    public async Task ThenListAsyncWithSpecificationWithoutCacheKeyEnabled_ShouldReturnCorrectListFromCache()
    {
        // Arrange
        var entity = new TestEntity { Id = 1, Name = "Test" };
        MemoryCache myCache = new MemoryCache(new MemoryCacheOptions());
        myCache.CreateEntry(entity);
        var mockLogger = new Mock<ILogger<CachedRepository<TestEntity>>>();
        ApplicationDbContext appDbContext = GetApplicationDbContext();
        var mockSourceRepository = new Mock<EfRepository<TestEntity>>(appDbContext);

        var list = new List<TestEntity> { new TestEntity { Id = 1, Name = "Test1" }, new TestEntity { Id = 2, Name = "Test2" } };
        mockSourceRepository.Setup(x => x.ListAsync(It.IsAny<ISpecification<TestEntity>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(list);

        var repository = new CachedRepository<TestEntity>(myCache, mockLogger.Object, mockSourceRepository.Object);
        ISpecification<TestEntity> specification = new TestSpecification();

        // Act
        var result = await repository.ListAsync(specification);

        // Assert
        mockSourceRepository.Verify(x => x.ListAsync(It.IsAny<ISpecification<TestEntity>>(), It.IsAny<CancellationToken>()), Times.Once);
        myCache.Dispose();
    }

    [Fact]
    public async Task ThenListAsyncWithSpecificationAndResultWithCacheKeyEnabled_ShouldReturnCorrectListFromCache()
    {
        // Arrange
        var entity = new TestEntity { Id = 1, Name = "Test" };
        MemoryCache myCache = new MemoryCache(new MemoryCacheOptions());
        myCache.CreateEntry(entity);
        var mockLogger = new Mock<ILogger<CachedRepository<TestEntity>>>();
        ApplicationDbContext appDbContext = GetApplicationDbContext();
        var mockSourceRepository = new Mock<EfRepository<TestEntity>>(appDbContext);

        var list = new List<TestEntity> { new TestEntity { Id = 1, Name = "Test1" }, new TestEntity { Id = 2, Name = "Test2" } };
        mockSourceRepository.Setup(x => x.ListAsync<TestResult>(It.IsAny<ISpecification<TestEntity, TestResult>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<TestResult>());

        var repository = new CachedRepository<TestEntity>(myCache, mockLogger.Object, mockSourceRepository.Object);
        var spec = new TestSpecificationWithResult();
        spec.SetCacheEnabled(true);
        ISpecification<TestEntity, TestResult> specification = spec;

        // Act
        var result = await repository.ListAsync<TestResult>(specification);

        // Assert
        mockSourceRepository.Verify(x => x.ListAsync<TestResult>(It.IsAny<ISpecification<TestEntity, TestResult>>(), It.IsAny<CancellationToken>()), Times.Once);
        myCache.Dispose();
    }

    [Fact]
    public async Task ThenListAsyncWithSpecificationAndResultWithoutCacheKeyEnabled_ShouldReturnCorrectListFromCache()
    {
        // Arrange
        var entity = new TestEntity { Id = 1, Name = "Test" };
        MemoryCache myCache = new MemoryCache(new MemoryCacheOptions());
        myCache.CreateEntry(entity);
        var mockLogger = new Mock<ILogger<CachedRepository<TestEntity>>>();
        ApplicationDbContext appDbContext = GetApplicationDbContext();
        var mockSourceRepository = new Mock<EfRepository<TestEntity>>(appDbContext);

        var list = new List<TestEntity> { new TestEntity { Id = 1, Name = "Test1" }, new TestEntity { Id = 2, Name = "Test2" } };
        mockSourceRepository.Setup(x => x.ListAsync<TestResult>(It.IsAny<ISpecification<TestEntity, TestResult>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<TestResult>());

        var repository = new CachedRepository<TestEntity>(myCache, mockLogger.Object, mockSourceRepository.Object);
        ISpecification<TestEntity, TestResult> specification = new TestSpecificationWithResult();

        // Act
        var result = await repository.ListAsync<TestResult>(specification);

        // Assert
        mockSourceRepository.Verify(x => x.ListAsync<TestResult>(It.IsAny<ISpecification<TestEntity, TestResult>>(), It.IsAny<CancellationToken>()), Times.Once);
        myCache.Dispose();
    }

    [Fact]
    public async Task ThenAddAsync_ShouldAddToCache()
    {
        // Arrange
        var entity = new TestEntity { Id = 1, Name = "Test" };
        MemoryCache myCache = new MemoryCache(new MemoryCacheOptions());
        myCache.CreateEntry(entity);
        var mockLogger = new Mock<ILogger<CachedRepository<TestEntity>>>();
        ApplicationDbContext appDbContext = GetApplicationDbContext();
        var mockSourceRepository = new Mock<EfRepository<TestEntity>>(appDbContext);

        mockSourceRepository.Setup(x => x.AddAsync(It.IsAny<TestEntity>(),It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);

        var repository = new CachedRepository<TestEntity>(myCache, mockLogger.Object, mockSourceRepository.Object);

        // Act
        var result = await repository.AddAsync(entity);

        // Assert
        mockSourceRepository.Verify(x => x.AddAsync(It.IsAny<TestEntity>(),It.IsAny<CancellationToken>()), Times.Once);
        myCache.Dispose();
    }

    [Fact]
    public async Task ThenAnyAsync_ShouldThrowNotImplemented()
    {
        // Arrange
        var entity = new TestEntity { Id = 1, Name = "Test" };
        MemoryCache myCache = new MemoryCache(new MemoryCacheOptions());
        myCache.CreateEntry(entity);
        var mockLogger = new Mock<ILogger<CachedRepository<TestEntity>>>();
        ApplicationDbContext appDbContext = GetApplicationDbContext();
        var mockSourceRepository = new Mock<EfRepository<TestEntity>>(appDbContext);

        var repository = new CachedRepository<TestEntity>(myCache, mockLogger.Object, mockSourceRepository.Object);
        CancellationTokenSource source = new CancellationTokenSource();
       

        // Act 
        Func<Task> act = () => repository.AnyAsync(source.Token);


        // Assert
        var exception = await Assert.ThrowsAsync<NotImplementedException>(act);
        myCache.Dispose();
    }

    [Fact]
    public async Task ThenAnyAsyncWithSpecification_ShouldThrowNotImplemented()
    {
        // Arrange
        var entity = new TestEntity { Id = 1, Name = "Test" };
        MemoryCache myCache = new MemoryCache(new MemoryCacheOptions());
        myCache.CreateEntry(entity);
        var mockLogger = new Mock<ILogger<CachedRepository<TestEntity>>>();
        ApplicationDbContext appDbContext = GetApplicationDbContext();
        var mockSourceRepository = new Mock<EfRepository<TestEntity>>(appDbContext);

        var list = new List<TestEntity> { new TestEntity { Id = 1, Name = "Test1" }, new TestEntity { Id = 2, Name = "Test2" } };
        mockSourceRepository.Setup(x => x.ListAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(list);

        var repository = new CachedRepository<TestEntity>(myCache, mockLogger.Object, mockSourceRepository.Object);
        CancellationTokenSource source = new CancellationTokenSource();
        ISpecification<TestEntity> spec = default!;


        // Act 
        Func<Task> act = () => repository.AnyAsync(spec, source.Token);


        // Assert
        var exception = await Assert.ThrowsAsync<NotImplementedException>(act);
        myCache.Dispose();
    }

    [Fact]
    public async Task ThenLCountAsync_ShouldReturnCorrectCountFromCache()
    {
        // Arrange
        var entity = new TestEntity { Id = 1, Name = "Test" };
        MemoryCache myCache = new MemoryCache(new MemoryCacheOptions());
        myCache.CreateEntry(entity);
        var mockLogger = new Mock<ILogger<CachedRepository<TestEntity>>>();
        ApplicationDbContext appDbContext = GetApplicationDbContext();
        var mockSourceRepository = new Mock<EfRepository<TestEntity>>(appDbContext);
        var repository = new CachedRepository<TestEntity>(myCache, mockLogger.Object, mockSourceRepository.Object);
        CancellationTokenSource source = new CancellationTokenSource();
        ISpecification<TestEntity> spec = default!;

        // Act
        var result = await repository.CountAsync(spec, source.Token);

        // Assert
        mockSourceRepository.Verify(x => x.CountAsync(It.IsAny<ISpecification<TestEntity>>(), It.IsAny<CancellationToken>()), Times.Once);
        myCache.Dispose();
    }

    [Fact]
    public async Task ThenDeleteAsync_ShouldReturnCorrectDeleteFromCache()
    {
        // Arrange
        var entity = new TestEntity { Id = 1, Name = "Test" };
        MemoryCache myCache = new MemoryCache(new MemoryCacheOptions());
        myCache.CreateEntry(entity);
        var mockLogger = new Mock<ILogger<CachedRepository<TestEntity>>>();
        ApplicationDbContext appDbContext = GetApplicationDbContext();
        var mockSourceRepository = new Mock<EfRepository<TestEntity>>(appDbContext);

        mockSourceRepository.Setup(x => x.DeleteAsync(It.IsAny<TestEntity>(), It.IsAny<CancellationToken>()));

        var repository = new CachedRepository<TestEntity>(myCache, mockLogger.Object, mockSourceRepository.Object);

        // Act
        await repository.DeleteAsync(entity);

        // Assert
        mockSourceRepository.Verify(x => x.DeleteAsync(It.IsAny<TestEntity>(), It.IsAny<CancellationToken>()), Times.Once);
        myCache.Dispose();
    }

    [Fact]
    public async Task ThenDeleteRangeAsync_ShouldReturnCorrectDeleteFromCache()
    {
        // Arrange
        var entity = new TestEntity { Id = 1, Name = "Test" };
        MemoryCache myCache = new MemoryCache(new MemoryCacheOptions());
        myCache.CreateEntry(entity);
        var mockLogger = new Mock<ILogger<CachedRepository<TestEntity>>>();
        ApplicationDbContext appDbContext = GetApplicationDbContext();
        var mockSourceRepository = new Mock<EfRepository<TestEntity>>(appDbContext);

        var list = new List<TestEntity> { new TestEntity { Id = 1, Name = "Test1" }, new TestEntity { Id = 2, Name = "Test2" } };
        mockSourceRepository.Setup(x => x.DeleteRangeAsync(It.IsAny<IEnumerable<TestEntity>>(), It.IsAny<CancellationToken>()));
        
        var repository = new CachedRepository<TestEntity>(myCache, mockLogger.Object, mockSourceRepository.Object);

        // Act
        await repository.DeleteRangeAsync(list);

        // Assert
        mockSourceRepository.Verify(x => x.DeleteRangeAsync(It.IsAny<IEnumerable<TestEntity>>(), It.IsAny<CancellationToken>()), Times.Once);
        myCache.Dispose();
    }

    [Fact]
    public async Task ThenFirstOrDefaultAsync_ShouldThrowNotImplemented()
    {
        // Arrange
        var entity = new TestEntity { Id = 1, Name = "Test" };
        MemoryCache myCache = new MemoryCache(new MemoryCacheOptions());
        myCache.CreateEntry(entity);
        var mockLogger = new Mock<ILogger<CachedRepository<TestEntity>>>();
        ApplicationDbContext appDbContext = GetApplicationDbContext();
        var mockSourceRepository = new Mock<EfRepository<TestEntity>>(appDbContext);

        mockSourceRepository.Setup(x => x.FirstOrDefaultAsync(It.IsAny<ISpecification<TestEntity>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);

        var repository = new CachedRepository<TestEntity>(myCache, mockLogger.Object, mockSourceRepository.Object);
        CancellationTokenSource source = new CancellationTokenSource();
        ISpecification<TestEntity> spec = default!;


        // Act 
        Func<Task> act = () => repository.FirstOrDefaultAsync(spec, source.Token);


        // Assert
        var exception = await Assert.ThrowsAsync<NotImplementedException>(act);
        myCache.Dispose();
    }

    [Fact]
    public async Task ThenFirstOrDefaultAsyncWithResulkt_ShouldThrowNotImplemented()
    {
        // Arrange
        var entity = new TestEntity { Id = 1, Name = "Test" };
        MemoryCache myCache = new MemoryCache(new MemoryCacheOptions());
        myCache.CreateEntry(entity);
        var mockLogger = new Mock<ILogger<CachedRepository<TestEntity>>>();
        ApplicationDbContext appDbContext = GetApplicationDbContext();
        var mockSourceRepository = new Mock<EfRepository<TestEntity>>(appDbContext);

        mockSourceRepository.Setup(x => x.FirstOrDefaultAsync(It.IsAny<ISpecification<TestEntity, TestResult>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new TestResult());

        var repository = new CachedRepository<TestEntity>(myCache, mockLogger.Object, mockSourceRepository.Object);
        CancellationTokenSource source = new CancellationTokenSource();
        ISpecification<TestEntity, TestResult> spec = default!;


        // Act 
        Func<Task> act = () => repository.FirstOrDefaultAsync(spec, source.Token);


        // Assert
        var exception = await Assert.ThrowsAsync<NotImplementedException>(act);
        myCache.Dispose();
    }

    [Fact]
    public async Task ThenGetBySpecAsync_ShouldThrowNotImplemented()
    {
        // Arrange
        var entity = new TestEntity { Id = 1, Name = "Test" };
        MemoryCache myCache = new MemoryCache(new MemoryCacheOptions());
        myCache.CreateEntry(entity);
        var mockLogger = new Mock<ILogger<CachedRepository<TestEntity>>>();
        ApplicationDbContext appDbContext = GetApplicationDbContext();
        var mockSourceRepository = new Mock<EfRepository<TestEntity>>(appDbContext);

#pragma warning disable CS0612
        mockSourceRepository.Setup(x => x.GetBySpecAsync(It.IsAny<ISpecification<TestEntity>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);
#pragma warning restore CS0612

        var repository = new CachedRepository<TestEntity>(myCache, mockLogger.Object, mockSourceRepository.Object);
        CancellationTokenSource source = new CancellationTokenSource();
        ISpecification<TestEntity> spec = default!;


        // Act 
        Func<Task> act = () => repository.GetBySpecAsync(spec, source.Token);


        // Assert
        var exception = await Assert.ThrowsAsync<NotImplementedException>(act);
        myCache.Dispose();
    }

    [Fact]
    public async Task ThenGetBySpecAsyncWithResulkt_ShouldThrowNotImplemented()
    {
        // Arrange
        var entity = new TestEntity { Id = 1, Name = "Test" };
        MemoryCache myCache = new MemoryCache(new MemoryCacheOptions());
        myCache.CreateEntry(entity);
        var mockLogger = new Mock<ILogger<CachedRepository<TestEntity>>>();
        ApplicationDbContext appDbContext = GetApplicationDbContext();
        var mockSourceRepository = new Mock<EfRepository<TestEntity>>(appDbContext);

#pragma warning disable CS0612 
        mockSourceRepository.Setup(x => x.GetBySpecAsync<TestResult>(It.IsAny<ISpecification<TestEntity, TestResult>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new TestResult());
#pragma warning restore CS0612 



        var repository = new CachedRepository<TestEntity>(myCache, mockLogger.Object, mockSourceRepository.Object);
        CancellationTokenSource source = new CancellationTokenSource();
        ISpecification<TestEntity, TestResult> spec = default!;


        // Act 
        Func<Task> act = () => repository.GetBySpecAsync(spec, source.Token);


        // Assert
        var exception = await Assert.ThrowsAsync<NotImplementedException>(act);
        myCache.Dispose();
    }

    [Fact]
    public async Task ThenSingleOrDefaultAsync_ShouldThrowNotImplemented()
    {
        // Arrange
        var entity = new TestEntity { Id = 1, Name = "Test" };
        MemoryCache myCache = new MemoryCache(new MemoryCacheOptions());
        myCache.CreateEntry(entity);
        var mockLogger = new Mock<ILogger<CachedRepository<TestEntity>>>();
        ApplicationDbContext appDbContext = GetApplicationDbContext();
        var mockSourceRepository = new Mock<EfRepository<TestEntity>>(appDbContext);

        mockSourceRepository.Setup(x => x.SingleOrDefaultAsync(It.IsAny<ISingleResultSpecification<TestEntity>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);

        var repository = new CachedRepository<TestEntity>(myCache, mockLogger.Object, mockSourceRepository.Object);
        CancellationTokenSource source = new CancellationTokenSource();
        ISingleResultSpecification<TestEntity> spec = default!;


        // Act 
        Func<Task> act = () => repository.SingleOrDefaultAsync(spec, source.Token);


        // Assert
        var exception = await Assert.ThrowsAsync<NotImplementedException>(act);
        myCache.Dispose();
    }

    [Fact]
    public async Task ThenSingleOrDefaultAsyncWithResulkt_ShouldThrowNotImplemented()
    {
        // Arrange
        var entity = new TestEntity { Id = 1, Name = "Test" };
        MemoryCache myCache = new MemoryCache(new MemoryCacheOptions());
        myCache.CreateEntry(entity);
        var mockLogger = new Mock<ILogger<CachedRepository<TestEntity>>>();
        ApplicationDbContext appDbContext = GetApplicationDbContext();
        var mockSourceRepository = new Mock<EfRepository<TestEntity>>(appDbContext);

        mockSourceRepository.Setup(x => x.FirstOrDefaultAsync(It.IsAny<ISingleResultSpecification<TestEntity, TestResult>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new TestResult());

        var repository = new CachedRepository<TestEntity>(myCache, mockLogger.Object, mockSourceRepository.Object);
        CancellationTokenSource source = new CancellationTokenSource();
        ISingleResultSpecification<TestEntity, TestResult> spec = default!;


        // Act 
        Func<Task> act = () => repository.SingleOrDefaultAsync(spec, source.Token);


        // Assert
        var exception = await Assert.ThrowsAsync<NotImplementedException>(act);
        myCache.Dispose();
    }

    [Fact]
    public async Task ThenSaveChanges_ShouldBeOK()
    {
        // Arrange
        var entity = new TestEntity { Id = 1, Name = "Test" };
        MemoryCache myCache = new MemoryCache(new MemoryCacheOptions());
        myCache.CreateEntry(entity);
        var mockLogger = new Mock<ILogger<CachedRepository<TestEntity>>>();
        ApplicationDbContext appDbContext = GetApplicationDbContext();
        var mockSourceRepository = new Mock<EfRepository<TestEntity>>(appDbContext);

        mockSourceRepository.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()));

        var repository = new CachedRepository<TestEntity>(myCache, mockLogger.Object, mockSourceRepository.Object);

        // Act
        await repository.SaveChangesAsync();

        // Assert
        mockSourceRepository.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        myCache.Dispose();
    }

    [Fact]
    public async Task ThenUpdateAsync_ShouldBeOK()
    {
        // Arrange
        var entity = new TestEntity { Id = 1, Name = "Test" };
        MemoryCache myCache = new MemoryCache(new MemoryCacheOptions());
        myCache.CreateEntry(entity);
        var mockLogger = new Mock<ILogger<CachedRepository<TestEntity>>>();
        ApplicationDbContext appDbContext = GetApplicationDbContext();
        var mockSourceRepository = new Mock<EfRepository<TestEntity>>(appDbContext);

        mockSourceRepository.Setup(x => x.UpdateAsync(It.IsAny<TestEntity>(), It.IsAny<CancellationToken>()));

        var repository = new CachedRepository<TestEntity>(myCache, mockLogger.Object, mockSourceRepository.Object);

        // Act
        await repository.UpdateAsync(entity);

        // Assert
        mockSourceRepository.Verify(x => x.UpdateAsync(It.IsAny<TestEntity>(), It.IsAny<CancellationToken>()), Times.Once);
        myCache.Dispose();
    }

}
