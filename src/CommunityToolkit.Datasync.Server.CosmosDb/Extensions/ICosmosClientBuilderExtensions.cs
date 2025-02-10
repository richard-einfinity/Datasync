// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using CommunityToolkit.Datasync.Server.CosmosDb.Configuration;
using Microsoft.Azure.Cosmos;

namespace CommunityToolkit.Datasync.Server.CosmosDb.Extensions;

public static class ICosmosClientBuilderExtensions
{

    public static ICosmosContainerBuilder Database(this ICosmosOptionsBuilderWithClient builder, string databaseId, string containerId, string partitionKey = "/entity", bool shouldUpdateTimestamp = true)
        => builder.Database(databaseId, new ContainerProperties(containerId, partitionKey), shouldUpdateTimestamp).UseDatasyncIndex();

}
