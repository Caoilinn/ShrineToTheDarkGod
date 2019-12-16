namespace GDLibrary
{
    public enum EventActionType : sbyte
    {
        //Sent by Audio, Video Managers
        OnPlay,
        OnPause,
        OnResume,
        OnStop,
        OnStopAll,

        //Sent by Multiple Managers - Menu, Sound, Object, UI, Physics & Video Controller
        OnStart,
        OnRestart,
        OnVolumeUp,
        OnVolumeDown,
        OnVolumeSet,
        OnVolumeChange,
        OnMute,
        OnUnMute,
        OnExit,
        OnInventoryPickUp,


        //Sent by Mouse or Gamepad Manager
        OnClick,
        OnHover,

        //Sent by game state manager
        OnLose,
        OnWin,

        //
        //Sent by Camera Manager
        OnCameraSetActive,
        OnCameraCycle,
        OnCameraToggle,
        OnToggleDebug,
        OnRemoveActor,
        OnAddActor,
        OnOpaqueToTransparent,
        OnTransparentToOpaque,
        OnNonePicked,
        OnObjectPicked,

        //Sent by CombatManager
        OnInitiateBattle,
        OnBattleEnd,
        OnPlayerDeath,
        OnPlayerAttack,
        OnPlayerDefend,
        OnPlayerDodge,
        OnEnemyAttack,
        OnEnemyDeath,
        PlayerHealthUpdate,
        EnemyHealthUpdate,

        //Sent by Object Manager
        OnDoorOpen,

        //Sent By Inventory Manager
        OnItemAdded,
        OnItemRemoved,
        OnCollideWith,
        NewTurn,
        OnMove,
        EnemyTurn,
        PlayerTurn,
        OnAddToInventory,
        OnUpdateHud,
        OnWayBlocked,
        PlayerHealthPickup,
        OnDisplayInfo,
        OnHealthDelta,
        OnHealthSet,
        OnScreenLayoutChange,

        OnKeybind
    }
}