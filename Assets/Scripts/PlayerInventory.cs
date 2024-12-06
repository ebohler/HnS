using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerInventory : MonoBehaviour
{
    public int NumCollected { get; private set; }
    public UnityEvent<PlayerInventory> OnCollected;

    public void Collected()
    {
        NumCollected++;
        OnCollected.Invoke(this);
    }
}
