using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterState : MonoBehaviour
{
   /// <summary>
   /// Decides how the character is generally able to use their controls.
   /// If disabled or locked, the character can't move and the CombatState will be handled differently.
   /// </summary>
   public enum MovementState
   {
      Neutral,
      Moving,
      Airborne,
      Disabled,
      Locked,
      Free
   }

   /// <summary>
   /// Keeps track of the character's state in combat.
   /// </summary>
   public enum CombatState
   {
      /// <summary>
      /// The character isn't doing anything.
      /// </summary>
      Neutral,
      /// <summary>
      /// The character is winding up an attack.
      /// </summary>
      Attacking,
      /// <summary>
      /// The character has hit an enemy and is frozen for some frames.
      /// </summary>
      Hitlag,
      /// <summary>
      /// The character has been hit and cannot act until this state changes.
      /// </summary>
      Hitstun,
      /// <summary>
      /// The character is winding back from their attack.
      /// </summary>
      Recovery,
      
   }

   public MovementState currentMovementState;

   public CombatState currentCombatState;

   
   
}
