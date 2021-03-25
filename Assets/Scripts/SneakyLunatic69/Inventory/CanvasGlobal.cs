using UnityEngine;

namespace SneakyLunatic.Inventory
{
    public class CanvasGlobal : MonoBehaviour
    {
        public static float SCALE_FACTOR { get; private set; }
        void Awake()
        {
            SCALE_FACTOR = GetComponent<Canvas>().scaleFactor;
        }
    }
}
