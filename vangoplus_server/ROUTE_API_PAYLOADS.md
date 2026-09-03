# Route API Testing Guide - Complete JSON Payloads

## Overview
The modified Route APIs now support creating and updating route stops along with routes. When creating or updating a route, you can include a `routeStops` array in the request body.

---

## 1. CREATE ROUTE WITH ROUTE STOPS

### Endpoint
```
POST /api/route
Content-Type: application/json
```

### Request Payload
```json
{
  "name": "Downtown Express Route",
  "status": "Active",
  "driverId": 1,
  "description": "Main downtown route for morning commute",
  "routeStops": [
	{
	  "stopName": "Central Station",
	  "arrivalTime": "07:00:00",
	  "orderIndex": 1
	},
	{
	  "stopName": "Market Square",
	  "arrivalTime": "07:15:00",
	  "orderIndex": 2
	},
	{
	  "stopName": "City Hall",
	  "arrivalTime": "07:30:00",
	  "orderIndex": 3
	},
	{
	  "stopName": "University Campus",
	  "arrivalTime": "08:00:00",
	  "orderIndex": 4
	}
  ]
}
```

### Response Example (201 Created)
```json
{
  "id": 5,
  "name": "Downtown Express Route",
  "status": "Active",
  "driverId": 1,
  "description": "Main downtown route for morning commute",
  "routeStops": [
	{
	  "stopName": "Central Station",
	  "arrivalTime": "07:00:00",
	  "orderIndex": 1
	},
	{
	  "stopName": "Market Square",
	  "arrivalTime": "07:15:00",
	  "orderIndex": 2
	},
	{
	  "stopName": "City Hall",
	  "arrivalTime": "07:30:00",
	  "orderIndex": 3
	},
	{
	  "stopName": "University Campus",
	  "arrivalTime": "08:00:00",
	  "orderIndex": 4
	}
  ]
}
```

---

## 2. GET ALL ROUTES (with Route Stops)

### Endpoint
```
GET /api/route
```

### Response Example (200 OK)
```json
[
  {
	"id": 1,
	"name": "Morning Route A",
	"status": "Active",
	"driverId": 1,
	"description": "Northern part of city",
	"routeStops": [
	  {
		"stopName": "Station A",
		"arrivalTime": "06:30:00",
		"orderIndex": 1
	  },
	  {
		"stopName": "Station B",
		"arrivalTime": "06:45:00",
		"orderIndex": 2
	  }
	]
  },
  {
	"id": 2,
	"name": "Morning Route B",
	"status": "Active",
	"driverId": 2,
	"description": "Southern part of city",
	"routeStops": [
	  {
		"stopName": "Terminal C",
		"arrivalTime": "07:00:00",
		"orderIndex": 1
	  },
	  {
		"stopName": "Terminal D",
		"arrivalTime": "07:20:00",
		"orderIndex": 2
	  },
	  {
		"stopName": "Terminal E",
		"arrivalTime": "07:40:00",
		"orderIndex": 3
	  }
	]
  }
]
```

**Note:** RouteStops are automatically sorted by `orderIndex` in ascending order.

---

## 3. GET ROUTE BY ID (with Route Stops)

### Endpoint
```
GET /api/route/{id}
```

### Example Request
```
GET /api/route/1
```

### Response Example (200 OK)
```json
{
  "id": 1,
  "name": "Morning Route A",
  "status": "Active",
  "driverId": 1,
  "description": "Northern part of city",
  "routeStops": [
	{
	  "stopName": "Station A",
	  "arrivalTime": "06:30:00",
	  "orderIndex": 1
	},
	{
	  "stopName": "Station B",
	  "arrivalTime": "06:45:00",
	  "orderIndex": 2
	},
	{
	  "stopName": "Station C",
	  "arrivalTime": "07:00:00",
	  "orderIndex": 3
	}
  ]
}
```

### Response if Not Found (404 Not Found)
```
(Empty response body with 404 status)
```

---

## 4. UPDATE ROUTE WITH NEW ROUTE STOPS

### Endpoint
```
PUT /api/route/{id}
Content-Type: application/json
```

### Important Notes
- **All existing route stops for this route will be deleted and replaced**
- OrderIndex determines the sequence (sorted automatically)
- The Id field in request body is optional but should match the route ID in URL

### Request Payload
```json
{
  "id": 1,
  "name": "Downtown Express Route - Updated",
  "status": "Active",
  "driverId": 1,
  "description": "Updated main downtown route",
  "routeStops": [
	{
	  "stopName": "Central Station",
	  "arrivalTime": "07:00:00",
	  "orderIndex": 1
	},
	{
	  "stopName": "Market Square",
	  "arrivalTime": "07:15:00",
	  "orderIndex": 2
	},
	{
	  "stopName": "New Park Location",
	  "arrivalTime": "07:45:00",
	  "orderIndex": 3
	},
	{
	  "stopName": "City Hall",
	  "arrivalTime": "08:00:00",
	  "orderIndex": 4
	},
	{
	  "stopName": "University Campus",
	  "arrivalTime": "08:30:00",
	  "orderIndex": 5
	}
  ]
}
```

### Response (204 No Content)
```
(No response body)
```

### Error Response Example (404 Not Found)
```
(No response body with 404 status)
```

---

## 5. UPDATE ROUTE WITH EMPTY ROUTE STOPS (Remove all stops)

### Endpoint
```
PUT /api/route/{id}
Content-Type: application/json
```

### Request Payload
```json
{
  "id": 1,
  "name": "Downtown Express Route - No Stops",
  "status": "Active",
  "driverId": 1,
  "description": "Route with no stops",
  "routeStops": []
}
```

**Result:** All route stops for this route will be deleted from database.

---

## 6. UPDATE ROUTE STATUS ONLY

### Endpoint
```
PATCH /api/route/{id}/status
Content-Type: application/json
```

### Request Payload
```json
{
  "status": "Inactive"
}
```

### Valid Status Values
- "Active"
- "Inactive"
- Any other string (validation is minimal)

### Response (204 No Content)
```
(No response body)
```

---

## 7. DELETE ROUTE

### Endpoint
```
DELETE /api/route/{id}
```

### Response (204 No Content)
```
(No response body)
```

**Important:** Deleting a route also deletes all associated route stops due to cascade delete constraint in database.

---

## Field Validation Rules

### RouteDto (Route)
| Field | Type | Required | Min | Max | Notes |
|-------|------|----------|-----|-----|-------|
| id | int | No (auto-generated) | - | - | Used in updates/responses only |
| name | string | **Yes** | 1 | 200 | Cannot be empty or whitespace |
| status | string | **Yes** | 1 | 50 | Cannot be empty or whitespace |
| driverId | int | **Yes** | 1 | - | Must exist in Drivers table |
| description | string | No | - | 500 | Optional, can be null |
| routeStops | array | No | 0 | - | Can be empty array or omitted |

### RouteStopInputDto (Route Stop)
| Field | Type | Required | Min | Max | Notes |
|-------|------|----------|-----|-----|-------|
| stopName | string | **Yes** | 1 | 200 | Cannot be empty or whitespace |
| arrivalTime | TimeSpan? | No | - | - | Optional, format: "HH:mm:ss" |
| orderIndex | int | **Yes** | 0 | - | 0 or higher, determines sort order |

---

## Error Scenarios

### 1. Invalid DriverId (doesn't exist)
```
Status: 400 Bad Request or 500 Internal Server Error
Response: Error message about driver not found
```

### 2. Empty Required Field
```
Status: 400 Bad Request
Response: Validation error message
```

### 3. Negative OrderIndex for Route Stop
```
Status: 400 Bad Request
Response: "Route stop OrderIndex cannot be negative."
```

### 4. Empty Stop Name
```
Status: 400 Bad Request
Response: "Route stop name is required."
```

### 5. Route Not Found for Update
```
Status: 404 Not Found
Response: (No body)
```

---

## Testing Sequence Recommendations

1. **First, verify drivers exist:**
   - GET /api/drivers or create test drivers

2. **Create simple route without stops:**
   ```json
   {
	 "name": "Test Route",
	 "status": "Active",
	 "driverId": 1,
	 "routeStops": []
   }
   ```

3. **Get the created route (note the ID)**

4. **Create route with stops:**
   - Use the ID from step 3 or create new

5. **Update route with different stops**

6. **Verify stops are sorted by orderIndex**

7. **Update to empty stops array and verify deletion**

8. **Delete the route**

---

## Example: Complete Flow

### Step 1: Create Route with Stops
```bash
curl -X POST http://localhost:5000/api/route \
  -H "Content-Type: application/json" \
  -d '{
	"name": "Route 1",
	"status": "Active",
	"driverId": 1,
	"routeStops": [
	  {"stopName": "Stop A", "orderIndex": 1, "arrivalTime": "08:00:00"},
	  {"stopName": "Stop B", "orderIndex": 2, "arrivalTime": "08:30:00"}
	]
  }'
```
Response: `201 Created` with route ID (e.g., 5)

### Step 2: Get the Route and Verify Stops
```bash
curl -X GET http://localhost:5000/api/route/5
```
Response: `200 OK` with route and stops sorted by orderIndex

### Step 3: Update with More Stops
```bash
curl -X PUT http://localhost:5000/api/route/5 \
  -H "Content-Type: application/json" \
  -d '{
	"id": 5,
	"name": "Route 1 Updated",
	"status": "Active",
	"driverId": 1,
	"routeStops": [
	  {"stopName": "Stop A", "orderIndex": 1},
	  {"stopName": "Stop B", "orderIndex": 2},
	  {"stopName": "Stop C", "orderIndex": 3},
	  {"stopName": "Stop D", "orderIndex": 4}
	]
  }'
```
Response: `204 No Content`

### Step 4: Verify Update
```bash
curl -X GET http://localhost:5000/api/route/5
```
Response: Route with 4 stops sorted by orderIndex

### Step 5: Delete Route and Stops
```bash
curl -X DELETE http://localhost:5000/api/route/5
```
Response: `204 No Content` (all stops also deleted)

---

## Summary of Changes

✅ **RouteDto** now includes `routeStops: List<RouteStopInputDto>`  
✅ **RouteStopInputDto** created for input without RouteId  
✅ **CreateAsync** - Creates route and route stops in single operation  
✅ **UpdateAsync** - Replaces all route stops when updating  
✅ **GetAllAsync** - Returns routes with stops (sorted by orderIndex)  
✅ **GetByIdAsync** - Returns single route with all stops (sorted by orderIndex)  
✅ **Handler validation** - Validates stop names and orderIndex  
✅ **Cascade delete** - Deleting route also deletes associated stops
