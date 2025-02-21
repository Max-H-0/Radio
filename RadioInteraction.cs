using HarmonyLib;
using Ship.Parts.Common;
using Ship.Network.Service;
using Ship.Interface.Model.Parts;
using Ship;
using Ship.Interface.Model.Parts.State;
using MelonLoader;
using Managers.Interface.Model;

namespace Radio;

public class RadioInteraction : PartInteraction
{
    public override string InteractionLabel
    {
        get
        {
            return GetEnabledState() ? "Disable" : "Enable";
        }
    }

    public override void Interact()
    {
        MelonLogger.Msg("F");

        SetEnabledState(!GetEnabledState());
    }

    private bool GetEnabledState()
    {
        return GetState().IsRadioEnabled();
    }

    private void SetEnabledState(bool state)
    {
        /*
        Traverse.Create(GetShip())
                .Property("NetworkState")
                .GetValue<ShipNetworkState>()
                .ServerSetPartState(PartContext.PartId, GetState().WithRadioEnabled(state));
        */

        InteractionService.StartInteractWithPart(new PartInteractNetworkRequest
        {
            ShipId = PartContext.ShipId,
            PartId = PartContext.PartId
        });
        InteractionService.UpdateLeverHeight(GetState().WithRadioEnabled(state).LeverHeight);
        InteractionService.StopInteract();
    }


    private LeverState GetState()
    {
        Traverse.Create(InteractionService)
                .Field("_ships")
                .GetValue<ShipsServerTracker>()
                .GetShip(PartContext.ShipId).TryGetStatefulPart(PartContext.PartId, out StatefulPart part);

        return (LeverState)part.State;
    }

    private TrackedShipServer GetShip()
    {
        return Traverse.Create(InteractionService)
                       .Field("_ships")
                       .GetValue<ShipsServerTracker>()
                       .GetShip(PartContext.ShipId);
    }
}
