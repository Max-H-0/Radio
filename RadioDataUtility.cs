using System;
using Ship.Interface.Model.Parts.State;

namespace Radio;

public static class RadioDataUtility
{
    public struct Indecies
    {
        public const int IsEnabledByte = 0;
    }


    public static byte[] RadioDataBytes(this LeverState state)
    {
        return BitConverter.GetBytes(state.LeverHeight);
    }


    public static bool IsRadioEnabled(this LeverState state)
    {
        byte isEnabledByte = state.RadioDataBytes()[Indecies.IsEnabledByte];
        bool isEnabled = isEnabledByte != 0;

        return isEnabled;
    }

    public static LeverState WithRadioEnabled(this LeverState state, bool isEnabled)
    {
        byte[] data = state.RadioDataBytes();
        data[Indecies.IsEnabledByte] = isEnabled? (byte)1 : (byte)0;
        
        float convertedData = BitConverter.ToSingle(data);


        return new LeverState { LeverHeight = convertedData }; 
    }
}
