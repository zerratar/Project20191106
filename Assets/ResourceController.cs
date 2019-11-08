using UnityEngine;

public class ResourceController : MonoBehaviour
{
    public long ResourceLimit = 10000;
    public float ResourceGenerationTime = 1f;
    private bool destroyed;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (destroyed) return;
        if (ResourceLimit <= 0)
        {
            destroyed = true;
            Destroy(gameObject);
        }
    }
}
