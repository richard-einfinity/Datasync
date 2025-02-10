// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace CommunityToolkit.Datasync.Server.CosmosDb.Configuration;

/// <summary>
/// Interface for a service that can create and delete the Cosmos database and containers.
/// </summary>
public interface ICosmosService
{
    /// <summary>
    /// Ensure that the database and containers are deleted. NOT RECOMMENDED FOR PRODUCTION USE.
    /// </summary>
    /// <returns>A <see cref="Task"/> that when completed will remove all configured databases from the account</returns>
    Task EnsureDeletedAsync();
    /// <summary>
    /// Ensure that the database and containers are created.
    /// </summary>
    /// <returns>
    /// A <see cref="Task" />That when completed will ensure all the databases and containers
    /// are available in the configured Cosmos account. If the database or containers already exist,
    /// they will not be modified.
    /// </returns>

    Task EnsureCreatedAsync();

}
