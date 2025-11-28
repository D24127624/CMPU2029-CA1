
namespace kms.View.OwnerPet
{

    using log4net;
    using Terminal.Gui;

    using kms.Util;
    using kms.Models;
    using kms.Services;

    /// <summary>
    /// Create/Update Owner&Pet details.
    /// </summary>
    public class ManageOwnerPet : AbstractOwnerPetWindow
    {

        protected override ILog _log => LogManager.GetLogger(typeof(ManageOwnerPet));

        public ManageOwnerPet(
            BaseNavigation navigation,
            IOwnerService ownerService,
            IPetService petService
        ) : base(navigation, "Manage Owner/Pet", ownerService, petService)
        {
            InitializeWindowContent();
        }

        protected async override void InitializeWindowContent()
        {
            ResetWindow(ApplicationWindowsEnum.OWNER_PET_MENU);
            // owner search frame
            FrameView ownerFrame = ViewUtils.CreateFrame(this, "Search by Owner", 2, 1, 10, Dim.Percent(50) - 2);
            TextField fieldOwner = ViewUtils.CreateTextField(ownerFrame, "Name:", 1);
            TextField fieldPhone = ViewUtils.CreateTextField(ownerFrame, "Phone Number:", 3);
            Button searchOwner = ViewUtils.CreateButton(ownerFrame, "Search", Pos.Percent(20), Pos.AnchorEnd(1), Colors.Menu);
            searchOwner.Clicked += async () => SearchOwner($"{fieldOwner.Text}".Trim(), $"{fieldPhone.Text}".Trim());
            Button addOwner = ViewUtils.CreateButton(ownerFrame, "Add Owner", Pos.Percent(70), Pos.AnchorEnd(1), Colors.Menu);
            addOwner.Clicked += async () => ShowOwnerDetails();
        }

        private async void SearchOwner(string owner, string phone)
        {
            _log.Info($"Owner search: owner='{owner}' / phone='{phone}'");
            // validate inputs
            if (StringUtils.AllNullOrWhiteSpace([owner, phone]))
            {
                ViewUtils.DisplayError("No valid search criteria", "Please ensure you have entered a valid owner name or phone before attempting your search!");
                return;
            }
            ShowOwnersTable(
                BuildResultsFrame("Search Results", 2, 11, Dim.Fill() - 1, Dim.Percent(50) - 2),
                await _ownerService.SearchOwnersAsync(StringUtils.ValueOrNull(owner), StringUtils.ValueOrNull(phone)),
                OwnersTableAction
            );
        }

        private void OwnersTableAction(FrameView parent, Button? action, int id, Owner owner)
        {
            if (action == null)
            {
                action = ViewUtils.CreateButton(parent, "Edit", Pos.Center(), Pos.AnchorEnd(1), Colors.Menu);
                action.Clicked += async () =>
                {
                    ShowOwnerDetails(owner);
                };
            }
        }

        private async void ShowOwnerDetails(Owner? owner = null)
        {
            // owner details frame
            FrameView frame;
            if (owner == null)
            {
                frame = BuildDetailsFrame("Owner Details", Pos.Center() + 1, 1, Dim.Fill() - 1, Dim.Fill() - 2);
            }
            else
            {
                ResetWindow(ApplicationWindowsEnum.OWNER_PET_MENU);
                frame = ViewUtils.CreateFrame(this, "Owner Details", 2, 1, 10, Dim.Percent(50) - 2);
            }

            // populate frame
            TextField fieldOwner = ViewUtils.CreateTextField(frame, "Name:", 1, owner?.Name);
            TextField fieldAddress = ViewUtils.CreateTextField(frame, "Address:", 3, owner?.Address);
            TextField fieldPhone = ViewUtils.CreateTextField(frame, "Phone Number:", 5, owner?.PhoneNumber);

            // execute testion buttons
            Button updateOwner = ViewUtils.CreateButton(frame, owner == null ? "Register" : "Update", owner == null ? Pos.Center() : Pos.Percent(20), Pos.AnchorEnd(1), Colors.Menu);
            updateOwner.Clicked += async () => RegisterUpdateOwner($"{fieldOwner.Text}".Trim(), $"{fieldAddress.Text}".Trim(), $"{fieldPhone.Text}".Trim(), owner?.Id);
            if (owner != null)
            {
                Button addPet = ViewUtils.CreateButton(frame, "Add Pet", Pos.Percent(70), Pos.AnchorEnd(1), Colors.Menu);
                addPet.Clicked += () => ShowPetDetails(owner);
                ShowPetsTable(
                    BuildResultsFrame("Registered Pets", 2, 11, Dim.Fill() - 1, Dim.Percent(50) - 2),
                    await _petService.GetPetsByOwnerAsync(owner.Id),
                    PetsTableAction
                );
            }
        }

        private void PetsTableAction(FrameView parent, Button? action, int id, Pet pet)
        {
            if (action == null)
            {
                action = ViewUtils.CreateButton(parent, "Edit", Pos.Center(), Pos.AnchorEnd(1), Colors.Menu);
                action.Clicked += () => ShowPetDetails(pet.Owner, pet);
            }
        }

        private async void ShowPetDetails(Owner owner, Pet? pet = null)
        {
            FrameView frame = BuildDetailsFrame("Pet Details", Pos.Center() + 1, 1, Dim.Fill() - 1, Dim.Fill() - 2);

            // populate frame
            TextField name = ViewUtils.CreateTextField(frame, "Name:", 1, pet?.Name);
            TextField age = ViewUtils.CreateTextField(frame, "Age:", 2, 3, 6, pet?.Age.ToString());
            RadioGroup type = ViewUtils.CreateRadioGroup<PetType>(frame, "Type:", Pos.Percent(30), 3, pet?.PetType.ToString());
            TextField breed = ViewUtils.CreateTextField(frame, "Breed:", 5, pet?.Breed, pet?.PetType != PetType.CAT);
            TextField foodPref = ViewUtils.CreateTextField(frame, "Food Preferences:", 7, pet?.FoodPreferences);

            Models.Size? petSize = pet?.PetType == PetType.DOG ? ((Dog)pet).Size : null;
            (Label? sizeLabel, RadioGroup? size) = ShowOption<Models.Size>(frame, "Size:", 9, pet?.PetType, petSize);

            WalkingPreference? petWalk = pet?.PetType == PetType.DOG ? ((Dog)pet).WalkingPreference : null;
            (Label? walkLabel, RadioGroup? walkPref) = ShowOption<WalkingPreference>(frame, "Walking Preference:", 11, pet?.PetType, petWalk);

            type.SelectedItemChanged += (args) =>
            {
                // clean-up UI on type change
                RemoveOptions(frame, [sizeLabel, size, walkLabel, walkPref]);

                // show/hide fields based on the selected pet type
                PetType petType = EnumUtils.FromString<PetType>($"{type.RadioLabels[args.SelectedItem]}");
                if (petType == PetType.DOG)
                {
                    (sizeLabel, size) = ShowOption<Models.Size>(frame, "Size:", 9, petType);
                    (walkLabel, walkPref) = ShowOption<WalkingPreference>(frame, "Walking Preference:", 11, petType);
                }
            };

            Button updatePet = ViewUtils.CreateButton(frame, pet == null ? "Register" : "Update", Pos.Center(), Pos.AnchorEnd(1), Colors.Menu);
            updatePet.Clicked += async () =>
            {
                string typeTxt = type != null ? $"{type.RadioLabels[type.SelectedItem]}" : "";
                string sizeTxt = size != null ? $"{size.RadioLabels[size.SelectedItem]}" : "";
                string walkTxt = walkPref != null ? $"{walkPref.RadioLabels[walkPref.SelectedItem]}" : "";
                RegisterUpdatePet(owner, typeTxt, $"{name.Text}".Trim(), $"{age.Text}".Trim(), $"{breed.Text}".Trim(),
                        $"{foodPref.Text}".Trim(), sizeTxt, walkTxt, pet?.Id);
            };
        }

        private (Label?, RadioGroup?) ShowOption<TEnum>(FrameView frame, string labelText, Pos row, PetType? type, TEnum? value = null) where TEnum : struct, Enum
        {
            if (type != null && type == PetType.DOG)
            {
                Label label = ViewUtils.CreateLabel(frame, labelText, row);
                RadioGroup field = ViewUtils.CreateRadioGroup<TEnum>(frame, label, value?.ToString());
                return (label, field);
            }
            return (null, null);
        }

        private void RemoveOptions(FrameView frame, List<View?> views)
        {
            foreach (View? view in views)
            {
                if (view != null) frame.Remove(view);
            }
        }

        private async void RegisterUpdateOwner(string name, string address, string phone, int? id)
        {
            Owner? owner = ModelUtils.CreateOwner(name, address, phone, id);
            // validate inputs
            if (owner == null)
            {
                ViewUtils.DisplayError("Incomplete Owner Details", "Please ensure you have entered all fields before attempting your registration!");
                return;
            }
            // adding a new owner
            if (id == null)
            {
                owner = await _ownerService.RegisterOwnerAsync(owner);
            }
            // updating an existing owner
            else
            {
                await _ownerService.UpdateOwnerAsync(owner);
            }
            ShowOwnerDetails(owner);
        }

        private async void RegisterUpdatePet(Owner owner, string type, string name, string age, string breed, string foodPref, string? size, string? walkPref, int? id)
        {
            bool validAge = Int32.TryParse(age, out int numAge);
            Pet? pet = validAge ? ModelUtils.CreatePet(owner, type, name, numAge, breed, foodPref, size, walkPref, id) : null;
            if (pet == null)
            {
                ViewUtils.DisplayError("Incomplete Pet Details", "Please ensure you have entered all fields before attempting your registration or update!");
                return;
            }
            // adding a new pet
            if (id == null)
            {
                await _petService.RegisterPetAsync(pet);
            }
            // updating an existing pet
            else
            {
                await _petService.UpdatePetAsync(pet);
            }
            ShowOwnerDetails(owner);
        }

    }

}
