using HarmonyLib;
using Ship.Parts.Common;
using Ship.Network.Service;
using Ship.Interface.Model.Parts;
using Ship;
using Ship.Interface.Model.Parts.State;
using MelonLoader;
using Managers.Interface.Model;
using Ship.Interface.Model;

namespace Radio;

public class RadioInteraction : PartInteraction
{
    public override string InteractionLabel => GetEnabledState() ? "Disable" : "Enable";
    public override float DetectionDistance => 1.5f;


    public override void Interact()
    {
        SetEnabledState(!GetEnabledState());
    }


    private bool GetEnabledState()
    {
        if (GetState() is not LeverState state) return false;


        return state.IsRadioEnabled();
    }

    private void SetEnabledState(bool state)
    {
        if (GetState() is not LeverState currentState) return;


        InteractionService.StartInteractWithPart(new PartInteractNetworkRequest
        {
            ShipId = PartContext.ShipId,
            PartId = PartContext.PartId
        });
        InteractionService.UpdateLeverHeight(currentState.WithRadioEnabled(state).LeverHeight);
        InteractionService.StopInteract();
    }


    private LeverState? GetState()
    {
        if (GetShip() is not TrackedShipServer ship) return null;

        ship.TryGetStatefulPart(PartContext.PartId, out StatefulPart part);
        if (part is null) return null;

        return (LeverState)part.State;
    }

    private TrackedShipServer? GetShip()
    {
        return Traverse.Create(InteractionService).Field("_ships").GetValue<ShipsServerTracker>().GetShip(PartContext.ShipId);
    }
}
