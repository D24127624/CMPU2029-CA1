
namespace kms.View
{
    using log4net;

    using kms.Util;

    /// <summary>
    /// Main menu for navigating kennel management operations.
    /// </summary>
    public class MainMenu(BaseNavigation navigation) : AbstractMenu(navigation, "Main Menu")
    {

        protected override ILog _log => LogManager.GetLogger(typeof(MainMenu));

        protected override Dictionary<string, ApplicationWindowsEnum?> _items => new()
            {
                {"1. Manage Bookings", ApplicationWindowsEnum.BOOKING_MENU},
                {"2. Pet and Owner Management", ApplicationWindowsEnum.OWNER_PET_MENU},
                {"3. Kennel Management", ApplicationWindowsEnum.KENNEL_MGMT},
                {"0. Exit", null}
            };

        protected override void GotoSelectedView(int selectedItem)
        {
            string key = _items.Keys.ElementAt(selectedItem);
            _navigation.GotoWindow(_items[key]);
        }

    }

}
