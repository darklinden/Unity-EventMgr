using UnityEngine;

[DisallowMultipleComponent]
public class EventSubscriber : MonoBehaviour
{
    private bool _unsubscribeOnDisable = false;
    public bool unsubscribeOnDisable
    {
        get
        {
            return _unsubscribeOnDisable;
        }

        set
        {
            _unsubscribeOnDisable = value;
            if (value && !gameObject.activeInHierarchy)
            {
                UnsubscribeAll();
            }
        }
    }

    private void OnDisable()
    {
        if (unsubscribeOnDisable)
        {
            UnsubscribeAll();
        }
    }

    private void OnDestroy()
    {
        UnsubscribeAll();
    }

    public void UnsubscribeAll()
    {
        EventMgr.RemoveAllListeners(gameObject);
    }

    public static EventSubscriber Bind(GameObject tar)
    {
        if (tar != null)
        {
            var comp = tar.GetComponent<EventSubscriber>();
            if (comp == null) comp = tar.AddComponent<EventSubscriber>();
            return comp;
        }

        return null;
    }
}
