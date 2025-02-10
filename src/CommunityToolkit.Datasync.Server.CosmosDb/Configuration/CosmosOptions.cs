// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Azure;
using Azure.Core;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;

namespace CommunityToolkit.Datasync.Server.CosmosDb.Configuration;

/// <summary>
/// Options to be passed to the CosmosDB service for cosmos initialization.
/// </summary>
public sealed class CosmosOptions : ICosmosOptionsBuilder, ICosmosOptionsBuilderWithClient
{
    /// <summary>
    /// The default indexing index for Datasync.
    /// </summary>
    public static readonly Collection<CompositePath> DatasyncCompositeIndex = [
        new() { Path = "/updatedAt", Order = CompositePathSortOrder.Ascending },
        new() { Path = "/id", Order = CompositePathSortOrder.Ascending }
    ];

    /// <summary>
    /// The default indexing policy for Datasync.
    /// </summary>
    public static readonly IndexingPolicy DefaultDatasyncIndexingPolicy = new()
    {
        CompositeIndexes = { DatasyncCompositeIndex }
    };

    private readonly Dictionary<string, CosmosDatabaseOptions> databases = new Dictionary<string, CosmosDatabaseOptions>();
    private readonly List<ServiceDescriptor> services = new List<ServiceDescriptor>();

    internal CosmosClient? CosmosClient { get; private set; }
    internal IReadOnlyCollection<CosmosDatabaseOptions> Databases => this.databases.Values;
    internal IReadOnlyCollection<ServiceDescriptor> Services => this.services;
    internal CosmosOptions() { }

    ICosmosOptionsBuilderWithClient ICosmosOptionsBuilder.UseConnectionString(string connectionString, CosmosClientOptions? clientOptions)
    {
        CosmosClient = new CosmosClient(connectionString, clientOptions);
        return this;
    }

    ICosmosOptionsBuilderWithClient ICosmosOptionsBuilder.UseTokenCredential(string accountEndpoint, TokenCredential tokenCredential, CosmosClientOptions? clientOptions)
    {
        CosmosClient = new CosmosClient(accountEndpoint, tokenCredential, clientOptions);
        return this;
    }

    ICosmosOptionsBuilderWithClient ICosmosOptionsBuilder.UseAuthKeyOrResourceToken(string accountEndpoint, string authKeyOrResourceToken, CosmosClientOptions? clientOptions)
    {
        CosmosClient = new CosmosClient(accountEndpoint, authKeyOrResourceToken, clientOptions);
        return this;
    }

    ICosmosOptionsBuilderWithClient ICosmosOptionsBuilder.UseAzureKeyCredential(string accountEndpoint, AzureKeyCredential azureKeyCredential, CosmosClientOptions? clientOptions)
    {
        CosmosClient = new CosmosClient(accountEndpoint, azureKeyCredential, clientOptions);
        return this;
    }

    ICosmosDatabaseBuilder ICosmosOptionsBuilderWithClient.Database(string databaseName) => Database(databaseName);

    internal CosmosDatabaseOptions Database(string databaseName)
    {
        if (string.IsNullOrEmpty(databaseName))
        {
            throw new ArgumentException($"'{nameof(databaseName)}' cannot be null or empty.", nameof(databaseName));
        }

        if (this.databases.ContainsKey(databaseName))
        {
            throw new InvalidOperationException($"Database {databaseName} has already been added");
        }

        CosmosDatabaseOptions databaseBuilder = new(databaseName, this.services);

        this.databases.Add(databaseName, databaseBuilder);

        return databaseBuilder;
    }

    internal CosmosOptions Validate()
    {
        if (CosmosClient == null)
        {
            throw new InvalidOperationException("CosmosClient must be configured before building");
        }

        if (this.databases.Count == 0)
        {
            throw new InvalidOperationException("At least one database must be configured");
        }

        if (this.services.Count == 0)
        {
            throw new InvalidOperationException("At least one entity must be added");
        }

        return this;
    }

    ICosmosContainerBuilder ICosmosOptionsBuilderWithClient.Database(string databaseId, ContainerProperties containerProperties, bool shouldUpdateTimestamp)
    {
        CosmosDatabaseOptions databaseBuilder = Database(databaseId);

        CosmosContainerOptions containerOptions = databaseBuilder.Container(containerProperties, shouldUpdateTimestamp);

        this.services.Add(ServiceDescriptor.Singleton(containerOptions));
        this.services.Add(ServiceDescriptor.Singleton(typeof(ICosmosTableOptions<>), typeof(CosmosSharedTableOptions<>)));
        this.services.Add(ServiceDescriptor.Singleton(typeof(IRepository<>), typeof(CosmosTableRepository<>)));

        return containerOptions;
    }
}
