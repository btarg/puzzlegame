using UnityEngine;
public class PulseLight : MonoBehaviour
{
    private Light myLight;
    public float PULSE_RANGE = 4.0f;
    public float PULSE_SPEED = 3.0f;
    
    public float PULSE_MINIMUM = 1.0f;


    void Start()
    {
        myLight = GetComponent<Light>();
    }
    void Update()
    {
         myLight.range = PULSE_MINIMUM + Mathf.PingPong(Time.time * PULSE_SPEED, PULSE_RANGE - PULSE_MINIMUM);
    }
}