## Billing Service Charge API Documentation

### Overview
The Billing Service Charge API allows you to manage billing service charge associations using stored procedures.

## Endpoints

### 1. Insert Billing Service Charge
**POST** `/api/billingservicecharge`

Associates a service charge with a billing record.

#### Request Body
```json
{
  "billingId": 123,
  "serviceChargeId": 456
}
```

#### Request Parameters
- `billingId` (int, required): The ID of the billing record
- `serviceChargeId` (int, required): The ID of the service charge to associate

**Note**: The `CreatedBy` field is automatically set to `'admin@gmail.com'` and `CreatedDateTime` is set to `GETDATE()` in the stored procedure.

#### Success Response (200 OK)
```json
{
  "success": true,
  "message": "Billing service charge inserted successfully",
  "data": {
    "billingId": 123,
    "serviceChargeId": 456,
    "createdBy": "admin@gmail.com",
    "createdDateTime": "2025-09-06T10:30:00"
  },
  "timestamp": "2025-09-06T10:30:00.123Z",
  "processedBy": "dilshan-jolanka"
}
```

### 2. Get Billing Service Charges
**GET** `/api/billingservicecharge/{billingId}`

Retrieves all service charges associated with a billing record.

#### Path Parameters
- `billingId` (int, required): The billing ID to retrieve service charges for

#### Success Response (200 OK)
```json
{
  "success": true,
  "message": "Retrieved 2 billing service charges for Billing ID: 123",
  "data": [
    {
      "billingServiceChargeId": 1,
      "billingId": 123,
      "serviceChargeId": 456,
      "createdBy": "admin@gmail.com",
      "createdDateTime": "2025-09-06T10:30:00",
      "billingInfo": {
        "billingDate": "2025-09-06T00:00:00",
        "totalAmount": 1500.50,
        "isNoShowCharge": false,
        "paymentStatus": "Pending",
        "paymentMethod": 2,
        "reservationId": 789,
        "companyMasterId": 1
      },
      "serviceChargeInfo": {
        "serviceType": "Room Service",
        "amount": 25.00,
        "serviceDate": "2025-09-06T14:30:00",
        "isFree": false,
        "description": "Late night room service",
        "status": "Active"
      }
    }
  ],
  "count": 2,
  "timestamp": "2025-09-06T10:30:00.123Z",
  "processedBy": "dilshan-jolanka"
}
```

#### Not Found Response (404 Not Found)
```json
{
  "success": false,
  "message": "No billing service charges found for Billing ID: 123",
  "data": [],
  "count": 0,
  "timestamp": "2025-09-06T10:30:00.123Z",
  "processedBy": "dilshan-jolanka"
}
```

### 3. Test Connection
**GET** `/api/billingservicecharge/test-connection`

Returns API status and available endpoints.

### 4. Available Billing IDs
**GET** `/api/billingservicecharge/available-billing-ids`

Returns guidance on how to find valid billing IDs.

## Error Responses

#### Validation Error (400 Bad Request)
```json
{
  "success": false,
  "message": "Validation failed",
  "errors": [
    "BillingId: Billing ID is required",
    "ServiceChargeId: Service Charge ID is required"
  ],
  "timestamp": "2025-09-06T10:30:00.123Z",
  "processedBy": "dilshan-jolanka"
}
```

#### Foreign Key Error (500 Internal Server Error)
```json
{
  "success": false,
  "message": "An error occurred while processing your billing service charge insertion request",
  "error": "The INSERT statement conflicted with the FOREIGN KEY constraint...",
  "timestamp": "2025-09-06T10:30:00.123Z",
  "processedBy": "dilshan-jolanka"
}
```

## Stored Procedures

### InsertBillingServiceCharge
```sql
ALTER PROCEDURE [dbo].[InsertBillingServiceCharge]
    @Billing_Id INT,
    @ServiceCharge_Id INT
AS
BEGIN
    INSERT INTO Billing_ServiceCharge (
        Billing_Id,
        ServiceCharge_Id,
        CreatedBy,
        CreatedDateTime
    )
    VALUES (
        @Billing_Id,
        @ServiceCharge_Id,
        'admin@gmail.com',
        GETDATE()
    );
END;
```

### GetBillingServiceCharges
```sql
ALTER PROCEDURE GetBillingServiceCharges
@BillingId INT
AS
BEGIN
    SELECT *
    FROM Billing_ServiceCharge AS BSC
    LEFT JOIN Billing AS B ON BSC.Billing_Id = B.Id
    LEFT JOIN ServiceCharge AS SC ON BSC.ServiceCharge_Id = SC.Id
    WHERE Billing_Id = @BillingId
END;
```

## Usage Examples

### Insert a billing service charge
```bash
curl -X POST "https://yourapi.com/api/billingservicecharge" \
     -H "Content-Type: application/json" \
     -d '{
       "billingId": 123,
       "serviceChargeId": 456
     }'
```

### Get billing service charges
```bash
curl -X GET "https://yourapi.com/api/billingservicecharge/123"
```

### Test connection
```bash
curl -X GET "https://yourapi.com/api/billingservicecharge/test-connection"
```

## Troubleshooting

### Foreign Key Constraint Error
If you get a foreign key constraint error:

1. **Check if billing record exists:**
   ```bash
   curl -X GET "https://yourapi.com/api/billing/0"
   ```

2. **Create a billing record first:**
   ```bash
   curl -X POST "https://yourapi.com/api/billing" \
        -H "Content-Type: application/json" \
        -d '{
          "billingDate": "2025-09-06",
          "totalAmount": 100.00,
          "isNoShowCharge": false,
          "paymentStatus": "Pending",
          "paymentMethod": 1,
          "createdBy": "admin@gmail.com",
          "reservationId": 1
        }'
   ```

3. **Use the returned billing ID in your service charge request**

### Service Charge ID Validation
Make sure the `serviceChargeId` you're using exists in the ServiceCharge table.