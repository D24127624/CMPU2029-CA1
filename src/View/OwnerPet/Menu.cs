
namespace kms.View.OwnerPet
{

    using log4net;
    using Terminal.Gui;

    using kms.Util;
    using kms.Models;
    using kms.Services;

    /// <summary>
    /// Owner&Pet management menu.
    /// </summary>
    public class OwnerPetMenu(BaseNavigation navigation) : AbstractMenu(navigation, "Owner & Pet Management")
    {
        protected override ILog _log => LogManager.GetLogger(typeof(OwnerPetMenu));

        protected override Dictionary<string, ApplicationWindowsEnum?> _items => new()
            {
                {"1. Create/Update Pet and Owner", ApplicationWindowsEnum.MANAGE_OWNER_PET},
                {"3. View/Remove Owner and Pet", ApplicationWindowsEnum.VIEW_OWNER_PET},
                {"0. Return", ApplicationWindowsEnum.MAIN_MENU}
            };

        protected override void GotoSelectedView(int selectedItem) =>
            _navigation.GotoWindow(_items[_items.Keys.ElementAt(selectedItem)]);

    }

}
