﻿using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Rocket.API;
using Rocket.API.Plugins;

namespace Rocket.EntityFrameworkCore
{
    public abstract class PluginDbContext : DbContext
    {
        private readonly IPlugin _plugin;

        protected PluginDbContext(IPlugin plugin, IEntityFrameworkConnectionDescriptor descriptor)
        {
            ConnectProviderInfo = descriptor.ConnectionProviderInfo;
            _plugin = plugin;
        }

        public ConnectionProviderInfo ConnectProviderInfo { get; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            var databaseProvider = _plugin.Container.ResolveAll<IEntityFrameworkDatabaseProvider>()
                .FirstOrDefault(c => c.DriverName.Equals(ConnectProviderInfo.ProviderName, StringComparison.OrdinalIgnoreCase));

            if (databaseProvider == null)
                throw new Exception("Failed to resolve database provider: " + ConnectProviderInfo.ProviderName);

            var connectionString = ConnectProviderInfo.ConnectionString
                .Replace("{PluginDir}", _plugin.WorkingDirectory)
                .Replace("{PluginName}", _plugin.Name)
                .Replace("{Game}", _plugin.Container.Resolve<IHost>().Name)
                .Replace("{RocketDir}", _plugin.Container.Resolve<IRuntime>().WorkingDirectory);

            databaseProvider.UseFor(optionsBuilder, connectionString);
        }
    }
}