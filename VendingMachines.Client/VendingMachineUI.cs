using RAGE.Game;

namespace VendingMachines.Client
{
    internal static class VendingMachineUI
    {
        internal const string INTERACT_HELP_TEXT = "~h~Drücke ~g~ ~INPUT_CONTEXT~ ~s~ um etwas zu Kaufen!";

        internal static void Show()
        {
            var text = INTERACT_HELP_TEXT;
            Ui.BeginTextCommandDisplayHelp("STRING");
            Ui.AddTextComponentSubstringPlayerName(text);
            Ui.EndTextCommandDisplayHelp(0, false, true, -1);
        }
    }
}
