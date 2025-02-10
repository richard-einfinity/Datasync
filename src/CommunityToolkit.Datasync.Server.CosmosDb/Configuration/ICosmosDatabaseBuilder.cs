// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Azure.Cosmos;

namespace CommunityToolkit.Datasync.Server.CosmosDb.Configuration;

public interface ICosmosDatabaseBuilder
{
    ICosmosDatabaseBuilder WithThroughput(int throughput);

    ICosmosDatabaseBuilder WithThroughput(ThroughputProperties throughputProperties);

    ICosmosSharedContainerBuilder Container(ContainerProperties containerProperties, bool shouldUpdateTimestamp = true);

    ICosmosContainerBuilder Container<TEntity>(ContainerProperties containerProperties, bool shouldUpdateTimestamp = true) where TEntity : CosmosTableData;

    ICosmosContainerBuilder Container<TEntity, TEntityOptions>(ContainerProperties containerProperties, bool shouldUpdateTimestamp = true) where TEntity : CosmosTableData where TEntityOptions : class, ICosmosTableOptions<TEntity>;

    ICosmosContainerBuilder Container<TEntity, TEntityOptions>(ContainerProperties containerProperties, Func<CosmosContainerOptions, TEntityOptions> implementationFactory, bool shouldUpdateTimestamp = true) where TEntity : CosmosTableData where TEntityOptions : class, ICosmosTableOptions<TEntity>;

    ICosmosContainerBuilder Container<TEntity, TEntityOptions>(ContainerProperties containerProperties, Func<CosmosContainerOptions, IServiceProvider, TEntityOptions> implementationFactory, bool shouldUpdateTimestamp = true) where TEntity : CosmosTableData where TEntityOptions : class, ICosmosTableOptions<TEntity>;

    //TODO: Add support for custom repository builders *TRepository
}
