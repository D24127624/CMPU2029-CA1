
SELECT * FROM (
    SELECT k.*, count(b.Date) AS BookingCount
      FROM Kennels k
      LEFT JOIN Bookings b
        ON b.KennelId = k.Id
       AND b.Date >= @start
       AND b.Date <= @end
     WHERE k.IsOutOfService = 0
       AND k.SuitableFor = @type
       AND (@size IS NULL OR Size = @size)
  GROUP BY k.Id
  ORDER BY Id ASC
)
