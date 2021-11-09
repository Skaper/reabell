
using UnityEngine;

public class ScannerData : MonoBehaviour
{
    [SerializeField]
    [TextArea(1, 3)]
    private string data = "Type: garbage \n";

    public string Data { get; private set; }

    private void Start()
    {
        Data = data;
    }
}
