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
        private ObjectManager objectManager;
        private CameraManager cameraManager;
        private MouseManager mouseManager;
        private KeyboardManager keyboardManager;
        private GamePadManager gamePadManager;
        private SoundManager soundManager;
        private PhysicsManager physicsManager;
        private InventoryManager inventoryManager;
        private CombatManager combatManager;
        private UIManager uiManager;
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

        public GamePadManager GamePadManager
        {
            get
            {
                return this.gamePadManager;
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
        #endregion

        #region Constructors
        public ManagerParameters(
            ObjectManager objectManager,
            CameraManager cameraManager,
            MouseManager mouseManager,
            KeyboardManager keyboardManager,
            GamePadManager gamePadManager,
            SoundManager soundManager,
            PhysicsManager physicsManager,
            InventoryManager inventoryManager,
            CombatManager combatManager,
            UIManager uiManager
        ) {
            this.objectManager = objectManager;
            this.cameraManager = cameraManager;
            this.mouseManager = mouseManager;
            this.keyboardManager = keyboardManager;
            this.gamePadManager = gamePadManager;
            this.soundManager = soundManager;
            this.physicsManager = physicsManager;
            this.inventoryManager = inventoryManager;
            this.combatManager = combatManager;
            this.uiManager = uiManager;
        }
        #endregion
    }
}