using System;

namespace Platinio.DependencyInjection
{
    public class InjectableInterfaceDependency : InjectableDependency
    {
        public Type InterfaceType;

        public InjectableInterfaceDependency(Type interfaceType)
        {
            InterfaceType = interfaceType;
        }

    }

}

