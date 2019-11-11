using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace GDLibrary
{
    public class CombatManager : PausableGameComponent
    {
        #region Fields
        private Keys[] moveKeys;
        private List<CharacterObject> characters;
        private PlayerObject player;
        private List<Enemy> enemies;
        private bool playerTurn;
        private ManagerParameters managerParameters;
        private Random random = new Random();
        #endregion

        public CombatManager(Game game, EventDispatcher eventDispatcher, StatusType statusType, 
            PlayerObject player, ManagerParameters managerParameters, Keys[] moveKeys) : 
            base(game, eventDispatcher, statusType)
        {
            this.enemies = new List<Enemy>();
            this.player = player;
            this.managerParameters = managerParameters;
            this.playerTurn = true;
            this.moveKeys = moveKeys;
        }

        public void Add(Enemy enemy)
        {
            this.enemies.Add(enemy);
        }

        public bool Remove(Predicate<Enemy> predicate)
        {
            Enemy enemy= this.enemies.Find(predicate);
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
            if(enemies != null)
            {
                //finds enemy where ID is equal to the passed ID
                return this.enemies.Find(x=> x.ID == id);
            }
            return null;
        }

        public override void Update(GameTime gameTime)
        {
            HandleKeyBoardInput(gameTime);
            base.Update(gameTime);
        }

        protected virtual void HandleKeyBoardInput(GameTime gameTime)
        {
            /** Old Combat Code - TOFIX or Remove
            if (playerTurn)
            {
                Enemy enemy = enemies[0];
                if (this.managerParameters.KeyboardManager.IsFirstKeyPress(this.moveKeys[8]))
                {
                    

                    float playerAttack = this.player.Attack;
                    float enemyDefence = enemy.Defence;

                    float damage = playerAttack - enemyDefence;

                    if(damage > 0)
                    {
                        enemy.takeDamage(damage);
                    }


                } else if(this.managerParameters.KeyboardManager.IsFirstKeyPress(this.moveKeys[9]))
                {
                    float playerDefence = this.player.Defence;
                    float enemyAttack = enemy.Attack;

                    float damage = enemyAttack - playerDefence;
                    
                    if(damage > 0)
                    {
                        this.player.takeDamage(damage);
                    }


                } else if(this.managerParameters.KeyboardManager.IsFirstKeyPress(this.moveKeys[10]))
                {
                    int dodge = random.Next(1, 7);

                    if(dodge % 2 == 0)
                    {
                        return;
                    } else
                    {
                        player.takeDamage(enemy.Attack);
                    }
                }
            }
            **/

            //Enemy enemyOnFocus = GetEnemy();


            if(playerTurn)
            {
                Console.WriteLine("Player: \n" + "Health: " + this.player.Health + "\n"
                                    + " Attack: " + this.player.Attack + "\n" 
                                    + "Defence: " + this.player.Defence);
            }
            else{
                Console.WriteLine("Player: \n" + "Health: " + this.player.Health + "\n"
                                    + " Attack: " + this.player.Attack + "\n"
                                    + "Defence: " + this.player.Defence);
            }

        }
    }
}
