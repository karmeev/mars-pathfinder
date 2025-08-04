using NUnit.Framework;

namespace Nasa.Pathfinder.Tests;

public class TestRunner<TSut, TResult>
{
    private readonly Func<TSut> _arrange;
    private TResult? _result;
    private TSut? _sut;

    private TestRunner(Func<TSut> arrange)
    {
        _arrange = arrange;
    }

    public static TestRunner<TSut, TResult> Arrange(Func<TSut> arrange)
    {
        return new TestRunner<TSut, TResult>(arrange);
    }

    public TestRunner<TSut, TResult> Act(Func<TSut, TResult> act)
    {
        _sut = _arrange();
        _result = act(_sut);
        return this;
    }

    public async Task<TestRunner<TSut, TResult>> ActAsync(Func<TSut, Task<TResult>> act)
    {
        _sut = _arrange();
        _result = await act(_sut);
        return this;
    }

    public void Assert(Action<TResult> assert)
    {
        if (_result is null)
            throw new InvalidOperationException("Act must be called before Assert.");

        assert(_result);
    }

    public async Task<TestRunner<TSut, TResult>> AssertAsync(Func<TResult, Task> assert)
    {
        if (_result is null)
            throw new InvalidOperationException("Act must be called before Assert.");

        await assert(_result);
        return this;
    }
}

public static class TestRunnerExtensions
{
    public static async Task ThenAssertAsync<TSut, TResult>(
        this Task<TestRunner<TSut, TResult>> runnerTask,
        Func<TResult, Task> assert)
    {
        var runner = await runnerTask;
        await runner.AssertAsync(assert);
    }

    public static async Task ThenAssertAsync<TSut, TResult>(
        this Task<TestRunner<TSut, TResult>> runnerTask,
        Action<TResult> assert)
    {
        var runner = await runnerTask;
        runner.Assert(assert);
    }
    
    public static async Task ThenThrowAsync<TSut, TResult, TException>(
        this Task<TestRunner<TSut, TResult>> runnerTask) where TException : Exception
    {
        await Task.CompletedTask;
        
        Assert.ThrowsAsync<TException>(async () =>
        {
            await runnerTask;
        });
    }
}
