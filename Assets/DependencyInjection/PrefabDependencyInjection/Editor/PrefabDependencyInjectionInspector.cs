using System.Collections.Generic;
using Platinio.SDK.EditorTools;
using UnityEditor;
using UnityEditor.Experimental.SceneManagement;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Platinio.DependencyInjection
{
    [CustomEditor(typeof(PrefabDependencyInjection))]
    public class PrefabDependencyInjectionInspector : Editor
    {
        private bool m_showResolvedDependencies = false;
        
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            var prefabDependencyInjection = (target as PrefabDependencyInjection);
            if (prefabDependencyInjection == null) return;
            
            if (GUILayout.Button("Resolve Dependencies"))
            {
                m_showResolvedDependencies = true;
                prefabDependencyInjection.FailedDependencies = new List<DependencyInfo>();
                prefabDependencyInjection.ResolveDependencies();
                serializedObject.ApplyModifiedProperties();
                
                var prefabStage = PrefabStageUtility.GetCurrentPrefabStage();
                if (prefabStage != null)
                {
                    EditorSceneManager.MarkSceneDirty(prefabStage.scene);
                }
            }
            
            PlatinioEditorGUILayout.Space(3);

            if (prefabDependencyInjection.FailedDependencies.Count > 0)
            {
                foreach (var dependencyInfo in prefabDependencyInjection.FailedDependencies)
                {
                    if (dependencyInfo == null || dependencyInfo.FieldInfo == null) continue;
                    
                    PlatinioEditorGUILayout.DrawTooltipBox(MessageType.Error, "Failed Dependency", 
                        $"The dependency from {dependencyInfo.Component.GetType()}.{dependencyInfo.FieldInfo.Name} can't be resolved with current values!");
                }
            }
            else if (m_showResolvedDependencies)
            {
                PlatinioEditorGUILayout.DrawTooltipBox(MessageType.Info, "Success!", "All dependencies where resolved!");
            }
        }
    }
}

