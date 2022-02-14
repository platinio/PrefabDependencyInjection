using System;
using UnityEngine;

namespace Platinio.DependencyInjection
{
    public class MonoTypeRestriction : PropertyAttribute
    {
        public Type RestrictedType = null;

        public MonoTypeRestriction(Type restrictedType)
        {
            RestrictedType = restrictedType;
        }
    }
}