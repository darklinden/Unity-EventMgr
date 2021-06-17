using System.Collections;
using UnityEngine;

public class Test : MonoBehaviour
{
    private void Start()
    {
        EventMgr.AddListener<DataInstanceChangedEvent>(OnDataInstanceChangedEvent);
    }

    private void OnDataInstanceChangedEvent(DataInstanceChangedEvent e)
    {
        D.Log("Test.OnDataInstanceChangedEvent", e.data);
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

        DataInstance.Instance.data = null;

        yield return new WaitForSeconds(1);

        EventMgr.Trigger(new DataInstanceShouldChangeEvent { data = "hello" });

        yield return new WaitForSeconds(1);

        DataInstance.ClearInstance();

        yield return new WaitForSeconds(1);

        EventMgr.Trigger(new DataInstanceShouldChangeEvent { data = "hello" });

        yield return new WaitForSeconds(1);

        DataInstance.Instance.data = null;

        yield return new WaitForSeconds(1);

        EventMgr.Trigger(new DataInstanceShouldChangeEvent { data = "world" });

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
