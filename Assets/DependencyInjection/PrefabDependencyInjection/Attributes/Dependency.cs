using System;

namespace Platinio.DependencyInjection
{
    public class Dependency : Attribute
    {
        public bool Optional = false;
        public bool GetFromChildren = true;
    }
}

