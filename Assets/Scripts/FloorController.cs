using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Pool;

public class FloorController : MonoBehaviour
{
    private FloorItem exitingFloorItem;
    private FloorItem enteringFloorItem;
    private IObjectPool<FloorItem> floorItemsPool;
    private SurfaceSpawner surfaceSpawner;

    private void Awake()
    {
        surfaceSpawner = GetComponentInParent<SurfaceSpawner>();
        if (surfaceSpawner == null)
        {
            Debug.LogError("SurfaceSpawner not found in parent hierarchy!");
            return;
        }
        floorItemsPool = surfaceSpawner.FloorItemsPool;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("RunningSurfaceItem"))
        {
            Debug.Log(other.gameObject.name + "has hit the end of the collider area");
            floorItemsPool.Release(other.GetComponent<FloorItem>());
        }
    }
}
