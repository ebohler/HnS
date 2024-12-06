using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InventoryUI : MonoBehaviour
{
    private TextMeshProUGUI collectibleText;

    // Start is called before the first frame update
    void Start()
    {
        collectibleText = GetComponent<TextMeshProUGUI>();
    }

    public void UpdateCollectibleText(PlayerInventory playerInventory)
    {
        collectibleText.text = playerInventory.NumCollected.ToString() + "/5";
    }
}

