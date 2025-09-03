using UnityEngine;

namespace Voodoo.Optimization.ChestRoom.InGame
{
    public class ChestRoomKeySpawnPoint : MonoBehaviour
    {
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(transform.position, 3f);
        }
    }
}