using System;
using System.Collections;
using System.Collections.Generic;
using _Ball;
using _Main.Scripts.Utils;
using UnityEngine;

public class TowerTrigger : MonoBehaviour
{
    [SerializeField] private Transform m_Rigidbodies;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.TryGetComponent(out BallObject ballObject))
        {
            ReleaseTower();
            Destroy(this);
        }
    }
    
    public void ReleaseTower()
    {
        foreach (var VARIABLE in m_Rigidbodies.GetChilds())
        {
            var rbList = VARIABLE.GetComponentsInChildren<Rigidbody>();
            foreach (var rb in rbList)
            {
                if (rb)
                {
                    rb.constraints = RigidbodyConstraints.None;
                }
            }
        }
    }
}
