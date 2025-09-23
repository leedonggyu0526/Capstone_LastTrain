using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class test : MonoBehaviour
{

    public TextMeshProUGUI testObj;
    public InventoryUIManager ium;
    private int n = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        testObj.text = n.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        if (ium.HasItemByID(1))
        {
            testObj.text = (n + 250).ToString();
        }

    }
}
