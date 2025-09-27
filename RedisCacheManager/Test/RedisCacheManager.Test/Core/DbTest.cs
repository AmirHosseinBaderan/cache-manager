using Microsoft.Extensions.DependencyInjection;
using CacheManager.Configuration;
using CacheManager.Core;

namespace RedisCacheManager.Test.Core;

public class DbTest
{
    private IServiceCollection _services;

    [SetUp]
    public void SetUp()
    {
        _services = new ServiceCollection();
        _services.AddRedisCacheManager(() => new("127.0.0.1:6379", 1, null, QueueName: "QueueTest"));
    }

    public async Task<ICacheDb?> GetDbService()
    {
        IServiceProvider provider = _services.BuildServiceProvider();
        return provider.GetService<ICacheDb?>();
    }

    [Test]
    public async Task GetDataBaseInstanceWithDefaultConfig()
    {
        var service = await GetDbService();
        if (service is null)
        {
            Assert.Fail("Cant inject services");
            return;
        }

        var db = await service.GetDataBaseAsync();
        if (db is null)
        {
            Assert.Fail("Cant open data base with default configs");
            return;
        }

        Assert.Pass("Data base opend successfuly with default configs");
    }

    [Test]
    public async Task GetDataBaseInstanceWithCustomeConfig()
    {
        var service = await GetDbService();
        if (service is null)
        {
            Assert.Fail("Cant inject services");
            return;
        }

        var db = await service.GetDataBaseAsync("127.0.0.1:6379", 1);
        if (db is null)
        {
            Assert.Fail("Cant open data base with custome configs");
            return;
        }

        Assert.Pass("Data base opend successfuly with custome configs");
    }
}