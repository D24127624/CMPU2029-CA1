
namespace kms.View.Bookings
{

    using log4net;
    using Terminal.Gui;

    using kms.Util;
    using kms.Models;
    using kms.Services;

    /// <summary>
    /// Processing of new booking.
    /// </summary>
    public class NewBooking : AbstractBookingWindow
    {

        protected override ILog _log => LogManager.GetLogger(typeof(NewBooking));

        private readonly IBookingService _bookingService;
        private readonly IKennelService _kennelService;
        private readonly IPetService _petService;

        private readonly DateTime _tomorrow = DateTime.Today.AddDays(1);

        public NewBooking(
            BaseNavigation navigation,
            IBookingService bookingService,
            IKennelService kennelService,
            IPetService petService
        ) : base(navigation, "New Booking")
        {
            _bookingService = bookingService;
            _kennelService = kennelService;
            _petService = petService;

            InitializeWindowContent();
        }

        protected override void InitializeWindowContent()
        {
            string title = "Matching Pets";
            ShowSearchScreen(
                (owner, phone) => StartOwnerSearch<List<Pet>>(title, owner, phone, DisplayPetResults, _petService.SearchPetsByOwnerAsync),
                (name, type) => StartPetSearch<List<Pet>>(title, name, type, DisplayPetResults, _petService.SearchPetsAsync)
            );
        }

        private void DisplayPetResults(FrameView frame, List<Pet> results)
        {
            // initialize frame
            if (InitResultFrame(frame, results.Count))
            {
                // build data table values
                List<Dictionary<string, object?>> values = [];
                foreach (Pet pet in results)
                {
                    values.Add(new Dictionary<string, object?>
                        {
                            {"ID", pet.Id},
                            {"Pet", pet.Name},
                            {"Age", pet.Age},
                            {"type", $"{pet.PetType}"},
                            {"Owner", pet.Owner.Name},
                            {"Phone", pet.Owner.PhoneNumber}
                        }
                    );
                }

                // display table and action buttons
                TableView resultTable = ViewUtils.CreateTable(frame, 1, 1, Dim.Fill() - 2, Dim.Fill() - 1, values);
                Button? nextStep = null;
                resultTable.SelectedCellChanged += (args) =>
                {
                    if (nextStep == null)
                    {
                        nextStep = ViewUtils.CreateButton(frame, "Next >>", Pos.Center(), Pos.AnchorEnd(1), Colors.Menu);
                        nextStep.Clicked += () =>
                        {
                            int petId = (int)args.Table.Rows[resultTable.SelectedRow][0];
                            Pet pet = results.First(p => p.Id == petId);
                            ShowBookingScreen(pet);
                        };
                    }
                };
            }
        }

        private void ShowBookingScreen(Pet pet)
        {
            ShowOwnerPetDetails(pet);

            // booking frame
            FrameView bookingFrame = ViewUtils.CreateFrame(this, "New Booking", 2, 9, 5);
            DateField fieldStart = ViewUtils.CreateDateField(bookingFrame, "Start Date:", 2, 1, 12, _tomorrow);
            DateField fieldEnd = ViewUtils.CreateDateField(bookingFrame, "End Date:", Pos.Percent(40), 1, 12, _tomorrow.AddDays(5));
            Button findKennels = ViewUtils.CreateButton(bookingFrame, "Find Available Kennels", Pos.Percent(70), 1, Colors.Menu);
            findKennels.Clicked += async () => StartKennelSearch(pet, fieldStart.Date, fieldEnd.Date);
        }

        private async void StartKennelSearch(Pet pet, DateTime startDate, DateTime endDate)
        {
            _log.Info($"Kennels search: start-date='{startDate}' / end-date='{endDate}'");
            // validate inputs
            if (endDate <= startDate || startDate < _tomorrow)
            {
                ViewUtils.DisplayError("No valid search criteria", "Please ensure you have entered a valid start/end dates!");
                return;
            }
            ShowKennelsTable(
                BuildResultsFrame("Available Kennels", 2, 15),
                await _kennelService.FindAvailableKennelsForDateRange(pet, startDate, endDate),
                (frame, action, id, kennel) =>
                {
                    if (action == null)
                    {
                        action = ViewUtils.CreateButton(frame, "Create Booking", Pos.Center(), Pos.AnchorEnd(1), Colors.Menu);
                        action.Clicked += async () =>
                        {
                            BookingGroup group = ModelUtils.CreateBookingGroup(pet, kennel, startDate, endDate);
                            ShowConfirmationScreen(pet, await _bookingService.CreateBookingsAsync(group));
                        };
                    }
                }
            );
        }

        private void DisplayKennelResults(FrameView frame, Pet pet, DateTime startDate, DateTime endDate, List<Kennel> results)
        {
            // initialize frame
            if (InitResultFrame(frame, results.Count, "No kennels available for the selected period!"))
            {
                // build data table values
                List<Dictionary<string, object?>> values = [];
                foreach (Kennel kennel in results)
                {
                    values.Add(new Dictionary<string, object?>
                        {
                            {"ID", kennel.Id},
                            {"Name", kennel.Name},
                            {"Size", $"{kennel.Size}"},
                            {"Suitable For", $"{kennel.SuitableFor}"}
                        }
                    );
                }

                // display table and action buttons
                TableView resultTable = ViewUtils.CreateTable(frame, 1, 1, Dim.Fill() - 2, Dim.Fill() - 1, values);
                Button? complete = null;
                resultTable.SelectedCellChanged += (args) =>
                {

                };
            }
        }

        private void ShowConfirmationScreen(Pet pet, List<Booking> results)
        {
            ShowOwnerPetDetails(pet);

            // no booking created
            if (results.Count == 0)
            {
                ViewUtils.DisplayError("Problem creating you booking", "There was an issues crerating your booking. Please contact the system administrator!");
                _navigation.GotoWindow(ApplicationWindowsEnum.BOOKING_MENU);
                return;
            }

            // build data table values
            List<Dictionary<string, object?>> values = [];
            foreach (Booking booking in results)
            {
                values.Add(new Dictionary<string, object?>
                {
                    {"ID", booking.Id},
                    {"Name", booking.Kennel.Name},
                    {"Size", $"{booking.Kennel.Size}"},
                    {"Date", booking.Date.ToShortDateString()}
                });
            }

            // display booking confirmation
            FrameView resultsFrame = BuildResultsFrame("Booking Confirmation", 2, 9);
            TableView resultTable = ViewUtils.CreateTable(resultsFrame, 2, 1, Dim.Fill() - 2, Dim.Fill() - 1, values);
        }

    }

}
