using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Platinio.DependencyInjection
{
    public class PrefabDependencyInjection : MonoBehaviour
    {
        [SerializeField] private Transform m_root = null;
        private Transform[] m_children = null;
        private List<Component> m_dependencies = new List<Component>();
        private Dictionary<Type, Component> m_interfaceDependencyDic = new Dictionary<Type, Component>();
       
        private Transform Root =>  m_root == null ? transform : m_root;

        [HideInInspector] public List<DependencyInfo> FailedDependencies = new List<DependencyInfo>();
        
        public void ResolveDependencies()
        {
            m_children = Root.GetComponentsInChildren<Transform>();
            m_interfaceDependencyDic = new Dictionary<Type, Component>();
            
            m_dependencies = GetAllDependencies();
            InjectDependencies();
        }

        private List<Component> GetAllDependencies()
        {
            List<Component> dependencies = new List<Component>();
            FieldInfo[] fields = GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            
            foreach (FieldInfo fieldInfo in fields)
            {
                Dependency dependencyAtt = GetAttributeFromFieldInfo<Dependency>(fieldInfo);
                if(dependencyAtt == null) continue;

                if (dependencyAtt is DependencyInterface dependencyInterface)
                {
                    HandlerInterfaceDependency(dependencyInterface, fieldInfo);
                }
                else
                {
                    HandleDependency(dependencyAtt, fieldInfo, dependencies);
                }
            }

            return dependencies;
        }

        private void HandlerInterfaceDependency(DependencyInterface dependencyInterface, FieldInfo fieldInfo)
        {
            if (m_interfaceDependencyDic.ContainsKey(dependencyInterface.InterfaceType))
            {
                Debug.LogError($"Dependency for interface type {dependencyInterface.InterfaceType} is register multiple times");
            }
            else
            {
                var dependency = GetDependencyInChildrenFromFieldInfo(fieldInfo);
                fieldInfo.SetValue(this, dependency);
                m_interfaceDependencyDic.Add(dependencyInterface.InterfaceType, dependency);
            }
        }

        private void HandleDependency(Dependency dependencyAtt, FieldInfo fieldInfo, List<Component> dependencies)
        {
            var fieldType = fieldInfo.FieldType;
            if (dependencyAtt.GetFromChildren && (fieldType.IsSubclassOf(typeof(Component)) || fieldType == typeof(Component)))
            {
                var dependency = GetDependencyInChildrenFromFieldInfo(fieldInfo);

                if (dependency != null)
                {
                    fieldInfo.SetValue(this, dependency);
                    dependencies.Add(dependency);
                }
                else
                {
                    string componentName = fieldInfo.FieldType.ToString();
                        
                    if (!dependencyAtt.Optional)
                    {
                        Debug.LogError("Can't find prefab dependency of type: " + componentName, gameObject);
                    }
                    else
                    {
                        Debug.Log("Can't find optional dependency of type: " + componentName, gameObject);
                    }
                }
            }
        }

        private Component GetDependencyInChildrenFromFieldInfo(FieldInfo fieldInfo)
        {
            string componentName = fieldInfo.FieldType.ToString();
            componentName = componentName.Replace("UnityEngine.", "");

            foreach (var child in m_children)
            {
                var component = child.GetComponent(componentName);
                if (component != null) return component;
            }

            return null;
        }

        private T GetAttributeFromFieldInfo<T>(FieldInfo fieldInfo) where T : Attribute
        {
            object[] attrs = fieldInfo.GetCustomAttributes(true);

            foreach (var attr in attrs)
            {
                if (attr is T attrType) return attrType;
            }

            return null;
        }

        private void InjectDependencies()
        {
            Component[] dependencyReceiverArray = Root.GetComponentsInChildren<Component>();

            foreach (var dependencyReceiver in dependencyReceiverArray)
            {
                FieldInfo[] fields = GetFieldInfosIncludingBaseClasses(dependencyReceiver.GetType(), BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                
                foreach (FieldInfo fieldInfo in fields)
                {
                    InjectableDependency injectableAtt = GetAttributeFromFieldInfo<InjectableDependency>(fieldInfo);
                    if (injectableAtt == null) continue;

                    if (injectableAtt is InjectableInterfaceDependency injectableInterface)
                    {
                        if (!m_interfaceDependencyDic.ContainsKey(injectableInterface.InterfaceType))
                        {
                            FailedDependencies.Add(new DependencyInfo(dependencyReceiver, fieldInfo));
                            Debug.LogError($"can't find dependency interface value for interface type {injectableInterface.InterfaceType}");
                        }
                        else
                        {
                            var value = fieldInfo.GetValue(dependencyReceiver);

                            if (value == null)
                            {
                                Debug.LogError($"You are trying to resolve a dependency of type {injectableInterface.InterfaceType} with an object of type {fieldInfo.FieldType}");
                                continue;
                            }

                            var type = value.GetType();
                            var propertyInfo = type.GetField("ObjectValue");
                        
                            propertyInfo.SetValue(value, m_interfaceDependencyDic[injectableInterface.InterfaceType]);
                        }
                    }
                    else
                    {
                        var dependency = GetDependency(fieldInfo.FieldType);

                        if (dependency != null)
                        {
                            fieldInfo.SetValue(dependencyReceiver, dependency);
                        }
                        else
                        {
                            FailedDependencies.Add(new DependencyInfo(dependencyReceiver, fieldInfo));
                            Debug.LogError($"can't find dependency value for type {fieldInfo.FieldType}");
                        }
                    }
                }
            }
        }

        private Component GetDependency(Type type)
        {
            foreach (var component in m_dependencies)
            {
                if (component.GetType() == type || component.GetType().IsSubclassOf(type)) return component;
            }

            return null;
        }
        
        public FieldInfo[] GetFieldInfosIncludingBaseClasses(Type type, BindingFlags bindingFlags)
        {
            FieldInfo[] fieldInfos = type.GetFields(bindingFlags);

            // If this class doesn't have a base, don't waste any time
            if (type.BaseType == typeof(object))
            {
                return fieldInfos;
            }
            else
            {   // Otherwise, collect all types up to the furthest base class
                var currentType = type;
                var fieldComparer = new FieldInfoComparer();
                var fieldInfoList = new HashSet<FieldInfo>(fieldInfos, fieldComparer);
                while (currentType != typeof(object))
                {
                    fieldInfos = currentType.GetFields(bindingFlags);
                    fieldInfoList.UnionWith(fieldInfos);
                    currentType = currentType.BaseType;
                }
                return fieldInfoList.ToArray();
            }
        }
    }
    
    public class FieldInfoComparer : IEqualityComparer<FieldInfo>
    {
        public bool Equals(FieldInfo x, FieldInfo y)
        {
            return x.DeclaringType == y.DeclaringType && x.Name == y.Name;
        }

        public int GetHashCode(FieldInfo obj)
        {
            return obj.Name.GetHashCode() ^ obj.DeclaringType.GetHashCode();
        }
    }

    [System.Serializable]
    public class DependencyInfo
    {
        public Component Component;
        public FieldInfo FieldInfo;

        public DependencyInfo(Component component, FieldInfo fieldInfo)
        {
            Component = component;
            FieldInfo = fieldInfo;
        }
    }
}