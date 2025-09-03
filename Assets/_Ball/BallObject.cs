using System;
using UnityEngine;

namespace _Ball
{
    public class BallObject : MonoBehaviour
    {
        [SerializeField] private Rigidbody m_Rigidbody;
        
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.T))
            {
                var dir = HoleDirection.Instance.transform.forward;
                m_Rigidbody.AddForce(dir*20, ForceMode.VelocityChange);
            }
            if (Input.GetKeyDown(KeyCode.U))
            {
                var dir = HoleDirection.Instance.transform.forward;
                m_Rigidbody.AddForce(dir*5, ForceMode.VelocityChange);
            }
        }
    }
}