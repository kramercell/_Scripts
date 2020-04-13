

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Net.Sockets;
using System;


public class Instantiate512Cubesv2 : MonoBehaviour
{
    //Single[] msgIn = {0f, 0f, 0f, 0f }; //works but only 4 values
	Single[] msgIn = new Single[16];

    public string conHost = "127.0.0.1";
    public int conPort = 12345;
    UInt32 magicMessageHeaderNumber = 0xf2b49e2c;
    TcpClient mySocket;
    NetworkStream theStream;
    
///
    public GameObject _sampleCubePrefab;            //FROM INSTANTIATE 512 CUBES
    GameObject[] _sampleCube = new GameObject[512];
    public float _maxScale;
///

    // Use this for initialization
    void Start()
    {
        mySocket = new TcpClient(conHost, conPort);
        theStream = mySocket.GetStream();

        //Application.targetFrameRate = 30; POSSIBLY DIFFERENT VERSION UNITY?
        //TRY TO SET FRAMERATE HERE TO 30HZ
        
        //SendMessageToSocket(new int[] { 1, 0, 127 });
///
        for (int i = 0; i < 512; i++)                                                     //FROM INSTANTIATE 512 CUBES
        {
            GameObject _instanceSampleCube = (GameObject)Instantiate (_sampleCubePrefab);
            _instanceSampleCube.transform.position = this.transform.position;
            _instanceSampleCube.transform.parent = this.transform;
            _instanceSampleCube.name = "SampleCube" + i;
            this.transform.eulerAngles = new Vector3(0, -0.703125f * i, 0);
            _instanceSampleCube.transform.position = Vector3.forward * 100;
            _sampleCube[i] = _instanceSampleCube;
        }
///
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log("test");
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
	            //incerement by sizeof Float after header (int)
                //Debug.Log("Message Received: " + msgIn[0] + " " + msgIn[1] + " " + msgIn[2]); //ADDED THIS FROM COPIED SERVER CODE TO DISPLAY MESSAGE
	            Debug.Log(msgIn[i]);

	            //msgIn[1] = BitConverter.ToInt32(data, 12); //2nd value if int will be at 12. if floats probably need to change this number
	            //msgIn[2] = BitConverter.ToInt32(data, 16);

                //if (_sampleCube != null)
               // {
                //    _sampleCube[i].transform.localScale = new Vector3(10, (msgIn[i] *_maxScale) +2, 10);
               // }
        	}
        }
///
       for(int i = 0; i <16; i++)             //FROM INSTANTIATE 512 CUBES
        {
            if (_sampleCube != null)
            {
                _sampleCube[i].transform.localScale = new Vector3(10, (AudioPeer._samples[i]*_maxScale) +2, 10);
            }
        }

///

    }

    public void SendMessageToSocket(int[] msg)
    {
        UInt32[] messageHeader = { magicMessageHeaderNumber, ((UInt32)(msg.Length * 4)) };
        byte[] data = new byte[(messageHeader.Length * 4) + (msg.Length * 4)];
        Buffer.BlockCopy(messageHeader, 0, data, 0, (messageHeader.Length * 4));
        Buffer.BlockCopy(msg, 0, data, (messageHeader.Length * 4), (msg.Length * 4));
        theStream.Write(data, 0, data.Length);
    }

}