using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI; //added from comms.cs
using System.Net.Sockets;
using System;

public class Instantiate512cubes : MonoBehaviour
{
    public GameObject _sampleCubePrefab;
    GameObject[] _sampleCube = new GameObject[16];
    public float _maxScale;

    public static float[] _juceArray = new float[16]; //declare new array of floats to take in 16 values from JUCE fft


// copied from comms.cs
    Single[] msgIn = new Single[16];

    public string conHost = "127.0.0.1";
    public int conPort = 12345;
    UInt32 magicMessageHeaderNumber = 0xf2b49e2c;
    TcpClient mySocket;
    NetworkStream theStream;
// end copied code

    // Start is called before the first frame update
    void Start()
    {


        mySocket = new TcpClient(conHost, conPort); //being server stream copied from comms.cs
        theStream = mySocket.GetStream();


        for (int i = 0; i < 16; i++)
        {
            GameObject _instanceSampleCube = (GameObject)Instantiate (_sampleCubePrefab);
            _instanceSampleCube.transform.position = this.transform.position;
            _instanceSampleCube.transform.parent = this.transform;
            _instanceSampleCube.name = "SampleCube" + i;
            this.transform.eulerAngles = new Vector3(0, -0.703125f * i, 0);
            _instanceSampleCube.transform.position = Vector3.forward * 100;
            _sampleCube[i] = _instanceSampleCube;
        }
    }

    // Update is called once per frame
    void Update()
    {



//copied from comms.cs
        while (theStream.DataAvailable)
        {
            UInt32[] messageHeader = { magicMessageHeaderNumber, ((UInt32)(msgIn.Length * sizeof(Single))) }; //header always sends before data - length of array * size of float
            byte[] data = new byte[(messageHeader.Length * sizeof(int)) + (msgIn.Length * sizeof(Single))]; //receive how many total bytes
            theStream.Read(data, 0, data.Length);
            Debug.Log("Message Received: ");
            Debug.Log(msgIn.Length);    


            for(int i=0; i < msgIn.Length; i++)
            {
                msgIn[i] = BitConverter.ToSingle(data, ((messageHeader.Length * sizeof(int)) + (i * sizeof(Single)))); //everything before byte 8 is just the magic header we dont care

                Debug.Log(msgIn[i]);
                _juceArray[i] = msgIn[i]; //transfer msgIn to _juceArray, maybe unneccessary...
            }
        }
//end copied code



     for(int i = 0; i <16; i++)
        {
            if (_sampleCube != null)
            {
                //_sampleCube[i].transform.localScale = new Vector3(10, (AudioPeer._samples[i]*_maxScale) +2, 10);
                _sampleCube[i].transform.localScale = new Vector3(10, (_juceArray[i]*_maxScale) +2, 10); //Test with different Array
            }
        }   
    }

    public void SendMessageToSocket(int[] msg) //added from comms.cs
    {
        UInt32[] messageHeader = { magicMessageHeaderNumber, ((UInt32)(msg.Length * 4)) };
        byte[] data = new byte[(messageHeader.Length * 4) + (msg.Length * 4)];
        Buffer.BlockCopy(messageHeader, 0, data, 0, (messageHeader.Length * 4));
        Buffer.BlockCopy(msg, 0, data, (messageHeader.Length * 4), (msg.Length * 4));
        theStream.Write(data, 0, data.Length);
    }
}
