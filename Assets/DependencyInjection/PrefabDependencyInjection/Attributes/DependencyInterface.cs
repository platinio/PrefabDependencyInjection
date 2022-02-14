using System;

namespace Platinio.DependencyInjection
{
    public class DependencyInterface : Dependency
    {
        public Type InterfaceType = null;
        
        public DependencyInterface(Type interfaceType)
        {
            InterfaceType = interfaceType;
            GetFromChildren = false;
        }
    }

}

