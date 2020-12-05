# Iftm.AsyncUtilities

This project contains several useful utilities for dealing with async code.

## NuGet

The binaries from this repository are available as the [Iftm.AsyncUtilities](https://www.nuget.org/packages/Iftm.AsyncUtilities/) NuGet packge.

## Usage

To use this project, get it from NuGet and then add the Iftm.AsyncUtilities namespace in your .cs files:

```C#
using Iftm.AsyncUtilities
```

## Documentation

### NoAsyncExceptions

The NoAsyncExceptions extension methods provide awaiters that don't throw exceptions if the awaited task is faulted or cancelled. Instead, they return
a TaskResult or TaskResult&lt;T&gt; which contain the IsCancelled, Exception (null if no exception) and Result properies.

For example, instead of writing

```C#
await Task.Delay(1000, cancellationToken);
```

you can write:

```C#
var result = await Task.Delay(1000, cancellationToken).NoAsyncExceptions();
if (result.IsCancelled) {
	// task was cancelled
}
else {
	// task completed successfully
}
```



