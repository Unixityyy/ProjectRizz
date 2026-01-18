using UnityEngine;

namespace Liv.Lck.Tablet
{
    public abstract class LckBaseNotification : MonoBehaviour
    {
        [field: SerializeField, Header("Settings")] 
        public bool RemainOnScreen { get; private set; } = false;

        public GameObject SpawnedGameObject { get; private set; }

        public virtual void ShowNotification()
        {
            if (SpawnedGameObject != null) SpawnedGameObject.SetActive(true);
        }
        public virtual void HideNotification()
        {
            if (SpawnedGameObject != null) SpawnedGameObject.SetActive(false);
        }

        public void SetSpawnedGameObject(GameObject go)
        {
            SpawnedGameObject = go;
        }
    }
}
