using Spawn.Domain.Round;
using UnityEditor;
using UnityEngine;

namespace Spawn.Domain.Editor
{
    [CustomEditor(typeof(RoundHolder)), CanEditMultipleObjects]
    public class RoundEditor : UnityEditor.Editor
    {
        public SerializedProperty 
            state_Prop,
            waveRound,
            timedWaveRound,
            normalRound,
            onceRound,
            survivalRound;
     
        void OnEnable () {
            // Setup the SerializedProperties
            state_Prop = serializedObject.FindProperty ("type");
            waveRound = serializedObject.FindProperty("waveRound");
            timedWaveRound = serializedObject.FindProperty ("timedWaveRound");
            normalRound = serializedObject.FindProperty ("normalRound");
            onceRound = serializedObject.FindProperty ("onceRound");       
            survivalRound = serializedObject.FindProperty ("survivalRound");    
        }
     
        public override void OnInspectorGUI() {
            serializedObject.Update ();
         
            EditorGUILayout.PropertyField( state_Prop );
         
            var st = (RoundTypes)state_Prop.enumValueIndex;
         
            switch(st) {
                case RoundTypes.Wave:            
                    EditorGUILayout.ObjectField(waveRound, typeof(WaveRound), new GUIContent("Wave Round"));            
                    break;
                case RoundTypes.TimedWave:            
                    EditorGUILayout.ObjectField(timedWaveRound, typeof(TimedWaveRound), new GUIContent("Timed Wave Round"));            
                    break;
                case RoundTypes.Normal:            
                    EditorGUILayout.ObjectField(normalRound, typeof(NormalRound), new GUIContent("Normal Round"));            
                    break;
                case RoundTypes.Once:            
                    EditorGUILayout.ObjectField(onceRound, typeof(OnceRound), new GUIContent("Once Round"));            
                    break;
                case RoundTypes.Survival:            
                    EditorGUILayout.ObjectField(survivalRound, typeof(SurvivalRound), new GUIContent("Survival Round"));            
                    break;
            }
         
         
            serializedObject.ApplyModifiedProperties ();
        }
    }
}