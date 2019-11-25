using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using System.Collections.Generic;
using System;

namespace GDLibrary
{
    //See http://rbwhitaker.wikidot.com/audio-tutorials
    //See http://msdn.microsoft.com/en-us/library/ff827590.aspx
    //See http://msdn.microsoft.com/en-us/library/dd940200.aspx
    public class SoundManager : PausableGameComponent, IDisposable
    {
        #region Fields
        private static readonly float DefaultVolume = 0.5f;

        protected AudioEngine audioEngine;
        protected WaveBank waveBank;
        protected SoundBank soundBank;

        protected List<Cue> playingCues2D;
        protected List<Cue3D> playingCues3D;
        protected HashSet<string> playSet2D;
        protected HashSet<string> playSet3D;

        protected AudioListener audioListener;
        protected List<string> categories;
        private float volume;
        #endregion

        #region Properties
        public float Volume
        {
            get
            {
                return this.volume;
            }
            set
            {
                this.volume = (value >= 0 && value <= 1) ? value : DefaultVolume;
            }
        }
        #endregion

        #region Constructor
        public SoundManager(
            Game game,
            EventDispatcher eventDispatcher,
            StatusType statusType,
            string folderPath,
            string audioEngineStr,
            string waveBankStr,
            string soundBankStr
        ) : base(game, eventDispatcher, statusType) {
            this.audioEngine = new AudioEngine(@"" + folderPath + "/" + audioEngineStr);
            this.waveBank = new WaveBank(audioEngine, @"" + folderPath + "/" + waveBankStr);
            this.soundBank = new SoundBank(audioEngine, @"" + folderPath + "/" + soundBankStr);
            this.playingCues2D = new List<Cue>();
            this.playingCues3D = new List<Cue3D>();
            this.playSet2D = new HashSet<string>();
            this.playSet3D = new HashSet<string>();
            this.audioListener = new AudioListener();
        }
        #endregion

        #region Event Handling
        protected override void RegisterForEventHandling(EventDispatcher eventDispatcher)
        {
            eventDispatcher.GlobalSoundChanged += EventDispatcher_GlobalSoundChanged;
            eventDispatcher.Sound3DChanged += EventDispatcher_Sound3DChanged;
            eventDispatcher.Sound2DChanged += EventDispatcher_Sound2DChanged;

            base.RegisterForEventHandling(eventDispatcher);
        }

        //See https://www.youtube.com/watch?v=eG-FW6RAyHU
        //Construct event to pass in volume (e.g. 0.5f) and category (game_sound_effects) that you group the sounds by in XACT
        protected virtual void EventDispatcher_GlobalSoundChanged(EventData eventData)
        {
            //MuteEvent
            if (eventData.EventType == EventActionType.OnMute)
            {
                //2D sounds
                SoundEffect.MasterVolume = 0;

                //3D sounds
                float volume = (float) eventData.AdditionalParameters[0];
                string soundCategory = (string) eventData.AdditionalParameters[1];
                SetVolume(volume, soundCategory);
            }

            //UnMuteEvenet
            else if (eventData.EventType == EventActionType.OnUnMute)
            {
                //2D sounds
                SoundEffect.MasterVolume = DefaultVolume;
                
                //3D sounds
                float volume = (float)eventData.AdditionalParameters[0];
                string soundCategory = (string)eventData.AdditionalParameters[1];
                SetVolume(volume, soundCategory);

            }

            //ChangeVolumeEvent
            else if (eventData.EventType == EventActionType.OnVolumeChange)
            {
                //2D sounds
                float volumeDelta = (float)eventData.AdditionalParameters[0];
                SoundEffect.MasterVolume = MathHelper.Clamp(SoundEffect.MasterVolume + volumeDelta, 0, 1);
                
                //3D sounds
                string soundCategory = (string)eventData.AdditionalParameters[1];
                ChangeVolume(volumeDelta, soundCategory);
            }
        }

        //3D Sound Changed
        protected virtual void EventDispatcher_Sound3DChanged(EventData eventData)
        {
            //Control 3D sounds through events

            //If != OnStopAll Event
            if (eventData.EventType != EventActionType.OnStopAll)
            {
                //ID - Name
                string cueName = eventData.AdditionalParameters[0] as string;

                //OnPlay Event
                if (eventData.EventType == EventActionType.OnPlay)
                {
                    //Sender
                    AudioEmitter audioEmitter = eventData.AdditionalParameters[1] as AudioEmitter;
                    this.Play3DCue(cueName, audioEmitter);
                }

                //OnPause Event
                else if (eventData.EventType == EventActionType.OnPause)
                    this.Pause3DCue(cueName);

                //OnResume Event
                else if (eventData.EventType == EventActionType.OnResume)
                    this.Resume3DCue(cueName);

                //OnStop Event
                else if (eventData.EventType == EventActionType.OnStop)
                    this.Stop3DCue(cueName, AudioStopOptions.Immediate);
            }

            //OnStopAll Event
            else
            {
                //Since we can only pass refereneces in AdditionalParameters and AudioStopOption is an enum (i.e. a primitive type) then we need to hack the code a little.
                //Notice that the AdditionalParameters[0] parameter is now used to send the stop option (vs. above where it sent the cue name). be careful!

                if ((int) eventData.AdditionalParameters[0] == 0)
                    this.StopAll3DCues(AudioStopOptions.Immediate);
                else
                    this.StopAll3DCues(AudioStopOptions.AsAuthored);
            }
        }

        //2D Sound Changed
        protected virtual void EventDispatcher_Sound2DChanged(EventData eventData)
        {
            //ID - Name
            string cueName = eventData.AdditionalParameters[0] as string;

            //OnPlay Event
            if (eventData.EventType == EventActionType.OnPlay)
                this.PlayCue(cueName);

            //OnPause Event
            else if (eventData.EventType == EventActionType.OnPause)
                this.PauseCue(cueName);

            //OnResume Event
            else if (eventData.EventType == EventActionType.OnResume)
                this.ResumeCue(cueName);

            //OnStop Event
            else
            {
                //Since we can only pass refereneces in AdditionalParameters and AudioStopOption is an enum (i.e. a primitive type) then we need to hack the code a little

                //Sender
                if ((int) eventData.AdditionalParameters[1] == 0)
                    this.StopCue(cueName, AudioStopOptions.Immediate);
                else
                    this.StopCue(cueName, AudioStopOptions.AsAuthored);
            }
        }

        //Do we want sound to play in the menu? In this case, we should remove this code and set statusType to Update in the constructor.
        //protected override void EventDispatcher_MenuChanged(EventData eventData)
        //{
        //    //Did the event come from the main menu?
        //    //And is it a start game event?
        //    if (eventData.EventType == EventActionType.OnStart)
        //    {
        //        //Turn on update and draw
        //        //Hide the menu
        //        this.StatusType = StatusType.Update;
        //    }

        //    //Did the event come from the main menu? 
        //    //And is it a pause game event?
        //    else if (eventData.EventType == EventActionType.OnPause)
        //    {
        //        //Turn off update and draw
        //        //Show the menu since the game is paused
        //        this.StatusType = StatusType.Off;
        //    }
        //}
        #endregion

        #region 2D Cues
        ////Plays a 2D cue - Menu/Game Music etc.
        //public void PlayCue(string cueName)
        //{
        //    if (!playSet2D.Contains(cueName))
        //    {
        //        playSet2D.Add(cueName);
        //        Cue cue = this.soundBank.GetCue(cueName);
        //        playingCues2D.Add(cue);
        //        cue.Play();
        //    }
        //}

        ////Pauses a 2D cue
        //public void PauseCue(string cueName)
        //{
        //    //If we have not already been asked to play this in the current update loop then play it
        //    if (!playSet3D.Contains(cueName))
        //    {
        //        Cue cue = this.soundBank.GetCue(cueName);
        //        playingCues2D.Add(cue);
        //        playin
        //        cue.Play();
        //    }
        //}

        ////Resumes a paused 2D cue
        //public void ResumeCue(string cueName)
        //{
        //    if (!playSet2D.Contains(cueName))
        //    {
        //        playSet2D.Add(cueName);
        //        Cue cue = this.soundBank.GetCue(cueName);
        //        if ((cue != null) && (cue.IsPaused)) {
        //            playingCues2D.Add(cue);
        //            cue.Resume();
        //        }
        //    }
        //}

        ////Stops a 2D cue - AudioStopOptions: AsAuthored and Immediate
        //public void StopCue(string cueName, AudioStopOptions audioStopOptions)
        //{
        //    if (!playSet2D.Contains(cueName))
        //    {
        //        playSet2D.Remove(cueName);
        //        Cue cue = this.soundBank.GetCue(cueName);
        //        if ((cue != null) && (cue.IsPlaying)) {
        //            playingCues2D.Remove(cue);
        //            cue.Stop(audioStopOptions);
        //        }
        //    }
        //}
        //Plays a 2D cue - Menu/Game Music etc.
        public void PlayCue(string cueName)
        {
            //If we have not already been asked to play this in the current update loop then play it
            if (!playSet3D.Contains(cueName))
            {
                Cue cue = this.soundBank.GetCue(cueName);
                playingCues2D.Add(cue);
                cue.Play();
            }
        }

        //Pauses a 2D cue
        public void PauseCue(string cueName)
        {
            Cue cue = playingCues2D.Find(x => x.Name == cueName);

            if (playingCues2D.Contains(cue))
            {
                if (cue.IsPlaying)
                {
                    cue.Pause();
                    playingCues2D.Remove(cue);
                }
            }
            //Cue cue = this.soundBank.GetCue(cueName);
            //if ((cue != null) && (cue.IsPlaying))
            //    cue.Pause();
        }

        //Resumes a paused 2D cue
        public void ResumeCue(string cueName)
        {
            Cue cue = this.soundBank.GetCue(cueName);
            if ((cue != null) && (cue.IsPaused))
                cue.Resume();
        }

        //Stops a 2D cue - AudioStopOptions: AsAuthored and Immediate
        public void StopCue(string cueName, AudioStopOptions audioStopOptions)
        {
            Cue cue = this.soundBank.GetCue(cueName);
            if ((cue != null) && (cue.IsPlaying))
                cue.Stop(audioStopOptions);
        }
        #endregion

        #region 3D Cues
        //Plays a cue to be heard from the perspective of a player or camera in the game i.e. in 3D
        public void Play3DCue(string cueName, AudioEmitter audioEmitter)
        {
            Cue3D sound = new Cue3D
            {
                Cue = soundBank.GetCue(cueName)
            };

            if (!this.playSet3D.Contains(cueName)) //if we have not already been asked to play this in the current update loop then play it
            {
                sound.Emitter = audioEmitter;
                sound.Cue.Apply3D(audioListener, audioEmitter);
                sound.Cue.Play();
                this.playingCues3D.Add(sound);
                this.playSet3D.Add(cueName);
            }
        }

        //Pause a 3D cue
        public void Pause3DCue(string cueName)
        {
            Cue3D cue3D = Get3DCue(cueName);
            if ((cue3D != null) && (cue3D.Cue.IsPlaying))
                cue3D.Cue.Pause();
        }

        //Resumes a paused 3D cue
        public void Resume3DCue(string cueName)
        {
            Cue3D cue3D = Get3DCue(cueName);
            if ((cue3D != null) && (cue3D.Cue.IsPaused))
                cue3D.Cue.Resume();
        }

        //Stop a 3D cue - AudioStopOptions: AsAuthored and Immediate
        public void Stop3DCue(string cueName, AudioStopOptions audioStopOptions)
        {
            Cue3D cue3D = Get3DCue(cueName);
            if (cue3D != null)
            {
                cue3D.Cue.Stop(audioStopOptions);
                this.playSet3D.Remove(cue3D.Cue.Name);
                this.playingCues3D.Remove(cue3D);
            }
        }

        //Stops all 3D cues - AudioStopOptions: AsAuthored and Immediate
        public void StopAll3DCues(AudioStopOptions audioStopOptions)
        {
            foreach (Cue3D cue3D in this.playingCues3D)
            {
                cue3D.Cue.Stop(audioStopOptions);
                this.playingCues3D.Remove(cue3D);
                this.playSet3D.Remove(cue3D.Cue.Name);
            }
        }

        //Retrieves a 3D cue from the list of currently active cues
        public Cue3D Get3DCue(string name)
        {
            foreach (Cue3D cue3D in this.playingCues3D)
            {
                if (cue3D.Cue.Name.Equals(name))
                    return cue3D;
            }
            return null;
        }
        #endregion

        #region Volume
        //We can control the volume for each category in the sound bank (i.e. diegetic and non-diegetic)
        public void SetVolume(float newVolume, string soundCategoryStr)
        {
            try
            {
                AudioCategory soundCategory = this.audioEngine.GetCategory(soundCategoryStr);
                if (soundCategory != null)
                {
                    //Requested volume will be in appropriate range (0-1)
                    this.volume = MathHelper.Clamp(newVolume, 0, 1);
                    soundCategory.SetVolume(this.volume);
                }
            }
            catch (InvalidOperationException e)
            {
                System.Diagnostics.Debug.WriteLine("Does category (soundCategoryStr) exist in your Xact file?");
            }
        }

        public void ChangeVolume(float deltaVolume, string soundCategoryStr)
        {
            try
            {
                AudioCategory soundCategory = this.audioEngine.GetCategory(soundCategoryStr);
                if (soundCategory != null)
                {
                    //Requested volume will be in appropriate range (0-1)
                    this.volume = MathHelper.Clamp(this.volume + deltaVolume, 0, 1);
                    soundCategory.SetVolume(this.volume);
                }
            }
            catch (InvalidOperationException e)
            {
                System.Diagnostics.Debug.WriteLine("Does category (soundCategoryStr) exist in your Xact file?");
            }
        }
        #endregion

        #region Methods
        //Called by the listener to update relative positions (i.e. everytime the 1st Person camera moves it should call this method so that the 3D sounds heard reflect the new camera position)
        public void UpdateListenerPosition(Vector3 position)
        {
            this.audioListener.Position = position;
        }

        //Pause All Playing Sounds
        public void PauseAll()
        {
            foreach (Cue3D cue in playingCues3D) cue.Cue.Pause();
        }

        //Resume All Playing Sounds
        public void ResumeAll()
        {
            foreach (Cue3D cue in playingCues3D) cue.Cue.Resume();
        }

        protected override void HandleInput(GameTime gameTime)
        {

        }

        public override void Update(GameTime gameTime)
        {
            this.audioEngine.Update();
            for (int i = 0; i < playingCues3D.Count; i++)
            {
                if (this.playingCues3D[i].Cue.IsPlaying)
                    this.playingCues3D[i].Cue.Apply3D(audioListener, this.playingCues3D[i].Emitter);

                else if (this.playingCues3D[i].Cue.IsStopped)
                {
                    this.playSet3D.Remove(this.playingCues3D[i].Cue.Name);
                    this.playingCues3D.RemoveAt(i--);
                }
            }

            base.Update(gameTime);
        }

        protected override void Dispose(bool disposing)
        {
            this.audioEngine.Dispose();
            this.soundBank.Dispose();
            this.waveBank.Dispose();
            base.Dispose(disposing);
        }
        #endregion
    }
}