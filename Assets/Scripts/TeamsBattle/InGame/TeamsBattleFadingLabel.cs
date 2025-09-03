using TMPro;
using UnityEngine;

namespace TeamsBattle
{
    public class TeamsBattleFadingLabel : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _quantityText;
        
        public void Setup(int quantity)
        {
            _quantityText.text = $"+{quantity}";
            Destroy(gameObject, 3f);
        }
    }
}