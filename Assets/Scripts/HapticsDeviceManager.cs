using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

public class HapticsDeviceManager : MonoBehaviour
{
    [DllImport("GeomagicCommDLL")]
    protected static extern void getProxyPosition(double[] array);

    public static Vector3 GetProxyPosition()
    {
        double[] arrayToUse = new double[3];
        getProxyPosition(arrayToUse);
        return new Vector3((float)arrayToUse[0], (float)arrayToUse[1], (float)arrayToUse[2]);
    }

    [DllImport("GeomagicCommDLL")]
    protected static extern void setForce(double[] array);

    public static void SetForce(Vector3 force)
    {
        double[] arrayToUse = new double[3];
        arrayToUse[0] = force.x;
        arrayToUse[1] = force.y;
        arrayToUse[2] = force.z;
        setForce(arrayToUse);
    }

    [DllImport("GeomagicCommDLL")]
    protected static extern void getProxyRotation(double[] array);

    public static Vector4 GetProxyRotation()
    {
        double[] arrayToUse = new double[4];
        getProxyRotation(arrayToUse);
        return new Vector4((float)arrayToUse[0], (float)arrayToUse[1], (float)arrayToUse[2], (float)arrayToUse[3]);
    }

    //On application quit
    void OnApplicationQuit()
    {
        print("closeDLL " + stopHaptics());
    }

    [DllImport("GeomagicCommDLL")]
    public static extern bool prepareHaptics(double hapticScale);

    [DllImport("GeomagicCommDLL")]
    public static extern void startHaptics();

    [DllImport("GeomagicCommDLL")]
    public static extern int stopHaptics();

    //get end effector position
    [DllImport("GeomagicCommDLL")]
    public static extern void getOmniPosition(double[] position);


}
