# Route APIs - Route Stops Integration Summary

## What Was Changed

The Route APIs have been enhanced to support creating and updating route stops along with routes. The changes maintain backward compatibility while adding powerful new functionality.

---

## Modified Files

### 1. **RouteDto.cs** (Data Transfer Object)
**Change:** Added `RouteStops` collection
```csharp
public List<RouteStopInputDto> RouteStops { get; set; } = new();
```
- Routes now include a list of stops in API requests and responses
- Stops are included automatically in GET endpoints
- OrderIndex determines sort order in responses

### 2. **RouteStopInputDto.cs** (NEW FILE)
**Purpose:** Input DTO for route stops without RouteId
```csharp
public class RouteStopInputDto
{
	public string StopName { get; set; } = null!;
	public TimeSpan? ArrivalTime { get; set; }
	public int OrderIndex { get; set; }
}
```
- Used for input/output in route endpoints
- RouteId is automatically set on backend based on route ID
- Keeps API cleaner and prevents client from setting RouteId

### 3. **RouteService.cs** (Service Layer)
**Changes:**
- **CreateAsync()**: Now creates route stops along with the route
  - Route created first, then stops using the generated route ID
  - Stops only created if RouteStops list has items

- **UpdateAsync()**: Handles route stop replacement
  - Deletes all existing stops for the route
  - Creates new stops from the DTO
  - Called atomically after route update

- **GetAllAsync()**: Now returns routes with stops
  - Includes related route stops in response
  - Stops auto-sorted by OrderIndex (ascending)
  - Returns empty array if route has no stops

- **GetByIdAsync()**: Now returns route with stops
  - Uses Include() to fetch route stops
  - Stops auto-sorted by OrderIndex (ascending)
  - Single route response with complete stop information

### 4. **RouteHandler.cs** (Validation Layer)
**Changes:**
- **CreateAsync()**: Added route stop validation
  - Validates stop names are not empty
  - Validates OrderIndex is >= 0
  - Skipped if RouteStops is empty or null

- **UpdateAsync()**: Added same route stop validation
  - Ensures data integrity before service call
  - Allows routes without stops (empty array)

### 5. **RouteController.cs** (No changes needed)
- Existing endpoints automatically serve updated data
- POST, PUT, GET endpoints now handle route stops
- PATCH /status endpoint unchanged (updates status only)

---

## API Endpoints - Unchanged URLs, Enhanced Functionality

### 1. POST /api/route - Create Route with Stops
**Before:** Created route without stops  
**After:** Can include route stops in request body  
**Stops:** Created immediately with the route

### 2. GET /api/route - Get All Routes
**Before:** Returned routes without stops  
**After:** Includes route stops for each route, sorted by orderIndex

### 3. GET /api/route/{id} - Get Single Route
**Before:** Returned route without stops  
**After:** Includes all stops sorted by orderIndex

### 4. PUT /api/route/{id} - Update Route
**Before:** Updated route only  
**After:** Can update route + completely replace all stops

### 5. PATCH /api/route/{id}/status - Update Status
**Before/After:** Unchanged (updates status only)

### 6. DELETE /api/route/{id} - Delete Route
**Before/After:** Unchanged (cascade delete removes stops automatically)

---

## Database Behavior

### Creating a Route with Stops
1. Route entity created and saved → assigned ID
2. RouteStop entities created with the route ID
3. All stops saved in single transaction context
4. If stops creation fails, route still created (separate SaveChangesAsync)

### Updating a Route with Stops
1. Route properties updated
2. ALL existing stops for that route deleted
3. NEW stops created from request data
4. Clean replacement, not merge

### Deleting a Route
1. Route deleted
2. ALL associated stops auto-deleted due to cascade delete in FK

---

## Field Validation

### Route Fields
| Field | Required | Validation |
|-------|----------|-----------|
| name | YES | Not empty/whitespace, max 200 chars |
| status | YES | Not empty/whitespace, max 50 chars |
| driverId | YES | > 0, must exist in Drivers table |
| description | NO | Optional, max 500 chars |
| routeStops | NO | Array of stop objects, can be empty |

### RouteStop Fields
| Field | Required | Validation |
|-------|----------|-----------|
| stopName | YES | Not empty/whitespace, max 200 chars |
| arrivalTime | NO | Optional format "HH:mm:ss" |
| orderIndex | YES | >= 0, determines sort order |

---

## Testing Guide

### Quick Start Test Flow

1. **Create Route with Stops**
   ```json
   POST /api/route
   {
	 "name": "Test Route",
	 "status": "Active",
	 "driverId": 1,
	 "routeStops": [
	   {"stopName": "Stop 1", "orderIndex": 1, "arrivalTime": "08:00:00"},
	   {"stopName": "Stop 2", "orderIndex": 2, "arrivalTime": "08:30:00"}
	 ]
   }
   ```
   Expected: 201 Created with route ID

2. **Get Specific Route**
   ```
   GET /api/route/{id_from_step_1}
   ```
   Expected: 200 OK with route + 2 stops sorted by orderIndex

3. **Update Route with More Stops**
   ```json
   PUT /api/route/{id}
   {
	 "name": "Test Route",
	 "status": "Active",
	 "driverId": 1,
	 "routeStops": [
	   {"stopName": "Stop 1", "orderIndex": 1},
	   {"stopName": "Stop 2", "orderIndex": 2},
	   {"stopName": "Stop 3", "orderIndex": 3}
	 ]
   }
   ```
   Expected: 204 No Content

4. **Verify Update**
   ```
   GET /api/route/{id}
   ```
   Expected: 3 stops returned and sorted

---

## Important Notes

✅ **RouteStops array is optional** - Can be omitted or empty  
✅ **OrderIndex controls sorting** - Returned stops always sorted by this field  
✅ **Update replaces all stops** - Old stops deleted, new ones created  
✅ **Null ArrivalTime allowed** - TimeSpan is nullable  
✅ **Cascade delete works** - Deleting route deletes all stops  
✅ **Get endpoints sorted** - Both GetAll and GetById return sorted stops  
✅ **Backward compatible** - Existing clients still work (routeStops just empty array)

---

## Performance Considerations

- **Create:** 2 SaveChangesAsync calls (route, then stops)
- **Update:** 2 SaveChangesAsync calls (route, then delete+create stops)
- **Get:** Uses AsNoTracking for read performance
- **Get by ID:** Uses Include() for single query with eager loading

---

## Error Scenarios & Responses

### Missing Required Driver
```
400 Bad Request
"Driver not found."
```

### Invalid Stop Name (empty)
```
400 Bad Request
"Route stop name is required."
```

### Negative OrderIndex
```
400 Bad Request
"Route stop OrderIndex cannot be negative."
```

### Route Not Found (on update)
```
404 Not Found
(No response body)
```

---

## Files Provided for Testing

1. **ROUTE_API_TESTING.sh** - Bash script with curl examples
2. **ROUTE_API_PAYLOADS.md** - Detailed JSON payloads and documentation
3. **Route_API_Postman_Collection.json** - Import into Postman for easy testing

---

## How to Use Postman Collection

1. Open Postman
2. Click "Import" → Select `Route_API_Postman_Collection.json`
3. Collections loaded with 8 pre-configured requests
4. Update route IDs in requests as needed
5. Run requests in sequence to test full workflow

---

## Summary of Benefits

✨ **Single API Call** - Create route + stops together  
✨ **Complete Data Retrieval** - GET endpoints return full route structure  
✨ **Flexible Updates** - Can modify both route and stops atomically  
✨ **Clean Replacement** - Old stops deleted when updating (no stale data)  
✨ **Ordered Results** - Stops always returned sorted by OrderIndex  
✨ **Full Validation** - Server validates all stop data  
✨ **Type Safe** - No dynamic types, strongly typed DTOs  

---

## Next Steps

1. Verify no driver exists in your test database
2. Create test drivers first if needed
3. Use provided test files to validate API behavior
4. Update frontend code to include routeStops in requests
5. Monitor for any edge cases in production use
