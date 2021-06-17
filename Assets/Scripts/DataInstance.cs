public class DataInstanceShouldChangeEvent : EventMgr.Event
{
    public string data;
}

public class DataInstanceChangedEvent : EventMgr.Event
{
    public string data;
}

public class DataInstance
{
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

    private string _data = null;
    public string data
    {
        get
        {
            return _data;
        }

        set
        {
            var changed = _data != null ? !_data.Equals(value) : value != null;
            _data = value;

            if (changed) EventMgr.Trigger(new DataInstanceChangedEvent { data = value });
        }
    }

    DataInstance()
    {
        EventMgr.AddListener<DataInstanceShouldChangeEvent>(OnDataShouldChange, this);
    }

    private void OnDataShouldChange(DataInstanceShouldChangeEvent e)
    {
        D.Log("DataInstance.OnDataShouldChange", e.data);
        data = e.data;
    }
}
