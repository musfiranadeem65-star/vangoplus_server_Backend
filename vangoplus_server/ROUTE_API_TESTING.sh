#!/bin/bash
# Route API Testing with Route Stops
# Base URL: http://localhost:5000

# ============================================================================
# 1. CREATE ROUTE WITH ROUTE STOPS
# ============================================================================
echo "1. Creating a new route with route stops..."

curl -X POST http://localhost:5000/api/route \
  -H "Content-Type: application/json" \
  -d '{
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
}'

echo -e "\n\n"

# ============================================================================
# 2. GET ALL ROUTES (with route stops included)
# ============================================================================
echo "2. Get all routes with their route stops..."

curl -X GET http://localhost:5000/api/route \
  -H "Content-Type: application/json"

echo -e "\n\n"

# ============================================================================
# 3. GET ROUTE BY ID (with route stops included)
# ============================================================================
echo "3. Get a specific route by ID (replace ID with actual route ID from create response)..."

curl -X GET http://localhost:5000/api/route/1 \
  -H "Content-Type: application/json"

echo -e "\n\n"

# ============================================================================
# 4. UPDATE ROUTE WITH NEW ROUTE STOPS (replaces existing stops)
# ============================================================================
echo "4. Update route with new/modified route stops..."

curl -X PUT http://localhost:5000/api/route/1 \
  -H "Content-Type: application/json" \
  -d '{
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
}'

echo -e "\n\n"

# ============================================================================
# 5. UPDATE ROUTE STATUS ONLY (route stops unchanged)
# ============================================================================
echo "5. Update route status only..."

curl -X PATCH http://localhost:5000/api/route/1/status \
  -H "Content-Type: application/json" \
  -d '{
  "status": "Inactive"
}'

echo -e "\n\n"

# ============================================================================
# 6. DELETE ROUTE (also deletes associated route stops due to cascade)
# ============================================================================
echo "6. Delete a route (and its route stops)..."

curl -X DELETE http://localhost:5000/api/route/1 \
  -H "Content-Type: application/json"

echo -e "\n\n"

# ============================================================================
# ALTERNATIVE: Create route without route stops (empty array)
# ============================================================================
echo "7. Create route without any route stops initially..."

curl -X POST http://localhost:5000/api/route \
  -H "Content-Type: application/json" \
  -d '{
  "name": "Evening Route",
  "status": "Active",
  "driverId": 2,
  "description": "Evening commute route",
  "routeStops": []
}'

echo -e "\n\n"

# ============================================================================
# NOTES FOR TESTING
# ============================================================================
# - Replace driverId with actual driver IDs from your database
# - OrderIndex must be >= 0 and determines the sequence of stops
# - RouteStops array can be empty
# - When updating, all route stops are replaced (old ones deleted, new ones created)
# - ArrivalTime is optional (can be null)
# - Use OrderIndex to control the order stops appear in responses (auto-sorted)
