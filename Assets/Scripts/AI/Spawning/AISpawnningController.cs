// Script by Tristan Bapton

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace AnyMeansNecessaryAI
{
    public enum AITypes { Standard, Hunter, Sniper, Armoured, Civilian };
}

public class AISpawnningController : MonoBehaviour
{
    public List<Base_Enemy> SceneAI = new List<Base_Enemy>();
    public List<CivilianAI_Main> SceneCivilians = new List<CivilianAI_Main>();

}

#if UNITY_EDITOR
[CustomEditor(typeof(AISpawnningController))]
public class AISpawnningControllerEditor : Editor
{

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
    }

}
#endif