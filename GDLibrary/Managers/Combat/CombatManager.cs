using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System;

namespace GDLibrary
{
    public class CombatManager : PausableGameComponent
    {
        #region Fields
        private readonly Keys[] combatKeys;
        private readonly List<EnemyObject> enemies;
        private readonly InventoryManager inventoryManager;
        private readonly KeyboardManager keyboardManager;
        private readonly ObjectManager objectManager;
        private readonly GridManager gridManager;

        private Random random = new Random();
        private EnemyObject enemyOnFocus;
        private PlayerObject player;
        #endregion

        #region Properties

        public PlayerObject Player {
            get {
                return this.player;
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
            ObjectManager objectManager,
            GridManager gridManager,
            Keys[] combatKeys
        ) : base(game, eventDispatcher, statusType)
        {
            this.inventoryManager = inventoryManager;
            this.keyboardManager = keyboardManager;
            this.objectManager = objectManager;
            this.gridManager = gridManager;
            this.combatKeys = combatKeys;

            this.enemies = new List<EnemyObject>();

            RegisterForEventHandling(eventDispatcher);
        }
        #endregion

        #region Event Handling
        protected override void RegisterForEventHandling(EventDispatcher eventDispatcher)
        {
            eventDispatcher.CombatEvent += EventDispatcher_CombatEvent;
            eventDispatcher.GameChanged += EventDispatcher_GameChanged;

            base.RegisterForEventHandling(eventDispatcher);
        }

        private void EventDispatcher_GameChanged(EventData eventData)
        {
            if (eventData.EventType.Equals(EventActionType.EnemyTurn))
            {
                EnemyTurn();
            }
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
<<<<<<< HEAD
            }
            else if (eventData.EventType == EventActionType.OnPlayerDeath)
            {
                //Exit to menu for now
                Game.Exit();
            }
            else if (eventData.EventType == EventActionType.PlayerHealthPickup)
=======
            } else if (eventData.EventType == EventActionType.OnPlayerDeath)
            {
                //Exit to menu for now
                EventDispatcher.Publish(new EventData(EventActionType.OnStart, EventCategoryType.Menu));
                EventDispatcher.Publish(new EventData(EventActionType.OnPause, EventCategoryType.Menu));
            } else if (eventData.EventType == EventActionType.PlayerHealthPickup)
>>>>>>> origin/master
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

        public bool Remove(Predicate<EnemyObject> predicate)
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

        public int RemoveAll(Predicate<EnemyObject> predicate)
        {
            return this.enemies.RemoveAll(predicate);
        }

        public EnemyObject GetEnemy(string id)
        {
            if (enemies != null)

                //Finds enemy where ID is equal to the passed ID
                return this.enemies.Find(x => x.ID == id);

            return null;
        }

        public override void Update(GameTime gameTime)
        {
            HandleKeyboardInput(gameTime);
            base.Update(gameTime);
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

        protected virtual void HandleKeyboardInput(GameTime gameTime)
        {
            //If not in combat, return
            if (!StateManager.InCombat) return;

            //If not the players' turn, return
            if (!StateManager.PlayerTurn) return;

<<<<<<< HEAD
=======
            //Publish player health ui event
            EventDispatcher.Publish(new EventData(EventActionType.PlayerHealthUpdate, EventCategoryType.Textbox, new object[] { this.player.Health }));
            
>>>>>>> origin/master
            #region Attack
            //If the player attacks
            if (this.keyboardManager.IsFirstKeyPress(this.combatKeys[0]))
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

<<<<<<< HEAD
                //Publish play attack sound event
                if (this.inventoryManager.HasItem(PickupType.Sword))
                    EventDispatcher.Publish(new EventData(EventActionType.OnPlay, EventCategoryType.Sound2D, new object[] { "h_attack" }));
                else
                    EventDispatcher.Publish(new EventData(EventActionType.OnPlay, EventCategoryType.Sound2D, new object[] { "punch_01" }));

=======
>>>>>>> origin/master
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
<<<<<<< HEAD
                else
                {
                    //Publish a UI player attack event
                    EventDispatcher.Publish(new EventData(EventActionType.OnPlayerAttack, EventCategoryType.Textbox, new object[] { damage, this.player.Health, this.enemyOnFocus.Health }));
                }

                //Publish enemy turn event
                EventDispatcher.Publish(new EventData(EventActionType.EnemyTurn, EventCategoryType.Game));
=======

                //Publish play attack sound event
                if (this.inventoryManager.HasItem(PickupType.Sword))
                    EventDispatcher.Publish(new EventData(EventActionType.OnPlay, EventCategoryType.Sound2D, new object[] { "h_attack" }));
                else
                    EventDispatcher.Publish(new EventData(EventActionType.OnPlay, EventCategoryType.Sound2D, new object[] { "punch_01" }));

                //Publish player attack event
                //EventDispatcher.Publish(new EventData(EventActionType.OnPlayerAttack, EventCategoryType.Combat));

                //Publish a UI player attack event
                EventDispatcher.Publish(new EventData(EventActionType.OnPlayerAttack, EventCategoryType.Textbox, new object[] { damage }));

                //Publish enemy turn event
                EventDispatcher.Publish(new EventData(EventActionType.EnemyTurn, EventCategoryType.Game));

>>>>>>> origin/master
                return;
            }
            #endregion

            #region Defence
            //If the player defends
            if (this.keyboardManager.IsFirstKeyPress(this.combatKeys[1]))
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

<<<<<<< HEAD
                //If the enemy has killed the player
                if (this.player.Health <= 0) EventDispatcher.Publish(new EventData(EventActionType.OnPlayerDeath, EventCategoryType.Combat));

                //Publish play block sound event
                EventDispatcher.Publish(new EventData(EventActionType.OnPlay, EventCategoryType.Sound2D, new object[] { "block" }));

                //Publish a UI player attack event
                EventDispatcher.Publish(new EventData(EventActionType.OnPlayerDefend, EventCategoryType.Textbox, new object[] { damage, this.player.Health, this.enemyOnFocus.Health }));
=======
                //Publish play block sound event
                EventDispatcher.Publish(new EventData(EventActionType.OnPlay, EventCategoryType.Sound2D, new object[] { "block" }));

                //Publish player defend event
                //EventDispatcher.Publish(new EventData(EventActionType.OnPlayerDefend, EventCategoryType.Combat));

                //Publish a UI player attack event
                EventDispatcher.Publish(new EventData(EventActionType.OnPlayerDefend, EventCategoryType.Textbox, new object[] { damage }));

                //If the enemy has killed the player
                if (this.player.Health <= 0) EventDispatcher.Publish(new EventData(EventActionType.OnPlayerDeath, EventCategoryType.Combat));

                //Publish enemy turn event
                //EventDispatcher.Publish(new EventData(EventActionType.EnemyTurn, EventCategoryType.Game));


>>>>>>> origin/master
                return;
            }
            #endregion

            #region Dodge
            //If the player dodges
            if (this.keyboardManager.IsFirstKeyPress(this.combatKeys[2]))
            {
                //Info
                Console.WriteLine("Player dodge event");
                PrintStats(this.enemyOnFocus);

                //Calculate dodge chance
                int dodge = random.Next(1, 7);

                //If the player has successfully dodged
                if (dodge % 2 == 0)
                {
                    float damage = 0;

                    //Publish play dodge sound event
                    EventDispatcher.Publish(new EventData(EventActionType.OnPlay, EventCategoryType.Sound2D, new object[] { "dodge" }));

<<<<<<< HEAD
                    //Publish a UI player dodge event
                    EventDispatcher.Publish(new EventData(EventActionType.OnPlayerDodge, EventCategoryType.Textbox, new object[] { damage, this.player.Health, this.enemyOnFocus.Health }));
                    return;
                }
                else
                {
=======
                    //Publish player dodge event
                    //EventDispatcher.Publish(new EventData(EventActionType.OnPlayerDodge, EventCategoryType.Combat));

                    //Publish a UI player dodge event
                    EventDispatcher.Publish(new EventData(EventActionType.OnPlayerDodge, EventCategoryType.Textbox, new object[] { damage }));

                    //Publish enemy turn event
                    //EventDispatcher.Publish(new EventData(EventActionType.EnemyTurn, EventCategoryType.Game));
                    return;
                } else
                {
                    //Publish a UI player dodge event
                    EventDispatcher.Publish(new EventData(EventActionType.OnPlayerDodge, EventCategoryType.Textbox, new object[] { enemyOnFocus.Attack }));

>>>>>>> origin/master
                    //Take damage
                    this.player.TakeDamage(enemyOnFocus.Attack);

                    //If the enemy has killed the player
                    if (this.player.Health <= 0) EventDispatcher.Publish(new EventData(EventActionType.OnPlayerDeath, EventCategoryType.Combat));

<<<<<<< HEAD
                    //Publish a UI player dodge event
                    EventDispatcher.Publish(new EventData(EventActionType.OnPlayerDodge, EventCategoryType.Textbox, new object[] { this.enemyOnFocus.Attack, this.player.Health, this.enemyOnFocus.Health }));
                    
                    //Publish enemy turn event
                    EventDispatcher.Publish(new EventData(EventActionType.EnemyTurn, EventCategoryType.Game));
                    return;
=======
>>>>>>> origin/master
                }
            }
            #endregion
        }

        public void EnemyTurn()
        {
            //If not in combat, return
            if (!StateManager.InCombat) return;

            //If not the enemys' turn, return
            if (!StateManager.EnemyTurn) return;

<<<<<<< HEAD
=======
            //Publish player health ui event
            EventDispatcher.Publish(new EventData(EventActionType.EnemyHealthUpdate, EventCategoryType.Textbox, new object[] { this.enemyOnFocus.Health }));

>>>>>>> origin/master
            #region Attack
            //Enemy Attack - Default for release 1
            if (true)
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
<<<<<<< HEAD
                EventDispatcher.Publish(new EventData(EventActionType.OnEnemyAttack, EventCategoryType.Textbox, new object[] { damage, this.player.Health, this.enemyOnFocus.Health }));
=======
                EventDispatcher.Publish(new EventData(EventActionType.OnEnemyAttack, EventCategoryType.Textbox, new object[] { damage }));
>>>>>>> origin/master

                //Publish player game turn event
                EventDispatcher.Publish(new EventData(EventActionType.PlayerTurn, EventCategoryType.Game));
                return;
            }
            #endregion

            #region Defence
            //To be implemented
            #endregion

            #region Block
            //To be implemented
            #endregion
        }

        public void InitiateBattle(CharacterObject character)
        {
            StateManager.InCombat = true;
            StateManager.PlayerTurn = true;

            Console.WriteLine("UPDATED PLAYER DAMAGE: " + this.player.Attack);

            if (character.ActorType.Equals(ActorType.Enemy))
                this.enemyOnFocus = character as EnemyObject;
        }
        #endregion
    }
}