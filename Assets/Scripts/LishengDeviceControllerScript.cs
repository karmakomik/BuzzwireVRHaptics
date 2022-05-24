using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LishengDeviceControllerScript : MonoBehaviour
{
    public SerialController arduinoSerialController;
    public int vibration_level; //20 - Minimum, 63 - Medium, 127 - Maximum

    // Start is called before the first frame update
    void Start()
    {
        vibration_level = 63;
    }

    // Update is called once per frame
    void Update()
    {
        //arduinoSerialController.
    }

    void OnApplicationQuit()
    {
        vibrateInDirection(new Vector3(0, 0, 0));
    }
    

    public void vibrateInDirection(Vector3 direction)
    {
        print("vibrate in direction" + direction);
        int vibration_a = 0, vibration_b = 0, vibration_c = 0, vibration_d = 0, vibration_e = 0, vibration_f = 0;

        if (direction.magnitude != 0)
        {
            if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
            {
                if(Mathf.Abs(direction.x) > Mathf.Abs(direction.z))
                {
                    if(direction.x > 0)
                    {
                        vibration_b = vibration_level;
                    }
                    else
                    {
                        vibration_d = vibration_level;
                    }
                }
                else
                {
                    if (direction.z > 0)
                    {
                        vibration_e = vibration_level;
                    }
                    else
                    {
                        vibration_f = vibration_level;
                    }
                }
            }
            else
            {
                if (Mathf.Abs(direction.y) > Mathf.Abs(direction.z))
                {
                    if (direction.y > 0)
                    {
                        vibration_c = vibration_level;
                    }
                    else
                    {
                        vibration_a = vibration_level;
                    }
                }
                else
                {
                    if (direction.z > 0)
                    {
                        vibration_e = vibration_level;
                    }
                    else
                    {
                        vibration_f = vibration_level;
                    }
                }

            }
        }

        setVibrationLevelForMotor("a", vibration_a);
        setVibrationLevelForMotor("b", vibration_b);
        setVibrationLevelForMotor("c", vibration_c);
        setVibrationLevelForMotor("d", vibration_d);
        setVibrationLevelForMotor("e", vibration_e);
        setVibrationLevelForMotor("f", vibration_f);
    }

    void setVibrationLevelForMotor(string _motor, int level)
    {
        if (arduinoSerialController != null)
        {
            string motor = _motor;
            string intensity = level.ToString("D3");
            arduinoSerialController.SendSerialMessage("" + motor + intensity);
        }
    }

}
