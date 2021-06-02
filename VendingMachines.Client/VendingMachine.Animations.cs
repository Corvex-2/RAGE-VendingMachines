using RAGE.Elements;
using System;
using System.Collections.Generic;
using System.Text;

using static RAGE.Events;
using RAGE.Game;

namespace VendingMachines.Client
{
    public partial class VendingMachine
    {
        internal void BeginAnimation()
        {
            Streaming.RequestAnimDict(ANIMATION_DICTIONARY_STRING);
            Audio.RequestAmbientAudioBank(AUDIO_BANK_VENDING_MACHINE, false, -1);

            while ((IsAnimationSetLoaded = Streaming.HasAnimDictLoaded(ANIMATION_DICTIONARY_STRING)) == false)
                Invoker.Wait(0);

            Player.TaskPlayAnim(ANIMATION_DICTIONARY_STRING, ANIMATION_BUY_DRINK_PART1, 2f, -4f, -1, 1048576, 0, false, false, false);
        }
        internal void UpdateAnimation()
        {
            if (!IsAnimationSetLoaded)
                return;

            if(Player.IsPlayingAnim(ANIMATION_DICTIONARY_STRING, ANIMATION_BUY_DRINK_PART1, 1))
            {
                if (Player.GetAnimCurrentTime(ANIMATION_DICTIONARY_STRING, ANIMATION_BUY_DRINK_PART1) > 0.1f)
                    AttachObject("ng_proc_sodacan_01a");

                if(Player.GetAnimCurrentTime(ANIMATION_DICTIONARY_STRING, ANIMATION_BUY_DRINK_PART1) > 0.95f)
                {
                    Player.TaskPlayAnim(ANIMATION_DICTIONARY_STRING, ANIMATION_BUY_DRINK_PART2, 4f, -1000f, -1, 1048576, 0f, false, false, false);
                    Invoker.Invoke(Natives._0x2208438012482A1A, Player.Handle, false, false);
                }
            }

            if(Player.IsPlayingAnim(ANIMATION_DICTIONARY_STRING, ANIMATION_BUY_DRINK_PART2, 1))
            {
                if (Player.GetAnimCurrentTime(ANIMATION_DICTIONARY_STRING, ANIMATION_BUY_DRINK_PART2) > 0.95f)
                {
                    Player.TaskPlayAnim(ANIMATION_DICTIONARY_STRING, ANIMATION_BUY_DRINK_PART3, 1000f, -4f, -1, 1048624, 0f, false, false, false);
                    Invoker.Invoke(Natives._0x2208438012482A1A, Player.Handle, false, false);
                }
            }

            if(Player.IsPlayingAnim(ANIMATION_DICTIONARY_STRING, ANIMATION_BUY_DRINK_PART3, 1))
            {
                if (Player.GetAnimCurrentTime(ANIMATION_DICTIONARY_STRING, ANIMATION_BUY_DRINK_PART3) > 0.334f)
                    DetachObject("ng_proc_sodacan_01a");

                if (Player.GetAnimCurrentTime(ANIMATION_DICTIONARY_STRING, ANIMATION_BUY_DRINK_PART3) > 0.95f)
                    EndAnimation();
            }
        }
        internal void EndAnimation()
        {
            Streaming.RemoveAnimDict(ANIMATION_DICTIONARY_STRING);
            Audio.ReleaseAmbientAudioBank();

            SetVendingMachineUseState(false);

            CallRemote("vendingMachines.Interaction.Finish");
        }

        private bool IsAnimationSetLoaded { get; set; }
    }
}
