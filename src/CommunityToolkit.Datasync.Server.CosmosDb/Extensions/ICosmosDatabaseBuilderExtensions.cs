// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using CommunityToolkit.Datasync.Server.CosmosDb.Configuration;
using Microsoft.Azure.Cosmos;

namespace CommunityToolkit.Datasync.Server.CosmosDb.Extensions;

public static class ICosmosDatabaseBuilderExtensions
{
    public static ICosmosContainerBuilder Container<TEntity>(this ICosmosDatabaseBuilder builder, string containerId, string partitionKey = "/id", bool shouldUpdateTimestamp = true)
        where TEntity : CosmosTableData, new() =>
        builder.Container<TEntity>(new ContainerProperties(containerId, partitionKey), shouldUpdateTimestamp).UseDatasyncIndex();

    public static ICosmosContainerBuilder Container<TEntity>(this ICosmosDatabaseBuilder builder, string containerId, string[] partitionKeys, bool shouldUpdateTimestamp = true)
        where TEntity : CosmosTableData, new() =>
        builder.Container<TEntity>(new ContainerProperties(containerId, partitionKeys), shouldUpdateTimestamp).UseDatasyncIndex();

    public static ICosmosContainerBuilder Container<TEntity, TOptions>(this ICosmosDatabaseBuilder builder, string containerId, string partitionKey = "/id", bool shouldUpdateTimestamp = true)
        where TEntity : CosmosTableData, new()
        where TOptions : class, ICosmosTableOptions<TEntity> =>
        builder.Container<TEntity, TOptions>(new ContainerProperties(containerId, partitionKey), shouldUpdateTimestamp).UseDatasyncIndex();

    public static ICosmosContainerBuilder Container<TEntity, TOptions>(this ICosmosDatabaseBuilder builder, string containerId, string[] partitionKeys, bool shouldUpdateTimestamp = true)
        where TEntity : CosmosTableData, new()
        where TOptions : class, ICosmosTableOptions<TEntity> =>
        builder.Container<TEntity, TOptions>(new ContainerProperties(containerId, partitionKeys), shouldUpdateTimestamp).UseDatasyncIndex();

    public static ICosmosContainerBuilder Container<TEntity, TOptions>(
        this ICosmosDatabaseBuilder builder,
        string containerId,
        Func<CosmosContainerOptions, TOptions> implementationFactory,
        string partitionKey = "/id", bool shouldUpdateTimestamp = true)
        where TEntity : CosmosTableData, new()
        where TOptions : class, ICosmosTableOptions<TEntity> =>
        builder.Container<TEntity, TOptions>(new ContainerProperties(containerId, partitionKey), implementationFactory, shouldUpdateTimestamp).UseDatasyncIndex();


    public static ICosmosContainerBuilder Container<TEntity, TOptions>(
        this ICosmosDatabaseBuilder builder,
        string containerId,
        string[] partitionKeys,
        Func<CosmosContainerOptions, TOptions> implementationFactory,
        bool shouldUpdateTimestamp = true)
        where TEntity : CosmosTableData, new()
        where TOptions : class, ICosmosTableOptions<TEntity> =>
        builder.Container<TEntity, TOptions>(new ContainerProperties(containerId, partitionKeys), implementationFactory, shouldUpdateTimestamp).UseDatasyncIndex();

    public static ICosmosContainerBuilder Container<TEntity, TOptions>(
        this ICosmosDatabaseBuilder builder,
        string containerId,
        Func<CosmosContainerOptions, IServiceProvider, TOptions> implementationFactory,
        string partitionKey = "/id", bool shouldUpdateTimestamp = true)
        where TEntity : CosmosTableData, new()
        where TOptions : class, ICosmosTableOptions<TEntity>
        => builder.Container<TEntity, TOptions>(new ContainerProperties(containerId, partitionKey), implementationFactory, shouldUpdateTimestamp).UseDatasyncIndex();

    public static ICosmosContainerBuilder Container<TEntity, TOptions>(
        this ICosmosDatabaseBuilder builder,
        string containerId,
        string[] partitionKeys,
        Func<CosmosContainerOptions, IServiceProvider, TOptions> implementationFactory,
        bool shouldUpdateTimestamp = true)
        where TEntity : CosmosTableData, new()
        where TOptions : class, ICosmosTableOptions<TEntity> =>
        builder.Container<TEntity, TOptions>(new ContainerProperties(containerId, partitionKeys), implementationFactory, shouldUpdateTimestamp)
        .UseDatasyncIndex();

    public static ICosmosSharedContainerBuilder Container(this ICosmosDatabaseBuilder builder, string containerId, string partitionKey = "/entity", bool shouldUpdateTimestamp = true) =>
        builder.Container(new ContainerProperties(containerId, partitionKey), shouldUpdateTimestamp)
        .UseDatasyncIndex();

    public static ICosmosSharedContainerBuilder Container(this ICosmosDatabaseBuilder builder, string containerId, string[] partitionKeys, bool shouldUpdateTimestamp = true)
        => builder.Container(new ContainerProperties(containerId, partitionKeys), shouldUpdateTimestamp)
        .UseDatasyncIndex();
}
