using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightScript : MonoBehaviour
{
    Light daylight, moonlight;

    float secondsPerDay;

    bool startMenu = false;

    void Start()
    {
        daylight = GameObject.Find("Daylight").GetComponent<Light>();
        moonlight = GameObject.Find("Moonlight").GetComponent<Light>();

        try
        {
            secondsPerDay = GameObject.Find("Board").GetComponent<Logic>().turnDurationSeconds;
        }
        catch
        {
            secondsPerDay = 120f;
            startMenu = true;
        }
    }

    void Update()
    {
        Rotate();
        Enable();
    }

    void Rotate()
    {
        daylight.transform.Rotate(new Vector3(0,
            360 / (secondsPerDay) * Time.deltaTime,
            0));

        moonlight.transform.Rotate(new Vector3(0,
            360 / (secondsPerDay) * Time.deltaTime,
            0));
    }

    void Enable()
    {
        if (startMenu) { return; }

        if (daylight.transform.localEulerAngles.y >= 90 &&
            daylight.transform.localEulerAngles.y < 270)
        {
            daylight.enabled = false;
            moonlight.enabled = true;
        }
        else
        {
            daylight.enabled = true;
            moonlight.enabled = false;
        }
    }
}
