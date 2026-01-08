# Fix Summary for MealPlan_id Zero-to-Null Conversion

## Issue Identified
In the `InsertReservationAsync` method in `ReservationRepository.cs`, the `MealPlan_id` parameter is not properly converting 0 values to NULL for the stored procedure.

## Current Implementation (Incorrect)
```csharp
command.Parameters.Add(new SqlParameter("@MealPlan_id", SqlDbType.Int) { Value = (object?)dto.MealPlan_id ?? DBNull.Value });
```

**Problem**: This only converts `null` values to `DBNull.Value`, but when `MealPlan_id` is 0, it passes 0 to the stored procedure instead of NULL.

## Required Fix
Replace the line with:
```csharp
command.Parameters.Add(new SqlParameter("@MealPlan_id", SqlDbType.Int) 
{ 
    Value = ConvertZeroToNull(dto.MealPlan_id)
});
```

## Why This Fix Works
1. **Consistent with UpdateReservation**: The `UpdateReservationAsync` method already uses `ConvertZeroToNull(dto.MealPlan_id)` for the same parameter
2. **Uses Existing Helper**: The `ConvertZeroToNull` helper method is already implemented in the class
3. **Proper Logic**: The helper method converts both `null` and `0` values to `DBNull.Value`

## ConvertZeroToNull Helper Method
```csharp
private static object ConvertZeroToNull(int? value)
{
    // Convert null or 0 to DBNull.Value, otherwise return the actual value
    return (!value.HasValue || value.Value == 0) ? DBNull.Value : value.Value;
}
```

## Expected Behavior After Fix
- `MealPlan_id = null` ? `NULL` in stored procedure ?
- `MealPlan_id = 0` ? `NULL` in stored procedure ? (This is the fix)
- `MealPlan_id = 5` ? `5` in stored procedure ?

## Location of Fix
**File**: `QueensHotelAPI/Repositories/ReservationRepository.cs`  
**Method**: `InsertReservationAsync`  
**Line**: Around line 464 (in the SQL parameter setup section)

## Impact
This ensures consistency between the INSERT and UPDATE operations for reservations, and allows the frontend to send 0 to indicate "no meal plan selected."

## Testing
After applying this fix, test with:
```json
{
  "checkInDate": "2025-12-01T00:00:00.000Z",
  "checkOutDate": "2025-12-05T00:00:00.000Z",
  "numberOfGuests": 2,
  "paymentMethodI_Id": 1,
  "createBy": "admin001",
  "customer_Id": 46,
  "mealPlan_id": 0,  // Should be converted to NULL
  "suite_id": 0,
  "room_ID": 101,
  "travalAgency_Id": 0
}
```

The `mealPlan_id: 0` should now be properly converted to NULL in the stored procedure.