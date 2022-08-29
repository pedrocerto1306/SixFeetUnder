using UnityEngine;

public class BatteryController : MonoBehaviour
{
    float speed = 1f;
    float controle = 0f;
    int multiplier = 1;

    void Update()
    {
        if (controle == 10)
            multiplier = -1;
        else if (controle == 0)
            multiplier = 1;

        controle = multiplier * Mathf.Clamp(controle + Time.deltaTime, 0, 10);
        transform.position += new Vector3(0, speed * Mathf.Sin(controle)/300, 0);
    }
}
