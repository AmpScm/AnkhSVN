using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace AnkhBot
{
	class ServiceProvider
	{
		public ServiceProvider( AnkhBot bot )
		{
			this.bot = bot;
			this.FindServices();
		}		

		public T GetService<T>() where T: class
		{
			if ( services.ContainsKey(typeof(T)) )
				return (T)services[typeof(T)];
			else
				return null;
		}

		private void FindServices()
		{
			this.services = new Dictionary<Type, IService>();
			foreach (Module module in this.GetType().Assembly.GetModules( false ))
			{
				foreach (Type type in module.FindTypes(
					delegate( Type t, object obj )
					{
						return typeof( IService ).IsAssignableFrom( t ) && !t.IsAbstract;
					}, null ))
				{
					IService service = (IService)Activator.CreateInstance( type );
					service.Initialize( this.bot );
					this.services[type] = service;
				}
			}
		}

		private AnkhBot bot;
		private Dictionary<Type, IService> services;
	}
}
