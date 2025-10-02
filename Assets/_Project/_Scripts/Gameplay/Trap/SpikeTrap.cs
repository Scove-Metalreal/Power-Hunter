using UnityEngine;

public class SpikeTrap : MonoBehaviour
{
    public GameObject SpikePre;
    public float speedSpike = 5f;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void ActiceSpikeTrap()
    {
        
        var SpawmSpike = Instantiate(SpikePre, transform.position,Quaternion.identity);
        
    }
}
