using UnityEngine;

namespace Platinio.DependencyInjection
{
    [System.Serializable]
    public class MonoInterface<T> where T : class
    {
        private T m_value = null;
        
        public T Value
        {
            get
            {
                if (m_value != null) return m_value;
                m_value = ObjectValue as T;

                if (m_value == null)
                {
                    Debug.LogError($"{ObjectValue.GetType()} can't be cast to {typeof(T)}");
                }

                return m_value;
            }
        }

        public Object ObjectValue;
    }
}