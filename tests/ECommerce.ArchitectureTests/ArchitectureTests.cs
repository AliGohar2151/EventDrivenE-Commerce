namespace ECommerce.ArchitectureTests;

using FluentAssertions;
using NetArchTest.Rules;
using Xunit;

public class ArchitectureTests
{
    private const string DomainNamespace = "ECommerce.Domain";
    private const string ApplicationNamespace = "ECommerce.Application";
    private const string InfrastructureNamespace = "ECommerce.Infrastructure";
    private const string ApiNamespace = "ECommerce.Api";

    [Fact]
    public void Domain_ShouldNot_HaveDependencyOnOtherProjects()
    {
        var assembly = typeof(Domain.AssemblyReference).Assembly;

        var otherProjects = new[]
        {
            ApplicationNamespace,
            InfrastructureNamespace,
            ApiNamespace
        };

        var result = Types.InAssembly(assembly)
            .ShouldNot()
            .HaveDependencyOnAll(otherProjects)
            .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void Application_ShouldNot_HaveDependencyOnInfrastructureOrApi()
    {
        var assembly = typeof(Application.DependencyInjection).Assembly;

        var otherProjects = new[]
        {
            InfrastructureNamespace,
            ApiNamespace
        };

        var result = Types.InAssembly(assembly)
            .ShouldNot()
            .HaveDependencyOnAll(otherProjects)
            .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }
}
