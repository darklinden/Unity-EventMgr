using System;

public class CtrlStaticEvent { }

public static class CtrlStatic
{
    private static bool _init = false;
    public static void InitOnce(Action completion = null)
    {
        if (!_init)
        {
#if UNITY_EDITOR
            D.Log(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "InitOnce");
#endif
            _init = true;
            // init code

            EventMgr.AddListener<CtrlStaticEvent>(OnEvent);
        }
        else
        {
            completion?.Invoke();
        }
    }

    private static void OnEvent(CtrlStaticEvent e)
    {
        D.Log("CtrlStatic.OnEvent CtrlStaticEvent");
    }
}
