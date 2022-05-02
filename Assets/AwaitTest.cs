using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Threading;
using System.Threading.Tasks;

public class AwaitTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        TStart();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private async void TStart()
    {
        Task task1 = FirstAsync(2);
        Task task2 = SecondAsync(3);

        Debug.Log($"�񵿱� ����");

        var data = await WebRequest("C:/Projects/AllNewRpg/Assets/AwaitTest.cs");
        Debug.Log($"webtest Result:{data}");
        await Task.Delay(3000);

        data = await WebRequest("C:/Projects/AllNewRpg/Assets/AwaitTest.cs");
        Debug.Log($"second webtest Result:{data}");
        await Task.Delay(3000);

        var result = await FirstAsync(5);
        Debug.Log($"First Result:{result}");
        var result2 = await SecondAsync(8);
        Debug.Log($"Second Result:{result2}");


        Debug.Log($"�񵿱� ����");
    }

    /// <summary>
    /// ù ��°, �񵿱� �޼��� ����
    /// </summary>
    /// <returns></returns>
    private async Task<int> FirstAsync(int times)
    {
        //UI Thread���� ����
//        Debug.Log("FirstAsync");
        int result = 0;
        for (int i = 0; i < times; i++)
        {
            result += i;
            await Task.Delay(1000);
        }
        return result;
    }
    private async Task<string> WebRequest(string path)
    {
        var rr = UnityWebRequest.Get(path);
        await rr.SendWebRequest();
        return rr.downloadHandler.text;
    }
    //private async Task<T> WebRequest<T>(string path)
    //{
    //    var rr = UnityWebRequest.Get(path);
    //    await rr.SendWebRequest();
    //    return rr.downloadHandler.data;
    //}
    /// <summary>
    /// �� ��°, �񵿱� �޼��� ����
    /// </summary>
    /// <returns></returns>
    private async Task<int> SecondAsync(int times)
    {
        //UI Thread���� ����
//        Debug.Log("SecondAsync");
        int result = 0;
        for (int i = 0; i < times; i++)
        {
            result += i;
            await Task.Delay(1000);
        }
        return result;
    }
}
