
namespace kms.View.OwnerPet
{

    using log4net;
    using Terminal.Gui;

    using kms.Util;
    using kms.Models;
    using kms.Services;
    using System.Reflection.Metadata;

    /// <summary>
    /// Create/Update Owner&Pet details.
    /// </summary>
    public class KennelMgmt : AbstractWindow
    {

        protected override ILog _log => LogManager.GetLogger(typeof(KennelMgmt));

        private IKennelService _kennelService;

        private Label? _reasonLabel = null;
        private TextField? _reason = null;
        private Button? _removeBtn = null;

        public KennelMgmt(
            BaseNavigation navigation,
            IKennelService kennelService
        ) : base(navigation, "Manage Kennels")
        {
            _kennelService = kennelService;
            InitializeWindowContent();
        }

        protected async override void InitializeWindowContent()
        {
            ResetWindow(ApplicationWindowsEnum.MAIN_MENU);
            // show kennels table
            FrameView frame = BuildResultsFrame("All Kennels", 2, 1, Dim.Fill() - 2, Dim.Percent(50) - 2);
            ShowKennelsTable(
                frame,
                await _kennelService.GetKennelsAsync(),
                KennelTableAction,
                false
            );
            Button addKennel = ViewUtils.CreateButton(frame, "Add Kennel", Pos.Percent(75), Pos.AnchorEnd(1), Colors.Menu);
            addKennel.Clicked += async () => ShowKennelDetails();
        }

        private void KennelTableAction(FrameView parent, Button? action, int id, Kennel kennel)
        {
            if (action == null)
            {
                action = ViewUtils.CreateButton(parent, "Edit", Pos.Percent(15), Pos.AnchorEnd(1), Colors.Menu);
                action.Clicked += () => ShowKennelDetails(kennel);
                _removeBtn = ViewUtils.CreateButton(parent, "Remove", Pos.Center(), Pos.AnchorEnd(1), Colors.Error);
                _removeBtn.Clicked += async () => RemoveKennel(await _kennelService.RemoveKennelAsync(id));
            }
        }

        private async void ShowKennelDetails(Kennel? kennel = null)
        {
            // kennel details frame
            FrameView frame = BuildDetailsFrame("Kennel Details", Pos.Center() + 1, 1);

            // populate frame
            TextField name = ViewUtils.CreateTextField(frame, "Name:", 1, kennel?.Name);
            RadioGroup size = ViewUtils.CreateRadioGroup<Models.Size>(frame, "Size:", 3, kennel?.Size.ToString());
            RadioGroup suitableFor = ViewUtils.CreateRadioGroup<PetType>(frame, "Suitable For:", 5, kennel?.SuitableFor.ToString());
            RadioGroup outOfService = ViewUtils.CreateRadioGroup(frame, "Is out-of-service?", 7, ["Yes", "No"], kennel?.IsOutOfService == true ? "Yes" : "No");
            ShowHideReason(frame, kennel?.IsOutOfService == true, kennel?.OutOfServiceComment);

            outOfService.SelectedItemChanged += (args) =>
                ShowHideReason(frame, outOfService.RadioLabels[args.SelectedItem] == "Yes", kennel?.OutOfServiceComment);

            Button updateKennel = ViewUtils.CreateButton(frame, kennel == null ? "Register" : "Update", Pos.Center(), Pos.AnchorEnd(1), Colors.Menu);
            updateKennel.Clicked += async () =>
            {
                string sizeTxt = $"{size.RadioLabels[size.SelectedItem]}";
                string suitableForTxt = $"{suitableFor.RadioLabels[suitableFor.SelectedItem]}";
                bool outOfServiceTxt = $"{outOfService.RadioLabels[outOfService.SelectedItem]}" == "Yes";
                RegisterUpdateKennel($"{name.Text}".Trim(), sizeTxt, suitableForTxt, outOfServiceTxt, _reason?.Text.ToString(), kennel?.Id);
            };
        }

        private void ShowHideReason(FrameView frame, bool isOutOfService, string? OutOfServiceComment)
        {
            if (isOutOfService)
            {
                _reasonLabel = ViewUtils.CreateLabel(frame, "Reason:", 9);
                _reason = ViewUtils.CreateTextField(frame, _reasonLabel, OutOfServiceComment);
            }
            else
            {
                frame.Remove(_reasonLabel);
                frame.Remove(_reason);
            }
        }

        private void RemoveKennel(bool success)
        {
            if (!success)
            {
                ViewUtils.DisplayError("Failed to remove kennel", "You requerst failed! This is due to there being future bookings!");
                return;
            }
            InitializeWindowContent();
        }

        private async void RegisterUpdateKennel(string name, string size, string suitableFor, bool outOfService, string? reason, int? id)
        {
            Kennel? kennel = ModelUtils.CreateKennel(name, size, suitableFor, outOfService, reason, id);
            // validate inputs
            if (kennel == null)
            {
                ViewUtils.DisplayError("Incomplete Kennel Details", "Please ensure you have entered all fields before attempting your registration!");
                return;
            }
            // adding a new owner
            if (id == null)
            {
                await _kennelService.AddKennelAsync(kennel);
            }
            // updating an existing owner
            else
            {
                await _kennelService.UpdateKennelAsync(kennel);
            }
            InitializeWindowContent();
        }

    }

}
