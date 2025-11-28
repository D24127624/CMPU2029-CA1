
namespace kms.View.Bookings
{

    using log4net;

    using kms.Util;

    /// <summary>
    /// Booking sub-menu used to manging kennel bookings.
    /// </summary>
    public class BookingMenu(BaseNavigation navigation) : AbstractMenu(navigation, "Manage Bookings")
    {

        protected override ILog _log => LogManager.GetLogger(typeof(BookingMenu));

        protected override Dictionary<string, ApplicationWindowsEnum?> _items => new()
            {
                {"1. Create new Booking", ApplicationWindowsEnum.NEW_BOOKING},
                {"2. Cancel existing Booking", ApplicationWindowsEnum.CANCEL_BOOKING},
                {"3. View all current and upcomming Bookings", ApplicationWindowsEnum.VIEW_BOOKINGS},
                {"0. Return", ApplicationWindowsEnum.MAIN_MENU}
            };

        protected override void GotoSelectedView(int selectedItem)
        {
            ApplicationWindowsEnum? window = _items[_items.Keys.ElementAt(selectedItem)];
            // if creating a new booking check if the pet/owner is registered
            if (selectedItem == 0)
            {
                int checkResults = ViewUtils.DisplayMessage(
                    "Is Pet/Owner Registered",
                    "Please confirm with the owner that they have been registered in the system?",
                    ["_Yes", "_No"]);
                // cancel the operation
                if (checkResults == -1)
                {
                    return;
                }
                // redirect to pet/owner registration menu
                else if (checkResults == 1)
                {
                    window = ApplicationWindowsEnum.OWNER_PET_MENU;
                }
            }
            _navigation.GotoWindow(window);
        }

    }

}
