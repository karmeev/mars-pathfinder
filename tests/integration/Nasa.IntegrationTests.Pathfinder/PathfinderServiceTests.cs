using Grpc.Net.Client;
using NUnit.Framework;
using Pathfinder.Proto;

namespace Nasa.IntegrationTests.Pathfinder;

[TestFixture]
[Category("Integration")]
public class PathfinderServiceTests
{
    private PathfinderService.PathfinderServiceClient _sut;
    
    [SetUp]
    public void Setup()
    {
        _sut = new PathfinderService.PathfinderServiceClient(GrpcChannel.ForAddress("http://localhost:5000"));
    }

    [Test]
    public void Test()
    {
        Assert.Pass();
    }
}