using System.Collections.Generic;
using System.Linq;

using RAGE;
using RAGE.Elements;
using RAGE.Game;

using Entity = RAGE.Game.Entity;
using Player = RAGE.Elements.Player;
using Object = RAGE.Game.Object;

using static RAGE.Events;


namespace VendingMachines.Client
{
    public partial class VendingMachine : Events.Script
    {
        internal Player Player { get; } = Player.LocalPlayer;
        internal readonly uint[] VendingMachineHashes = 
        {
            Misc.GetHashKey("prop_vend_coffe_01"),
            Misc.GetHashKey("prop_vend_snak_01"),
            Misc.GetHashKey("prop_vend_snak_01_tu"),
            Misc.GetHashKey("prop_vend_soda_01"),
            Misc.GetHashKey("prop_vend_soda_02"),
        };

        public static bool IsUsingVendingMachine { get; internal set; }

        public bool IsNearVendingMachine
        {
            get
            {
                return GetNearestVendingMachineToPlayer() != -1;
            }
        }

        public VendingMachine()
        {
            Tick += OnUpdate;
        }

        internal void OnUpdate(List<TickNametagData> Nametags)
        {
            if (!IsUsingVendingMachine)
            {
                if (IsNearVendingMachine)
                    VendingMachineUI.Show();
                InteractionCheck();
            }

            UpdateAnimation();
        }

        internal Vector3 GetOffsetWorldCoordinatesForVendingMachine(int Handle)
        {
            if (Handle == -1)
                return null;

            return Entity.GetOffsetFromEntityInWorldCoords(Handle, 0, -1f, 0.1f);
        }

        public int GetNearestVendingMachineToPlayer()
        {
            for(int i = 0; i < VendingMachineHashes.Length; i++)
            {
                var handle = Object.GetClosestObjectOfType(Player.Position.X, Player.Position.Y, Player.Position.Z, 1f, VendingMachineHashes[i], false, false, false);

                if (Entity.IsAnEntity(handle))
                    return handle;
            }
            return -1;
        }


        internal void TriggerVendingMachine()
        {
            var handle = GetNearestVendingMachineToPlayer();
            var coordinates = GetOffsetWorldCoordinatesForVendingMachine(handle);

            if (IsUsingVendingMachine || handle == -1 || coordinates == null || IsVendingMachineInUse(handle))
            {
                IsUsingVendingMachine = false;
                return;
            }

            Player.SetData("IsUsingVendingMachine", (handle, true));
            Player.SetCurrentWeaponVisible(false, true, true, true);
            Player.SetStealthMovement(false, "DEFAULT_ACTION");
            Player.TaskLookAtEntity(handle, 2000, 2048, 3);
            Player.SetResetFlag(322, true);
            Player.TaskGoStraightToCoord(coordinates.X, coordinates.Y, coordinates.Z, 1f, 20000, Entity.GetEntityHeading(handle), 0.1f);

            while (Ai.GetScriptTaskStatus(Player.Handle, 2106541073) != 7 && !Player.IsAtCoord(coordinates.X, coordinates.Y, coordinates.Z, 0.1f, 0f, 0f, false, true, 0))
                Invoker.Wait(0);

            BeginAnimation();
        }

        internal void InteractionCheck()
        {
            if (Pad.IsControlJustPressed(INPUT_GROUP_ALL, (int)Control.Context) && IsNearVendingMachine && !IsUsingVendingMachine && !IsVendingMachineInUse(GetNearestVendingMachineToPlayer()))
            {
                SetVendingMachineUseState(true);
            }

            if (Pad.IsControlJustPressed(INPUT_GROUP_ALL, (int)Control.FrontendCancel) && IsUsingVendingMachine)
            {
                SetVendingMachineUseState(false);
            }
        }

        internal void SetVendingMachineUseState(bool State)
        {
            IsUsingVendingMachine = State;

            if (State)
                TriggerVendingMachine();
            else
            {
                Player.ClearTasksImmediately();
                Player.SetData("IsUsingVendingMachine", (-1, false));
            }
        }

        internal bool IsVendingMachineInUse(int Handle)
        {
            return Entities.Players.Streamed.Any(p => p.GetData<(int handle, bool state)>("IsUsingVendingMachine").handle == Handle);
        }
    }
}
