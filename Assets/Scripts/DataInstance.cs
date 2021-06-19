public class DataInstanceShouldChangeEvent
{
    public string key;
    public string data;
}

public class DataInstanceChangedEvent
{
    public string data;
}

public class DataInstance
{
    public const string DataInstanceData0Changed = "DataInstanceData0Changed";
    public const string DataInstanceData1Changed = "DataInstanceData1Changed";

    private static DataInstance _instance;

    public static DataInstance Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new DataInstance();
            }

            return _instance;
        }
    }

    ~DataInstance()
    {
        D.Log("DataInstance.Destructor");
    }

    public static void ClearInstance()
    {
        D.Log("DataInstance.ClearInstance");

        // set _instance to null and call GC.Collect() does not release memory and invoke the last instance's destructor 
        // so remove listeners manually
        EventMgr.RemoveAllListeners(_instance);

        _instance = null;
    }

    private string _data0 = null;
    public string data0
    {
        get
        {
            return _data0;
        }

        set
        {
            var changed = _data0 != null ? !_data0.Equals(value) : value != null;
            _data0 = value;

            if (changed) EventMgr.Trigger(new DataInstanceChangedEvent { data = value });
            if (changed) EventMgr.Trigger(DataInstanceData0Changed, new DataInstanceChangedEvent { data = value });
        }
    }

    private string _data1 = null;
    public string data1
    {
        get
        {
            return _data1;
        }

        set
        {
            var changed = _data1 != null ? !_data1.Equals(value) : value != null;
            _data1 = value;

            if (changed) EventMgr.Trigger(new DataInstanceChangedEvent { data = value });
            if (changed) EventMgr.Trigger(DataInstanceData1Changed, new DataInstanceChangedEvent { data = value });
        }
    }

    DataInstance()
    {
        EventMgr.AddListener<DataInstanceShouldChangeEvent>(OnDataShouldChange, this);
    }

    private void OnDataShouldChange(DataInstanceShouldChangeEvent e)
    {
        D.Log("DataInstance.OnDataShouldChange", e.key, e.data);

        if (e.key.Equals("data0"))
            data0 = e.data;
        else if (e.key.Equals("data1"))
            data1 = e.data;
    }
}
