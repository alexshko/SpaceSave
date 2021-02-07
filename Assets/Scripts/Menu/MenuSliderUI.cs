using TMPro;
using UnityEngine;
using UnityEngine.UI;
namespace AirBattle.UI.Menu
{
    public class MenuSliderUI : MonoBehaviour
    {
        public Slider sl;
        public TextMeshProUGUI TextToUpdate;
        public float MinValueAllowed = 1;
        public float currentVal
        {
            get { return sl.value; }
            set
            {
                if ((value >= MinValueAllowed) && (value <= sl.maxValue) && (value >= sl.minValue))
                {
                    sl.value = value;
                    TextToUpdate.text = value.ToString();
                }
            }
        }

        private void Start()
        {
            currentVal = MinValueAllowed;
        }

        public void AddToCount()
        {
            Debug.Log("add to count");
            currentVal++;
        }
        public void SubFromCount()
        {
            Debug.Log("sub from count");
            currentVal--;
        }
    }
}
