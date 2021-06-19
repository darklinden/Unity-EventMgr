using System.Linq;
using System;
using System.Collections.Generic;
using UnityEngine;

public class EventMgr
{
    public delegate void EventDelegate<T>(T e);

    private delegate void EventDelegate(System.Object e);

    private class DelegateContainer
    {
        public Delegate SrcDel;
        public EventDelegate Delegate;
        public WeakReference<System.Object> WeakTarget;
    }

    private Dictionary<string, List<DelegateContainer>> delegateCache = new Dictionary<string, List<DelegateContainer>>();

    private static EventMgr _instance;

    public static EventMgr Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new EventMgr();
            }

            return _instance;
        }
    }

    public EventMgr()
    {
        D.Log("EventMgr.Constructor");
    }

    public static string EventName(System.Type eventType)
    {
        return eventType.Name;
    }

    private void _AddDelegate<T>(string eventName, EventDelegate<T> del, System.Object target)
    {
        // get gameObject for auto unsubscribe

        GameObject go = null;
        if (target != null)
        {
            var comp = target as MonoBehaviour;
            if (comp != null)
            {
                go = comp.gameObject;
            }

            if (go == null)
            {
                go = target as GameObject;
            }
        }

        if (go == null)
        {
            var comp = (del.Target as MonoBehaviour);
            if (comp != null)
            {
                go = comp.gameObject;
            }
        }

        if (go != null) EventSubscriber.Bind(go);

        WeakReference<object> weakTarget = null;
        if (target != null)
        {
            weakTarget = new WeakReference<object>(target);
        }
        else if (go != null)
        {
            weakTarget = new WeakReference<object>(go);
        }

        // cache delegate
        EventDelegate internalDelegate = (e) => del((T)e);

        List<DelegateContainer> delegateList;
        if (delegateCache.ContainsKey(eventName))
        {
            delegateList = delegateCache[eventName];
        }
        else
        {
            delegateList = new List<DelegateContainer>();
        }

        var alreadyHas = false;
        foreach (var d in delegateList)
        {
            if (d.SrcDel.Equals(del))
            {
                alreadyHas = true;
                break;
            }
        }

        if (!alreadyHas)
        {
            delegateList.Add(new DelegateContainer
            {
                SrcDel = del,
                Delegate = internalDelegate,
                WeakTarget = weakTarget
            });
        }

        delegateCache[eventName] = delegateList;
    }

    public static void AddListener<T>(string eventName, EventDelegate<T> del, System.Object target = null)
    {
        Instance._AddDelegate(eventName, del, target);
    }

    public static void AddListener<T>(EventDelegate<T> del, System.Object target = null)
    {
        Instance._AddDelegate(EventName(typeof(T)), del, target);
    }

    private void _RemoveListener<T>(string eventName, EventDelegate<T> del)
    {
        List<DelegateContainer> delegateList = null;
        if (delegateCache.TryGetValue(eventName, out delegateList))
        {
            if (delegateList != null)
            {
                for (int i = delegateList.Count - 1; i >= 0; i--)
                {
                    var d = delegateList[i];

                    if (d.SrcDel.Equals(del))
                    {
                        delegateList.Remove(d);
                    }
                }
            }
        }
        delegateCache[eventName] = delegateList;
    }

    private void _RemoveAllListeners(System.Object target)
    {
        if (target == null || (target is GameObject && (GameObject)target == null)) return;

        var keys = delegateCache.Keys.ToList();
        for (int k = keys.Count - 1; k >= 0; k--)
        {
            var key = keys[k];
            var delegateList = delegateCache[key];

            if (delegateList != null)
            {
                for (int i = delegateList.Count - 1; i >= 0; i--)
                {
                    var dc = delegateList[i];

                    var shouldRemove = false;
                    if (dc.WeakTarget != null)
                    {
                        System.Object tar = null;
                        if (dc.WeakTarget.TryGetTarget(out tar))
                        {
                            if (tar == null || (tar is GameObject && (GameObject)tar == null))
                            {
                                // weak null remove
                                shouldRemove = true;
                            }
                            else if (tar.Equals(target))
                            {
                                // target match remove 
                                shouldRemove = true;
                            }
                        }
                        else
                        {
                            // weak null remove 
                            shouldRemove = true;
                        }
                    }

                    if (shouldRemove) delegateList.Remove(dc);
                }

                delegateCache[key] = delegateList;
            }
        }
    }

    public static void RemoveListener<T>(string eventName, EventDelegate<T> del)
    {
        Instance._RemoveListener(eventName, del);
    }

    public static void RemoveListener<T>(EventDelegate<T> del)
    {
        Instance._RemoveListener(EventName(typeof(T)), del);
    }

    public static void RemoveAllListeners(System.Object target)
    {
        Instance._RemoveAllListeners(target);
    }

    public void RemoveAll()
    {
        delegateCache.Clear();
    }

    private void _Trigger<T>(string eventName, T e)
    {
        var invoked = false;

        List<DelegateContainer> delegateList;
        if (delegateCache.TryGetValue(eventName, out delegateList))
        {
            for (int i = delegateList.Count - 1; i >= 0; i--)
            {
                var dc = delegateList[i];

                var canInvoke = true;

                if (dc.WeakTarget != null)
                {
                    System.Object target = null;
                    if (dc.WeakTarget.TryGetTarget(out target))
                    {
                        if (target == null || (target is GameObject && (GameObject)target == null))
                        {
                            canInvoke = false;
                        }
                    }
                    else
                    {
                        canInvoke = false;
                    }
                }

                if (dc.Delegate == null) canInvoke = false;

                if (canInvoke)
                {
                    invoked = true;
                    dc.Delegate.Invoke(e);
                }
                else
                {
                    delegateList.Remove(dc);
                }
            }

            delegateCache[eventName] = delegateList;
        }

        if (!invoked)
        {
            D.Warning("EventMgr.Trigger Event has no listener:", e);
        }
    }

    public static void Trigger<T>(string eventName, T e)
    {
        Instance._Trigger(eventName, e);
    }

    public static void Trigger<T>(T e)
    {
        Instance._Trigger(EventName(typeof(T)), e);
    }
}