﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BNG {

    /// <summary>
    /// Allows the Player to climb objects by Grabbing them
    /// </summary>
    public class Climbable : Grabbable {

        void Start() {
            // Make sure Climbable is set to dual grab
            SecondaryGrabBehavior = OtherGrabBehavior.DualGrab;

            // Make sure we don't try tp keep this in our hand
            GrabPhysics = GrabPhysics.None;

            CanBeSnappedToSnapZone = false;

            // Disable Break Distance entirely if default from Grabbable was used
            if(BreakDistance == 1) {
                BreakDistance = 0;
            }
        }

        public override void GrabItem(Grabber grabbedBy) {

            // Add the climber so we can track it's position for Character movement
            player.AddClimber(this, grabbedBy);
            
            base.GrabItem(grabbedBy);        
        }

        public override void DropItem(Grabber droppedBy) {
            if(droppedBy != null) {
                player.RemoveClimber(droppedBy);
            }
            
            base.DropItem(droppedBy);
        }
    }
}