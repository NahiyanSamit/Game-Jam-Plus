using System;
using UnityEngine;

public class SystemCollections : MonoBehaviour
{
   private bool addedInTheInventory = false;
   public event Action OnAddedToInventory;
   public bool AddedInTheInventory
   {
      get => addedInTheInventory;
      set => addedInTheInventory = value;
   }
   public void OnCollisionEnter(Collision other)
   {
      if (other.gameObject.tag == "Player")
      {
         addedInTheInventory = true;
          OnAddedToInventory?.Invoke();
      }
   }
}
