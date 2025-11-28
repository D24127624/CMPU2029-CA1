
namespace kms.View
{

    using log4net;
    using Terminal.Gui;

    using kms.Services.Impl;
    using kms.Util;
    using System.Reflection;
    using kms.View.Bookings;
    using kms.View.OwnerPet;

    /// <summary>
    /// Enum of all the available UI screens.
    /// </summary>
    public enum ApplicationWindowsEnum
    {
        MAIN_MENU,

        // Booking management
        BOOKING_MENU,
        NEW_BOOKING,
        CANCEL_BOOKING,
        VIEW_BOOKINGS,
        SHOW_BOOKING,

        // Owner+Pet management
        OWNER_PET_MENU,
        MANAGE_OWNER_PET,
        VIEW_OWNER_PET,

        // System management
        KENNEL_MGMT,

    }

    /// <summary>
    /// Base navigation class used to manage the users interaction through the screens of the application.
    /// </summary>
    public class BaseNavigation
    {

        private static readonly ILog _log = LogManager.GetLogger(typeof(BaseNavigation));

        private bool _activeMenu = false;
        private Window _activeWindow;

        private readonly ConfigurationService _configurationService;
        private Dialog? _about = null;
        private readonly Toplevel _top;

        public BaseNavigation(ConfigurationService configurationService)
        {
            _configurationService = configurationService;
            Application.Init();
            _activeWindow = new LoadScreen(this);
            _top = Application.Top;
        }

        public int Launch()
        {
            _log.Info(StringUtils.GetTitle("Startup ..."));
            int exitCode = 0;
            try
            {
                _top.Add(_activeWindow);
                Application.Run();
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
                exitCode = 1;
            }
            _log.Info(StringUtils.GetTitle($"Shutdown [exit code: {exitCode}]"));
            Application.Shutdown();
            return exitCode;
        }

        public void GotoWindow(ApplicationWindowsEnum? target, object? data = null)
        {
            Window window;
            switch (target)
            {
                case ApplicationWindowsEnum.MAIN_MENU:
                    window = new MainMenu(this);
                    break;

                case ApplicationWindowsEnum.BOOKING_MENU:
                    window = new BookingMenu(this);
                    break;

                case ApplicationWindowsEnum.NEW_BOOKING:
                    window = new NewBooking(this, _configurationService.GetBookingService(), _configurationService.GetKennelService(), _configurationService.GetPetService());
                    break;

                case ApplicationWindowsEnum.CANCEL_BOOKING:
                    window = new CancelBooking(this, _configurationService.GetBookingService());
                    break;

                case ApplicationWindowsEnum.VIEW_BOOKINGS:
                    window = new ViewBookings(this, _configurationService.GetBookingService());
                    break;

                case ApplicationWindowsEnum.OWNER_PET_MENU:
                    window = new OwnerPetMenu(this);
                    break;

                case ApplicationWindowsEnum.MANAGE_OWNER_PET:
                    window = new ManageOwnerPet(this, _configurationService.GetOwnerService(), _configurationService.GetPetService());
                    break;

                case ApplicationWindowsEnum.VIEW_OWNER_PET:
                    window = new ViewOwnerPet(this, _configurationService.GetOwnerService(), _configurationService.GetPetService());
                    break;

                case ApplicationWindowsEnum.KENNEL_MGMT:
                    window = new KennelMgmt(this, _configurationService.GetKennelService());
                    break;

                default:
                    Application.RequestStop();
                    return;
            }
            _top.Remove(_activeWindow);
            if (!_activeMenu)
            {
                _top.Add(GetMenuBar());
                _activeMenu = true;
            }
            _activeWindow = window;
            _top.Add(_activeWindow);
            _activeWindow.FocusFirst();
            _activeWindow.LayoutSubviews();
        }

        private MenuBar GetMenuBar()
        {
            return new MenuBar([
                new MenuBarItem("_File", [
                    new MenuItem("_Home", "", () => GotoWindow(ApplicationWindowsEnum.MAIN_MENU)),
                    new MenuItem("_Quit", "", () => Application.RequestStop())
                ]),
                new MenuBarItem("_Help", [
                    new MenuItem("_About", "", ShowAboutDialog)
                ])
            ]);
        }

        private void ShowAboutDialog()
        {
            int height = 28;
            int width = 70;
            string product = $"{Assembly.GetEntryAssembly()?.GetCustomAttribute<AssemblyProductAttribute>()?.Product}";
            if (_about != null)
            {
                _about.Dispose();
                _top.Remove(_about);
            }
            _about = new Dialog
            {
                Title = $"About {product}",
                Height = height,
                Width = width
            };
            _top.Add(_about);

            Label splash = ViewUtils.CreateLabel(_about, ResourceUtils.SPLASH_TEXT, Pos.Center(), 0, Colors.Dialog);
            Label divider = ViewUtils.CreateLabel(_about, new string('-', width - 2), Pos.Center(), Pos.Bottom(splash) - 1, Colors.Dialog);

            string copyright = $"{Assembly.GetEntryAssembly()?.GetCustomAttribute<AssemblyCopyrightAttribute>()?.Copyright}";
            string version = $"{Assembly.GetEntryAssembly()?.GetCustomAttribute<AssemblyFileVersionAttribute>()?.Version}";
            ViewUtils.CreateLabel(_about, $"Version: {version}", 1, Pos.Bottom(divider), Colors.Dialog);
            ViewUtils.CreateLabel(_about, copyright, Pos.Center() + 2, Pos.Bottom(divider), Colors.Dialog);

            string author = $"{Assembly.GetEntryAssembly()?.GetCustomAttribute<AuthorAttribute>()?.Author}";
            string studentId = $"{Assembly.GetEntryAssembly()?.GetCustomAttribute<StudentIdAttribute>()?.StudentId}";
            string sourceUrl = $"{Assembly.GetEntryAssembly()?.GetCustomAttribute<SourceUrlAttribute>()?.SourceUrl}";
            ViewUtils.CreateLabel(_about, $"Author: {author}", 1, Pos.Bottom(divider) + 2, null, Dim.Percent(50) - 1, Colors.Dialog);
            ViewUtils.CreateLabel(_about, $"Student ID: {studentId}", Pos.Center() + 2, Pos.Bottom(divider) + 2, null, Dim.Fill() - 1, Colors.Dialog);
            ViewUtils.CreateLabel(_about, $"Source URL: {sourceUrl}", 1, Pos.Bottom(divider) + 4, null, Dim.Fill() - 1, Colors.Dialog);

            Button okBtn = ViewUtils.CreateButton(_about, "_OK", Pos.Center(), Pos.AnchorEnd(1), Colors.Menu);
            okBtn.IsDefault = true;
            okBtn.Clicked += () =>
            {
                _about.Dispose();
                _top.Remove(_about);
            };
        }

    }

}
