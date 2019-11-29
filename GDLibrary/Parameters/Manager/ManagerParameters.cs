/*
Function: 		Encapsulates manager parameters for those classes (e.g. MouseManager) that need access to a large number of managers.
Author: 		NMCG
Version:		1.0
Bugs:			None
Fixes:			None
*/

namespace GDLibrary
{
    public class ManagerParameters
    {
        #region Fields
        private readonly ObjectManager objectManager;
        private readonly CameraManager cameraManager;
        private readonly MouseManager mouseManager;
        private readonly KeyboardManager keyboardManager;
        private readonly GamepadManager gamepadManager;
        private readonly SoundManager soundManager;
        private readonly PhysicsManager physicsManager;
        private readonly InventoryManager inventoryManager;
        private readonly CombatManager combatManager;
        private readonly UIManager uiManager;
        private readonly TextboxManager textboxManager;
        #endregion

        #region Properties
        public ObjectManager ObjectManager
        {
            get
            {
                return this.objectManager;
            }
        }

        public CameraManager CameraManager
        {
            get
            {
                return this.cameraManager;
            }
        }

        public MouseManager MouseManager
        {
            get
            {
                return this.mouseManager;
            }
        }

        public KeyboardManager KeyboardManager
        {
            get
            {
                return this.keyboardManager;
            }
        }

        public GamepadManager GamepadManager
        {
            get
            {
                return this.gamepadManager;
            }
        }

        public SoundManager SoundManager
        {
            get
            {
                return this.soundManager;
            }
        }

        public PhysicsManager PhysicsManager
        {
            get
            {
                return this.physicsManager;
            }
        }

        public InventoryManager InventoryManager
        {
            get
            {
                return this.inventoryManager;
            }
        }

        public CombatManager CombatManager
        {
            get
            {
                return this.combatManager;
            }
        }

        public UIManager UIManager
        {
            get
            {
                return this.uiManager;
            }
        }

        public TextboxManager TextboxManager
        {
            get
            {
                return this.textboxManager;
            }
        }
        #endregion

        #region Constructors
        public ManagerParameters(
            ObjectManager objectManager,
            CameraManager cameraManager,
            MouseManager mouseManager,
            KeyboardManager keyboardManager,
            GamepadManager gamepadManager,
            SoundManager soundManager,
            PhysicsManager physicsManager,
            InventoryManager inventoryManager,
            CombatManager combatManager,
            UIManager uiManager,
            TextboxManager textboxManager
        ) {
            this.objectManager = objectManager;
            this.cameraManager = cameraManager;
            this.mouseManager = mouseManager;
            this.keyboardManager = keyboardManager;
            this.gamepadManager = gamepadManager;
            this.soundManager = soundManager;
            this.physicsManager = physicsManager;
            this.inventoryManager = inventoryManager;
            this.combatManager = combatManager;
            this.uiManager = uiManager;
            this.textboxManager = textboxManager;
        }
        #endregion
    }
}