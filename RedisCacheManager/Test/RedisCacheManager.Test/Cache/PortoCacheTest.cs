using Microsoft.Extensions.DependencyInjection;
using CacheManager.Abstraction;
using CacheManager.Configuration;

namespace RedisCacheManager.Test.Cache;

[TestFixture]
public class PortoCacheTest
{
    private IServiceCollection _services;

    private Person _model;

    private readonly string _key = "Cached-Proto-Item";

    [SetUp]
    public void SetUp()
    {
        _model = new()
        {
            Id = 1,
            Email = "amirhossein@gmail.com",
            Name = "Amir hossein baderan",
        };

        _services = new ServiceCollection();
        _services.AddLogging();
        _services.AddRedisCacheManager(() => new("127.0.0.1:6379", 1, null,QueueName:"QueueTest"));
    }

    public async Task<IProtoCache?> GetService()
    {
        IServiceProvider provider = _services.BuildServiceProvider();
        return provider.GetService<IProtoCache?>();
    }

    [Test, Order(1)]
    public async Task SetCache()
    {
        var service = await GetService();
        if (service is null)
        {
            Assert.Fail("Cant inject services");
            return;
        }

        var result = await service.SetItemAsync(_key, _model);
        if (result != _model)
        {
            Assert.Fail("Set item fail");
            return;
        }

        Assert.Pass("Item set successfuly");
        return;
    }

    [Test, Order(2)]
    public async Task GetOrSetCache()
    {
        var service = await GetService();
        if (service is null)
        {
            Assert.Fail("Cant inject services");
            return;
        }

        var result = await service.GetOrSetItemAsync<Person>(_key + "GetOrSet", async () =>
        {
            Person model = new()
            {
                Id = 2,
                Name = "Amir2",
                Email = "Baderan@gmail2.com",
            };
            return model;
        });
        if (result is not Person { Id: 2, Name: "Amir2", Email: "Baderan@gmail2.com" })
        {
            Assert.Fail("Get or set item fail");
            return;
        }

        Assert.Pass("Item Get or set successfuly");
        return;
    }

    [Test, Order(3)]
    public async Task GetCache()
    {
        var service = await GetService();
        if (service is null)
        {
            Assert.Fail("Cant inject services");
            return;
        }

        var result = await service.GetItemAsync<Person>(_key);
        if (result is Person and
            {
                Id: 1, Email: "amirhossein@gmail.com", Name: "Amir hossein baderan",
            })
        {
            Assert.Pass("Item Get successfuly");
            return;
        }
        Assert.Fail("Get item fail");
        return;
    }
}
