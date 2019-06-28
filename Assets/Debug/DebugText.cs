using UnityEngine;
using UnityEngine.UI;

public class DebugText : MonoBehaviour
{

    public Text _ui;
    private static DebugText _instance;

    // Start is called before the first frame update
    void Start()
    {
        if (_instance != null)
        {
            Debug.Log("duplicate DebugText object? SelfDestruction initiated");
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    public static void ShowVector(string label, Vector3 v)
    {
        if (_instance == null) return;
        _instance._ui.text = string.Format(label + " ({0,-5:F2},{1,-5:F2},{2,-5:F2}", v.x, v.y, v.z);
    }
    public static string text
    {
        set { if (_instance != null) _instance._ui.text = value; else Debug.Log("(could not find Text UI, so logging to console: " + value); }
        get { if (_instance != null) return _instance._ui.text; else return null; }
    }
}
