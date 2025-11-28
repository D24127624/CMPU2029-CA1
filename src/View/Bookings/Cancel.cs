
namespace kms.View.Bookings
{

    using log4net;
    using Terminal.Gui;

    using kms.Util;
    using kms.Models;
    using kms.Services;

    /// <summary>
    /// Cancel an existing booking.
    /// </summary>
    public class CancelBooking : AbstractBookingWindow
    {

        protected override ILog _log => LogManager.GetLogger(typeof(CancelBooking));

        private readonly IBookingService _bookingService;

        public CancelBooking(
            BaseNavigation navigation,
            IBookingService bookingService
        ) : base(navigation, "Cancel Booking")
        {
            _bookingService = bookingService;

            InitializeWindowContent();
        }

        protected override void InitializeWindowContent()
        {
            string title = "Matching Bookings";
            ShowSearchScreen(
                (owner, phone) => StartOwnerSearch<List<BookingGroup>>(title, owner, phone, DisplayBookingResults, _bookingService.SearchBookingsByOwnerAsync),
                (name, type) => StartPetSearch<List<BookingGroup>>(title, name, type, DisplayBookingResults, _bookingService.SearchBookingsByPetAsync)
            );
        }

        private void DisplayBookingResults(FrameView frame, List<BookingGroup> results)
        {
            // initialize frame
            if (InitResultFrame(frame, results.Count))
            {
                // build data table values
                List<Dictionary<string, object?>> values = [];
                foreach (BookingGroup group in results)
                {
                    Pet pet = group.Pet;
                    values.Add(new Dictionary<string, object?>
                        {
                            {"ID", group.GroupId},
                            {"Start-Date", group.StartDate.ToShortDateString()},
                            {"End-Date", group.EndDate.ToShortDateString()},
                            {"Pet", pet.Name},
                            {"Owner", pet.Owner.Name},
                            {"Phone", pet.Owner.PhoneNumber},
                            {"Kennel", group.Kennel.Name},
                            {"Size", $"{group.Kennel.Size}"}
                        }
                    );
                }

                // display table and action buttons
                TableView resultTable = ViewUtils.CreateTable(frame, 1, 1, Dim.Fill() - 2, Dim.Fill() - 1, values);
                Button? complete = null;
                resultTable.SelectedCellChanged += (args) =>
                {
                    if (complete == null)
                    {
                        complete = ViewUtils.CreateButton(frame, "Cancel Booking", Pos.Center(), Pos.AnchorEnd(1), Colors.Menu);
                        complete.Clicked += async () =>
                        {
                            int confirm = ViewUtils.DisplayMessage(
                                "Confirm Cancellation",
                                "Are you sure you want to cancel the selected booking?",
                                ["_Yes", "_No"]
                            );
                            // continue the cancellation
                            if (confirm == 0)
                            {
                                string groupId = (string)args.Table.Rows[resultTable.SelectedRow][0];
                                await _bookingService.CancelBookingsAsync(groupId);
                                _navigation.GotoWindow(ApplicationWindowsEnum.BOOKING_MENU);
                            }
                        };
                    }
                };
            }
        }

    }

}
