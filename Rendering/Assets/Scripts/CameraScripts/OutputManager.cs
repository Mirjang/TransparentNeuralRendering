using System.IO;
using System.Threading;
using UnityEngine;
using System.Collections.Generic;
using System.Threading.Tasks;

class OutputManager
{

    private static OutputManager instace = null;
    private Semaphore pool;
    private List<Thread> threads = new List<Thread>();

    private OutputManager()
    {
        pool = new Semaphore(RenderOptions.getInstance().maxActiveWriteThreads, RenderOptions.getInstance().maxActiveWriteThreads);
    }

    public void writeTextureEXR(string name, Texture2D tex)
    {
        tex.LoadRawTextureData(tex.GetRawTextureData());
        string filename = RenderOptions.getInstance().outputDir /*+ cameraID.ToString() + "_"*/ + name + ".exr";

    }

  
    public void flushAll()
    {

    }


    public static OutputManager getInstance()
    {
        if(instace==null)
        {
            instace = new OutputManager(); 
        }
        return instace; 
    }
}


//class OutputManager
//{

//    private static OutputManager instace = null;
//    private Semaphore pool;
//    private List<Thread> threads = new List<Thread>();

//    private OutputManager()
//    {
//        pool = new Semaphore(RenderOptions.getInstance().maxActiveWriteThreads, RenderOptions.getInstance().maxActiveWriteThreads);
//    }

//    public void writeTextureEXR(string name, Texture2D tex)
//    {

//        foreach (var th in threads)
//        {
//            if (th.Join(0))
//                threads.Remove(th);
//        }
//        pool.WaitOne();
//        Thread t = new Thread(() =>
//        {
//            var blob = tex.EncodeToEXR(Texture2D.EXRFlags.OutputAsFloat | RenderOptions.getInstance().exrCompression);
//            string filename = RenderOptions.getInstance().outputDir /*+ cameraID.ToString() + "_"*/ + name + ".exr";
//            File.WriteAllBytes(filename, blob);
//            if (RenderOptions.getInstance().logOutputVerbose)
//                Debug.Log("Wrote: " + filename);

//            UnityEngine.Object.Destroy(tex);
//            pool.Release();
//        });
//        t.Start();
//        threads.Add(t);
//    }

//    public void flushAll()
//    {
//        //for(int i = 0; i< RenderOptions.getInstance().maxActiveWriteThreads; ++i)
//        //{
//        //    pool.WaitOne(); 
//        //}
//        //pool.Release(RenderOptions.getInstance().maxActiveWriteThreads); 
//        foreach (var t in threads)
//        {
//            t.Join();
//        }
//    }


//    public static OutputManager getInstance()
//    {
//        if (instace == null)
//        {
//            instace = new OutputManager();
//        }
//        return instace;
//    }
//}

