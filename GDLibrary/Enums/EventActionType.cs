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

        //Sent by Multiple Managers  - Menu, Sound, Object, UI, Physics & Video Controller
        OnStart,
        OnRestart,
        OnVolumeUp,
        OnVolumeDown,
        OnVolumeSet,
        OnVolumeChange,
        OnMute,
        OnUnMute,
        OnExit,

        //Sent by Mouse or Gamepad Manager
        OnClick,
        OnHover,

        //sent by game state manager
        OnLose,
        OnWin,

        //Sent by Camera Manager
        OnCameraSetActive,
        OnCameraCycle,
        OnCameraToggle,
    }
}
