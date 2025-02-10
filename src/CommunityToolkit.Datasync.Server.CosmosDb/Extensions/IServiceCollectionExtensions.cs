// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using CommunityToolkit.Datasync.Server;
using CommunityToolkit.Datasync.Server.CosmosDb.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace CommunityToolkit.Datasync.Server.CosmosDb.Extensions;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddCosmosRepositories(
        this IServiceCollection services,
        Action<ICosmosOptionsBuilder> configure)
    {
        CosmosOptions options = new();
        configure(options);

        options.Validate();

        services.AddSingleton(options.CosmosClient ?? throw new InvalidOperationException("Cosmos client is not configured"));
        services.AddSingleton(options);

        services.AddScoped<ICosmosService, CosmosService>();

        services.TryAdd(options.Services);

        return services;
    }

    internal static void ThrowIfExists<TEntity>(this IEnumerable<ServiceDescriptor> serviceDescriptors) where TEntity : CosmosTableData

    {
        if (serviceDescriptors.Any(descriptor => descriptor.ServiceType == typeof(IRepository<TEntity>)))
        {
            throw new InvalidOperationException($"Repository for {typeof(TEntity).Name} is already registered");
        }

        if (serviceDescriptors.Any(s => s.ServiceType == typeof(ICosmosTableOptions<TEntity>)))
        {
            throw new InvalidOperationException($"Entity {typeof(TEntity).Name} has already been added");
        }
    }
}
