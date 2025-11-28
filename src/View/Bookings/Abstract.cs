
namespace kms.View.Bookings
{

    using Terminal.Gui;

    using kms.Util;
    using kms.Models;

    /// <summary>
    /// Abstract booking window class with shared UI code
    /// </summary>
    public abstract class AbstractBookingWindow(BaseNavigation navigation, string title) : AbstractWindow(navigation, title)
    {

        protected bool InitResultFrame(FrameView frame, int resultsCount, string? message = null)
        {
            // no results found
            if (resultsCount == 0)
            {
                ViewUtils.CreateLabel(frame, message ?? "No results found!", Pos.Center(), Pos.Center());
                return false;
            }
            return true;
        }

        protected async void StartOwnerSearch<T>(
            string frameTitle,
            string owner,
            string phone,
            Action<FrameView, T> displayAction,
            Func<string?, string?, Task<T>> searchFunction
        )
        {
            _log.Info($"{frameTitle} search: owner='{owner}' / phone='{phone}'");
            // validate inputs
            if (StringUtils.AllNullOrWhiteSpace([owner,phone]))
            {
                ViewUtils.DisplayError("No valid search criteria", "Please ensure you have entered a valid owner name or phone before attempting your search!");
                return;
            }
            displayAction(
                BuildResultsFrame(frameTitle, 2, 9),
                await searchFunction(StringUtils.ValueOrNull(owner), StringUtils.ValueOrNull(phone))
            );
        }

        protected async void StartPetSearch<T>(
            string frameTitle,
            string name,
            string type,
            Action<FrameView, T> displayAction,
            Func<string, PetType, Task<T>> searchFunction
        )
        {
            _log.Info($"{frameTitle} search: pet='{name}' / type='{type}'");
            // validate inputs
            PetType petType = EnumUtils.FromString<PetType>(type);
            if (StringUtils.AllNullOrWhiteSpace([name]))
            {
                ViewUtils.DisplayError("No valid search criteria", "Please ensure you have entered a valid pet name and selected the correct type before attempting your search!");
                return;
            }
            displayAction(
                BuildResultsFrame(frameTitle, 2, 9),
                await searchFunction(name, petType)
            );
        }

        protected void ShowSearchScreen(Action<string, string> ownerSearchAction, Action<string, string> petSearchAction)
        {
            ResetWindow(ApplicationWindowsEnum.BOOKING_MENU);
            // owner search frame
            FrameView ownerFrame = ViewUtils.CreateFrame(this, "Search by Owner", 2, 1, 8, Dim.Percent(50) - 2);
            TextField fieldOwner = ViewUtils.CreateTextField(ownerFrame, "Name:", 1);
            TextField fieldPhone = ViewUtils.CreateTextField(ownerFrame, "Phone Number:", 3);
            Button searchOwner = ViewUtils.CreateButton(ownerFrame, "Search by Owner", Pos.Center(), Pos.AnchorEnd(1), Colors.Menu);
            searchOwner.Clicked += async () => ownerSearchAction($"{fieldOwner.Text}".Trim(), $"{fieldPhone.Text}".Trim());

            // pet search frame
            FrameView petFrame = ViewUtils.CreateFrame(this, "Search by Pet", Pos.Right(ownerFrame) + 1, 1, 8);
            TextField fieldPet = ViewUtils.CreateTextField(petFrame, "Name:", 1);
            RadioGroup fieldType = ViewUtils.CreateRadioGroup<PetType>(petFrame, "Type:", 3);
            Button searchPet = ViewUtils.CreateButton(petFrame, "Search by Pet", Pos.Center(), Pos.AnchorEnd(1), Colors.Menu);
            searchPet.Clicked += async () => petSearchAction($"{fieldPet.Text}", $"{fieldType.RadioLabels[fieldType.SelectedItem]}");
        }

        protected void ShowOwnerPetDetails(Pet pet)
        {
            ResetWindow(ApplicationWindowsEnum.BOOKING_MENU);
            // selected owner frame
            Owner owner = pet.Owner;
            FrameView ownerFrame = ViewUtils.CreateFrame(this, "Booking for: Owner", 2, 1, 8, Dim.Percent(50) - 2);
            ViewUtils.CreateTextField(ownerFrame, "Owner:", 1, owner.Name, false);
            ViewUtils.CreateTextField(ownerFrame, "Phone Number:", 3, owner.PhoneNumber, false);
            ViewUtils.CreateTextField(ownerFrame, "Address:", 5, owner.Address, false);

            // selected pet frame
            FrameView petFrame = ViewUtils.CreateFrame(this, "Booking for: Pet", Pos.Right(ownerFrame) + 1, 1, 8);
            ViewUtils.CreateTextField(petFrame, "Name:", 1, pet.Name, false);
            ViewUtils.CreateTextField(petFrame, "Age:", 2, 3, 6, $"{pet.Age}", false);
            ViewUtils.CreateRadioGroup<PetType>(petFrame, "Type:", Pos.Percent(30), 3, $"{pet.PetType}", false);
            ViewUtils.CreateTextField(petFrame, "Breed:", 5, pet.Breed, false);

            if (pet is Dog dog)
            {
                ViewUtils.CreateTextField(petFrame, "Size:", Pos.Percent(65), 3, 10, $"{dog.Size}", false);
            }
        }

    }

}
