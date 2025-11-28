
namespace kms.View.OwnerPet
{

    using log4net;
    using Terminal.Gui;

    using kms.Util;
    using kms.Models;
    using kms.Services;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Show all registered Owners&Pets and allow removal.
    /// </summary>
    public class ViewOwnerPet : AbstractOwnerPetWindow
    {

        protected override ILog _log => LogManager.GetLogger(typeof(ViewOwnerPet));

        public ViewOwnerPet(
            BaseNavigation navigation,
            IOwnerService ownerService,
            IPetService petService
        ) : base(navigation, "View Registered Owner+Pets", ownerService, petService)
        {
            InitializeWindowContent();
        }

        protected async override void InitializeWindowContent()
        {
            ResetWindow(ApplicationWindowsEnum.OWNER_PET_MENU);
            ShowOwnersTable(
                BuildDetailsFrame("Registered Owners", 2, 1, Dim.Percent(50) - 2, Dim.Fill() - 2),
                await _ownerService.GetAllOwnersAsync(),
                OwnerSelectedAction
            );
        }

        private async void OwnerSelectedAction(FrameView parent, Button? action, int id, Owner owner)
        {
            if (action == null)
            {
                action = ViewUtils.CreateButton(parent, "Remove", Pos.Center(), Pos.AnchorEnd(1), Colors.Menu);
                action.Clicked += async () =>
                {
                    await _ownerService.RemoveOwnerAsync(id);
                    InitializeWindowContent();
                };
            }
            BuildPetsTable(parent, id);
        }

        private void PetSelectedAction(FrameView parent, Button? action, int id, Pet pet)
        {
            if (action == null)
            {
                action = ViewUtils.CreateButton(parent, "Remove", Pos.Center(), Pos.AnchorEnd(1), Colors.Menu);
                action.Clicked += async () =>
                {
                    await _petService.RemovePetAsync(id);
                    BuildPetsTable(parent, pet.Owner.Id);
                };
            }
        }

        private async void BuildPetsTable(FrameView parent, int ownerId)
        {
            ShowPetsTable(
                BuildResultsFrame("Registered Pets", 2, Pos.Bottom(parent), Dim.Fill() - 1, Dim.Fill() - 2),
                await _petService.GetPetsByOwnerAsync(ownerId),
                PetSelectedAction,
                false
            );
        }

    }

}
