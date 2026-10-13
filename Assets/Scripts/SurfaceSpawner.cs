using UnityEngine;
using UnityEngine.Pool;

public class SurfaceSpawner : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private FloorItem floorSquarePrefab;
    [SerializeField] private Transform firePoint;

    [Header("Pool Settings")]
    [SerializeField] private int defaultCapacity = 6;
    [SerializeField] private int maxPoolSize = 10;

    [Header("Exposed Pool Settings")]
    private IObjectPool<FloorItem> floorItemsPool;
    public IObjectPool<FloorItem> FloorItemsPool => floorItemsPool;

    private void Awake()
    {
        floorItemsPool = new ObjectPool<FloorItem>(
            CreateProjectile,      // Func to create a new item if pool is empty
            OnTakeFromPool,        // Action called when retrieving an item
            OnReturnedToPool,      // Action called when returning an item
            OnDestroyPoolObject,   // Action called when pool exceeds max size
            true,                  // Collection check (throws error if you release an item twice)
            defaultCapacity,
            maxPoolSize
        );
    }

    private void Update()
    {
        // Add the trigger here to be based on time. For not this will be run through this function, but in the future this should be a
        // run by a unviersal signalBus script, so that there is never any lag between each floor only the same amount of lag between the
        // floor and the SignalBus. CURRENTLY LIVES AS 3 SECONDS IN FLOORMANAGER
        if (Input.GetButtonDown("Fire1"))
        {
            CreateFloor();
        }
    }

    private void CreateFloor()
    {
        // Request a projectile from the pool instead of Instantiating a new one
        FloorItem projectile = floorItemsPool.Get();

        // Position and rotate the projectile at the weapon's muzzle/fire point
        projectile.transform.position = firePoint.position;
        projectile.transform.rotation = firePoint.rotation;
    }

    // --- Pool Callback Methods ---

    private FloorItem CreateProjectile()
    {
        // Physical instantiation happens here ONLY when the pool runs out of inactive objects
        FloorItem projectile = Instantiate(floorSquarePrefab);
        projectile.SetPool(floorItemsPool);
        return projectile;
    }

    // This method physically creates a new projectile.
    private void OnTakeFromPool(FloorItem projectile) // Thsi activates the projectile so it can participate in gameplay.
    {
        // "Spawn" command: Activate the object
        projectile.gameObject.SetActive(true);
    }

    
    private void OnReturnedToPool(FloorItem projectile) // This deactivates the projectile rather than destroying it
    {
        // "Queue Free" command: Deactivate the object so it can be reused
        projectile.gameObject.SetActive(false);
    }

    private void OnDestroyPoolObject(FloorItem projectile) // Destroy an object when the pool doesn't want to retain it.
    {
        // If the pool grows too large, physically destroy the excess items
        Destroy(projectile.gameObject);
    }
}
