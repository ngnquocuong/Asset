using System;
using UnityEditor;
using UnityEngine;

public class Map : MonoBehaviour
{
    
    [SerializeField] private Bounds m_Bounds;
    [SerializeField] private Transform m_Walls;

    public MapSpawnPoint[] spawnPoints;
    public GameObject groundCombine;
    public bool combine = true;

    public Bounds WorldBounds => m_Bounds;
    // public string ambientalSoundName;
    // public int ambientalSoundsTotal;

    void Start()
    {
        // var getAmbientalSoundName = GetAmbientalSoundName();
        // SoundManager.PlayAmbientalSound(getAmbientalSoundName);
    }

    public void Rotate(float degree)
    {
        transform.rotation = Quaternion.Euler(new Vector3(0, degree, 0));
        if (gameObject && combine)
            StaticBatchingUtility.Combine(groundCombine);
    }

    public Transform[] GetSpawnPoints(int spawnIndex)
    {
        return spawnPoints[spawnIndex % spawnPoints.Length].points;
    }

    public Bounds GetLocalBounds()
    {
        Bounds bounds = new Bounds();
        bounds.center = transform.rotation * WorldBounds.center;
        bounds.size = transform.rotation * WorldBounds.size;
        return bounds;
    }

    // string GetAmbientalSoundName()
    // {
    //     return ambientalSoundName + Random.Range(1, ambientalSoundsTotal + 1);
    // }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireCube(m_Bounds.center, m_Bounds.size);

        if (m_Walls == null)
            return;

        BoxCollider[] colliders = m_Walls.GetComponentsInChildren<BoxCollider>();

        if (colliders.Length == 0)
            return;

        Gizmos.color = Color.green;
        for (int i = 0; i < colliders.Length; i++)
        {
            Gizmos.matrix = colliders[i].transform.localToWorldMatrix;
            Gizmos.DrawWireCube(colliders[i].center, colliders[i].size);
        }
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        spawnPoints = GetComponentsInChildren<MapSpawnPoint>();
    }
#endif
}