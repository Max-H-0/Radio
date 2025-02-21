using Ship.Parts.Common;

namespace Radio
{
    public class RadioVisuals : DefaultShipPartVisuals
    {
        public bool IsRadioEnabled { get; private set; } = false;


        public void ToggleRadio()
        {
            if (IsRadioEnabled)
            {
                DisableRadio();
            }
            else
            {
                EnableRadio();
            }
        }


        public void EnableRadio()
        {
            IsRadioEnabled = true;
        }

        public void DisableRadio()
        {
            IsRadioEnabled = false;
        }
    }
}
