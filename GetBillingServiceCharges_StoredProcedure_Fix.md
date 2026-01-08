## Fixed Stored Procedure for GetBillingServiceCharges

### Issue
The current stored procedure uses `SELECT *` which can cause column name conflicts when joining multiple tables that have columns with the same name (like "Amount", "Id", etc.).

### Solution
Update your stored procedure to explicitly select and alias columns:

```sql
ALTER PROCEDURE GetBillingServiceCharges
@BillingId INT
AS
BEGIN
    SELECT 
        -- Billing_ServiceCharge fields
        BSC.Id AS BillingServiceChargeId,
        BSC.Billing_Id AS BillingId,
        BSC.ServiceCharge_Id AS ServiceChargeId,
        BSC.CreatedBy AS BSC_CreatedBy,
        BSC.CreatedDateTime AS BSC_CreatedDateTime,
        
        -- Billing fields
        B.Id AS B_Id,
        B.BillingDate AS B_BillingDate,
        B.TotalAmount AS B_TotalAmount,
        B.IsNoShowCharge AS B_IsNoShowCharge,
        B.PaymentStatus AS B_PaymentStatus,
        B.PaymentMethod AS B_PaymentMethod,
        B.Reservation_ID AS B_ReservationId,
        B.CompanyMaster_Id AS B_CompanyMasterId,
        
        -- ServiceCharge fields
        SC.Id AS SC_Id,
        SC.ServiceType AS SC_ServiceType,
        SC.Amount AS SC_Amount,
        SC.ServiceDate AS SC_ServiceDate,
        SC.IsFree AS SC_IsFree,
        SC.Description AS SC_Description,
        SC.Status AS SC_Status
        
    FROM Billing_ServiceCharge AS BSC
    LEFT JOIN Billing AS B ON BSC.Billing_Id = B.Id
    LEFT JOIN ServiceCharge AS SC ON BSC.ServiceCharge_Id = SC.Id
    WHERE BSC.Billing_Id = @BillingId
END;
```

This approach eliminates column name conflicts and makes the API more reliable.