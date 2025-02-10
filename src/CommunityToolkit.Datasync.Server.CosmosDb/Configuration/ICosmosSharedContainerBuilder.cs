// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Azure.Cosmos;

namespace CommunityToolkit.Datasync.Server.CosmosDb.Configuration;

public interface ICosmosSharedContainerBuilder
{
    ICosmosSharedContainerBuilder Entity<TEntity>() where TEntity : CosmosTableData<TEntity>;

    ICosmosSharedContainerBuilder Entity<TEntity, TEntityOptions>() where TEntity : CosmosTableData where TEntityOptions : class, ICosmosTableOptions<TEntity>;

    ICosmosSharedContainerBuilder Entity<TEntity, TEntityOptions>(Func<CosmosContainerOptions, TEntityOptions> implementationFactory) where TEntity : CosmosTableData where TEntityOptions : class, ICosmosTableOptions<TEntity>;

    ICosmosSharedContainerBuilder Entity<TEntity, TEntityOptions>(Func<CosmosContainerOptions, IServiceProvider, TEntityOptions> implementationFactory) where TEntity : CosmosTableData where TEntityOptions : class, ICosmosTableOptions<TEntity>;
    ICosmosSharedContainerBuilder WithThroughput(int throughput);
    ICosmosSharedContainerBuilder WithThroughput(ThroughputProperties throughputProperties);

    ICosmosSharedContainerBuilder UseDatasyncIndex();

}
