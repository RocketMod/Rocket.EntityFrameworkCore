﻿using Rocket.API.Plugins;

namespace Rocket.EntityFrameworkCore
{
    public static class EntityFrameworkCoreExtensions
    {
        public static IEntityFrameworkBuilder GetEntityFrameworkCoreBuilder(this IPlugin plugin)
        {
            var service = plugin.Container.Resolve<IEntityFrameworkService>();
            return new EntityFrameworkBuilder(service, plugin);
        }

        public static T GetDbContext<T>(this IPlugin plugin) where T : PluginDbContext
        {
            return plugin.Container.Resolve<IEntityFrameworkService>().GetDbContext<T>(plugin);
        }

        public static void Migrate(this IPlugin plugin, PluginDbContext context)
        {
            plugin.Container.Resolve<IEntityFrameworkService>().Migrate(plugin, context);
        }
    }
}