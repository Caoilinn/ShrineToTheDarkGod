using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System;

namespace GDLibrary
{
    public class CombatManager : PausableGameComponent
    {
        #region Fields
        private List<EnemyObject> enemies;
        private InventoryManager inventoryManager;
        private KeyboardManager keyboardManager;
        private GamepadManager gamepadManager;
        private ObjectManager objectManager;
        private GridManager gridManager;

        private PlayerIndex playerIndex;
        private Buttons[] combatButtons;
        private Keys[] combatKeys;

        private EnemyObject enemyOnFocus;
        private PlayerObject player;

        private Random random = new Random();
        #endregion

        #region Properties
        public List<EnemyObject> Enemies
        {
            get
            {
                return this.enemies;
            }
            set
            {
                this.enemies = value;
            }
        }

        public InventoryManager InventoryManager
        {
            get
            {
                return this.inventoryManager;
            }
            set
            {
                this.inventoryManager = value;
            }
        }

        public KeyboardManager KeyboardManager
        {
            get
            {
                return this.keyboardManager;
            }
            set
            {
                this.keyboardManager = value;
            }
        }

        public GamepadManager GamepadManager
        {
            get
            {
                return this.gamepadManager;
            }
            set
            {
                this.gamepadManager = value;
            }
        }

        public ObjectManager ObjectManager
        {
            get
            {
                return this.objectManager;
            }
            set
            {
                this.objectManager = value;
            }
        }

        public GridManager GridManager
        {
            get
            {
                return this.gridManager;
            }
            set
            {
                this.gridManager = value;
            }
        }

        public PlayerIndex PlayerIndex
        {
            get
            {
                return this.playerIndex;
            }
            set
            {
                this.playerIndex = value;
            }
        }

        public Buttons[] CombatButtons
        {
            get
            {
                return this.combatButtons;
            }
            set
            {
                this.combatButtons = value;
            }
        }

        public Keys[] CombatKeys
        {
            get
            {
                return this.combatKeys;
            }
            set
            {
                this.combatKeys = value;
            }
        }

        public EnemyObject EnemyOnFocus
        {
            get
            {
                return this.enemyOnFocus;
            }
            set
            {
                this.enemyOnFocus = value;
            }
        }

        public PlayerObject Player
        {
            get
            {
                return this.player;
            }
            set
            {
                this.player = value;
            }
        }

        public Random Random
        {
            get
            {
                return this.random;
            }
            set
            {
                this.random = value;
            }
        }
        #endregion

        #region Constructor
        public CombatManager(
            Game game,
            EventDispatcher eventDispatcher,
            StatusType statusType,
            InventoryManager inventoryManager,
            KeyboardManager keyboardManager,
            GamepadManager gamepadManager,
            ObjectManager objectManager,
            GridManager gridManager,
            PlayerIndex playerIndex,
            Buttons[] combatButtons,
            Keys[] combatKeys
        ) : base(game, eventDispatcher, statusType) {
            this.InventoryManager = inventoryManager;
            this.KeyboardManager = keyboardManager;
            this.GamepadManager = gamepadManager;
            this.ObjectManager = objectManager;
            this.GridManager = gridManager;
            this.PlayerIndex = playerIndex;
            this.CombatButtons = combatButtons;
            this.CombatKeys = combatKeys;

            this.enemies = new List<EnemyObject>();
            RegisterForEventHandling(eventDispatcher);
        }
        #endregion

        #region Event Handling
        protected override void RegisterForEventHandling(EventDispatcher eventDispatcher)
        {
            eventDispatcher.CombatEvent += EventDispatcher_CombatEvent;

            base.RegisterForEventHandling(eventDispatcher);
        }
        
        protected void EventDispatcher_CombatEvent(EventData eventData)
        {
            if (eventData.EventType == EventActionType.OnEnemyDeath)
            {
                //Publish remove actor event
                EventDispatcher.Publish(new EventData(EventActionType.OnRemoveActor, EventCategoryType.SystemRemove, eventData.AdditionalParameters));

                //Publish player turn event
                EventDispatcher.Publish(new EventData(EventActionType.PlayerTurn, EventCategoryType.Game));

                //Update combat state
                StateManager.InCombat = false;
            }
            else if (eventData.EventType == EventActionType.OnPlayerDeath)
            {
                //Exit to menu for now
                Game.Exit();
            }
            else if (eventData.EventType == EventActionType.PlayerHealthPickup)
            {
                //Update player health
                Console.WriteLine("Player health before: " + this.player.Health);
                Console.WriteLine("Player health after: " + (this.player.Health += 10));

                //If the player has regained enough health
                if (this.player.Health > 30) EventDispatcher.Publish(new EventData(EventActionType.OnPause, EventCategoryType.Sound2D, new object[] { "player_health_low" }));
            }
        }
        #endregion

        #region Methods
        public void AddPlayer(PlayerObject player)
        {
            this.player = player;
        }

        public void PopulateEnemies(EnemyObject enemy)
        {
            this.enemies.Add(enemy);
        }

        public EnemyObject GetEnemy(string id)
        {
            if (enemies != null)

                //Finds enemy where ID is equal to the passed ID
                return this.enemies.Find(x => x.ID == id);

            return null;
        }

        public bool RemoveEnemy(Predicate<EnemyObject> predicate)
        {
            //Retrieve enemy
            EnemyObject enemy = this.enemies.Find(predicate);

            //If enemy exists
            if (enemy != null)

                //Remove enemy
                return this.enemies.Remove(enemy);

            //Enemy does not exist
            return false;
        }

        public int RemoveAllEnemies(Predicate<EnemyObject> predicate)
        {
            return this.enemies.RemoveAll(predicate);
        }

        public void PrintStats(EnemyObject enemy)
        {
            Console.WriteLine(
                "Player: \n"
                + "Health: " + this.player.Health + "\n"
                + " Attack: " + this.player.Attack + "\n"
                + "Defence: " + this.player.Defence
            );

            Console.WriteLine(
                "Enemy: " + enemy.ID + "\n"
                + "Health: " + enemy.Health + "\n"
                + "Attack: " + enemy.Attack + "\n"
                + "Defence: " + enemy.Defence + "\n"
            );
        }

        protected virtual void TakeTurn(GameTime gameTime)
        {
            //If not in combat, return
            if (!StateManager.InCombat) return;

            //If it is the players' turn
            if (StateManager.PlayerTurn)

                //If a controller is connected for the current player
                if (this.gamepadManager.IsPlayerConnected(this.PlayerIndex))

                    //Handle gamepad input
                    HandleGamepadInput(gameTime);

                //Otherwise
                else

                    //Handle keyboard input
                    HandleKeyboardInput(gameTime);

            //Otherwise
            else

                //Handle the enemys' turn
                EnemyTurn();
        }

        protected virtual void HandleKeyboardInput(GameTime gameTime)
        {
            #region Attack
            //If the player presses the attack key
            if (this.keyboardManager.IsFirstKeyPress(this.combatKeys[0]))
                PlayerAttack();
            #endregion

            #region Block
            //If the player presses the block key
            if (this.keyboardManager.IsFirstKeyPress(this.combatKeys[1]))
                PlayerBlock();
            #endregion

            #region Dodge
            //If the player presses the dodge key
            if (this.keyboardManager.IsFirstKeyPress(this.combatKeys[2]))
                PlayerDodge();
            #endregion
        }

        protected virtual void HandleGamepadInput(GameTime gameTime)
        {
            #region Attack
            //If the player presses the attack button
            if (this.gamepadManager.IsFirstButtonPress(this.PlayerIndex, this.combatButtons[0]))
            {
                PlayerAttack();
                return;
            }
            #endregion

            #region Block
            //If the player presses the block button
            if (this.gamepadManager.IsFirstButtonPress(this.PlayerIndex, this.combatButtons[1]))
            {
                PlayerBlock();
                return;
            }
            #endregion

            #region Dodge
            //If the player presses the dodge button
            if (this.gamepadManager.IsFirstButtonPress(this.PlayerIndex, this.combatButtons[2]))
            {
                PlayerDodge();
                return;
            }
            #endregion
        }

        public void PlayerAttack()
        {
            //Info
            Console.WriteLine("Player attack event");
            PrintStats(this.enemyOnFocus);

            //Calculate player attack damage
            float playerAttack = this.player.Attack;

            //Update damage if the player is holding a sword
            if (this.inventoryManager.HasItem(PickupType.Sword)) playerAttack += 15;

            float enemyDefence = this.enemyOnFocus.Defence;
            float damage = playerAttack - enemyDefence;

            //If the player has dealt damage
            if (damage > 0) this.enemyOnFocus.TakeDamage(damage);

            //Publish play attack sound event
            if (this.inventoryManager.HasItem(PickupType.Sword))
                EventDispatcher.Publish(new EventData(EventActionType.OnPlay, EventCategoryType.Sound2D, new object[] { "h_attack" }));
            else
                EventDispatcher.Publish(new EventData(EventActionType.OnPlay, EventCategoryType.Sound2D, new object[] { "punch_01" }));

            //If the player has killed the enemy
            if (this.enemyOnFocus.Health <= 0)
            {
                //Publish sound event
                EventDispatcher.Publish(new EventData(EventActionType.OnPlay, EventCategoryType.Sound2D, new object[] { "death" }));

                //Publish UI event
                EventDispatcher.Publish(new EventData(EventActionType.OnEnemyDeath, EventCategoryType.Textbox));

                //Publish enemy death event
                EventDispatcher.Publish(new EventData(EventActionType.OnEnemyDeath, EventCategoryType.Combat, new object[] { this.enemyOnFocus }));
            }
            else
            {
                //Publish a UI player attack event
                EventDispatcher.Publish(new EventData(EventActionType.OnPlayerAttack, EventCategoryType.Textbox, new object[] { damage, this.player.Health, this.enemyOnFocus.Health }));
            }

            //Publish enemy turn event
            EventDispatcher.Publish(new EventData(EventActionType.EnemyTurn, EventCategoryType.Game));
        }

        public void PlayerBlock()
        {
            //Info
            Console.WriteLine("Player defend event");
            PrintStats(this.enemyOnFocus);

            //Calculate player defend damage
            float playerDefence = this.player.Defence;
            float enemyAttack = this.enemyOnFocus.Attack;
            float damage = enemyAttack - playerDefence;

            //If the player has taken damage
            if (damage > 0) this.player.TakeDamage(damage);

            //If the enemy has killed the player
            if (this.player.Health <= 0) EventDispatcher.Publish(new EventData(EventActionType.OnPlayerDeath, EventCategoryType.Combat));

            //Publish play block sound event
            EventDispatcher.Publish(new EventData(EventActionType.OnPlay, EventCategoryType.Sound2D, new object[] { "block" }));

            //Publish a UI player attack event
            EventDispatcher.Publish(new EventData(EventActionType.OnPlayerDefend, EventCategoryType.Textbox, new object[] { damage, this.player.Health, this.enemyOnFocus.Health }));
        }

        public void PlayerDodge()
        {
            //Info
            Console.WriteLine("Player dodge event");
            PrintStats(this.enemyOnFocus);

            //Calculate dodge chance
            int dodge = random.Next(1, 7);

            //If the player has successfully dodged
            if (dodge % 2 == 0)
            {
                //Reset damage
                float damage = 0;

                //Publish play dodge sound event
                EventDispatcher.Publish(new EventData(EventActionType.OnPlay, EventCategoryType.Sound2D, new object[] { "dodge" }));

                //Publish a UI player dodge event
                EventDispatcher.Publish(new EventData(EventActionType.OnPlayerDodge, EventCategoryType.Textbox, new object[] { damage, this.player.Health, this.enemyOnFocus.Health }));
            }
            else
            {
                //Take damage
                this.player.TakeDamage(enemyOnFocus.Attack);

                //If the enemy has killed the player
                if (this.player.Health <= 0) EventDispatcher.Publish(new EventData(EventActionType.OnPlayerDeath, EventCategoryType.Combat));

                //Publish a UI player dodge event
                EventDispatcher.Publish(new EventData(EventActionType.OnPlayerDodge, EventCategoryType.Textbox, new object[] { this.enemyOnFocus.Attack, this.player.Health, this.enemyOnFocus.Health }));

                //Publish enemy turn event
                EventDispatcher.Publish(new EventData(EventActionType.EnemyTurn, EventCategoryType.Game));
            }
        }

        public void EnemyTurn()
        {
            #region Attack
            EnemyAttack();
            #endregion

            #region Block
            EnemyBlock();
            #endregion

            #region Dodge
            EnemyDodge();
            #endregion
        }

        public void EnemyAttack()
        {
            //Info
            Console.WriteLine("Enemy attack event");
            PrintStats(this.enemyOnFocus);

            //Calculate enemy attack damage
            float enemyAttack = this.enemyOnFocus.Attack;
            float playerDefence = this.player.Defence;
            float damage = enemyAttack - playerDefence;

            //If the enemy has dealt damage
            if (damage > 0) this.player.TakeDamage(damage);

            //If the players' health is low
            if (this.player.Health <= 30) EventDispatcher.Publish(new EventData(EventActionType.OnPlay, EventCategoryType.Sound2D, new object[] { "player_health_low" }));

            //If the enemy has killed the player
            if (this.player.Health <= 0) EventDispatcher.Publish(new EventData(EventActionType.OnPlayerDeath, EventCategoryType.Combat));

            //Publish play attack sound event
            EventDispatcher.Publish(new EventData(EventActionType.OnPlay, EventCategoryType.Sound2D, new object[] { "enemy_attack" }));

            //Publish enemy attack event to the UI Manager
            EventDispatcher.Publish(new EventData(EventActionType.OnEnemyAttack, EventCategoryType.Textbox, new object[] { damage, this.player.Health, this.enemyOnFocus.Health }));

            //Publish player game turn event
            EventDispatcher.Publish(new EventData(EventActionType.PlayerTurn, EventCategoryType.Game));
        }

        public void EnemyBlock()
        {
            //To be implemented
        }

        public void EnemyDodge()
        {
            //To be implemented
        }

        public void InitiateBattle(CharacterObject character)
        {
            StateManager.InCombat = true;
            StateManager.PlayerTurn = true;
            
            if (character.ActorType.Equals(ActorType.Enemy))
                this.enemyOnFocus = character as EnemyObject;
        }

        public override void Update(GameTime gameTime)
        {
            TakeTurn(gameTime);
            base.Update(gameTime);
        }
        #endregion
    }
}