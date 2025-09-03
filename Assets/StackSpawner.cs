using System.Collections;
using System.Collections.Generic;
using _Main.Scripts.Utils;
using UnityEditor;
using UnityEngine;

public class StackSpawner : MonoBehaviour
{
    [SerializeField] private GameObject m_SpawnObject;
    [SerializeField] private float m_Distance;
    [SerializeField] private float m_AngleOffset;
    [SerializeField] private int m_Amount;
    [SerializeField] private float m_Scale=1;
    
    private void Awake()
    {
        transform.DestroyChilds();
            
        for (int i = 0; i < m_Amount; i++)
        {
            var ins = PrefabUtility.InstantiatePrefab(m_SpawnObject) as GameObject;
            ins.transform.SetParentResetTransform(transform);
            var pos = Vector3.up * i * m_Distance;
            ins.transform.localPosition = pos;
            ins.transform.localScale = Vector3.one * m_Scale;
            ins.transform.Rotate(new Vector3(0,m_AngleOffset,0));
        }
        
        foreach (var VARIABLE in transform.GetChilds())
        {
            var rbList = VARIABLE.GetComponentsInChildren<Rigidbody>();
            foreach (var rb in rbList)
            {
                if (rb)
                {
                    rb.constraints = RigidbodyConstraints.FreezeAll;
                }
            }
        }
        
        Destroy(this);
    }

    private void OnValidate()
    {
        if (m_SpawnObject && !Application.isPlaying)
        {
            foreach (var VARIABLE in transform.GetChilds())
            {
                EditorApplication.delayCall += () =>
                {
                    if (VARIABLE)
                    {
                        DestroyImmediate(VARIABLE.gameObject);
                    }
                };
            }
            
            for (int i = 0; i < Mathf.Min(30, m_Amount); i++)
            {
                var ins = PrefabUtility.InstantiatePrefab(m_SpawnObject) as GameObject;
                ins.transform.SetParentResetTransform(transform);
                var pos = Vector3.up * i * m_Distance;
                ins.transform.localPosition = pos;
                ins.transform.localScale = Vector3.one * m_Scale;
                ins.transform.Rotate(new Vector3(0,m_AngleOffset,0));
            }
        }
    }

    public void ReleaseTower()
    {
        foreach (var VARIABLE in transform.GetChilds())
        {
            var rb = VARIABLE.GetComponentInChildren<Rigidbody>();
            if (rb)
            {
                rb.constraints = RigidbodyConstraints.None;
            }
        }
    }
}