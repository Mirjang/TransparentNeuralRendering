using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Net;
using System.Net.Sockets;
using UnityEngine.Networking;
using UnityEngine.Networking.Match; 

public class EncodeServer : MonoBehaviour
{
    const int commandPort = 10000;
    int namePort;
    int bytePort;

    TcpListener commandListener;
    TcpClient commandClient;
    StreamReader commandReader;

    TcpListener nameListener;
    TcpClient nameClient;
    StreamReader nameReader; 


    TcpListener byteListener;
    TcpClient byteClient;
    StreamReader byteReader; 

    Texture2D dummy;
    byte[] buffer; 
    // Start is called before the first frame update
    void Start()
    {
        commandListener = new TcpListener(IPAddress.Any, commandPort); 
        dummy = new Texture2D(512, 512, TextureFormat.RGBAFloat, false);
        buffer = new byte[(dummy.GetRawTextureData().Length)];
        
        commandClient = commandListener.AcceptTcpClient();
        commandReader = new StreamReader(commandClient.GetStream(), System.Text.Encoding.ASCII);
        namePort = int.Parse(commandReader.ReadLine());
        bytePort = int.Parse(commandReader.ReadLine());
        nameListener = new TcpListener(IPAddress.Any, namePort);
        nameClient = nameListener.AcceptTcpClient();
        nameReader = new StreamReader(nameClient.GetStream(), System.Text.Encoding.ASCII); 
        byteListener = new TcpListener(IPAddress.Any, bytePort);
        byteClient = nameListener.AcceptTcpClient();
        
    }

    // Update is called once per frame
    void Update()
    {

    }


    private void remoteWriteEXR(string path, byte[] raw)
    {
        Texture2D tex = new Texture2D(512, 512, TextureFormat.RGBAFloat, false);
        tex.LoadRawTextureData(raw);
        var blob = tex.EncodeToEXR(Texture2D.EXRFlags.OutputAsFloat | RenderOptions.getInstance().exrCompression);
        File.WriteAllBytes(path, blob);
        if (RenderOptions.getInstance().logOutputVerbose)
            Debug.Log("Wrote: " + path);
        Destroy(tex); 
    }


}
