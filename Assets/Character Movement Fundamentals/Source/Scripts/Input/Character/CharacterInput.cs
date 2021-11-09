using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CMF
{
    //This abstract character input class serves as a base for all other character input classes;
    //The 'Controller' component will access this script at runtime to get input for the character's movement (and jumping);
    //By extending this class, it is possible to implement custom character input;
    public abstract class CharacterInput : MonoBehaviour
    {
        public abstract float GetHorizontalMovementInput();
        public abstract float GetVerticalMovementInput();

        public abstract bool IsJumpKeyPressed();

        public abstract bool IsAttack1KeyDown();

        public abstract bool IsAttack1KeyUp();

        public abstract bool IsAttack2KeyDown();
    
        public abstract bool IsAttack2KeyUp();

        public abstract bool IsAction1KeyDown();

        public abstract bool IsAction1KeyUp();

        public abstract bool IsAction2KeyDown();

        public abstract bool IsAction2KeyUp();

        public abstract bool IsAction3KeyDown();

        public abstract bool IsAction3KeyUp();

        public abstract bool IsAction4KeyDown();

        public abstract bool IsAction4KeyUp();
    }
}
