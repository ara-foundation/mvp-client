using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public class WebClient
{
    public static Task<bool> Delay(double milliseconds)
    {
        var tcs = new TaskCompletionSource<bool>();
        System.Timers.Timer timer = new System.Timers.Timer();
        timer.Elapsed += (o, e) => tcs.TrySetResult(true);
        timer.Interval = milliseconds;
        timer.AutoReset = false;
        timer.Start();
        return tcs.Task;
    }

    public static async Task<string> Post(string url, string jsonBody)
    {
        UnityWebRequest request = UnityWebRequest.Post(url, jsonBody, "application/json");
        await request.SendWebRequest();
        if (request.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log(request.error);
            return "";
        }

        return request.downloadHandler.text;
    }

    public static async Task<string> Get(string url, int timeout)
    {
        UnityWebRequest request = UnityWebRequest.Get(url);
        request.timeout = timeout;
        await request.SendWebRequest();
        if (request.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.LogError(request.error);
            return "";
        }
        else if (request.responseCode != 200)
        {
            Debug.LogError("HTTP Status: " + request.responseCode + ", msg: " + request.downloadHandler.text);
            return "";
        }

        return request.downloadHandler.text;
    }


    public static async Task<string> Get(string url)
    {
        return await Get(url, 0);
    }

    public static async Task<Texture2D> GetRemoteTexture(string url)
    {
        using UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
        // begin request:
        var asyncOp = www.SendWebRequest();

        // await until it's done: 
        while (asyncOp.isDone == false)
            await WebClient.Delay(1000 / 30);//30 hertz

        // read results:
        if (www.result == UnityWebRequest.Result.ConnectionError || www.responseCode != 200)
        {
            // log error:
#if DEBUG
            Debug.Log($"{www.error}, URL:{www.url}, Response Code: {www.responseCode}");
#endif

            // nothing to return on error:
            return null;
        }
        else
        {
            // return valid results:
            return DownloadHandlerTexture.GetContent(www);
        }
    }
}
