using UnityEngine;
using System.Collections;

using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Linq;

public class UDPSend : MonoBehaviour
{
    private static int localPort;

    IPEndPoint remoteEndPoint;
    UDPDATA mUDPDATA = new UDPDATA();
    // prefs
    private string IP;  // define in init
    private int port;  // define in init
    public GameObject vehicle;

    public Transform engineMBC1;
    public Transform engineBC2;
    public Transform engineMA1;
    public Transform engineA2;
    public float SmoothEngine = 0.5f;
    public float longg;
    float valueMotor = 125;
    UdpClient client;
    string sendpack;
    bool active = false;
    // gui
    string strMessage = "";
    bool quit = false;

    float A = 0, B = 0, C = 0;

    // start from unity3d
    public void Start()
    {
        init();
    }
    void CalcularRotacion()
    {

        Quaternion rotY = vehicle.transform.rotation;
        rotY.x = 0;
        rotY.z = 0;

        // rotate left or Right
        Vector3 Vb1 = vehicle.transform.position + vehicle.transform.rotation * Vector3.right * longg;
        Vector3 Vb2 = vehicle.transform.position + rotY * Vector3.right * longg;

        // rotate forward or back
        Vector3 VbA1 = vehicle.transform.position + vehicle.transform.rotation * Vector3.forward * longg;
        Vector3 VbA2 = vehicle.transform.position + rotY * Vector3.forward * longg;

        float distBC = (Vb1 - Vb2).magnitude * 2;
        float distA = (VbA1 - VbA2).magnitude * 2;

        Vector3 crossX = Vector3.Cross(vehicle.transform.right, Vector3.right);
        Vector3 crossZ = Vector3.Cross(vehicle.transform.forward, Vector3.forward);

        if (vehicle.transform.eulerAngles.z < 180 && vehicle.transform.eulerAngles.z > 0)
        {
            B = Mathf.Lerp(B, (Mathf.Clamp(valueMotor + distBC * 10, 0, 250)), SmoothEngine);
            C = Mathf.Lerp(C, (Mathf.Clamp(valueMotor - distBC * 10, 0, 250)), SmoothEngine); ;
        }
        else

        if (vehicle.transform.eulerAngles.z > 180 && vehicle.transform.eulerAngles.z < 360)
        {
            B = Mathf.Lerp(B, (Mathf.Clamp(valueMotor - distBC * 10, 0, 250)), SmoothEngine);
            C = Mathf.Lerp(C, (Mathf.Clamp(valueMotor + distBC * 10, 0, 250)), SmoothEngine);
        }

        if (vehicle.transform.eulerAngles.x < 180 && vehicle.transform.eulerAngles.x > 0)
        {
            A = Mathf.Lerp(A, (Mathf.Clamp(valueMotor - distA * 10, 0, 250)), SmoothEngine);
            //B = Mathf.Lerp (B, (Mathf.Clamp (valueMotor + distBC * 10, 0, 250)), Time.deltaTime * SmoothEngine);
            //C = Mathf.Lerp (C, (Mathf.Clamp (valueMotor + distBC * 10, 0, 250)), Time.deltaTime * SmoothEngine);

        }
        else

        if (vehicle.transform.eulerAngles.x > 180 && vehicle.transform.eulerAngles.x < 360)
        {
            A = Mathf.Lerp(A, (Mathf.Clamp(valueMotor + distA * 10, 0, 250)), SmoothEngine);
            //B = Mathf.Lerp (B, (Mathf.Clamp (valueMotor - distBC * 10, 0, 250)), Time.deltaTime * SmoothEngine);
            //C = Mathf.Lerp (C, (Mathf.Clamp (valueMotor - distBC * 10, 0, 250)), Time.deltaTime * SmoothEngine);
        }

        if (engineMBC1 != null)
            engineMBC1.transform.position = vehicle.transform.position + vehicle.transform.rotation * Vector3.right * longg;

        if (engineBC2 != null)
            engineBC2.transform.position = vehicle.transform.position + rotY * Vector3.right * longg;

        if (engineMA1 != null)
            engineMA1.transform.position = vehicle.transform.position + vehicle.transform.rotation * Vector3.forward * longg;

        if (engineA2 != null)
            engineA2.transform.position = vehicle.transform.position + rotY * Vector3.forward * longg;

    }
    void ActiveSend()
    {
        active = true;
    }
    void ResertPositionEngine()
    {

        mUDPDATA.mAppDataField.RelaTime = "00001F40";

        mUDPDATA.mAppDataField.PlayMotorA = "00000000";
        mUDPDATA.mAppDataField.PlayMotorB = "00000000";
        mUDPDATA.mAppDataField.PlayMotorC = "00000000";

        sendString(mUDPDATA.GetToString());

        mUDPDATA.mAppDataField.RelaTime = "00000064";

    }
    void FixedUpdate()
    {

        CalcularRotacion();
        Debug.Log(" Piston A: " + A + " Piston B: " + B + " Piston C: " + C);
        mUDPDATA.mAppDataField.PlayMotorC = DecToHexMove(C);
        mUDPDATA.mAppDataField.PlayMotorA = DecToHexMove(A);
        mUDPDATA.mAppDataField.PlayMotorB = DecToHexMove(B);
        Debug.Log(mUDPDATA.GetToString());
        sendString(mUDPDATA.GetToString());


    }
    void OnApplicationQuit()
    {
        active = false;
        ResertPositionEngine();

        quit = true;

        if (client != null)
            client.Close();
        Application.Quit();
    }
    // init
    public void init()
    {

        // define
        IP = "192.168.15.201";
        port = 7408;

        // ----------------------------
        // Senden
        // ----------------------------
        remoteEndPoint = new IPEndPoint(IPAddress.Parse(IP), port);
        client = new UdpClient(53342);


        // AppControlField
        mUDPDATA.mAppControlField.ConfirmCode = "55aa";
        mUDPDATA.mAppControlField.PassCode = "0000";
        mUDPDATA.mAppControlField.FunctionCode = "1301";
        // AppWhoField
        mUDPDATA.mAppWhoField.AcceptCode = "ffffffff";
        mUDPDATA.mAppWhoField.ReplyCode = "";//"00000001";
                                             // AppDataField
        mUDPDATA.mAppDataField.RelaTime = "00000064";
        mUDPDATA.mAppDataField.PlayMotorA = "00000000";
        mUDPDATA.mAppDataField.PlayMotorB = "00000000";
        mUDPDATA.mAppDataField.PlayMotorC = "00000000";

        mUDPDATA.mAppDataField.PortOut = "12345678";
        ResertPositionEngine();


        Invoke("ActiveSend", 8);

    }
    byte[] StringToByteArray(string hex)
    {
        return Enumerable.Range(0, hex.Length)
                         .Where(x => x % 2 == 0)
                         .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                         .ToArray();
    }
    string DecToHexMove(float num)
    {
        int d = (int)((num / 5f) * 10000f);
        return "000" + d.ToString("X");
    }
    // sendData
    private void sendString(string message)
    {
        try
        {
            // Bytes empfangen.
            if (message != "")
            {
                byte[] data = StringToByteArray(message);
                // Den message zum Remote-Client senden.
                client.Send(data, data.Length, remoteEndPoint);

            }
        }
        catch (Exception err)
        {
            print(err.ToString());
        }
    }
    private double DegreeToRadian(double angle)
    {
        return Math.PI * angle / 180.0f;
    }
    private double RadianToDegree(double angle)
    {
        return angle * (180.0f / Math.PI);
    }
    void OnDisable()
    {


        client.Close();
    }
    void OnDrawGizmos()
    {

        //Gizmos.color = Color.red;
        //Gizmos.DrawLine(engineMBC1.transform.position, engineBC2.transform.position);

        //Gizmos.color = Color.red;
        //Gizmos.DrawLine (vehicle.transform.position, engineMBC1.transform.position);

        //Gizmos.color = Color.red;
        //Gizmos.DrawLine (vehicle.transform.position, engineBC2.transform.position);

        //Gizmos.color = Color.blue;
        //Gizmos.DrawLine (engineMA1.transform.position, engineA2.transform.position);

        //Gizmos.color = Color.blue;
        //Gizmos.DrawLine (vehicle.transform.position, engineMA1.transform.position);

        //Gizmos.color = Color.blue;
        //Gizmos.DrawLine (vehicle.transform.position, engineA2.transform.position);

        // Gizmos.color = Color.red;
        // Gizmos.DrawLine (transform.position, transform.position + transform.right * 100 * Input.GetAxis ("Horizontal_Helicop"));

        // Gizmos.color = Color.green;
        // Gizmos.DrawLine (transform.position, transform.position - transform.up * 100);

        // Gizmos.color = Color.white;
        // Gizmos.DrawLine (transform.position, transform.position - Vector3.up * angleForce * (speed / speedMax));

        //// Vector3 cross = -Vector3.Cross (transform.up, transform.right);



    }
}
