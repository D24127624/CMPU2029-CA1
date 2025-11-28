
namespace kms.View.Bookings
{

    using log4net;
    using NStack;
    using Terminal.Gui;

    using kms.Util;
    using kms.Models;
    using kms.Services;

    /// <summary>
    /// View current/future bookings.
    /// </summary>
    public class ViewBookings : AbstractBookingWindow
    {

        protected override ILog _log => LogManager.GetLogger(typeof(ViewBookings));

        private readonly IBookingService _bookingService;

        public ViewBookings(
            BaseNavigation navigation,
            IBookingService bookingService
        ) : base(navigation, "Current/Future Bookings")
        {
            _bookingService = bookingService;

            InitializeWindowContent();
        }

        protected async override void InitializeWindowContent()
        {
            ResetWindow(ApplicationWindowsEnum.BOOKING_MENU);
            DateTime today = DateTime.Today;
            DisplayBookingsTable(
                ViewUtils.CreateFrame(this, $"New arrivals for {today.ToShortDateString()}", 2, 1, Dim.Percent(50) - 2),
                await _bookingService.SearchBookingsScheduledAsync(today)
            );
            DisplayBookingsTable(
                ViewUtils.CreateFrame(this, $"Upcoming bookings (next 14 days)", 2, Pos.Percent(50) - 1, Dim.Fill() - 2),
                await _bookingService.SearchBookingsScheduledAsync(today.AddDays(1), today.AddDays(15)),
                false
            );
        }

        private void DisplayBookingsTable(FrameView frame, List<BookingGroup> results, bool simple = true)
        {
            if (results.Count > 0)
            {
                // build data table values
                List<Dictionary<string, object?>> values = [];
                foreach (BookingGroup group in results)
                {
                    Pet pet = group.Pet;
                    Dictionary<string, object?> entry = [];
                    entry["ID"] = group.GroupId;
                    if (!simple) entry["Date"] = $"{group.StartDate.ToShortDateString()}";
                    entry["Pet"] = pet.Name;
                    entry["Owner"] = pet.Owner.Name;
                    entry["Phone"] = pet.Owner.PhoneNumber;
                    entry["Kennel"] = group.Kennel.Name;
                    entry["Size"] = $"{group.Kennel.Size}";
                    values.Add(entry);
                }

                // display table
                ViewUtils.CreateTable(frame, 1, 1, Dim.Fill() - 2, Dim.Fill() - 1, values);
            }
        }
    }

}
