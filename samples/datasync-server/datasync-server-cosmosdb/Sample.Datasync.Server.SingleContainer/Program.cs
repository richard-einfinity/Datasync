// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using CommunityToolkit.Datasync.Server;
using CommunityToolkit.Datasync.Server.CosmosDb.Configuration;
using CommunityToolkit.Datasync.Server.CosmosDb.Extensions;
using CommunityToolkit.Datasync.Server.Swashbuckle;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

string connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new ApplicationException("DefaultConnection is not set");

builder.Services.AddCosmosRepositories(options =>
{
    _ = options.UseConnectionString(connectionString, new Microsoft.Azure.Cosmos.CosmosClientOptions()
    {
        UseSystemTextJsonSerializerWithOptions = new()
        {
            PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        }
    }).Database("TodoDb", "TodoContainer");
});
// Add services to the container.

builder.Services.AddDatasyncServices();

builder.Services.AddControllers();

_ = builder.Services
    .AddEndpointsApiExplorer()
    .AddSwaggerGen(options => options.AddDatasyncControllers());

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    _ = app.UseSwagger().UseSwaggerUI();
    _ = app.UseDeveloperExceptionPage();

    using (IServiceScope scope = app.Services.CreateScope())
    {
        ICosmosService context = scope.ServiceProvider.GetRequiredService<ICosmosService>();
        await context.EnsureCreatedAsync();
    }
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
