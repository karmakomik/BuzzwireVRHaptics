using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

public class GeomagicCommScript : MonoBehaviour
{
    //double[] tempPosition = new double[3];
    bool isHapticAvail = false;
    public GameObject proxy;
    // Start is called before the first frame update
    private void Awake()
    {     
        isHapticAvail = HapticsDeviceManager.prepareHaptics(0.01d);
    }

    void Start()
    {
        print("start haptics ");
        HapticsDeviceManager.startHaptics();
    }

    // Update is called once per frame
    void Update()
    {
        if (isHapticAvail)
        {
            Vector3 proxyPos = HapticsDeviceManager.GetProxyPosition();
            //print("Proxy Position " + proxyPos[0] + " " + proxyPos[1] + " " + proxyPos[2]);
            proxy.transform.position = proxyPos;

            Vector4 proxyRot = HapticsDeviceManager.GetProxyRotation();
            //print(proxyRot);
            proxy.transform.rotation = Quaternion.AngleAxis(Mathf.Rad2Deg * proxyRot[3], new Vector3(-proxyRot[1], -proxyRot[2], proxyRot[0]));

        }
    }

    public void giveForce()
    {
        HapticsDeviceManager.SetForce(new Vector3(0, 1.5f, 0));
    }

    public void stopForce()
    {
        HapticsDeviceManager.SetForce(new Vector3(0, 0, 0));
    }

    //On application quit
    void OnApplicationQuit()
    {
        print("Stopping haptics " + HapticsDeviceManager.stopHaptics());
    }

}
