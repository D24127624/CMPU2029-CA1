
namespace kms.View
{

    using log4net;
    using Terminal.Gui;

    using kms.Util;

    /// <summary>
    /// Startup splash screen.
    /// </summary>
    public class LoadScreen : AbstractWindow
    {

        protected override ILog _log => LogManager.GetLogger(typeof(LoadScreen));

        public LoadScreen(BaseNavigation navigation) : base(navigation, "Load Screen", false)
        {
            InitializeWindowContent();
        }

        protected override void InitializeWindowContent()
        {
            // override default window settings
            TextAlignment = TextAlignment.Centered;

            // add window content
            ViewUtils.CreateLabel(this, ResourceUtils.SPLASH_TEXT, Pos.Center(), Pos.Center(), 20, 64);
            Button launchBtn = ViewUtils.CreateButton(this, "_Launch", Pos.Center(), Pos.AnchorEnd(3));
            launchBtn.IsDefault = true;
            launchBtn.Clicked += () => _navigation.GotoWindow(ApplicationWindowsEnum.MAIN_MENU);
        }

    }

}
