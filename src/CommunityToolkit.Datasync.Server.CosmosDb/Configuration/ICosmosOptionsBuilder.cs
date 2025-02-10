// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Azure;
using Azure.Core;
using Microsoft.Azure.Cosmos;

namespace CommunityToolkit.Datasync.Server.CosmosDb.Configuration;

/// <summary>
/// Interface for configuring Cosmos repositories
/// </summary>
public interface ICosmosOptionsBuilder
{
    /// <summary>
    /// Intializes the Cosmos client with a connection string and optional <see cref="CosmosClientOptions"/>
    /// </summary>
    ICosmosOptionsBuilderWithClient UseConnectionString(string connectionString, CosmosClientOptions? clientOptions = null);
    /// <summary>
    /// Intializes the Cosmos client with a token credential and optional <see cref="CosmosClientOptions"/>
    /// </summary>
    /// <param name="accountEndpoint"></param>
    /// <param name="tokenCredential"></param>
    /// <param name="clientOptions"></param>
    /// <returns></returns>
    ICosmosOptionsBuilderWithClient UseTokenCredential(string accountEndpoint, TokenCredential tokenCredential, CosmosClientOptions? clientOptions = null);

    ICosmosOptionsBuilderWithClient UseAuthKeyOrResourceToken(string accountEndpoint, string authKeyOrResourceToken, CosmosClientOptions? clientOptions = null);

    ICosmosOptionsBuilderWithClient UseAzureKeyCredential(string accountEndpoint, AzureKeyCredential azureKeyCredential, CosmosClientOptions? clientOptions = null);

}
/// <summary>
/// Configures database for Cosmos DB.
/// </summary>
public interface ICosmosOptionsBuilderWithClient
{
    /// <summary>
    /// Creates a new database with the specified name.
    /// </summary>
    /// <param name="databaseId">The name of the database to create</param>
    /// <returns>A <see cref="ICosmosDatabaseBuilder" /> database builder to configure additional options</returns>
    ICosmosDatabaseBuilder Database(string databaseId);
    /// <summary>
    /// Creates a new database with the specified name and container. 
    /// Registers options and repositories when all entities derived from <see cref="CosmosTableData{TEntity}" />.
    /// </summary>
    /// <param name="databaseId"></param>
    /// <param name="containerProperties"></param>
    /// <returns></returns>

    ICosmosContainerBuilder Database(string databaseId, ContainerProperties containerProperties, bool shouldUpdateTimestamp = true);
}
