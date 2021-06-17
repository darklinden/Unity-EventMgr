using System.Linq;
using System;
using System.Collections.Generic;
using UnityEngine;

public class EventMgr
{
    public class Event { }

    public delegate void EventDelegate<T>(T e) where T : Event;

    private delegate void EventDelegate(Event e);

    private class DelegateContainer
    {
        public Delegate SrcDel;
        public EventDelegate Delegate;
        public WeakReference<System.Object> WeakTarget;
    }

    private Dictionary<Type, List<DelegateContainer>> delegates = new Dictionary<Type, List<DelegateContainer>>();

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

    private void _AddDelegate<T>(EventDelegate<T> del, System.Object target) where T : Event
    {
        EventDelegate internalDelegate = (e) => del((T)e);

        List<DelegateContainer> delegateList;
        if (delegates.ContainsKey(typeof(T)))
        {
            delegateList = delegates[typeof(T)];
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
                WeakTarget = target != null ? new WeakReference<object>(target) : null
            });
        }

        delegates[typeof(T)] = delegateList;
    }

    public static void AddListener<T>(EventDelegate<T> del, System.Object target = null) where T : Event
    {
        GameObject tar = target as GameObject;
        if (tar == null)
        {
            var comp = (del.Target as MonoBehaviour);
            if (comp != null)
            {
                tar = comp.gameObject;
            }
        }

        if (tar != null) EventSubscriber.Bind(tar);
        Instance._AddDelegate(del, target != null ? target : tar);
    }

    private void _RemoveListener<T>(EventDelegate<T> del) where T : Event
    {
        List<DelegateContainer> delegateList;
        if (delegates.TryGetValue(typeof(T), out delegateList))
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
    }

    private void _RemoveAllListeners(System.Object target)
    {
        if (target == null || (target is GameObject && (GameObject)target == null)) return;

        var keys = delegates.Keys.ToList();
        for (int k = keys.Count - 1; k >= 0; k--)
        {
            var key = keys[k];
            var delegateList = delegates[key];

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

                delegates[key] = delegateList;
            }
        }
    }

    public static void RemoveListener<T>(EventDelegate<T> del) where T : Event
    {
        Instance._RemoveListener(del);
    }

    public static void RemoveAllListeners(System.Object target)
    {
        Instance._RemoveAllListeners(target);
    }

    public void RemoveAll()
    {
        delegates.Clear();
    }

    private void _Trigger(Event e)
    {
        var invoked = false;

        List<DelegateContainer> delegateList;
        if (delegates.TryGetValue(e.GetType(), out delegateList))
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

            delegates[e.GetType()] = delegateList;
        }

        if (!invoked)
        {
            D.Warning("EventMgr.Trigger Event has no listener:", e);
        }
    }

    public static void Trigger(Event e)
    {
        Instance._Trigger(e);
    }
}