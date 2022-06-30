

/*
 
    -----------------------
    UDP-Send
    -----------------------
    // [url]http://msdn.microsoft.com/de-de/library/bb979228.aspx#ID0E3BAC[/url]
   
    // > gesendetes unter
    // 127.0.0.1 : 8050 empfangen
   
    // nc -lu 127.0.0.1 8050
 
        // todo: shutdown thread at the end
*/
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Linq;

public class UDPSendTest : MonoBehaviour
{
    private static int localPort;
    Thread receiveThread;
    IPEndPoint remoteEndPoint;
    UDPDATA mUDPDATA = new UDPDATA ();
    
    // prefs
    private string IP;  // define in init
    public int port;  // define in init
    public Text engineA;
    public Text engineAHex;
    public Slider sliderA;
    public Text engineB;
    public Text engineBHex;
    public Slider sliderB;
    public Text engineC;
    public Text engineCHex;
    public Slider sliderC;

    public Text Data;
    //IPEndPoint remoteEndPoint;
    UdpClient client;
    string sendpack ;
    bool active = false;
    // gui
    string strMessage = "";
    bool quit = false;

    int A = 0, B = 0, C = 0;
    // call it from shell (as program)

    // start from unity3d
    public void Start()
    {
        init();
        
    }

    public void ActiveSend () 
    {
        active = true;

    }
   public void ResertPositionEngine ()
    {

        mUDPDATA.mAppDataField.RelaTime = "00001F40";

        mUDPDATA.mAppDataField.PlayMotorA = "00000000";
        mUDPDATA.mAppDataField.PlayMotorB = "00000000";
        mUDPDATA.mAppDataField.PlayMotorC = "00000000";

        sendString (mUDPDATA.GetToString ());

        mUDPDATA.mAppDataField.RelaTime = "00000064";

    }
   // OnGUI
   
    void FixedUpdate()
    {

        //if (Input.GetKeyDown(KeyCode.Q))
        //{
        //    quit = true;
        //    if (receiveThread != null)
        //        receiveThread.Abort();
        //    if (client != null)
        //        client.Close();


        //    Application.Quit();
        //}

       

        if (active)
        {

            A = (int)sliderA.value;
            B = (int)sliderB.value;
            C = (int)sliderC.value;

            string HexA = DecToHexMove (A);
            string HexB = DecToHexMove (B);
            string HexC = DecToHexMove (C);

            engineAHex.text = "Engine A: " + HexA;
            engineBHex.text = "Engine B: " + HexB;
            engineCHex.text = "Engine C: " + HexC;
            
            mUDPDATA.mAppDataField.PlayMotorC = HexC;
            mUDPDATA.mAppDataField.PlayMotorA = HexA;
            mUDPDATA.mAppDataField.PlayMotorB = HexB;


            engineA.text = ((int)sliderA.value).ToString ();
            engineB.text = ((int)sliderB.value).ToString ();
            engineC.text = ((int)sliderC.value).ToString ();

            Data.text = "Data: " + mUDPDATA.GetToString ();

            sendString (mUDPDATA.GetToString ());

        }
        
    }
    void OnApplicationQuit()
    {
        active = false;
        ResertPositionEngine ();

        quit = true;
        if (receiveThread != null)
            receiveThread.Abort();
        if(client!=null)
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
        ResertPositionEngine ();


       // Invoke ("ActiveSend",8);

    }
    byte[] StringToByteArray (string hex) {
        return Enumerable.Range (0, hex.Length)
                         .Where (x => x % 2 == 0)
                         .Select (x => Convert.ToByte (hex.Substring (x, 2), 16))
                         .ToArray ();
    }
    string DecToHexMove (int num) {
        int d = (num / 5) * 10000;
        return "000"+d.ToString ("X");
    }

    // sendData
    private void sendString (string message) {
        try {
            // Bytes empfangen.
            if (message != "") {

                byte[] data = StringToByteArray (message);
                // Den message zum Remote-Client senden.
                client.Send (data, data.Length, remoteEndPoint);

            }


        }
        catch (Exception err) {
            print (err.ToString ());
        }
    }
    void OnDisable()
    {
        if (receiveThread != null)
            receiveThread.Abort();
        if(client!=null)
        client.Close();
    }

}

