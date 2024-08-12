using System;
using Cake.Core.Diagnostics;
using Xunit.Abstractions;

namespace Cake.GitHub.Tests
{
    /// <summary>
    /// Implementation of <see cref="ICakeLog"/> that logs to xunit's <see cref="ITestOutputHelper"/>
    /// </summary>
    internal class XunitCakeLog : ICakeLog
    {
        private readonly ITestOutputHelper _testOutputHelper;


        /// <inheritdoc />
        public Verbosity Verbosity { get; set; }


        public XunitCakeLog(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper ?? throw new ArgumentNullException(nameof(testOutputHelper));
        }

        /// <inheritdoc />
        public void Write(Verbosity verbosity, LogLevel level, string format, params object[] args)
        {
            var message = $"{level.ToString().ToUpper(),-12} | {String.Format(format, args)}";
            _testOutputHelper.WriteLine(message);
        }
    }
}
