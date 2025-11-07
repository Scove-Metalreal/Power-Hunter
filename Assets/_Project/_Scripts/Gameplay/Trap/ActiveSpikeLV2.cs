using UnityEngine;

public class ActiveSpikeLV2 : MonoBehaviour
{
    public GameObject Spike;
    void Start()
    {
        Spawm();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Spawm()
    {
        Quaternion rot = transform.rotation * Quaternion.Euler(0, 0, 180);

        var uu = Instantiate(Spike, transform.position,rot);
        uu.transform.SetParent(gameObject.transform);
    }
}
