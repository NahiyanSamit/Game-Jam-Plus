using System;
using UnityEngine;

public class SystemCollections : MonoBehaviour
{
   private bool addedInTheInventory = false;
   public event Action OnAddedToInventory;

   // NEW: mark what ability this collectible represents in the Inspector
   public AbilityType abilityType = AbilityType.Sound;

   // NEW: store the last collected ability so listeners can decide whether to react
   private AbilityType _lastCollectedAbility = AbilityType.Jump;
   public AbilityType LastCollectedAbility => _lastCollectedAbility;

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
         _lastCollectedAbility = abilityType;
         OnAddedToInventory?.Invoke();
      }
   }
}
