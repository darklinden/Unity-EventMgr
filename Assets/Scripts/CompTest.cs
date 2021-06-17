using UnityEngine;

public class CompTestEvent : EventMgr.Event { }

public class CompTest : MonoBehaviour
{
    void Start()
    {
        EventMgr.AddListener<CompTestEvent>(OnCompTestEvent);
    }

    private void OnCompTestEvent(CompTestEvent e)
    {
        D.Log(gameObject.name, "OnCompTestEvent");
    }

    private void OnDestroy()
    {
        D.Log(gameObject.name, "OnDestroy");
    }

    private void OnDisable()
    {
        D.Log(gameObject.name, "OnDisable");
    }
}
