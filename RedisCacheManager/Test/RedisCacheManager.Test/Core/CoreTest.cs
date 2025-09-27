using Microsoft.Extensions.DependencyInjection;
using CacheManager.Configuration;
using CacheManager.Core;

namespace RedisCacheManager.Test.Core;

public class CoreTest
{
    private IServiceCollection _services;

    [SetUp]
    public void SetUp()
    {
        _services = new ServiceCollection();
        _services.AddRedisCacheManager(() => new("127.0.0.1:6379", 1, null,QueueName:"QueueTest"));
    }

    [Test]
    public async Task ConnectionWithDefaultConfig()
    {
        IServiceProvider provider = _services.BuildServiceProvider();
        ICacheCore? core = provider.GetService<ICacheCore>();

        if (core is null)
        {
            Assert.Fail("Cant inject services");
            return;
        }

        var connection = await core.ConnectAsync();
        if (connection is null)
            Assert.Fail("Faild to connect with redid db");
        else
            Assert.Pass("Connect to db");
    }

    [Test]
    public async Task ConnectionWithCustomeConfig()
    {
        IServiceProvider provider = _services.BuildServiceProvider();
        ICacheCore? core = provider.GetService<ICacheCore>();

        if (core is null)
        {
            Assert.Fail("Cant inject services");
            return;
        }

        var connection = await core.ConnectAsync("127.0.0.1:6379");
        if (connection is null)
            Assert.Fail("Faild to connect with redid db");
        else
            Assert.Pass("Connect to db");
    }
}
