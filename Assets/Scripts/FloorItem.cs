using UnityEngine;
using UnityEngine.Pool; // Requires Unity 2021+

public class FloorItem : MonoBehaviour
{
    [SerializeField] private float speed = 20f;
    [SerializeField] private float lifetime = 3f;

    private IObjectPool<FloorItem> objectPool;
    private float currentLifetime;

    // Set the pool reference so the projectile knows where to return
    public void SetPool(IObjectPool<FloorItem> pool)
    {
        objectPool = pool;
    }

    private void OnEnable()
    {
        currentLifetime = lifetime;
    }

    private void Update()
    {
        // Move forward
        transform.Translate(Vector3.forward * speed * Time.deltaTime);

        // Countdown lifetime
        currentLifetime -= Time.deltaTime;
        if (currentLifetime <= 0f)
        {
            Deactivate();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Handle collision logic here (e.g., deal damage)

        // Return to pool on impact
        Deactivate();
    }

    public void Deactivate()
    {
        // Instead of Destroy(gameObject), we release it back to the pool
        objectPool?.Release(this);
    }
}
