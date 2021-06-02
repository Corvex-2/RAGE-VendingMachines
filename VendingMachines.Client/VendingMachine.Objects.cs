using static RAGE.Events;

namespace VendingMachines.Client
{
    public partial class VendingMachine
    {
        internal static void AttachObject(string ObjectName)
        {
            CallRemote("staticAttachments.Add", ObjectName);
        }
        internal static void DetachObject(string ObjectName)
        {
            CallRemote("staticAttachments.Remove", ObjectName);
        }
    }
}
