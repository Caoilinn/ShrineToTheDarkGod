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
        ) : base(game, eventDispatcher, statusType) {
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
                return;
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
            }
            else if (eventData.EventType == EventActionType.OnPlayerDeath)
            {
                //Exit the game for now
                this.Game.Exit();
            }
            else if(eventData.EventType == EventActionType.PlayerHealthPickup)
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

                //If the player has killed the enemy
                if (this.enemyOnFocus.Health <= 0) EventDispatcher.Publish(new EventData(EventActionType.OnEnemyDeath, EventCategoryType.Combat, new object[] { this.enemyOnFocus }));

                //Publish play attack sound event
                if (this.inventoryManager.HasItem(PickupType.Sword)) {
                    EventDispatcher.Publish(new EventData(EventActionType.OnPlay, EventCategoryType.Sound2D, new object[] { "h_attack" }));
                } else {
                    EventDispatcher.Publish(new EventData(EventActionType.OnPlay, EventCategoryType.Sound2D, new object[] { "punch_01" }));
                }

                //Publish player attack event
                EventDispatcher.Publish(new EventData(EventActionType.OnPlayerAttack, EventCategoryType.Combat));

                //Publish enemy turn event
                EventDispatcher.Publish(new EventData(EventActionType.EnemyTurn, EventCategoryType.Game));
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

                //Publish play block sound event
                EventDispatcher.Publish(new EventData(EventActionType.OnPlay, EventCategoryType.Sound2D, new object[] { "block" }));

                //Publish player defend event
                EventDispatcher.Publish(new EventData(EventActionType.OnPlayerDefend, EventCategoryType.Combat));

                //Publish enemy turn event
                EventDispatcher.Publish(new EventData(EventActionType.EnemyTurn, EventCategoryType.Game));
                return;
            }
            #endregion

            #region Block
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
                    //Publish play dodge sound event
                    EventDispatcher.Publish(new EventData(EventActionType.OnPlay, EventCategoryType.Sound2D, new object[] { "dodge" }));
                    
                    //Publish player dodge event
                    EventDispatcher.Publish(new EventData(EventActionType.OnPlayerDodge, EventCategoryType.Combat));

                    //Publish enemy turn event
                    EventDispatcher.Publish(new EventData(EventActionType.EnemyTurn, EventCategoryType.Game));
                    return;
                }
                else
                {
                    //Take damage
                    this.player.TakeDamage(enemyOnFocus.Attack);
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
                EventDispatcher.Publish(new EventData(EventActionType.OnPlay, EventCategoryType.Sound2D, new object[] { "l_attack" }));

                //Publish player attack event
                EventDispatcher.Publish(new EventData(EventActionType.OnEnemyAttack, EventCategoryType.Combat));

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

        //}
        //else
        //{
        //    PrintStats(enemyOnFocus);
        //    float enemyAttack = enemyOnFocus.Attack;
        //    EventDispatcher.Publish(
        //        new EventData(
        //            EventActionType.OnEnemyAttack,
        //            EventCategoryType.Combat
        //        )
        //    );



        //    //this.player.TakeDamage(enemyAttack);
        //    this.playerTurn = true;
        //}

        //if(this.player.Health <= 30)
        //{
        //    EventDispatcher.Publish(
        //        new EventData(
        //            EventActionType.OnPlay,
        //            EventCategoryType.Sound2D,
        //            new object[] { "player_health_low" }
        //        )
        //    );
        //}
        //else
        //{
        //    EventDispatcher.Publish(
        //        new EventData(
        //            EventActionType.OnPause,
        //            EventCategoryType.Sound2D,
        //            new object[] { "player_health_low" }
        //        )
        //    );
        //}

        //if(this.enemyOnFocus.Health <= 0)
        //{
        //    CombatManager.inCombat = false;
        //    Console.WriteLine("YOU HAVE WON!!!");

        //    EventDispatcher.Publish(
        //        new EventData(
        //            EventActionType.OnPause,
        //            EventCategoryType.Sound2D,
        //            new object[] { "battle_theme" }
        //        )
        //    );

        //    EventDispatcher.Publish(
        //        new EventData(
        //            EventActionType.OnPlay,
        //            EventCategoryType.Sound2D,
        //            new object[] { "death" }
        //        )
        //    );

        //    EventDispatcher.Publish(
        //        new EventData(
        //            EventActionType.OnEnemyDeath,
        //            EventCategoryType.EnemyDeath,
        //            new object[] { enemyOnFocus }
        //        )
        //    );

        //    EventDispatcher.Publish(
        //        new EventData(
        //            EventActionType.PlayerTurn,
        //            EventCategoryType.Game
        //        )
        //    );

        //    this.enemies.Remove(enemyOnFocus);
        //    this.playerTurn = true;
        //}

        //if(this.player.Health <= 0)
        //{
        //    Game.Exit();
        //}
    }
}