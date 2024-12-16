using Eco.Gameplay.Players;
using Eco.Gameplay.Property;
using Eco.Gameplay.Rooms;
using Eco.Shared.Localization;

namespace Eco.RM.Framework.Utility;

public class RequireNoRoomContainmentAttribute : RoomRequirementAttribute
{
    public override bool IsMet(Room room, User owner) => (room?.Valid ?? false) ? false : true;
    public override LocString Describe() => Localizer.Do($"Must be outside");
    public override LocString Describe(Room room, User owner) => Localizer.Do($"Used Outside");
}