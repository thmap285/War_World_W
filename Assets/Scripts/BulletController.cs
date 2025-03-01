using UnityEngine;

public class BulletController : MonoBehaviour
{
    [SerializeField] private float speed = 10f;

    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    [System.Obsolete]
    private void Start()
    {
        rb.velocity = transform.forward * speed;
        transform.rotation = Quaternion.LookRotation(transform.forward) * Quaternion.Euler(-90f,0,0);
    }

    private void OnTriggerEnter(Collider other)
    {
        Destroy(gameObject);
    }
}
