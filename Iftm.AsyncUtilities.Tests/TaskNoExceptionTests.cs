using System;
using System.Threading.Tasks;
using Xunit;
using Iftm.AsyncUtilities;
using System.Threading;

public class TaskNoExceptionTests {
    private static CancellationToken CreateCancelledToken() {
        var cts = new CancellationTokenSource();
        cts.Cancel();
        return cts.Token;
    }

    private static readonly CancellationToken CancelledToken = CreateCancelledToken();

    private static async Task CreateTask(bool cancelled, Exception? exception) {
        await Task.Yield();
        if (cancelled) throw new OperationCanceledException(CancelledToken);
        if (exception != null) throw exception;
    }

    private static async Task<int> CreateTask(bool cancelled, Exception? exception, int result) {
        await Task.Yield();
        if (cancelled) throw new OperationCanceledException(CancelledToken);
        if (exception != null) throw exception;
        return result;
    }

    private static async ValueTask CreateValueTask(bool cancelled, Exception? exception) {
        await Task.Yield();
        if (cancelled) throw new OperationCanceledException(CancelledToken);
        if (exception != null) throw exception;
    }

    private static async ValueTask<int> CreateValueTask(bool cancelled, Exception? exception, int result) {
        await Task.Yield();
        if (cancelled) throw new OperationCanceledException(CancelledToken);
        if (exception != null) throw exception;
        return result;
    }

    [Fact]
    public async Task TaskResult1() {
        var result = await CreateTask(false, null).NoAsyncExceptions(false);
        Assert.False(result.IsCancelled);
        Assert.Null(result.Exception);
    }

    [Fact]
    public async Task TaskCancellation1() {
        var result = await CreateTask(true, null).NoAsyncExceptions(false);
        Assert.True(result.IsCancelled);
        Assert.Null(result.Exception);
    }

    [Fact]
    public async Task TaskException1() {
        var result = await CreateTask(false, new ArgumentException("Test")).NoAsyncExceptions(false);
        Assert.False(result.IsCancelled);
        Assert.True(result.Exception is ArgumentException argEx && argEx.Message == "Test");
    }

    [Fact]
    public async Task TaskResult2() {
        var result = await CreateTask(false, null, 5).NoAsyncExceptions(false);
        Assert.False(result.IsCancelled);
        Assert.Null(result.Exception);
        Assert.Equal(5, result.Value);
    }

    [Fact]
    public async Task TaskCancellation2() {
        var result = await CreateTask(true, null, 5).NoAsyncExceptions(false);
        Assert.True(result.IsCancelled);
        Assert.Null(result.Exception);
        Assert.Equal(0, result.Value);
    }

    [Fact]
    public async Task TaskException2() {
        var result = await CreateTask(false, new ArgumentException("Test"), 5).NoAsyncExceptions(false);
        Assert.False(result.IsCancelled);
        Assert.True(result.Exception is ArgumentException argEx && argEx.Message == "Test");
        Assert.Equal(0, result.Value);
    }

    [Fact]
    public async Task TaskResult3() {
        var result = await CreateValueTask(false, null).NoAsyncExceptions(false);
        Assert.False(result.IsCancelled);
        Assert.Null(result.Exception);
    }

    [Fact]
    public async Task TaskCancellation3() {
        var result = await CreateValueTask(true, null).NoAsyncExceptions(false);
        Assert.True(result.IsCancelled);
        Assert.Null(result.Exception);
    }

    [Fact]
    public async Task TaskException3() {
        var result = await CreateValueTask(false, new ArgumentException("Test")).NoAsyncExceptions(false);
        Assert.False(result.IsCancelled);
        Assert.True(result.Exception is ArgumentException argEx && argEx.Message == "Test");
    }

    public async Task TaskResult4() {
        var result = await CreateValueTask(false, null, 5).NoAsyncExceptions(false);
        Assert.False(result.IsCancelled);
        Assert.Null(result.Exception);
        Assert.Equal(5, result.Value);
    }

    [Fact]
    public async Task TaskCancellation4() {
        var result = await CreateValueTask(true, null, 5).NoAsyncExceptions(false);
        Assert.True(result.IsCancelled);
        Assert.Null(result.Exception);
        Assert.Equal(0, result.Value);
    }

    [Fact]
    public async Task TaskException4() {
        var result = await CreateValueTask(false, new ArgumentException("Test"), 5).NoAsyncExceptions(false);
        Assert.False(result.IsCancelled);
        Assert.True(result.Exception is ArgumentException argEx && argEx.Message == "Test");
        Assert.Equal(0, result.Value);
    }

}
