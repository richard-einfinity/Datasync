// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Azure.Cosmos;

namespace CommunityToolkit.Datasync.Server.CosmosDb.Configuration;

/// <summary>
/// A builder for configuring a Cosmos container.
/// </summary>
public interface ICosmosContainerBuilder
{
    /// <summary>
    /// Specifies the throughput for the container. Ignored if the container already exists.
    /// </summary>
    /// <param name="throughput"></param>
    /// <returns>A <see cref="ICosmosContainerBuilder"/> to continue configuration of the container</returns>
    ICosmosContainerBuilder WithThroughput(int throughput);
    /// <summary>
    /// Specifies the throughput for the container. Ignored if the container already exists.
    /// </summary>
    /// <param name="throughputProperties">A <see cref="ThroughputProperties"/> specifying the throughput options for the container.</param>
    /// <returns>A <see cref="ICosmosContainerBuilder"/> to continue configuration of the container</returns>
    ICosmosContainerBuilder WithThroughput(ThroughputProperties throughputProperties);
    /// <summary>
    /// Specifies that the container should be created with the necessary indexing policy for Datasync. Ignored if the container already exists.
    /// </summary>
    /// <returns></returns>
    ICosmosContainerBuilder UseDatasyncIndex();
}
