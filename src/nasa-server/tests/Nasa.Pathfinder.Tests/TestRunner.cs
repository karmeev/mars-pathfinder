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
    
    public void ActAndAssert(Action<TSut> actAndAssert)
    {
        _sut = _arrange();
        actAndAssert(_sut);
    }
}

public class TestRunner<TSut>
{
    private readonly Func<TSut> _arrange;
    private TSut? _sut;

    private TestRunner(Func<TSut> arrange)
    {
        _arrange = arrange;
    }

    public static TestRunner<TSut> Arrange(Func<TSut> arrange)
    {
        return new TestRunner<TSut>(arrange);
    }

    public TestRunner<TSut> Act(Action<TSut> act)
    {
        _sut = _arrange();
        act(_sut);
        return this;
    }

    public async Task<TestRunner<TSut>> ActAsync(Func<TSut, Task> act)
    {
        _sut = _arrange();
        await act(_sut);
        return this;
    }

    public void Assert(Action assert)
    {
        assert();
    }

    public async Task<TestRunner<TSut>> AssertAsync(Func<Task> assert)
    {
        await assert();
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
    
    public static async Task ThenAssertDoesNotThrowAsync<TSut, TResult>(
        this Task<TestRunner<TSut, TResult>> runnerTask,
        Func<TResult, Task> assert)
    {
        Assert.DoesNotThrowAsync(async () =>
        {
            var runner = await runnerTask;
            await runner.AssertAsync(assert);
        });
    }

    public static async Task ThenAssertDoesNotThrowAsync<TSut, TResult>(
        this Task<TestRunner<TSut, TResult>> runnerTask,
        Action<TResult> assert)
    {
        Assert.DoesNotThrowAsync(async () =>
        {
            await runnerTask;
        });
    }
    
    public static async Task ThenAssertThrowsAsync<TSut, TResult, TException>(
        this Task<TestRunner<TSut, TResult>> runnerTask) where TException : Exception
    {
        await Task.CompletedTask;
        
        Assert.ThrowsAsync<TException>(async () =>
        {
            await runnerTask;
        });
    }

    /*---- Void ----*/
    
    public static async Task ThenAssertAsync<TSut>(
        this Task<TestRunner<TSut>> runnerTask,
        Func<Task> assert)
    {
        var runner = await runnerTask;
        await runner.AssertAsync(assert);
    }

    public static async Task ThenAssertAsync<TSut>(
        this Task<TestRunner<TSut>> runnerTask,
        Action assert)
    {
        var runner = await runnerTask;
        runner.Assert(assert);
    }
    
    public static async Task ThenAssertThrowsAsync<TSut, TException>(
        this Task<TestRunner<TSut>> runnerTask) where TException : Exception
    {
        await Task.CompletedTask;

        Assert.ThrowsAsync<TException>(async () =>
        {
            await runnerTask;
        });
    }
    
    public static async Task ThenAssertDoesNotThrowAsync<TSut>(
        this Task<TestRunner<TSut>> runnerTask)
    {
        Assert.DoesNotThrowAsync(async () =>
        {
            await runnerTask;
        });
    }
}
