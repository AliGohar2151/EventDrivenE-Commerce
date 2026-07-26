using ECommerce.Domain.Primitives;
using FluentAssertions;
using NetArchTest.Rules;
using Xunit;

namespace ECommerce.ArchitectureTests;

public class ArchitectureTests
{
    private const string DomainNamespace = "ECommerce.Domain";
    private const string ContractsNamespace = "ECommerce.Contracts";
    private const string ApplicationNamespace = "ECommerce.Application";
    private const string InfrastructureNamespace = "ECommerce.Infrastructure";
    private const string ApiNamespace = "ECommerce.Api";

    [Fact]
    public void Domain_ShouldNotHaveDependencyOnOtherProjects()
    {
        var assembly = typeof(IDomainEvent).Assembly;

        var otherProjects = new[]
        {
            ContractsNamespace,
            ApplicationNamespace,
            InfrastructureNamespace,
            ApiNamespace
        };

        var result = Types
            .InAssembly(assembly)
            .ShouldNot()
            .HaveDependencyOnAll(otherProjects)
            .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void Application_ShouldNotHaveDependencyOnInfrastructureOrApi()
    {
        var assembly = typeof(ECommerce.Application.Abstractions.IApplicationDbContext).Assembly;

        var forbiddenProjects = new[]
        {
            InfrastructureNamespace,
            ApiNamespace
        };

        var result = Types
            .InAssembly(assembly)
            .ShouldNot()
            .HaveDependencyOnAll(forbiddenProjects)
            .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void DomainEntities_ShouldResideInDomainEntitiesNamespace()
    {
        var assembly = typeof(IDomainEvent).Assembly;

        var result = Types
            .InAssembly(assembly)
            .That()
            .ResideInNamespace($"{DomainNamespace}.Entities")
            .And()
            .AreClasses()
            .Should()
            .BePublic()
            .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }
}
