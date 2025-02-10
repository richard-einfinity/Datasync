// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using System.Net;

namespace CommunityToolkit.Datasync.Server.CosmosDb.Configuration;

internal class CosmosService(CosmosOptions repositoryOptions, ILogger<CosmosService> logger) : ICosmosService
{
    private readonly CosmosOptions _repositoryOptions = repositoryOptions ?? throw new ArgumentNullException(nameof(repositoryOptions));
    private readonly CosmosClient _cosmosClient = repositoryOptions.CosmosClient ?? throw new InvalidOperationException("Cosmos client is not configured");
    private readonly ILogger<CosmosService> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    public async Task EnsureCreatedAsync()
    {
        foreach (CosmosDatabaseOptions database in this._repositoryOptions.Databases)
        {
            this._logger.LogInformation("Creating database {DatabaseName}", database.DatabaseId);

            DatabaseResponse databaseResponse;
            try
            { 
                databaseResponse = await this._cosmosClient.CreateDatabaseIfNotExistsAsync(database.DatabaseId);
            }
            catch (CosmosException ex)
            {
                this._logger.LogError(ex, "Failed to create database {DatabaseName}", database.DatabaseId);
                throw;
            }

            if(databaseResponse.StatusCode == HttpStatusCode.Created)
            {
                this._logger.LogInformation("Database {DatabaseName} created", database.DatabaseId);
            }

            if(databaseResponse.StatusCode == HttpStatusCode.OK)
            {
                this._logger.LogInformation("Database {DatabaseName} already exists", database.DatabaseId);
            }

            Database cosmosDatabase = databaseResponse.Database;

            foreach (CosmosContainerOptions container in database.Containers)
            {
                ContainerResponse containerResponse;
                try
                {
                    containerResponse = await cosmosDatabase.CreateContainerIfNotExistsAsync(container.ContainerProperties, container.ThroughputProperties);
                }
                catch (CosmosException ex)
                {
                    this._logger.LogError(ex, "Failed to create container {ContainerName} in database {DatabaseName}", container.ContainerId, database.DatabaseId);
                    throw;
                }

                if (containerResponse.StatusCode == HttpStatusCode.Created)
                {
                    this._logger.LogInformation("Container {ContainerName} created in database {DatabaseName}", container.ContainerId, database.DatabaseId);
                }

                if (containerResponse.StatusCode == HttpStatusCode.OK)
                {
                    this._logger.LogInformation("Container {ContainerName} already exists in database {DatabaseName} options not applied", container.ContainerId, database.DatabaseId);
                }
            }
        }
    }

    public async Task EnsureDeletedAsync()
    {
        foreach (CosmosDatabaseOptions database in this._repositoryOptions.Databases)
        {
            this._logger.LogInformation("Deleting database {DatabaseName}", database.DatabaseId);
            try
            {
                _ = await this._cosmosClient.GetDatabase(database.DatabaseId).DeleteAsync();
            }
            catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                this._logger.LogInformation("Database {DatabaseName} does not exist", database.DatabaseId);
                // Database does not exist
            }
        }
    }
}
