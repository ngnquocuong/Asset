using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DimensionSpawnableZone : MonoBehaviour
{

    [SerializeField] private Vector2 m_Size;

    public bool IsSpawnable { get; private set; } = false;
    public ref readonly Bounds ActualSpawnableZone => ref m_ActualSpawnableZone;

    private Bounds m_ActualSpawnableZone;

    public void CheckSpawn(float _DimensionRadius, in System.Span<Ray> _CameraCornerRays)
    {
        Vector3 position = transform.position;

        Plane plane = new Plane(Vector3.up, position);

        System.Span<Vector3> intersections = stackalloc Vector3[_CameraCornerRays.Length];
        for (int i = 0; i < intersections.Length; i++)
        {
            plane.Raycast(_CameraCornerRays[i], out float enter);
            intersections[i] = _CameraCornerRays[i].GetPoint(enter);
        }

        int flags = 0;
        Vector3 size = new(m_Size.x, 0f, m_Size.y);
        if (IsMapRotatedOnTheSide())
            (size.x, size.z) = (size.z, size.x);
        
        Bounds bounds = new(position, size);
        for (int i = 0; i < intersections.Length; i++)
        {
            if (bounds.Contains(intersections[i]))
                flags |= 1 << i;
        }

        if (flags > 0)
        {
            Vector3 min = bounds.min;

            float largestArea = 0f;
            int largestAreaIdx = -1;
            for (int i = 0; i < intersections.Length; i++)
            {
                if ((flags & (1 << i)) != 0)
                {
                    (Vector3 size1, Vector3 size2) = GetSizes(intersections[i], i, min, size);
                    float area = Mathf.Max(size1.x * size1.z, size2.x * size2.z);
                    if (area > largestArea)
                    {
                        largestArea = area;
                        largestAreaIdx = i;
                    }
                }
            }

            (Vector3 s1, Vector3 s2) = GetSizes(intersections[largestAreaIdx], largestAreaIdx, min, size);
            Vector3 max = min + size;
            Vector3 center = bounds.center;
            if (s1.x * s1.z > s2.x * s2.y)
            {
                center.x = (largestAreaIdx & 1) == 0 ? min.x + 0.5f * s1.x : max.x - 0.5f * s1.x;
                size = s1;
            }
            else
            {
                center.z = (largestAreaIdx & 2) == 0 ? min.z + 0.5f * s2.z : max.z - 0.5f * s2.z;
                size = s2;
            }
            bounds.center = center;
        }

        float diameter = 2f * _DimensionRadius;
        IsSpawnable = size.x >= diameter && size.z >= diameter;
        size.x -= diameter;
        size.z -= diameter;
        bounds.size = size;
        m_ActualSpawnableZone = bounds;
    }

    private (Vector3 size1, Vector3 size2) GetSizes(in Vector3 _Intersection, int _Index, in Vector3 _Min, in Vector3 _Size)
    {
        Vector3 max = _Min + _Size;
        Vector3 size1 = new((_Index & 1) == 0 ? (_Intersection.x - _Min.x) : (max.x - _Intersection.x), _Size.y, _Size.z);
        Vector3 size2 = new(_Size.x, _Size.y, (_Index & 2) == 0 ? (_Intersection.z - _Min.z) : (max.z - _Intersection.z));
        return (size1, size2);
    }

    public Vector3 GetRandomSpawnPosition()
    {
        Vector3 min = m_ActualSpawnableZone.min;
        Vector3 max = m_ActualSpawnableZone.max;
        return new(Random.Range(min.x, max.x), transform.position.y, Random.Range(min.z, max.z));
    }

    private bool IsMapRotatedOnTheSide()
    {
        Vector3 mapFwd = transform.root.forward;
        return Vector3.Dot(mapFwd, Vector3.right) is >= 0.9f or <= -0.9f;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = UnityEditor.Selection.activeGameObject == gameObject ? Color.yellow : Color.cyan;

        Vector3 size = m_Size;
        size.z = size.y;
        size.y = 0.1f;
        
        if (IsMapRotatedOnTheSide())
            (size.x, size.z) = (size.z, size.x);
        
        Gizmos.DrawWireCube(transform.position, size);

        if (IsSpawnable == false)
            return;

        Color c = Color.cyan;
        c.a = 0.5f;
        Gizmos.color = c;
        Gizmos.DrawCube(m_ActualSpawnableZone.center, m_ActualSpawnableZone.size + 10f * Vector3.up);
    }
#endif

}
