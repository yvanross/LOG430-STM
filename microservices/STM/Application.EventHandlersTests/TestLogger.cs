﻿using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Application.EventHandlersTests;

public class TestLogger : ILogger
{
    private readonly TestContext _testContext;

    public TestLogger(TestContext testContext)
    {
        _testContext = testContext ?? throw new ArgumentNullException(nameof(testContext));
    }

    public IDisposable BeginScope<TState>(TState state) => null;

    public bool IsEnabled(LogLevel logLevel) => true;

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
    {
        string message = formatter(state, exception);
        _testContext.WriteLine($"{logLevel}: {message}");
    }
}