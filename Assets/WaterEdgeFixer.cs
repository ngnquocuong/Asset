using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class WaterEdgeFixer : MonoBehaviour
{
    [SerializeField] private float m_Offset;
    [SerializeField] private float m_Scale;
    [SerializeField] private string m_TargetName;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void FixEdges()
    {
        foreach (var VARIABLE in FindObjectsOfType<MonoBehaviour>().Where((behaviour => behaviour.name.Contains(m_TargetName))))
        {
            VARIABLE.transform.position += Vector3.up*m_Offset;
            VARIABLE.transform.localScale = VARIABLE.transform.localScale + Vector3.up * m_Scale;
            // VARIABLE.gameObject.SetActive(false);
            EditorUtility.SetDirty(VARIABLE.transform);
            EditorUtility.SetDirty(VARIABLE.gameObject);
        }
    }
}


#if UNITY_EDITOR
[CustomEditor(typeof(WaterEdgeFixer))]
public class WaterEdgeFixerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("FixEdges"))
        {
            (target as WaterEdgeFixer).FixEdges();
        }
    }
}
#endif