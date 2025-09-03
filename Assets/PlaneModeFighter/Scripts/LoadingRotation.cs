using UnityEngine;

public class LoadingRotation : MonoBehaviour
{
    [SerializeField] Vector3 m_Rotation;
    void Update()
    {
        transform.rotation *= Quaternion.Euler(m_Rotation*Time.deltaTime);
    }
}
