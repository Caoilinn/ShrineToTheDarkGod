using System.Collections.Generic;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework;

namespace GDLibrary
{
    public class GridManager : PausableGameComponent
    {
        #region Fields
        private HashSet<Actor3D> players;
        private HashSet<Actor3D> enemies;
        private HashSet<Actor3D> items;
        private HashSet<Actor3D> gates;
        private HashSet<Actor3D> triggers;

        private ObjectManager objectManager;
        private SoundManager soundManager;
        private InventoryManager inventoryManager;
        private CombatManager combatManager;

        //Calculate the distance beween the center points of two adjacent cells
        float distanceBetweenAdjacentCells = Vector3.Distance(new Vector3(0, 0, 0), new Vector3(254, 0, 0));

        //Used to cancel out the y component of a given vector
        Vector3 vectorXZ = new Vector3(1, 0, 1);
        #endregion

        #region Properties
        public ObjectManager ObjectManager
        {
            get {
                return this.objectManager;
            }
        }

        public SoundManager SoundManager
        {
            get {
                return this.soundManager;
            }
        }

        public InventoryManager InventoryManager
        {
            get {
                return this.inventoryManager;
            }
        }

        public CombatManager CombatManager
        {
            get {
                return this.combatManager;
            }
        }
        #endregion

        #region Constructor
        public GridManager(
            Game game,
            EventDispatcher eventDispatcher,
            StatusType statusType,
            HashSet<Actor3D> players,
            HashSet<Actor3D> enemies,
            HashSet<Actor3D> items,
            HashSet<Actor3D> gates,
            HashSet<Actor3D> triggers,
            ObjectManager objectManager,
            SoundManager soundManager,
            InventoryManager inventoryManager,
            CombatManager combatManager
        ) : base(game, eventDispatcher, statusType)
        {
            this.players = players;
            this.enemies = enemies;
            this.items = items;
            this.gates = gates;
            this.triggers = triggers;

            this.objectManager = objectManager;
            this.soundManager = soundManager;
            this.inventoryManager = inventoryManager;
            this.combatManager = combatManager;
        }
        #endregion

        #region Event Handling
        protected override void RegisterForEventHandling(EventDispatcher eventDispatcher)
        {
            eventDispatcher.GameChanged += EventDispatcher_GameChanged;
            eventDispatcher.RemoveActorChanged += EventDispatcher_RemoveActorChanged;
        }

        private void EventDispatcher_RemoveActorChanged(EventData eventData)
        {
            if (eventData.EventType.Equals(EventActionType.OnRemoveActor) && eventData.AdditionalParameters[0] is Actor3D)
                this.Remove(eventData.AdditionalParameters[0] as Actor3D);
        }

        private void EventDispatcher_GameChanged(EventData eventData)
        {
            //Is it the players' turn?
            if (eventData.EventType == EventActionType.PlayerTurn)
            {
                DetectPlayerInteraction();

                foreach (PlayerObject player in this.players)
                {
                    player.ResetCollision();
                }
            }

            //Is it the enemys' turn?
            else if (eventData.EventType == EventActionType.EnemyTurn)
            {
                DetectPlayerInteraction();

                //If there are no enemies remaining
                if (this.enemies.Count <= 0 || !StateManager.InCombat)
                {
                    //Publish a player turn event
                    EventDispatcher.Publish(new EventData(EventActionType.PlayerTurn, EventCategoryType.Game));
                }

                foreach (AnimatedEnemyObject enemy in this.enemies)
                {
                    enemy.ResetCollision();
                }
            }
        }
        #endregion

        #region Add Methods
        public void Add(Actor3D actor)
        {
            switch (actor.ActorType)
            {
                case ActorType.Player:
                    AddPlayer(actor);
                    break;
                case ActorType.Enemy:
                    AddEnemy(actor);
                    break;
                case ActorType.CollidablePickup:
                    AddItem(actor);
                    break;
                case ActorType.Gate:
                    AddGate(actor);
                    break;
                case ActorType.Trigger:
                    AddTrigger(actor);
                    break;
            }
        }

        private void AddPlayer(Actor3D actor)
        {
            if (!this.players.Contains(actor))
                this.players.Add(actor);
        }

        private void AddEnemy(Actor3D actor)
        {
            if (!this.enemies.Contains(actor))
                this.enemies.Add(actor);
        }

        private void AddItem(Actor3D actor)
        {
            if (!this.items.Contains(actor))
                this.items.Add(actor);
        }

        private void AddGate(Actor3D actor)
        {
            if (!this.gates.Contains(actor))
                this.gates.Add(actor);
        }

        private void AddTrigger(Actor3D actor)
        {
            if (!this.triggers.Contains(actor))
                this.triggers.Add(actor);
        }
        #endregion

        #region Remove Methods
        public void Remove(Actor3D actor)
        {
            switch (actor.ActorType)
            {
                case ActorType.Player:
                    RemovePlayer(actor);
                    break;
                case ActorType.Enemy:
                    RemoveEnemy(actor);
                    break;
                case ActorType.CollidablePickup:
                    RemoveItem(actor);
                    break;
                case ActorType.Gate:
                    RemoveGate(actor);
                    break;
                case ActorType.Trigger:
                    RemoveTrigger(actor);
                    break;
            }
        }

        private void RemovePlayer(Actor3D actor)
        {
            if (this.players.Contains(actor))
                this.players.Remove(actor);
        }

        private void RemoveEnemy(Actor3D actor)
        {
            if (this.enemies.Contains(actor))
                this.enemies.Remove(actor);
        }

        private void RemoveItem(Actor3D actor)
        {
            if (this.items.Contains(actor))
                this.items.Remove(actor);
        }

        private void RemoveGate(Actor3D actor)
        {
            if (this.gates.Contains(actor))
                this.gates.Remove(actor);
        }

        private void RemoveTrigger(Actor3D actor)
        {
            if (this.triggers.Contains(actor))
                this.triggers.Remove(actor);
        }
        #endregion

        #region Detect Interaction
        public void DetectPlayerInteraction()
        {
            foreach (Actor3D player in this.players)
            {
                DetectInteractionsWithEnemies(player);
                DetectInteractionsWithItems(player);
                DetectInteractionsWithGates(player);
                DetectInteractionsWithTriggers(player);
                UpdateSound();
            }
        }
        #endregion

        #region Enemy Interaction
        public void DetectInteractionsWithEnemies(Actor3D player)
        {
            //Local var for checking if the player is in battle
            bool inCombat = false;

            //For each enemy in the enemies list
            foreach (Actor3D enemy in this.enemies)
            {
                if (enemy != null)
                {
                    //Create item audio emitter
                    AudioEmitter enemyAudioEmitter = new AudioEmitter();
                    enemyAudioEmitter.Position = enemy.Transform.Translation;

                    //If the enemys' position is equal to the players' position
                    if (enemy.Transform.Translation.Equals(player.Transform.Translation))
                    {
                        //Then the player is colliding with an enemy
                        HandlePlayerEnemyCollision(enemy as EnemyObject);
                        inCombat = true;
                    }

                    //If the distance between the enemys' position and the players' position, is less than or equal to the distance between two adjacent cells
                    if (Vector3.Distance(enemy.Transform.Translation * vectorXZ, player.Transform.Translation * vectorXZ) <= distanceBetweenAdjacentCells)
                    {
                        //Then the player is standing in a cell that is adjacent to an enemy
                        HandlePlayerEnemyInteraction(enemy as EnemyObject);
                        inCombat = true;
                    }

                    //If the distance between the enemys' position and the players' position, is less than or equal to the distance between two adjacent cells
                    if (Vector3.Distance(enemy.Transform.Translation * vectorXZ, player.Transform.Translation * vectorXZ) == (distanceBetweenAdjacentCells * 2))
                    {
                        //Publish sound event
                        if (enemy.GetID().Equals("Cultist1"))
                            EventDispatcher.Publish(new EventData(EventActionType.OnPlay, EventCategoryType.Sound3D, new object[] { "cultist_growl", enemyAudioEmitter }));
                        else
                            EventDispatcher.Publish(new EventData(EventActionType.OnPlay, EventCategoryType.Sound3D, new object[] { "skeleton_growl", enemyAudioEmitter }));
                    }

                    //If the player is looking towards the enemy
                    if (Vector3.Normalize(player.Transform.Look) == (Vector3.Normalize(player.Transform.Translation - enemy.Transform.Translation) * -1))
                    {
                        //Publish a ui event, to display the currently faced enemies health bar
                    }
                }
            }

            //Update current game state
            StateManager.InCombat = inCombat;
        }

        private void HandlePlayerEnemyCollision(EnemyObject enemy)
        {
            //This should never happen
            //Prevent this state somewhere else
        }

        private void HandlePlayerEnemyInteraction(EnemyObject enemy)
        {
            //Don't need to start combat twice
            if (StateManager.InCombat) return;

            //If the enemy has died, return
            if (enemy.Health <= 0) return;

            //Initialise battle
            this.CombatManager.InitiateBattle(enemy);

            //For each player in the game
            foreach (PlayerObject player in this.players)
            {

                //Block their path
                player.BlockedDirections.Add(Vector3.Normalize(enemy.Transform.Translation - player.Transform.Translation));
            }

            //Update HUD
            if (!StateManager.Dodged)
            {
                EventDispatcher.Publish(new EventData(EventActionType.OnInitiateBattle, EventCategoryType.Textbox));

                //Pause game music
                //this.SoundManager.PauseCue("music_main");

                //Play battle music
                //this.SoundManager.PlayCue("music_battle_001");
            }
        }
        #endregion

        #region Item Interaction
        public void DetectInteractionsWithItems(Actor3D player)
        {
            //Local var for checking if the player is standing beside an item
            bool inProximityOfAnItem = false;

            //For each item in the items list
            foreach (Actor3D item in this.items)
            {
                //Create item audio emitter
                AudioEmitter itemAudioEmitter = new AudioEmitter
                {
                    Position = item.Transform.Translation
                };

                //If the items xz position is equal to the players xz position
                if ((item.Transform.Translation * vectorXZ).Equals(player.Transform.Translation * vectorXZ))
                {
                    //Then the player is colliding with an item
                    HandlePlayerItemCollision(item as PickupObject, itemAudioEmitter);
                    inProximityOfAnItem = true;
                    break;
                }

                //If the distance between the items xz position and the players xz position, is less than or equal to the distance between two adjacent cells
                if (Vector3.Distance(item.Transform.Translation * vectorXZ, player.Transform.Translation * vectorXZ) <= distanceBetweenAdjacentCells)
                {
                    //Then the player is standing in a cell that is adjacent to an item
                    HandlePlayerItemIneraction(item as PickupObject, itemAudioEmitter);
                    inProximityOfAnItem = true;
                    break;
                }
            }

            //Update current game state
            StateManager.InProximityOfAnItem = inProximityOfAnItem;
        }

        private void HandlePlayerItemCollision(PickupObject item, AudioEmitter itemAudioEmitter)
        {
            //Select which sound to play, based on the pickup type
            switch (item.PickupParameters.PickupType)
            {
                //Play pickup sword sound
                case PickupType.Sword:
                    EventDispatcher.Publish(new EventData(EventActionType.OnItemAdded, EventCategoryType.Textbox, new object[] { "Sword" }));
                    EventDispatcher.Publish(new EventData(EventActionType.OnItemAdded, EventCategoryType.UIMenu, new object[] { "Sword" }));
                    EventDispatcher.Publish(new EventData(EventActionType.OnPlay, EventCategoryType.Sound3D, new object[] { "pickup_sword", itemAudioEmitter }));
                    break;

                //Play pickup key sound
                case PickupType.Key:
                    EventDispatcher.Publish(new EventData(EventActionType.OnItemAdded, EventCategoryType.Textbox, new object[] { "Key" }));
                    EventDispatcher.Publish(new EventData(EventActionType.OnItemAdded, EventCategoryType.UIMenu, new object[] { "Key" }));
                    EventDispatcher.Publish(new EventData(EventActionType.OnPlay, EventCategoryType.Sound3D, new object[] { "pickup_keys", itemAudioEmitter }));
                    break;

                //Play pickup potion sound
                case PickupType.Health:
                    EventDispatcher.Publish(new EventData(EventActionType.OnItemAdded, EventCategoryType.Textbox, new object[] { "Potion" }));
                    EventDispatcher.Publish(new EventData(EventActionType.OnItemAdded, EventCategoryType.UIMenu, new object[] { "Potion" }));
                    EventDispatcher.Publish(new EventData(EventActionType.OnPlay, EventCategoryType.Sound3D, new object[] { "pickup_potion", itemAudioEmitter }));
                    break;
            }

            //Add item to the inventory
            this.InventoryManager.AddItem(item);

            //Remove the item
            this.ObjectManager.Remove(item);
            this.Remove(item);
        }

        private void HandlePlayerItemIneraction(PickupObject item, AudioEmitter itemAudioEmitter)
        {
            //Play sound
            EventDispatcher.Publish(new EventData(EventActionType.OnPlay, EventCategoryType.Sound3D, new object[] { "pickup_twinkle", itemAudioEmitter }));
        }
        #endregion

        #region Gate Interaction
        public void DetectInteractionsWithGates(Actor3D player)
        {
            //Local var for checking if the player is standing beside a gate
            bool inProximityOfAGate = false;

            //For each gate in the gates list
            foreach (Actor3D gate in this.gates)
            {
                //Create item audio emitter
                AudioEmitter gateAudioEmitter = new AudioEmitter();
                gateAudioEmitter.Position = gate.Transform.Translation;

                //If the gates' position is equal to the player's position
                if (gate.Transform.Translation.Equals(player.Transform.Translation))
                {
                    //Then the player is colliding with a gate
                    HandlePlayerGateCollision(gate as CollidableArchitecture, gateAudioEmitter);
                    inProximityOfAGate = true;
                    break;
                }

                //If the distance between the gates' position and the players' position, is less than or equal to the distance between two adjacent cells
                if (Vector3.Distance(gate.Transform.Translation * vectorXZ, player.Transform.Translation * vectorXZ) <= distanceBetweenAdjacentCells)
                {
                    //Then the player is standing in a cell that is adjacent to it
                    HandlePlayerGateInteraction(gate as CollidableArchitecture, gateAudioEmitter);
                    inProximityOfAGate = true;
                    break;
                }
            }

            //Update current game state
            StateManager.InProximityOfAGate = inProximityOfAGate;
        }

        private void HandlePlayerGateCollision(CollidableArchitecture gate, AudioEmitter gateAudioEmitter)
        {
            //This should never happen
            //Prevent this elsewhere
        }

        private void HandlePlayerGateInteraction(CollidableArchitecture gate, AudioEmitter gateAudioEmitter)
        {
            //If the player is holding a key
            if (this.InventoryManager.HasItem(PickupType.Key))
            {
                //Use a key to open the gate
                this.InventoryManager.UseItem(PickupType.Key);

                //Update UI
                EventDispatcher.Publish(new EventData(EventActionType.OnItemRemoved, EventCategoryType.Textbox, new object[] { "Key" }));
                EventDispatcher.Publish(new EventData(EventActionType.OnItemRemoved, EventCategoryType.UIMenu, new object[] { "Key" }));

                //Play a sound
                EventDispatcher.Publish(new EventData(EventActionType.OnPlay, EventCategoryType.Sound3D, new object[] { "gate_open", gateAudioEmitter }));

                //Remove the gate
                this.Remove(gate);
                this.ObjectManager.Remove(gate);

                //Remove gate collision skin
                gate.Remove();
            }
            else
            {
                //Display Info
                EventDispatcher.Publish(new EventData(EventActionType.OnDisplayInfo, EventCategoryType.Textbox, new object[] { "I will need a key to open this gate" }));
            }
        }
        #endregion

        #region Trigger Interaction
        public void DetectInteractionsWithTriggers(Actor3D player)
        {
            //Local var for checking if the player is standing beside a gate
            bool inProximityOfATrigger = false;

            //For each trigger in the triggers list
            foreach (Actor3D trigger in this.triggers)
            {
                //If the triggers' xz position is equal to the player's position
                if ((trigger.Transform.Translation * vectorXZ).Equals(player.Transform.Translation * vectorXZ))
                {
                    //Then the player is colliding with the trigger
                    HandlePlayerTriggerCollision(trigger as CollidableArchitecture);
                    inProximityOfATrigger = true;
                    break;
                }
            }

            //Update current game state
            StateManager.InProximityOfATrigger = inProximityOfATrigger;
        }

        private void HandlePlayerTriggerCollision(CollidableArchitecture gate)
        {
            //Quit to menu for now
            EventDispatcher.Publish(new EventData(EventActionType.OnStart, EventCategoryType.Menu, new object[] { "win_scene" }));
            EventDispatcher.Publish(new EventData(EventActionType.OnPause, EventCategoryType.Menu));
        }
        #endregion

        #region Base Methods
        private void UpdateSound()
        {
            if (!StateManager.InProximityOfAnItem) EventDispatcher.Publish(new EventData(EventActionType.OnStop, EventCategoryType.Sound3D, new object[] { "pickup_twinkle", 0 }));

            if (!StateManager.InCombat)
            {
            }
        }

        public override void Update(GameTime gameTime)
        {
            UpdateSound();
            base.Update(gameTime);
        }
        #endregion
    }
}