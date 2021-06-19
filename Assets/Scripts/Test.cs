using System.Collections;
using UnityEngine;

public class Test : MonoBehaviour
{
    private void Start()
    {
        EventMgr.AddListener<DataInstanceChangedEvent>(OnDataInstanceChangedEvent);
        EventMgr.AddListener<DataInstanceChangedEvent>(DataInstance.DataInstanceData0Changed, OnDataInstanceData0);
        EventMgr.AddListener<DataInstanceChangedEvent>(DataInstance.DataInstanceData1Changed, OnDataInstanceData1);
    }

    private void OnDataInstanceChangedEvent(DataInstanceChangedEvent e)
    {
        D.Log("Test.OnDataInstanceChangedEvent", e.data);
    }

    private void OnDataInstanceData0(DataInstanceChangedEvent e)
    {
        D.Log("Test.OnDataInstanceData0", e.data);
    }

    private void OnDataInstanceData1(DataInstanceChangedEvent e)
    {
        D.Log("Test.OnDataInstanceData1", e.data);
    }

    public void zzStaticTest()
    {
        StartCoroutine(StaticTest());
    }

    IEnumerator StaticTest()
    {
        yield return new WaitForSeconds(1);

        CtrlStatic.InitOnce(() =>
        {
            D.Log("CtrlStatic Init Over");
        });

        yield return new WaitForSeconds(1);

        EventMgr.Trigger(new CtrlStaticEvent());

        yield return new WaitForSeconds(1);

        yield break;
    }

    public void zzInstanceTest()
    {
        StartCoroutine(InstanceTest());
    }

    IEnumerator InstanceTest()
    {
        yield return new WaitForSeconds(1);

        DataInstance.Instance.data0 = null;
        DataInstance.Instance.data1 = null;

        yield return new WaitForSeconds(1);

        EventMgr.Trigger(new DataInstanceShouldChangeEvent { key = "data0", data = "hello" });

        yield return new WaitForSeconds(1);

        EventMgr.Trigger(new DataInstanceShouldChangeEvent { key = "data1", data = "hello" });

        yield return new WaitForSeconds(1);

        DataInstance.ClearInstance();

        yield return new WaitForSeconds(1);

        EventMgr.Trigger(new DataInstanceShouldChangeEvent { key = "data0", data = "hello" });

        yield return new WaitForSeconds(1);

        DataInstance.Instance.data0 = null;
        DataInstance.Instance.data1 = null;

        yield return new WaitForSeconds(1);

        EventMgr.Trigger(new DataInstanceShouldChangeEvent { key = "data1", data = "world" });

        yield return new WaitForSeconds(1);

        DataInstance.ClearInstance();

        yield break;
    }

    public void zzComponentTest()
    {
        StartCoroutine(ComponentTest());
    }

    IEnumerator ComponentTest()
    {
        yield return new WaitForSeconds(1);

        var go = new GameObject("CompTest");
        go.AddComponent<CompTest>();

        yield return new WaitForSeconds(1);

        EventMgr.Trigger(new CompTestEvent());

        yield return new WaitForSeconds(1);

        go.SetActive(false);

        yield return new WaitForSeconds(1);

        EventMgr.Trigger(new CompTestEvent());

        yield return new WaitForSeconds(1);

        var es = EventSubscriber.Bind(go);
        es.unsubscribeOnDisable = true;
        D.Log("CompTest set unsubscribeOnDisable true");

        yield return new WaitForSeconds(1);

        EventMgr.Trigger(new CompTestEvent());

        yield return new WaitForSeconds(1);

        Destroy(go);

        go = new GameObject("CompTest1");
        go.AddComponent<CompTest>();

        yield return new WaitForSeconds(1);

        EventMgr.Trigger(new CompTestEvent());

        yield return new WaitForSeconds(1);

        Destroy(go);

        yield return new WaitForSeconds(1);

        EventMgr.Trigger(new CompTestEvent());

        yield break;
    }
}
