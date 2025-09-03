using System;
using System.Collections;
using System.Collections.Generic;
using _Main.Scripts.Utils;
using UnityEditor;
using UnityEngine;

public class CircleSpawner : MonoBehaviour
{
    [SerializeField] private GameObject m_SpawnObject;
    [SerializeField, Range(1,360)] private float m_Angle;
    [SerializeField] private float m_Radius;
    [SerializeField] private float m_AngleOffset;
    [SerializeField] private bool m_Center;
    [SerializeField] private float m_Scale=1;
    
    private void Awake()
    {
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

            if (m_Center)
            {
                var ins = PrefabUtility.InstantiatePrefab(m_SpawnObject) as GameObject;
                ins.transform.SetParentResetTransform(transform);
                ins.transform.localPosition = Vector3.zero;
                ins.transform.localScale = Vector3.one * m_Scale;
            }
            
            var amount = 360 / m_Angle;
            for (int i = 0; i < amount; i++)
            {
                var ins = PrefabUtility.InstantiatePrefab(m_SpawnObject) as GameObject;
                ins.transform.SetParentResetTransform(transform);
                var pi = 2 * Mathf.PI * (float)i / amount;
                var pos = Vector3.right*Mathf.Cos(pi) + Vector3.forward*Mathf.Sin(pi);
                pos *= m_Radius;
                ins.transform.localPosition = pos;
                ins.transform.localScale = Vector3.one * m_Scale;

                var right = ins.transform.position - transform.position;
                right.Normalize();
                ins.transform.right = right;
                ins.transform.Rotate(new Vector3(0,m_AngleOffset,0));
            }
        }
    }
}
