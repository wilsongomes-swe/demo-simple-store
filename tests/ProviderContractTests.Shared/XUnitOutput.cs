using PactNet.Infrastructure.Outputters;
using System;
using Xunit.Abstractions;

namespace ProviderContractTests.Shared;

public class XUnitOutput : IOutput
{
    private readonly ITestOutputHelper output;

    public XUnitOutput(ITestOutputHelper output)
    {
        this.output = output;
    }

    public void WriteLine(string line)
    {
        this.output.WriteLine(line);
    }
}
