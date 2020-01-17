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
        pool = new Semaphore(0, RenderOptions.getInstance().maxActiveWriteThreads); 
    }

    public void writeTextureEXR(string name, Texture2D tex)
    {
        pool.WaitOne();
        Task.Factory.StartNew(() =>
        {
            var blob = tex.EncodeToEXR(Texture2D.EXRFlags.OutputAsFloat | RenderOptions.getInstance().exrCompression);
            string filename = RenderOptions.getInstance().outputDir /*+ cameraID.ToString() + "_"*/ + name + ".exr";
            File.WriteAllBytes(filename, blob);
            if (RenderOptions.getInstance().logOutputVerbose)
                Debug.Log("Wrote: " + filename);

            UnityEngine.Object.Destroy(tex);
            pool.Release(); 
        }); 
    }

    public void flushAll()
    {
        for(int i = 0; i< RenderOptions.getInstance().maxActiveWriteThreads; ++i)
        {
            pool.WaitOne(); 
        }
        pool.Release(RenderOptions.getInstance().maxActiveWriteThreads); 
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

