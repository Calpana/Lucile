﻿using System;
using System.Reflection;
using Lucile.Configuration.Plugin;
using Lucile.Extensions.DependencyInjection;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class LucileServiceCollectionExtensions
    {
        public static IServiceCollection AddPlugins(this IServiceCollection collection, PluginOptions options, bool ignoreErrors = true)
        {
            foreach (var item in options.Assemblies)
            {
                Assembly assembly = null;
                try
                {
                    var name = new AssemblyName(item);
                    assembly = Assembly.Load(name);
                }
                catch (Exception)
                {
                    if (!ignoreErrors)
                    {
                        throw;
                    }
                }

                collection.FromConfiguration(assembly);
            }

            return collection;
        }

        public static IServiceCollection FromConfiguration(this IServiceCollection collection, Assembly assembly)
        {
            var config = assembly.GetCustomAttribute<ServiceConfigurationAttribute>();
            if (config != null)
            {
                var conf = Activator.CreateInstance(config.ConfigurationType) as IServiceConfiguration;
                if (conf != null)
                {
                    conf.Configure(collection);
                }
            }

            return collection;
        }
    }
}