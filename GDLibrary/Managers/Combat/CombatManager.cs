using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace GDLibrary
{
    public class CombatManager : PausableGameComponent
    {
        #region Fields
        public static bool inCombat;
        private Keys[] combatKeys;
        private List<CharacterObject> characters;
        private PlayerObject player;
        private List<Enemy> enemies;
        private bool playerTurn;
        private ManagerParameters managerParameters;
        private Random random = new Random();
        private Enemy enemyOnFocus;
        #endregion

        #region Properties
        public List<Enemy> Enemies
        {
            get {
                return this.enemies;
            }
        }

        public Enemy EnemyOnFocus
        {
            get {
                return this.enemyOnFocus;
            }
        }

        #endregion


        #region Constructor
        public CombatManager(
            Game game,
            EventDispatcher eventDispatcher,
            StatusType statusType,
            ManagerParameters managerParameters,
            Keys[] combatKeys
        ) : base(game, eventDispatcher, statusType)
        {
            this.enemies = new List<Enemy>();
            this.managerParameters = managerParameters;
            this.playerTurn = true;
            CombatManager.inCombat = false;
            this.combatKeys = combatKeys;
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

            #region Old If Statements
            if (eventData.EventType == EventActionType.OnInitiateBattle)
            {

                if (this.managerParameters.InventoryManager.HasSword())
                    this.player.AddWeaponDamage();
                    
                Console.WriteLine("UPDATED PLAYER DAMAGE: " + this.player.Attack);

                //Combat started
                CombatManager.inCombat = true;
                this.enemyOnFocus = eventData.AdditionalParameters[0] as Enemy;
            } else if (eventData.EventType == EventActionType.OnPlayerAttack)
            {
                Console.WriteLine("Player Attack Event");

                float damage = (float)eventData.AdditionalParameters[0];

                EventDispatcher.Publish(new EventData(
                                            EventActionType.OnPlayerAttack,
                                            EventCategoryType.UI,
                                            new object[] { damage })
                                            );


            } else if (eventData.EventType == EventActionType.OnPlayerDefend)
            {
                Console.WriteLine("Player Defend Event");
                //Object[] additionalParameters = {"playerDefend"};
                //EventDispatcher.Publish(new EventData(EventActionType.OnPlayerDefend, EventCategoryType.Sound2D, additionalParameters));

                float damage = (float)eventData.AdditionalParameters[0];


                EventDispatcher.Publish(new EventData(
                                            EventActionType.OnPlayerDefend,
                                            EventCategoryType.UI,
                                            new object[] { damage })
                    );


            } else if (eventData.EventType == EventActionType.OnPlayerDodge)
            {

                Console.WriteLine("Player Defend Event");
                float damage = (float)eventData.AdditionalParameters[0];
                if (damage <= 0)
                    Console.WriteLine("Dodge Success");

                EventDispatcher.Publish(new EventData(
                                            EventActionType.OnPlayerDefend,
                                            EventCategoryType.UI,
                                            new object[] { damage })
                    );

            } else if (eventData.EventType == EventActionType.OnEnemyAttack)
            {
                Console.WriteLine("Enemy Attack Event");

                float enemyAttack = (float)eventData.AdditionalParameters[0];

                EventDispatcher.Publish(
                           new EventData(
                               EventActionType.OnEnemyAttack,
                               EventCategoryType.UI,
                               new object[] { enemyAttack })
                           );

            } else if (eventData.EventType == EventActionType.OnBattleEnd)
            {
                //Combat ended
                CombatManager.inCombat = false;
            } else if (eventData.EventType == EventActionType.OnPlayerDeath)
            {
                //Exit the game for now
                this.Game.Exit();
            }
            else if(eventData.EventType == EventActionType.PlayerHealthPickup)
            {
                Console.WriteLine("Player Health before: " + this.player.Health);
                this.player.Health += 10;
                Console.WriteLine("Player Health after: " + this.player.Health);    
            }
        }
        #endregion

        #region Methods
        public void AddPlayer(PlayerObject player)
        {
            this.player = player;
        }

        public void PopulateEnemies(Enemy enemy)
        {
            this.enemies.Add(enemy);
        }

        public bool Remove(Predicate<Enemy> predicate)
        {
            Enemy enemy = this.enemies.Find(predicate);
            if (enemy != null)
                return this.enemies.Remove(enemy);


            return false;
        }

        public int RemoveAll(Predicate<Enemy> predicate)
        {
            return this.enemies.RemoveAll(predicate);
        }

        public Enemy GetEnemy(string id)
        {
            if (enemies != null)
            {
                //Finds enemy where ID is equal to the passed ID
                return this.enemies.Find(x => x.ID == id);

            return null;
        }

        public override void Update(GameTime gameTime)
        {
            //Publishing event to the UI manager so that player health is always up to date and the enemy
            //health is updated during combat
            EventDispatcher.Publish(new EventData(
                                        EventActionType.PlayerHealthUpdate,
                                        EventCategoryType.UI,
                                        new object[] { this.player.Health }));

            do
            {
                EventDispatcher.Publish(new EventData(
                                            EventActionType.EnemyHealthUpdate,
                                            EventCategoryType.UI,
                                            new object[] { this.enemyOnFocus.Health }));
            } while (CombatManager.inCombat);


            HandleKeyBoardInput(gameTime);
            base.Update(gameTime);
        }

        public void PrintStats(Enemy enemy)
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

        protected virtual void HandleKeyBoardInput(GameTime gameTime)
        {
            if (CombatManager.inCombat)
            {
                if (playerTurn)
                {
                    if (this.managerParameters.KeyboardManager.IsFirstKeyPress(this.combatKeys[0]))
                    {
                        PrintStats(enemyOnFocus);
                        float playerAttack = this.player.Attack;
                        float enemyDefence = enemyOnFocus.Defence;
                        float damage = playerAttack - enemyDefence;

                        EventDispatcher.Publish(
                            new EventData(
                                EventActionType.OnPlayerAttack,
                                EventCategoryType.Combat,
                                new object[] { damage }
                            )
                        );

                        if (damage > 0)
                        {
                            this.enemyOnFocus.TakeDamage(damage);
                        }

                        this.playerTurn = false;
                    } else if (this.managerParameters.KeyboardManager.IsFirstKeyPress(this.combatKeys[1]))
                    {
                        PrintStats(enemyOnFocus);
                        float playerDefence = this.player.Defence;
                        float enemyAttack = enemyOnFocus.Attack;
                        float damage = enemyAttack - playerDefence;

                        EventDispatcher.Publish(
                            new EventData(
                                EventActionType.OnPlayerDefend,
                                EventCategoryType.Combat,
                                new object[] { damage }
                            )
                        );

                        if (damage > 0)
                        {
                            this.player.TakeDamage(damage);
                        }

                        playerTurn = false;
                    } else if (this.managerParameters.KeyboardManager.IsFirstKeyPress(this.combatKeys[2]))
                    {
                        PrintStats(enemyOnFocus);
                        int dodge = random.Next(1, 7);
                        float damage = enemyOnFocus.Attack;

                        if (dodge % 2 == 0)
                        {
                            damage = 0f;

                            EventDispatcher.Publish(
                            new EventData(
                                EventActionType.OnPlayerDodge,
                                EventCategoryType.Combat,
                                new object[] { damage })
                            );

                            EventDispatcher.Publish(
                                new EventData(
                                    EventActionType.OnPlay,
                                    EventCategoryType.Sound2D,
                                    new object[] { "dodge" }
                                )
                            );

                            return;
                        } else
                        {
                            player.TakeDamage(damage);

                            EventDispatcher.Publish(
                            new EventData(
                                EventActionType.OnPlayerDodge,
                                EventCategoryType.Combat,
                                new object[] { damage })
                            );

                        }
                    }
                } else
                {
                    PrintStats(enemyOnFocus);

                    float enemyAttack = enemyOnFocus.Attack;

                    EventDispatcher.Publish(
                        new EventData(
                            EventActionType.OnEnemyAttack,
                            EventCategoryType.Combat,
                            new object[] { enemyAttack }
                        )
                    );


                    this.player.TakeDamage(enemyAttack);
                    this.playerTurn = true;
                }

                if(this.player.Health <= 30)
                {
                    EventDispatcher.Publish(
                        new EventData(
                            EventActionType.OnPlay,
                            EventCategoryType.Sound2D,
                            new object[] { "player_health_low" }
                        )
                    );
                }
                else
                {
                    EventDispatcher.Publish(
                        new EventData(
                            EventActionType.OnPause,
                            EventCategoryType.Sound2D,
                            new object[] { "player_health_low" }
                        )
                    );
                }

                if(this.enemyOnFocus.Health <= 0)
                {
                    CombatManager.inCombat = false;
                    Console.WriteLine("YOU HAVE WON!!!");

                    EventDispatcher.Publish(
                        new EventData(
                            EventActionType.OnPause,
                            EventCategoryType.Sound2D,
                            new object[] { "battle_theme" }
                        )
                    );

                    EventDispatcher.Publish(
                        new EventData(
                            EventActionType.OnPlay,
                            EventCategoryType.Sound2D,
                            new object[] { "death" }
                        )
                    );

                    EventDispatcher.Publish(
                        new EventData(
                            EventActionType.OnEnemyDeath,
                            EventCategoryType.EnemyDeath,
                            new object[] { enemyOnFocus }
                        )
                    );

                    EventDispatcher.Publish(
                        new EventData(
                            EventActionType.PlayerTurn,
                            EventCategoryType.Game
                        )
                    );

                    EventDispatcher.Publish(new EventData(
                        EventActionType.OnEnemyDeath,
                        EventCategoryType.UI));

                    this.enemies.Remove(enemyOnFocus);
                    this.playerTurn = true;
                }

                if(this.player.Health <= 0)
                {
                    Game.Exit();
                }
            }
        }
        #endregion
    }
}