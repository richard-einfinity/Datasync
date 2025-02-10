// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using CommunityToolkit.Datasync.Server.CosmosDb.Extensions;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.DependencyInjection;

namespace CommunityToolkit.Datasync.Server.CosmosDb.Configuration;

public class CosmosDatabaseOptions : ICosmosDatabaseBuilder
{
    private readonly Dictionary<string, CosmosContainerOptions> containers = new Dictionary<string, CosmosContainerOptions>();
    private readonly List<ServiceDescriptor> services;

    internal IReadOnlyCollection<CosmosContainerOptions> Containers => this.containers.Values;

    internal CosmosDatabaseOptions(string databaseId, List<ServiceDescriptor> services)
    {
        if (string.IsNullOrEmpty(databaseId))
        {
            throw new ArgumentException($"'{nameof(databaseId)}' cannot be null or empty.", nameof(databaseId));
        }

        DatabaseId = databaseId;
        this.services = services ?? throw new ArgumentNullException(nameof(services));
    }

    public string DatabaseId { get; }

    public ThroughputProperties? ThroughputProperties { get; private set; }

    ICosmosSharedContainerBuilder ICosmosDatabaseBuilder.Container(ContainerProperties containerProperties, bool shouldUpdateTimestamp) => Container(containerProperties, shouldUpdateTimestamp);

    internal CosmosContainerOptions Container(ContainerProperties containerProperties, bool shouldUpdateTimestamp = true)
    {
        if (this.containers.ContainsKey(containerProperties.Id))
        {
            throw new InvalidOperationException($"Container {containerProperties.Id} has already been added");
        }
        CosmosContainerOptions containerBuilder = new CosmosContainerOptions(DatabaseId, containerProperties, services, shouldUpdateTimestamp);
        this.containers.Add(containerProperties.Id, containerBuilder);
        return containerBuilder;
    }

    ICosmosDatabaseBuilder ICosmosDatabaseBuilder.WithThroughput(int throughput)
    {
        ThroughputProperties = ThroughputProperties.CreateManualThroughput(throughput);
        return this;
    }

    ICosmosDatabaseBuilder ICosmosDatabaseBuilder.WithThroughput(ThroughputProperties throughputProperties)
    {
        ThroughputProperties = throughputProperties;
        return this;
    }

    ICosmosContainerBuilder ICosmosDatabaseBuilder.Container<TEntity>(ContainerProperties containerProperties, bool shouldUpdateTimestamp)
    {
        CosmosContainerOptions options = Container(containerProperties, shouldUpdateTimestamp);

        this.services.ThrowIfExists<TEntity>();

        this.services.Add(ServiceDescriptor.Singleton<ICosmosTableOptions<TEntity>>(new CosmosSingleTableOptions<TEntity>(options)));
        this.services.Add(ServiceDescriptor.Singleton<IRepository<TEntity>, CosmosTableRepository<TEntity>>());

        return options;
    }

    ICosmosContainerBuilder ICosmosDatabaseBuilder.Container<TEntity, TEntityOptions>(ContainerProperties containerProperties, bool shouldUpdateTimestamp)
    {
        CosmosContainerOptions containerOptions = Container(containerProperties, shouldUpdateTimestamp);

        this.services.ThrowIfExists<TEntity>();

        this.services.Add(ServiceDescriptor.Singleton<ICosmosTableOptions<TEntity>, TEntityOptions>());
        this.services.Add(ServiceDescriptor.Singleton<IRepository<TEntity>, CosmosTableRepository<TEntity>>());

        return containerOptions;
    }

    ICosmosContainerBuilder ICosmosDatabaseBuilder.Container<TEntity, TEntityOptions>(ContainerProperties containerProperties, Func<CosmosContainerOptions, TEntityOptions> implementationFactory, bool shouldUpdateTimestamp)
    {
        CosmosContainerOptions containerOptions = Container(containerProperties, shouldUpdateTimestamp);

        this.services.ThrowIfExists<TEntity>();

        this.services.Add(ServiceDescriptor.Singleton<ICosmosTableOptions<TEntity>>(implementationFactory.Invoke(containerOptions)));
        this.services.Add(ServiceDescriptor.Singleton<IRepository<TEntity>, CosmosTableRepository<TEntity>>());
        return containerOptions;
    }

    ICosmosContainerBuilder ICosmosDatabaseBuilder.Container<TEntity, TEntityOptions>(ContainerProperties containerProperties, Func<CosmosContainerOptions, IServiceProvider, TEntityOptions> implementationFactory, bool shouldUpdateTimestamp)
    {
        CosmosContainerOptions containerOptions = Container(containerProperties, shouldUpdateTimestamp);

        this.services.ThrowIfExists<TEntity>();

        Func<IServiceProvider, TEntityOptions> factory = sp => implementationFactory.Invoke(containerOptions, sp);
        this.services.Add(ServiceDescriptor.Singleton<ICosmosTableOptions<TEntity>>(factory));
        this.services.Add(ServiceDescriptor.Singleton<IRepository<TEntity>, CosmosTableRepository<TEntity>>());
        return containerOptions;
    }
}
