// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using CommunityToolkit.Datasync.Server.CosmosDb.Extensions;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.DependencyInjection;

namespace CommunityToolkit.Datasync.Server.CosmosDb.Configuration;

public class CosmosContainerOptions : ICosmosSharedContainerBuilder, ICosmosContainerBuilder
{
    private readonly List<ServiceDescriptor> _services;
    internal CosmosContainerOptions(
        string databaseId,
        ContainerProperties containerProperties,
        List<ServiceDescriptor> services,
        bool shouldUpdateTimestamp = true)
    {
        if (string.IsNullOrEmpty(databaseId))
        {
            throw new ArgumentException($"'{nameof(databaseId)}' cannot be null or empty.", nameof(databaseId));
        }

        ContainerProperties = containerProperties ?? throw new ArgumentNullException(nameof(containerProperties));
        DatabaseId = databaseId;
        this._services = services ?? throw new ArgumentNullException(nameof(services));
        ShouldUpdateTimestamp = shouldUpdateTimestamp;
    }
    public ContainerProperties ContainerProperties { get; private set; }
    public bool ShouldUpdateTimestamp { get; }
    public ThroughputProperties? ThroughputProperties { get; private set; }

    public string DatabaseId { get; }
    public string ContainerId => ContainerProperties.Id;

    ICosmosSharedContainerBuilder ICosmosSharedContainerBuilder.Entity<TEntity>()
    {
        _services.ThrowIfExists<TEntity>();

        _services.Add(ServiceDescriptor.Singleton(typeof(ICosmosTableOptions<TEntity>), new CosmosSharedTableOptions<TEntity>(this)));
        _services.Add(ServiceDescriptor.Singleton(typeof(IRepository<TEntity>), typeof(CosmosTableRepository<TEntity>)));

        return this;
    }

    private CosmosContainerOptions WithThroughput(int throughput)
    {
        ThroughputProperties = ThroughputProperties.CreateManualThroughput(throughput);
        return this;
    }

    private CosmosContainerOptions WithThroughput(ThroughputProperties throughputProperties)
    {
        ThroughputProperties = throughputProperties;
        return this;
    }

    private CosmosContainerOptions UseDatasyncIndex()
    {
        if (ContainerProperties.IndexingPolicy == null)
        {
            ContainerProperties.IndexingPolicy = new IndexingPolicy()
            {
                CompositeIndexes = { CosmosOptions.DatasyncCompositeIndex }
            };
        }
        else
        {
            if (!ContainerProperties.IndexingPolicy.CompositeIndexes.Contains(CosmosOptions.DatasyncCompositeIndex))
            {
                ContainerProperties.IndexingPolicy.CompositeIndexes.Add(CosmosOptions.DatasyncCompositeIndex);
            }
        }

        return this;
    }
    ICosmosSharedContainerBuilder ICosmosSharedContainerBuilder.Entity<TEntity, TEntityOptions>()
    {
        _services.ThrowIfExists<TEntity>();

        _services.Add(ServiceDescriptor.Singleton(typeof(ICosmosTableOptions<TEntity>), typeof(TEntityOptions)));
        _services.Add(ServiceDescriptor.Singleton(typeof(IRepository<TEntity>), typeof(CosmosTableRepository<TEntity>)));

        return this;
    }
    ICosmosSharedContainerBuilder ICosmosSharedContainerBuilder.Entity<TEntity, TEntityOptions>(Func<CosmosContainerOptions, TEntityOptions> implementation)
    {
        _services.ThrowIfExists<TEntity>();

        _services.Add(ServiceDescriptor.Singleton(typeof(ICosmosTableOptions<TEntity>), implementation.Invoke(this)));
        _services.Add(ServiceDescriptor.Singleton(typeof(IRepository<TEntity>), typeof(CosmosTableRepository<TEntity>)));
        return this;
    }


    ICosmosSharedContainerBuilder ICosmosSharedContainerBuilder.Entity<TEntity, TEntityOptions>(Func<CosmosContainerOptions, IServiceProvider, TEntityOptions> implementationFactory)
    {
        _services.ThrowIfExists<TEntity>();

        Func<IServiceProvider, TEntityOptions> factory = (sp) => implementationFactory.Invoke(this, sp);

        _services.Add(ServiceDescriptor.Singleton(typeof(ICosmosTableOptions<TEntity>), factory));
        _services.Add(ServiceDescriptor.Singleton(typeof(IRepository<TEntity>), typeof(CosmosTableRepository<TEntity>)));
        return this;

    }

    ICosmosContainerBuilder ICosmosContainerBuilder.WithThroughput(int throughput) => WithThroughput(throughput);

    ICosmosContainerBuilder ICosmosContainerBuilder.WithThroughput(ThroughputProperties throughputProperties) => WithThroughput(throughputProperties);

    ICosmosSharedContainerBuilder ICosmosSharedContainerBuilder.WithThroughput(int throughput) => WithThroughput(throughput);

    ICosmosSharedContainerBuilder ICosmosSharedContainerBuilder.WithThroughput(ThroughputProperties throughputProperties) => WithThroughput(throughputProperties);

    ICosmosSharedContainerBuilder ICosmosSharedContainerBuilder.UseDatasyncIndex() => UseDatasyncIndex();

    ICosmosContainerBuilder ICosmosContainerBuilder.UseDatasyncIndex() => UseDatasyncIndex();
}
