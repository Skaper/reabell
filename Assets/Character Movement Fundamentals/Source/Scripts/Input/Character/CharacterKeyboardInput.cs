using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CMF
{
	//This character movement input class is an example of how to get input from a keyboard to control the character;
    public class CharacterKeyboardInput : CharacterInput
    {
		public string horizontalInputAxis = "Horizontal";
		public string verticalInputAxis = "Vertical";

		//If this is enabled, Unity's internal input smoothing is bypassed;
		public bool useRawInput = true;

        public override float GetHorizontalMovementInput()
		{
			if(useRawInput)
				return Input.GetAxisRaw(horizontalInputAxis);
			else
				return Input.GetAxis(horizontalInputAxis);
		}

		public override float GetVerticalMovementInput()
		{
			if(useRawInput)
				return Input.GetAxisRaw(verticalInputAxis);
			else
				return Input.GetAxis(verticalInputAxis);
		}

        public override bool IsAction1KeyDown()
        {
            return Input.GetKeyDown(InputManager.Action1);
        }

        public override bool IsAction1KeyUp()
        {
            return Input.GetKeyUp(InputManager.Action1);
        }

        public override bool IsAction2KeyDown()
        {
            return Input.GetKeyDown(InputManager.Action2);
        }

        public override bool IsAction2KeyUp()
        {
            return Input.GetKeyUp(InputManager.Action2);
        }

        public override bool IsAction3KeyDown()
        {
            return Input.GetKeyDown(InputManager.Action3);
        }

        public override bool IsAction3KeyUp()
        {
            throw new System.NotImplementedException();
        }

        public override bool IsAction4KeyDown()
        {
            return Input.GetKeyDown(InputManager.Action4);
        }

        public override bool IsAction4KeyUp()
        {
            return Input.GetKeyUp(InputManager.Action4);
        }

        public override bool IsAttack1KeyDown()
        {
            return Input.GetKeyDown(InputManager.Attack1);
        }

        public override bool IsAttack1KeyUp()
        {
            return Input.GetKeyUp(InputManager.Attack1);
        }

        public override bool IsAttack2KeyDown()
        {
            return Input.GetKeyDown(InputManager.Attack2);
        }

        public override bool IsAttack2KeyUp()
        {
            return Input.GetKeyUp(InputManager.Attack2);
        }

        public override bool IsJumpKeyPressed()
		{
            return Input.GetKey(InputManager.Jump);
        }
    }
}
