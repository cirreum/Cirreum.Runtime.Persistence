namespace Microsoft.AspNetCore.Hosting;

using Cirreum.Persistence;
using Cirreum.Persistence.Configuration;
using Cirreum.Persistence.Health;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

public static class HostExtensions {

	private class ConfigurePersistenceMarker { }

	/// <summary>
	/// Add support for data persistence (Database) by registering any configured providers
	/// and associated instances.
	/// </summary>
	public static IHostApplicationBuilder AddPersistence(this IHostApplicationBuilder builder) {

		// Check if already registered using a marker service		
		if (builder.Services.IsMarkerTypeRegistered<ConfigurePersistenceMarker>()) {
			return builder;
		}

		// Mark as registered
		builder.Services.MarkTypeAsRegistered<ConfigurePersistenceMarker>();

		// Service Providers...
		return builder
			.RegisterServiceProvider<
				AzureCosmosRegistrar,
				AzureCosmosSettings,
				AzureCosmosInstanceSettings,
				AzureCosmosHealthCheckOptions>()
			.RegisterServiceProvider<
				SqlServerRegistrar,
				SqlServerSettings,
				SqlServerInstanceSettings,
				SqlServerHealthCheckOptions>();

		// .RegisterServiceProvider<AWSNoSqlPersistenceRegistrar>();

	}

}