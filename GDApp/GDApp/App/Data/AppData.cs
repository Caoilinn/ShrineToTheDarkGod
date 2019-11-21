/*
Function: 		Stores common hard-coded variable values used within the game e.g. key mappings, mouse sensitivity
Author: 		NMCG
Version:		1.0
Date Updated:	5/10/17
Bugs:			None
Fixes:			None
*/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace GDLibrary
{
    public sealed class LerpSpeed
    {
        private static readonly float SpeedMultiplier = 2;
        public static readonly float VerySlow = 0.05f;
        public static readonly float Slow = SpeedMultiplier * VerySlow;
        public static readonly float Medium = SpeedMultiplier * Slow;
        public static readonly float Fast = SpeedMultiplier * Medium;
        public static readonly float VeryFast = SpeedMultiplier * Fast;
    }

    public sealed class AppData
    {
        #region Common
        public static int IndexMoveForward = 0;
        public static int IndexMoveBackward = 1;
        public static int IndexRotateLeft = 2;
        public static int IndexRotateRight = 3;
        public static int IndexMoveJump = 4;
        public static int IndexMoveCrouch = 5;
        public static int IndexStrafeLeft = 6;
        public static int IndexStrafeRight = 7;
        #endregion

        #region Camera
        public static readonly int CurveEvaluationPrecision = 4;

        public static readonly float CameraRotationSpeed = 0.3f;
        public static readonly float CameraMoveSpeed = 0.6f;
        public static readonly float CameraStrafeSpeed = 0.6f * CameraMoveSpeed;

        //JigLib related collidable camera properties
        public static readonly float CollidableCameraJumpHeight = 12;
        public static readonly float CollidableCameraMoveSpeed = 0.6f;
        public static readonly float CollidableCameraStrafeSpeed = 0.6f * CollidableCameraMoveSpeed;
        public static readonly float CollidableCameraCapsuleRadius = 2;
        public static readonly float CollidableCameraViewHeight = 8; //how tall is the first person player?
        public static readonly float CollidableCameraMass = 10;

        public static readonly Keys[] CameraMoveKeys = { Keys.W, Keys.S, Keys.A, Keys.D, Keys.Q, Keys.E, Keys.Space, Keys.C, Keys.LeftShift, Keys.RightShift};
        public static readonly Keys[] CombatKeys = {Keys.D1, Keys.D2, Keys.D3};
        public static readonly Keys[] CameraMoveKeys_Alt1 = { Keys.T, Keys.G, Keys.F, Keys.H };
        #endregion

        #region Character
        public static readonly float CharacterMass = 1;
        public static readonly float CharacterRadius = 77;
        public static readonly float CharacterHeight = 154;
        public static readonly float CharacterJumpHeight = 1;
        public static readonly float CharacterAccelerationRate = 1;
        public static readonly float CharacterDecelerationRate = 1;

        public static readonly float CharacterMoveSpeed = 0.6f;
        public static readonly float CharacterStrafeSpeed = 0.6f;
        public static readonly float CharacterRotateSpeed = 0.3f;

        public static readonly Vector3 CharacterMovementVector = new Vector3(254, 254, 254);
        public static readonly Vector3 CharacterRotationVector = new Vector3(90, 90, 90);

        public static readonly Keys[] CharacterMoveKeys = { Keys.W, Keys.S, Keys.A, Keys.D, Keys.Q, Keys.E, Keys.Space, Keys.C, Keys.LeftShift, Keys.RightShift };
        #endregion

        #region Character Stats
        public static readonly float PlayerHealth = 100;
        public static readonly float PlayerAttack = 25;
        public static readonly float PlayerDefence = 25;

        public static readonly float SkeletonHealth = 10;
        public static readonly float SkeletonAttack = 20;
        public static readonly float SkeletonDefence = 20;

        public static readonly float CultistHealth = 35;
        public static readonly float CultistAttack = 30;
        public static readonly float CultistDefence = 30;
        #endregion

        #region Menu
        public static readonly string MenuMainID = "main";
        public static readonly Keys MenuShowHideKey = Keys.Escape;
        public static readonly Keys KeyPauseShowMenu = Keys.Escape;
        public static readonly Keys KeyToggleCameraLayout = Keys.F1;
        #endregion

        #region Mouse
        //defines how much the mouse has to move in pixels before a movement is registered - see MouseManager::HasMoved()
        public static readonly float MouseSensitivity = 1;

        //always ensure that we start picking OUTSIDE the collidable first person camera radius - otherwise we will always pick ourself!
        public static readonly float PickStartDistance = CollidableCameraCapsuleRadius * 2f;
        public static readonly float PickEndDistance = 1000; //can be related to camera far clip plane radius but should be limited to typical level max diameter
        public static readonly bool EnablePickAndPlace = true;
        #endregion

        #region UI

        #endregion

        #region JigLibX
        public static readonly Vector3 Gravity = -10 * Vector3.UnitY;
        public static readonly Vector3 BigGravity = 5 * Gravity;
        #endregion

        #region Video
        public static readonly string VideoIDMainHall;
        public static readonly string ControllerIDSuffix = " controller";
        #endregion

        #region Primitive ids used by vertexData dictionary
        public static readonly string TexturedQuadID = "textured quad";
        public static readonly string TexturedBillboardQuadID = "textured billboard quad";
        #endregion

        #region Effect parameter ids used by the effect dictionary
        public static readonly string LitModelsEffectID = "lit models basic effect";
        public static readonly string UnlitModelsEffectID = "unlit models basic effect";
        public static readonly string UnLitPrimitivesEffectID = "unlit primitives basic effect";
        public static readonly string UnlitModelDualEffectID = "unlit models dual effect";
        public static readonly string UnlitBillboardsEffectID = "unlit billboards basic effect";
        public static readonly string WinZoneEffectID = "win zone basic effect";
        public static readonly string PickupEffectID = "pickup basic effect";
        #endregion
    }
}