# 🚀 Route APIs Quick Reference - Copy & Paste Ready

## Base URL
```
http://localhost:5000
```

---

## 1️⃣ CREATE ROUTE WITH STOPS

```bash
curl -X POST http://localhost:5000/api/route \
  -H "Content-Type: application/json" \
  -d '{
  "name": "Morning Route A",
  "status": "Active",
  "driverId": 1,
  "description": "Northern district",
  "routeStops": [
	{"stopName": "Central Hub", "orderIndex": 1, "arrivalTime": "07:00:00"},
	{"stopName": "Downtown Stop", "orderIndex": 2, "arrivalTime": "07:20:00"},
	{"stopName": "Mall Stop", "orderIndex": 3, "arrivalTime": "07:40:00"}
  ]
}'
```

**Response:** `201 Created`
```json
{
  "id": 5,
  "name": "Morning Route A",
  "status": "Active",
  "driverId": 1,
  "description": "Northern district",
  "routeStops": [
	{"stopName": "Central Hub", "orderIndex": 1, "arrivalTime": "07:00:00"},
	{"stopName": "Downtown Stop", "orderIndex": 2, "arrivalTime": "07:20:00"},
	{"stopName": "Mall Stop", "orderIndex": 3, "arrivalTime": "07:40:00"}
  ]
}
```

---

## 2️⃣ GET ALL ROUTES (with stops)

```bash
curl -X GET http://localhost:5000/api/route
```

**Response:** `200 OK`
```json
[
  {
	"id": 1,
	"name": "Route A",
	"status": "Active",
	"driverId": 1,
	"routeStops": [
	  {"stopName": "Stop 1", "orderIndex": 1},
	  {"stopName": "Stop 2", "orderIndex": 2}
	]
  },
  {
	"id": 2,
	"name": "Route B",
	"status": "Active",
	"driverId": 2,
	"routeStops": []
  }
]
```

---

## 3️⃣ GET ROUTE BY ID (with stops)

```bash
curl -X GET http://localhost:5000/api/route/1
```

**Response:** `200 OK`
```json
{
  "id": 1,
  "name": "Morning Route",
  "status": "Active",
  "driverId": 1,
  "description": "Main route",
  "routeStops": [
	{"stopName": "Station A", "orderIndex": 1, "arrivalTime": "06:30:00"},
	{"stopName": "Station B", "orderIndex": 2, "arrivalTime": "07:00:00"},
	{"stopName": "Station C", "orderIndex": 3, "arrivalTime": "07:30:00"}
  ]
}
```

---

## 4️⃣ UPDATE ROUTE + REPLACE ALL STOPS

```bash
curl -X PUT http://localhost:5000/api/route/1 \
  -H "Content-Type: application/json" \
  -d '{
  "id": 1,
  "name": "Morning Route - Updated",
  "status": "Active",
  "driverId": 1,
  "description": "Updated route",
  "routeStops": [
	{"stopName": "New Stop 1", "orderIndex": 1, "arrivalTime": "07:00:00"},
	{"stopName": "New Stop 2", "orderIndex": 2, "arrivalTime": "07:30:00"},
	{"stopName": "New Stop 3", "orderIndex": 3, "arrivalTime": "08:00:00"},
	{"stopName": "New Stop 4", "orderIndex": 4, "arrivalTime": "08:30:00"}
  ]
}'
```

**Response:** `204 No Content`

✅ **Old stops deleted, new stops created**

---

## 5️⃣ REMOVE ALL STOPS FROM ROUTE

```bash
curl -X PUT http://localhost:5000/api/route/1 \
  -H "Content-Type: application/json" \
  -d '{
  "id": 1,
  "name": "Morning Route",
  "status": "Active",
  "driverId": 1,
  "routeStops": []
}'
```

**Response:** `204 No Content`

✅ **All route stops deleted**

---

## 6️⃣ UPDATE STATUS ONLY

```bash
curl -X PATCH http://localhost:5000/api/route/1/status \
  -H "Content-Type: application/json" \
  -d '{"status": "Inactive"}'
```

**Response:** `204 No Content`

✅ **Stops unchanged, only status updated**

---

## 7️⃣ DELETE ROUTE (+ all stops)

```bash
curl -X DELETE http://localhost:5000/api/route/1
```

**Response:** `204 No Content`

✅ **Route and all associated stops deleted**

---

## 📋 Payload Structure

### RouteDto (Full Payload)
```json
{
  "id": 0,                          // Auto-generated on create, required on update
  "name": "string",                 // Required, max 200 chars
  "status": "string",               // Required, max 50 chars  
  "driverId": 0,                    // Required, must exist
  "description": "string",          // Optional, max 500 chars
  "routeStops": [                   // Optional, can be empty or omitted
	{
	  "stopName": "string",         // Required, max 200 chars
	  "arrivalTime": "HH:mm:ss",   // Optional, TimeSpan format
	  "orderIndex": 0               // Required, >= 0
	}
  ]
}
```

---

## ✅ Validation Rules

| Field | Rule |
|-------|------|
| name | Not empty, max 200 chars |
| status | Not empty, max 50 chars |
| driverId | > 0, must exist in DB |
| description | Max 500 chars |
| stopName | Not empty, max 200 chars |
| orderIndex | >= 0 |
| arrivalTime | Optional, format "HH:mm:ss" |

---

## 🔴 Common Errors

### "Driver not found"
- Driver ID doesn't exist
- **Fix:** Use valid driverId from Drivers table

### "Route stop name is required"
- Stop entry has empty stopName
- **Fix:** All stops must have non-empty stopName

### "Route stop OrderIndex cannot be negative"
- OrderIndex < 0
- **Fix:** Use 0 or positive numbers

### 404 Not Found
- Route doesn't exist
- **Fix:** Use valid route ID

---

## 📊 OrderIndex Behavior

Stops are **automatically sorted by OrderIndex** in responses:

Input (random order):
```json
"routeStops": [
  {"stopName": "C", "orderIndex": 3},
  {"stopName": "A", "orderIndex": 1},
  {"stopName": "B", "orderIndex": 2}
]
```

Response (sorted):
```json
"routeStops": [
  {"stopName": "A", "orderIndex": 1},
  {"stopName": "B", "orderIndex": 2},
  {"stopName": "C", "orderIndex": 3}
]
```

---

## 🧪 Test Sequence

**1. Create Route with Stops**
```bash
curl -X POST http://localhost:5000/api/route \
  -H "Content-Type: application/json" \
  -d '{"name":"Test","status":"Active","driverId":1,"routeStops":[{"stopName":"Stop1","orderIndex":1}]}'
```
Copy ID from response → Save as `ROUTE_ID`

**2. Verify with GET**
```bash
curl -X GET http://localhost:5000/api/route/{ROUTE_ID}
```

**3. Update with More Stops**
```bash
curl -X PUT http://localhost:5000/api/route/{ROUTE_ID} \
  -H "Content-Type: application/json" \
  -d '{"id":{ROUTE_ID},"name":"Test","status":"Active","driverId":1,"routeStops":[{"stopName":"Stop1","orderIndex":1},{"stopName":"Stop2","orderIndex":2}]}'
```

**4. Verify Again**
```bash
curl -X GET http://localhost:5000/api/route/{ROUTE_ID}
```

**5. Delete**
```bash
curl -X DELETE http://localhost:5000/api/route/{ROUTE_ID}
```

---

## 💡 Pro Tips

✨ **Minimal Create**
```json
{
  "name": "Route",
  "status": "Active", 
  "driverId": 1,
  "routeStops": []
}
```

✨ **Full Create with Stops**
```json
{
  "name": "Morning Route",
  "status": "Active",
  "driverId": 1,
  "description": "Peak hours route",
  "routeStops": [
	{"stopName": "Station", "orderIndex": 1, "arrivalTime": "07:00:00"},
	{"stopName": "Hub", "orderIndex": 2, "arrivalTime": "07:30:00"}
  ]
}
```

✨ **Update = Replace Stops**
Sending new routeStops array completely replaces old ones

✨ **Status Update = Preserve Stops**
PATCH to /status endpoint doesn't affect stops

---

## 📌 Key Points

✅ All route stops **MUST** have stopName (required)  
✅ All route stops **MUST** have orderIndex (required) >= 0  
✅ ArrivalTime is **optional** (can be null)  
✅ RouteStops array is **optional** (can be empty or omitted)  
✅ Update **REPLACES** all stops (not merge)  
✅ Stops always **SORTED by orderIndex** in responses  
✅ Delete route **CASCADES** to all stops  

---

## 📁 Files for Reference

- `ROUTE_API_TESTING.sh` - Bash/curl test script
- `ROUTE_API_PAYLOADS.md` - Detailed documentation
- `Route_API_Postman_Collection.json` - Postman import
- `ROUTE_STOPS_INTEGRATION.md` - Full integration guide
