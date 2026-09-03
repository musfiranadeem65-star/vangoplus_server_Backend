# VanGo Plus Backend - Complete API Documentation

## Table of Contents
1. [Project Overview](#project-overview)
2. [Technology Stack](#technology-stack)
3. [Folder Structure](#folder-structure)
4. [Database Relationships](#database-relationships)
5. [API Summary Table](#api-summary-table)
6. [API Documentation](#api-documentation)
7. [Error Handling & Validation](#error-handling--validation)
8. [Testing Guide](#testing-guide)

---

## Project Overview

**VanGo Plus Backend** is a comprehensive ASP.NET Core 10 Web API designed for managing student transportation, subscriptions, and communication systems. The platform enables:

- **User Management**: Registration and profile management for parents and administrators
- **Student Management**: Track students with parent associations
- **Guardian Management**: Manage emergency contacts and guardians for students
- **Transportation Management**: Manage drivers, routes, and route stops
- **Subscription Management**: Handle subscription plans and user subscriptions
- **Alert System**: Send and manage notifications for students

The backend follows a clean architecture with separation of concerns (Controllers → Handlers → Services → Database), ensuring maintainability, testability, and scalability.

---

## Technology Stack

- **Framework**: ASP.NET Core 10
- **Language**: C# (.NET 10)
- **Database**: PostgreSQL
- **ORM**: Entity Framework Core
- **API Pattern**: REST API
- **Authentication**: Supports JWT integration (future enhancement)
- **Architecture Pattern**: Service + Handler + Repository Pattern

---

## Folder Structure

### `/Controllers`
HTTP endpoint handlers that manage incoming requests and route them to appropriate handlers:
- Receives HTTP requests from clients
- Validates request format and content type
- Delegates business logic to Handlers
- Returns HTTP responses with appropriate status codes

### `/Application/DTOs` (Data Transfer Objects)
Plain C# classes that define the shape of data transferred between client and server:
- Used for all API request and response payloads
- Decouples domain entities from API contracts
- Enables frontend and backend to work independently

### `/Application/Interfaces`
Service and Handler interface contracts:
- `IXxxService`: Defines database operation methods (GetAll, GetById, Create, Update, Delete)
- `IXxxHandler`: Defines validation and business logic methods
- Enables dependency injection and loose coupling

### `/Application/Handlers`
Processing layer that validates business rules and requirements:
- Validates input data before processing
- Checks foreign key relationships and business constraints
- Throws appropriate exceptions with descriptive messages
- Delegates validated data to Services for database operations

### `/Application/Services`
Data access and database operation layer:
- Interacts directly with Entity Framework DbContext
- Performs CRUD operations on database
- Maps entities to DTOs for API responses
- Uses async/await for non-blocking operations

### `/Domain/Entities`
Core business objects representing database tables:
- Define relationships using navigation properties
- Mapped to database through Entity Framework
- Include primary keys, foreign keys, and constraints

### `/Infrastructure/Data`
Database context and configuration:
- `AppDbContext`: DbContext for database operations
- `OnModelCreating`: Fluent API configurations for entities
- Foreign key relationships and constraints
- Database connection management

---

## Database Relationships

### Entity Relationship Diagram (Summary)

```
User (1) ───────────┬──────────────── (Many) Student
					│
					└──────────────── (Many) Subscription

Guardian ─────────────────────────────── (Many) StudentGuardian (Join Table)
										 │
Student ──────────────────────────────────┘

Student (1) ────────────────────────── (Many) Alert

Driver (1) ────────────────────────── (Many) Route

Route (1) ────────────────────────── (Many) RouteStop

SubscriptionPlan (1) ────────────── (Many) Subscription

Subscription ──────────┬────────────── User
					   └────────────── SubscriptionPlan
```

### Detailed Relationships

| Entity | Relationship | Type | Notes |
|--------|--------------|------|-------|
| User → Student | ParentUserId | 1:Many | Cascade Delete |
| User → Subscription | UserId | 1:Many | Cascade Delete |
| Student → Guardian | StudentGuardian (Join) | M:Many | Cascade Delete |
| Student → Alert | StudentId | 1:Many | Cascade Delete |
| Driver → Route | DriverId | 1:Many | Cascade Delete |
| Route → RouteStop | RouteId | 1:Many | Cascade Delete |
| SubscriptionPlan → Subscription | PlanId | 1:Many | Cascade Delete |

---

## API Summary Table

| # | Module | Method | Endpoint | Description |
|---|--------|--------|----------|-------------|
| 1 | User | GET | `/api/users` | Get all users |
| 2 | User | GET | `/api/users/{id}` | Get user by ID |
| 3 | User | POST | `/api/users` | Create new user |
| 4 | User | PUT | `/api/users/{id}` | Update user |
| 5 | User | DELETE | `/api/users/{id}` | Delete user |
| 6 | Student | GET | `/api/students` | Get all students |
| 7 | Student | GET | `/api/students/{id}` | Get student by ID |
| 8 | Student | GET | `/api/students/parent/{parentId}` | Get students by parent |
| 9 | Student | POST | `/api/students` | Create new student |
| 10 | Student | PUT | `/api/students/{id}` | Update student |
| 11 | Student | DELETE | `/api/students/{id}` | Delete student |
| 12 | Guardian | POST | `/api/Guardian` | Create new guardian |
| 13 | Guardian | GET | `/api/students/{studentId}/guardians` | Get guardians by student |
| 14 | Guardian | POST | `/api/students/{studentId}/guardians` | Link guardian to student |
| 15 | Guardian | PUT | `/api/students/{studentId}/guardians/{id}` | Update guardian |
| 16 | Guardian | PATCH | `/api/students/{studentId}/guardians/{id}/status` | Update guardian status |
| 17 | Guardian | DELETE | `/api/students/{studentId}/guardians/{id}` | Delete guardian |
| 18 | Driver | GET | `/api/drivers` | Get all drivers |
| 19 | Driver | GET | `/api/drivers/{id}` | Get driver by ID |
| 20 | Driver | POST | `/api/drivers` | Create new driver |
| 21 | Driver | PUT | `/api/drivers/{id}` | Update driver |
| 22 | Driver | DELETE | `/api/drivers/{id}` | Delete driver |
| 23 | Driver | PATCH | `/api/drivers/{id}/status` | Update driver status |
| 24 | Route | GET | `/api/routes` | Get all routes |
| 25 | Route | GET | `/api/routes/{id}` | Get route by ID |
| 26 | Route | POST | `/api/routes` | Create new route |
| 27 | Route | PUT | `/api/routes/{id}` | Update route |
| 28 | Route | PATCH | `/api/routes/{id}/status` | Update route status |
| 29 | Route | DELETE | `/api/routes/{id}` | Delete route |
| 30 | RouteStop | GET | `/api/routes/{routeId}/stops` | Get stops by route |
| 31 | RouteStop | POST | `/api/routes/{routeId}/stops` | Create route stop |
| 32 | RouteStop | GET | `/api/routes/stops/{id}` | Get route stop by ID |
| 33 | RouteStop | PUT | `/api/routes/stops/{id}` | Update route stop |
| 34 | RouteStop | DELETE | `/api/routes/stops/{id}` | Delete route stop |
| 35 | SubscriptionPlan | GET | `/api/subscription/plans` | Get all plans |
| 36 | SubscriptionPlan | GET | `/api/subscription/plans/{id}` | Get plan by ID |
| 37 | SubscriptionPlan | POST | `/api/subscription/plans` | Create plan |
| 38 | SubscriptionPlan | PUT | `/api/subscription/plans/{id}` | Update plan |
| 39 | SubscriptionPlan | DELETE | `/api/subscription/plans/{id}` | Delete plan |
| 40 | Subscription | GET | `/api/subscriptions/users/{userId}/subscription` | Get user subscription |
| 41 | Subscription | GET | `/api/subscriptions/{id}` | Get subscription by ID |
| 42 | Subscription | POST | `/api/subscriptions` | Create subscription |
| 43 | Subscription | PUT | `/api/subscriptions/{id}` | Update subscription |
| 44 | Subscription | PATCH | `/api/subscriptions/{id}/status` | Update subscription status |
| 45 | Alert | GET | `/api/students/{studentId}/alerts` | Get alerts by student |
| 46 | Alert | GET | `/api/alerts/{id}` | Get alert by ID |
| 47 | Alert | POST | `/api/alerts` | Create alert |
| 48 | Alert | PATCH | `/api/alerts/{id}/read` | Mark alert as read |
| 49 | Alert | GET | `/api/students/{studentId}/schedule` | Get student schedule |

---

## API Documentation

### USER MODULE

#### 1. Get All Users

**Method**: `GET`

**URL**: `/api/users`

**Description**: Retrieves a list of all registered users in the system.

**Request Body**: None

**Response**:
```json
[
  {
	"id": 1,
	"name": "John Doe",
	"email": "john@example.com",
	"phone": "123-456-7890",
	"city": "New York",
	"role": "Parent",
	"status": "Active"
  },
  {
	"id": 2,
	"name": "Jane Smith",
	"email": "jane@example.com",
	"phone": "098-765-4321",
	"city": "Los Angeles",
	"role": "Admin",
	"status": "Active"
  }
]
```

**Status Codes**:
- `200 OK` - Users retrieved successfully

**Validation Rules**: None

**Relationships**: None

---

#### 2. Get User by ID

**Method**: `GET`

**URL**: `/api/users/{id}`

**Description**: Retrieves a specific user by their ID.

**Request Body**: None

**Response**:
```json
{
  "id": 1,
  "name": "John Doe",
  "email": "john@example.com",
  "phone": "123-456-7890",
  "city": "New York",
  "role": "Parent",
  "status": "Active"
}
```

**Status Codes**:
- `200 OK` - User found and returned
- `404 Not Found` - User with specified ID does not exist

**Validation Rules**: None

**Relationships**: None

---

#### 3. Create User

**Method**: `POST`

**URL**: `/api/users`

**Description**: Creates a new user account with email and password authentication.

**Request Body**:
```json
{
  "name": "John Doe",
  "email": "john@example.com",
  "password": "SecurePassword123",
  "phone": "123-456-7890",
  "city": "New York",
  "role": "Parent",
  "status": "Active"
}
```

**Response**:
```json
{
  "id": 1,
  "name": "John Doe",
  "email": "john@example.com",
  "phone": "123-456-7890",
  "city": "New York",
  "role": "Parent",
  "status": "Active"
}
```

**Status Codes**:
- `201 Created` - User created successfully
- `400 Bad Request` - Invalid input or validation error

**Validation Rules**:
- `Email` - Required, must be unique, valid email format
- `Password` - Required, minimum 6 characters
- Other fields - Optional

**Relationships**: None

---

#### 4. Update User

**Method**: `PUT`

**URL**: `/api/users/{id}`

**Description**: Updates an existing user's profile information.

**Request Body**:
```json
{
  "id": 1,
  "name": "John Updated",
  "email": "john.updated@example.com",
  "phone": "123-456-7891",
  "city": "Boston",
  "role": "Parent",
  "status": "Active"
}
```

**Response**: Empty body with 204 status

**Status Codes**:
- `204 No Content` - User updated successfully
- `400 Bad Request` - Invalid input or validation error

**Validation Rules**:
- `Email` - Required, must be valid email format
- Other fields - Optional

**Relationships**: None

---

#### 5. Delete User

**Method**: `DELETE`

**URL**: `/api/users/{id}`

**Description**: Deletes a user account and all associated data (students, subscriptions).

**Request Body**: None

**Response**: Empty body with 204 status

**Status Codes**:
- `204 No Content` - User deleted successfully

**Validation Rules**: None

**Relationships**: 
- User (1) → Student (Many) - Cascade Delete
- User (1) → Subscription (Many) - Cascade Delete

---

### STUDENT MODULE

#### 6. Get All Students

**Method**: `GET`

**URL**: `/api/students`

**Description**: Retrieves a list of all students in the system.

**Request Body**: None

**Response**:
```json
[
  {
	"id": 1,
	"parentUserId": 1,
	"name": "Alice Doe",
	"grade": "10",
	"section": "A",
	"status": "Active"
  },
  {
	"id": 2,
	"parentUserId": 1,
	"name": "Bob Doe",
	"grade": "9",
	"section": "B",
	"status": "Active"
  }
]
```

**Status Codes**:
- `200 OK` - Students retrieved successfully

**Validation Rules**: None

**Relationships**: None

---

#### 7. Get Student by ID

**Method**: `GET`

**URL**: `/api/students/{id}`

**Description**: Retrieves a specific student by their ID.

**Request Body**: None

**Response**:
```json
{
  "id": 1,
  "parentUserId": 1,
  "name": "Alice Doe",
  "grade": "10",
  "section": "A",
  "status": "Active"
}
```

**Status Codes**:
- `200 OK` - Student found and returned
- `404 Not Found` - Student does not exist

**Validation Rules**: None

**Relationships**: None

---

#### 8. Get Students by Parent

**Method**: `GET`

**URL**: `/api/students/parent/{parentId}`

**Description**: Retrieves all students belonging to a specific parent/guardian.

**Request Body**: None

**Response**:
```json
[
  {
	"id": 1,
	"parentUserId": 5,
	"name": "Alice Smith",
	"grade": "10",
	"section": "A",
	"status": "Active"
  },
  {
	"id": 3,
	"parentUserId": 5,
	"name": "Charlie Smith",
	"grade": "8",
	"section": "C",
	"status": "Active"
  }
]
```

**Status Codes**:
- `200 OK` - Students retrieved successfully
- `404 Not Found` - Parent not found

**Validation Rules**: None

**Relationships**: None

---

#### 9. Create Student

**Method**: `POST`

**URL**: `/api/students`

**Description**: Creates a new student record and associates with a parent user.

**Request Body**:
```json
{
  "parentUserId": 1,
  "name": "Alice Doe",
  "grade": "10",
  "section": "A",
  "status": "Active"
}
```

**Response**:
```json
{
  "id": 1,
  "parentUserId": 1,
  "name": "Alice Doe",
  "grade": "10",
  "section": "A",
  "status": "Active"
}
```

**Status Codes**:
- `201 Created` - Student created successfully
- `400 Bad Request` - Invalid input or validation error
- `404 Not Found` - Parent user not found

**Validation Rules**:
- `Name` - Required, non-empty string
- `Grade` - Required, non-empty string
- `ParentUserId` - Required, must be > 0, must reference existing User
- `Status` - Required, non-empty string

**Relationships**:
- `ParentUserId` → Users (FK) - Parent must exist

---

#### 10. Update Student

**Method**: `PUT`

**URL**: `/api/students/{id}`

**Description**: Updates an existing student's information.

**Request Body**:
```json
{
  "parentUserId": 1,
  "name": "Alice Updated",
  "grade": "11",
  "section": "A",
  "status": "Active"
}
```

**Response**: Empty body with 204 status

**Status Codes**:
- `204 No Content` - Student updated successfully
- `400 Bad Request` - Invalid input or validation error

**Validation Rules**:
- `Name` - Required, non-empty string
- `Grade` - Required, non-empty string
- `ParentUserId` - Required, must be > 0
- `Status` - Required, non-empty string

**Relationships**: None

---

#### 11. Delete Student

**Method**: `DELETE`

**URL**: `/api/students/{id}`

**Description**: Deletes a student record and all associated alerts.

**Request Body**: None

**Response**: Empty body with 204 status

**Status Codes**:
- `204 No Content` - Student deleted successfully

**Validation Rules**: None

**Relationships**:
- Student (1) → Alert (Many) - Cascade Delete
- Student (1) → StudentGuardian (Many) - Cascade Delete

---

### GUARDIAN MODULE

#### 12. Create Guardian

**Method**: `POST`

**URL**: `/api/Guardian`

**Description**: Creates a new guardian record for emergency contacts.

**Request Body**:
```json
{
  "userId": 1,
  "name": "Mary Doe",
  "relation": "Aunt",
  "phone": "555-1234",
  "status": "Active",
  "note": "Prefers morning calls"
}
```

**Response**:
```json
{
  "id": 1,
  "userId": 1,
  "name": "Mary Doe",
  "relation": "Aunt",
  "phone": "555-1234",
  "status": "Active",
  "note": "Prefers morning calls"
}
```

**Status Codes**:
- `201 Created` - Guardian created successfully
- `400 Bad Request` - Invalid input or validation error

**Validation Rules**:
- `Name` - Required, non-empty string
- `Relation` - Required, non-empty string
- `UserId` - Required, must be > 0

**Relationships**:
- `UserId` → Users (FK) - User must exist

---

#### 13. Get Guardians by Student

**Method**: `GET`

**URL**: `/api/students/{studentId}/guardians`

**Description**: Retrieves all guardians associated with a specific student.

**Request Body**: None

**Response**:
```json
[
  {
	"id": 1,
	"userId": 2,
	"name": "Mary Doe",
	"relation": "Aunt",
	"phone": "555-1234",
	"status": "Active",
	"note": "Prefers morning calls"
  },
  {
	"id": 2,
	"userId": 3,
	"name": "John Uncle",
	"relation": "Uncle",
	"phone": "555-5678",
	"status": "Active",
	"note": null
  }
]
```

**Status Codes**:
- `200 OK` - Guardians retrieved successfully

**Validation Rules**: None

**Relationships**: None

---

#### 14. Link Guardian to Student

**Method**: `POST`

**URL**: `/api/students/{studentId}/guardians`

**Description**: Associates an existing guardian with a student using the StudentGuardian join table.

**Request Body**:
```json
{
  "guardianId": 1,
  "isPrimary": true,
  "status": "Active"
}
```

**Response**: Created with location header

**Status Codes**:
- `201 Created` - Guardian linked to student successfully
- `400 Bad Request` - Invalid input or validation error

**Validation Rules**:
- `GuardianId` - Required, must be > 0, must reference existing Guardian
- `Status` - Required, non-empty string

**Relationships**:
- StudentGuardian (Join Table) → Student (FK)
- StudentGuardian (Join Table) → Guardian (FK)

---

#### 15. Update Guardian

**Method**: `PUT`

**URL**: `/api/students/{studentId}/guardians/{id}`

**Description**: Updates an existing guardian's information.

**Request Body**:
```json
{
  "userId": 1,
  "name": "Mary Updated",
  "relation": "Aunt",
  "phone": "555-9999",
  "status": "Active",
  "note": "Updated notes"
}
```

**Response**: Empty body with 204 status

**Status Codes**:
- `204 No Content` - Guardian updated successfully
- `400 Bad Request` - Invalid input or validation error

**Validation Rules**:
- `Name` - Required, non-empty string
- `Relation` - Required, non-empty string

**Relationships**: None

---

#### 16. Update Guardian Status

**Method**: `PATCH`

**URL**: `/api/students/{studentId}/guardians/{id}/status`

**Description**: Updates only the status of a guardian (Active/Inactive).

**Request Body**:
```json
{
  "status": "Inactive"
}
```

**Response**: Empty body with 204 status

**Status Codes**:
- `204 No Content` - Guardian status updated successfully
- `400 Bad Request` - Invalid status value

**Validation Rules**:
- `Status` - Required, non-empty string

**Relationships**: None

---

#### 17. Delete Guardian

**Method**: `DELETE`

**URL**: `/api/students/{studentId}/guardians/{id}`

**Description**: Deletes a guardian record.

**Request Body**: None

**Response**: Empty body with 204 status

**Status Codes**:
- `204 No Content` - Guardian deleted successfully

**Validation Rules**: None

**Relationships**:
- Guardian (1) → StudentGuardian (Many) - Cascade Delete

---

### DRIVER MODULE

#### 18. Get All Drivers

**Method**: `GET`

**URL**: `/api/drivers`

**Description**: Retrieves a list of all drivers.

**Request Body**: None

**Response**:
```json
[
  {
	"id": 1,
	"name": "James Wilson",
	"phone": "987-654-3210",
	"email": "james@example.com",
	"licenseNo": "DL123456",
	"status": "Active"
  },
  {
	"id": 2,
	"name": "Patricia Brown",
	"phone": "987-654-3211",
	"email": "patricia@example.com",
	"licenseNo": "DL123457",
	"status": "Inactive"
  }
]
```

**Status Codes**:
- `200 OK` - Drivers retrieved successfully

**Validation Rules**: None

**Relationships**: None

---

#### 19. Get Driver by ID

**Method**: `GET`

**URL**: `/api/drivers/{id}`

**Description**: Retrieves a specific driver by their ID.

**Request Body**: None

**Response**:
```json
{
  "id": 1,
  "name": "James Wilson",
  "phone": "987-654-3210",
  "email": "james@example.com",
  "licenseNo": "DL123456",
  "status": "Active"
}
```

**Status Codes**:
- `200 OK` - Driver found and returned
- `404 Not Found` - Driver does not exist

**Validation Rules**: None

**Relationships**: None

---

#### 20. Create Driver

**Method**: `POST`

**URL**: `/api/drivers`

**Description**: Creates a new driver record.

**Request Body**:
```json
{
  "name": "James Wilson",
  "phone": "987-654-3210",
  "email": "james@example.com",
  "licenseNo": "DL123456",
  "status": "Active"
}
```

**Response**:
```json
{
  "id": 1,
  "name": "James Wilson",
  "phone": "987-654-3210",
  "email": "james@example.com",
  "licenseNo": "DL123456",
  "status": "Active"
}
```

**Status Codes**:
- `201 Created` - Driver created successfully
- `400 Bad Request` - Invalid input or validation error

**Validation Rules**:
- `Name` - Required, non-empty string
- `LicenseNo` - Required, non-empty string
- `Status` - Required, non-empty string
- Phone and Email - Optional

**Relationships**: None

---

#### 21. Update Driver

**Method**: `PUT`

**URL**: `/api/drivers/{id}`

**Description**: Updates an existing driver's information.

**Request Body**:
```json
{
  "name": "James Updated",
  "phone": "987-654-3210",
  "email": "james.updated@example.com",
  "licenseNo": "DL123456",
  "status": "Active"
}
```

**Response**: Empty body with 204 status

**Status Codes**:
- `204 No Content` - Driver updated successfully
- `400 Bad Request` - Invalid input or validation error

**Validation Rules**:
- `Name` - Required, non-empty string
- `LicenseNo` - Required, non-empty string
- `Status` - Required, non-empty string

**Relationships**: None

---

#### 22. Delete Driver

**Method**: `DELETE`

**URL**: `/api/drivers/{id}`

**Description**: Deletes a driver record and all associated routes.

**Request Body**: None

**Response**: Empty body with 204 status

**Status Codes**:
- `204 No Content` - Driver deleted successfully

**Validation Rules**: None

**Relationships**:
- Driver (1) → Route (Many) - Cascade Delete

---

#### 23. Update Driver Status

**Method**: `PATCH`

**URL**: `/api/drivers/{id}/status`

**Description**: Updates only the status of a driver (Active/Inactive/On Leave).

**Request Body**:
```json
{
  "status": "On Leave"
}
```

**Response**: Empty body with 204 status

**Status Codes**:
- `204 No Content` - Driver status updated successfully
- `400 Bad Request` - Invalid status value

**Validation Rules**:
- `Status` - Required, non-empty string

**Relationships**: None

---

### ROUTE MODULE

#### 24. Get All Routes

**Method**: `GET`

**URL**: `/api/routes`

**Description**: Retrieves a list of all transportation routes.

**Request Body**: None

**Response**:
```json
[
  {
	"id": 1,
	"name": "Route A - North",
	"status": "Active",
	"driverId": 1,
	"description": "Morning route covering north side"
  },
  {
	"id": 2,
	"name": "Route B - South",
	"status": "Active",
	"driverId": 2,
	"description": "Morning route covering south side"
  }
]
```

**Status Codes**:
- `200 OK` - Routes retrieved successfully

**Validation Rules**: None

**Relationships**: None

---

#### 25. Get Route by ID

**Method**: `GET`

**URL**: `/api/routes/{id}`

**Description**: Retrieves a specific route by its ID.

**Request Body**: None

**Response**:
```json
{
  "id": 1,
  "name": "Route A - North",
  "status": "Active",
  "driverId": 1,
  "description": "Morning route covering north side"
}
```

**Status Codes**:
- `200 OK` - Route found and returned
- `404 Not Found` - Route does not exist

**Validation Rules**: None

**Relationships**: None

---

#### 26. Create Route

**Method**: `POST`

**URL**: `/api/routes`

**Description**: Creates a new transportation route.

**Request Body**:
```json
{
  "name": "Route A - North",
  "status": "Active",
  "driverId": 1,
  "description": "Morning route covering north side"
}
```

**Response**:
```json
{
  "id": 1,
  "name": "Route A - North",
  "status": "Active",
  "driverId": 1,
  "description": "Morning route covering north side"
}
```

**Status Codes**:
- `201 Created` - Route created successfully
- `400 Bad Request` - Invalid input or validation error
- `404 Not Found` - Driver not found

**Validation Rules**:
- `Name` - Required, non-empty string
- `Status` - Required, non-empty string
- `DriverId` - Required, must be > 0, must reference existing Driver
- `Description` - Optional

**Relationships**:
- `DriverId` → Drivers (FK) - Driver must exist

---

#### 27. Update Route

**Method**: `PUT`

**URL**: `/api/routes/{id}`

**Description**: Updates an existing route's information.

**Request Body**:
```json
{
  "name": "Route A - North Updated",
  "status": "Active",
  "driverId": 1,
  "description": "Updated morning route"
}
```

**Response**: Empty body with 204 status

**Status Codes**:
- `204 No Content` - Route updated successfully
- `400 Bad Request` - Invalid input or validation error

**Validation Rules**:
- `Name` - Required, non-empty string
- `Status` - Required, non-empty string
- `DriverId` - Required, must be > 0

**Relationships**: None

---

#### 28. Update Route Status

**Method**: `PATCH`

**URL**: `/api/routes/{id}/status`

**Description**: Updates only the status of a route (Active/Inactive/Maintenance).

**Request Body**:
```json
{
  "status": "Maintenance"
}
```

**Response**: Empty body with 204 status

**Status Codes**:
- `204 No Content` - Route status updated successfully
- `400 Bad Request` - Invalid status value

**Validation Rules**:
- `Status` - Required, non-empty string

**Relationships**: None

---

#### 29. Delete Route

**Method**: `DELETE`

**URL**: `/api/routes/{id}`

**Description**: Deletes a route and all associated stops.

**Request Body**: None

**Response**: Empty body with 204 status

**Status Codes**:
- `204 No Content` - Route deleted successfully

**Validation Rules**: None

**Relationships**:
- Route (1) → RouteStop (Many) - Cascade Delete

---

### ROUTESTOP MODULE

#### 30. Get Stops by Route

**Method**: `GET`

**URL**: `/api/routes/{routeId}/stops` 

**Description**: Retrieves all stops on a specific route in order.

**Request Body**: None

**Response**:
```json
[
  {
	"id": 1,
	"routeId": 1,
	"stopName": "Central Station",
	"arrivalTime": "08:00:00",
	"orderIndex": 1
  },
  {
	"id": 2,
	"routeId": 1,
	"stopName": "School Gate",
	"arrivalTime": "08:15:00",
	"orderIndex": 2
  }
]
```

**Status Codes**:
- `200 OK` - Route stops retrieved successfully

**Validation Rules**: None

**Relationships**: None

---

#### 31. Create Route Stop

**Method**: `POST`

**URL**: `/api/routes/{routeId}/stops`

**Description**: Adds a new stop to a route.

**Request Body**:
```json
{
  "stopName": "Central Station",
  "arrivalTime": "08:00:00",
  "orderIndex": 1
}
```

**Response**:
```json
{
  "id": 1,
  "routeId": 1,
  "stopName": "Central Station",
  "arrivalTime": "08:00:00",
  "orderIndex": 1
}
```

**Status Codes**:
- `201 Created` - Route stop created successfully
- `400 Bad Request` - Invalid input or validation error
- `404 Not Found` - Route not found

**Validation Rules**:
- `StopName` - Required, non-empty string
- `OrderIndex` - Required, must be > 0
- `RouteId` - Required, must be > 0, must reference existing Route
- `ArrivalTime` - Optional, TimeSpan format

**Relationships**:
- `RouteId` → Routes (FK) - Route must exist

---

#### 32. Get Route Stop by ID

**Method**: `GET`

**URL**: `/api/routes/stops/{id}`

**Description**: Retrieves a specific route stop by its ID.

**Request Body**: None

**Response**:
```json
{
  "id": 1,
  "routeId": 1,
  "stopName": "Central Station",
  "arrivalTime": "08:00:00",
  "orderIndex": 1
}
```

**Status Codes**:
- `200 OK` - Route stop found and returned
- `404 Not Found` - Route stop does not exist

**Validation Rules**: None

**Relationships**: None

---

#### 33. Update Route Stop

**Method**: `PUT`

**URL**: `/api/routes/stops/{id}`

**Description**: Updates an existing route stop's information.

**Request Body**:
```json
{
  "routeId": 1,
  "stopName": "Central Station Updated",
  "arrivalTime": "08:05:00",
  "orderIndex": 1
}
```

**Response**: Empty body with 204 status

**Status Codes**:
- `204 No Content` - Route stop updated successfully
- `400 Bad Request` - Invalid input or validation error

**Validation Rules**:
- `StopName` - Required, non-empty string
- `OrderIndex` - Required, must be > 0
- `RouteId` - Required, must be > 0

**Relationships**: None

---

#### 34. Delete Route Stop

**Method**: `DELETE`

**URL**: `/api/routes/stops/{id}`

**Description**: Deletes a stop from a route.

**Request Body**: None

**Response**: Empty body with 204 status

**Status Codes**:
- `204 No Content` - Route stop deleted successfully

**Validation Rules**: None

**Relationships**: None

---

### SUBSCRIPTION PLAN MODULE

#### 35. Get All Subscription Plans

**Method**: `GET`

**URL**: `/api/subscription/plans`

**Description**: Retrieves all available subscription plans.

**Request Body**: None

**Response**:
```json
[
  {
	"id": 1,
	"name": "Basic Plan",
	"price": 99.99,
	"maxChildren": 2,
	"features": "Track student location, receive alerts"
  },
  {
	"id": 2,
	"name": "Premium Plan",
	"price": 199.99,
	"maxChildren": 5,
	"features": "All features plus direct driver communication"
  }
]
```

**Status Codes**:
- `200 OK` - Plans retrieved successfully

**Validation Rules**: None

**Relationships**: None

---

#### 36. Get Subscription Plan by ID

**Method**: `GET`

**URL**: `/api/subscription/plans/{id}`

**Description**: Retrieves a specific subscription plan by its ID.

**Request Body**: None

**Response**:
```json
{
  "id": 1,
  "name": "Basic Plan",
  "price": 99.99,
  "maxChildren": 2,
  "features": "Track student location, receive alerts"
}
```

**Status Codes**:
- `200 OK` - Plan found and returned
- `404 Not Found` - Plan does not exist

**Validation Rules**: None

**Relationships**: None

---

#### 37. Create Subscription Plan

**Method**: `POST`

**URL**: `/api/subscription/plans`

**Description**: Creates a new subscription plan.

**Request Body**:
```json
{
  "name": "Basic Plan",
  "price": 99.99,
  "maxChildren": 2,
  "features": "Track student location, receive alerts"
}
```

**Response**:
```json
{
  "id": 1,
  "name": "Basic Plan",
  "price": 99.99,
  "maxChildren": 2,
  "features": "Track student location, receive alerts"
}
```

**Status Codes**:
- `201 Created` - Plan created successfully
- `400 Bad Request` - Invalid input or validation error

**Validation Rules**:
- `Name` - Required, non-empty string
- `Price` - Required, must be > 0
- `MaxChildren` - Required, must be > 0
- `Features` - Required, non-empty string

**Relationships**: None

---

#### 38. Update Subscription Plan

**Method**: `PUT`

**URL**: `/api/subscription/plans/{id}`

**Description**: Updates an existing subscription plan.

**Request Body**:
```json
{
  "name": "Basic Plan Updated",
  "price": 109.99,
  "maxChildren": 3,
  "features": "Updated features list"
}
```

**Response**: Empty body with 204 status

**Status Codes**:
- `204 No Content` - Plan updated successfully
- `400 Bad Request` - Invalid input or validation error

**Validation Rules**:
- `Name` - Required, non-empty string
- `Price` - Required, must be > 0
- `MaxChildren` - Required, must be > 0
- `Features` - Required, non-empty string

**Relationships**: None

---

#### 39. Delete Subscription Plan

**Method**: `DELETE`

**URL**: `/api/subscription/plans/{id}`

**Description**: Deletes a subscription plan. (May fail if subscriptions exist)

**Request Body**: None

**Response**: Empty body with 204 status

**Status Codes**:
- `204 No Content` - Plan deleted successfully

**Validation Rules**: None

**Relationships**:
- SubscriptionPlan (1) → Subscription (Many) - Cascade Delete

---

### SUBSCRIPTION MODULE

#### 40. Get User Subscription

**Method**: `GET`

**URL**: `/api/subscriptions/users/{userId}/subscription`

**Description**: Retrieves the current subscription for a specific user.

**Request Body**: None

**Response**:
```json
{
  "id": 1,
  "userId": 5,
  "planId": 1,
  "planName": "Basic Plan",
  "price": 99.99,
  "status": "Active",
  "paymentMethod": "Credit Card",
  "startedAt": "2024-01-15T08:00:00Z"
}
```

**Status Codes**:
- `200 OK` - Subscription found and returned
- `404 Not Found` - User or subscription does not exist

**Validation Rules**: None

**Relationships**:
- `UserId` → Users (FK)
- `PlanId` → SubscriptionPlans (FK)

---

#### 41. Get Subscription by ID

**Method**: `GET`

**URL**: `/api/subscriptions/{id}`

**Description**: Retrieves a specific subscription by its ID.

**Request Body**: None

**Response**:
```json
{
  "id": 1,
  "userId": 5,
  "planId": 1,
  "planName": "Basic Plan",
  "price": 99.99,
  "status": "Active",
  "paymentMethod": "Credit Card",
  "startedAt": "2024-01-15T08:00:00Z"
}
```

**Status Codes**:
- `200 OK` - Subscription found and returned
- `404 Not Found` - Subscription does not exist

**Validation Rules**: None

**Relationships**: None

---

#### 42. Create Subscription

**Method**: `POST`

**URL**: `/api/subscriptions`

**Description**: Creates a new subscription for a user.

**Request Body**:
```json
{
  "userId": 5,
  "planId": 1,
  "planName": "Basic Plan",
  "price": 99.99,
  "status": "Active",
  "paymentMethod": "Credit Card",
  "startedAt": "2024-01-15T08:00:00Z"
}
```

**Response**:
```json
{
  "id": 1,
  "userId": 5,
  "planId": 1,
  "planName": "Basic Plan",
  "price": 99.99,
  "status": "Active",
  "paymentMethod": "Credit Card",
  "startedAt": "2024-01-15T08:00:00Z"
}
```

**Status Codes**:
- `201 Created` - Subscription created successfully
- `400 Bad Request` - Invalid input or validation error
- `404 Not Found` - User or plan not found

**Validation Rules**:
- `UserId` - Required, must be > 0, must reference existing User
- `PlanId` - Required, must be > 0, must reference existing SubscriptionPlan
- `Status` - Required, non-empty string
- Other fields - Required

**Relationships**:
- `UserId` → Users (FK) - User must exist
- `PlanId` → SubscriptionPlans (FK) - Plan must exist

---

#### 43. Update Subscription

**Method**: `PUT`

**URL**: `/api/subscriptions/{id}`

**Description**: Updates an existing subscription.

**Request Body**:
```json
{
  "userId": 5,
  "planId": 2,
  "planName": "Premium Plan",
  "price": 199.99,
  "status": "Active",
  "paymentMethod": "Debit Card",
  "startedAt": "2024-01-15T08:00:00Z"
}
```

**Response**: Empty body with 204 status

**Status Codes**:
- `204 No Content` - Subscription updated successfully
- `400 Bad Request` - Invalid input or validation error

**Validation Rules**:
- `UserId` - Required, must be > 0
- `PlanId` - Required, must be > 0
- `Status` - Required, non-empty string
- Other fields - Required

**Relationships**: None

---

#### 44. Update Subscription Status

**Method**: `PATCH`

**URL**: `/api/subscriptions/{id}/status`

**Description**: Updates only the status of a subscription (Active/Inactive/Cancelled).

**Request Body**:
```json
{
  "status": "Cancelled"
}
```

**Response**: Empty body with 204 status

**Status Codes**:
- `204 No Content` - Subscription status updated successfully
- `400 Bad Request` - Invalid status value

**Validation Rules**:
- `Status` - Required, non-empty string

**Relationships**: None

---

### ALERT MODULE

#### 45. Get Alerts by Student

**Method**: `GET`

**URL**: `/api/students/{studentId}/alerts`

**Description**: Retrieves all alerts for a specific student.

**Request Body**: None

**Response**:
```json
[
  {
	"id": 1,
	"studentId": 1,
	"type": "SchoolEvent",
	"title": "School Assembly Tomorrow",
	"message": "Please ensure your child attends the school assembly tomorrow at 9 AM",
	"sentAt": "2024-01-20T10:30:00Z",
	"isRead": false
  },
  {
	"id": 2,
	"studentId": 1,
	"type": "RouteUpdate",
	"title": "Route Changed",
	"message": "The pickup route has been changed. Please check the new route schedule.",
	"sentAt": "2024-01-20T09:00:00Z",
	"isRead": true
  }
]
```

**Status Codes**:
- `200 OK` - Alerts retrieved successfully

**Validation Rules**: None

**Relationships**: None

---

#### 46. Get Alert by ID

**Method**: `GET`

**URL**: `/api/alerts/{id}`

**Description**: Retrieves a specific alert by its ID.

**Request Body**: None

**Response**:
```json
{
  "id": 1,
  "studentId": 1,
  "type": "SchoolEvent",
  "title": "School Assembly Tomorrow",
  "message": "Please ensure your child attends the school assembly tomorrow at 9 AM",
  "sentAt": "2024-01-20T10:30:00Z",
  "isRead": false
}
```

**Status Codes**:
- `200 OK` - Alert found and returned
- `404 Not Found` - Alert does not exist

**Validation Rules**: None

**Relationships**: None

---

#### 47. Create Alert

**Method**: `POST`

**URL**: `/api/alerts`

**Description**: Creates a new alert for a student.

**Request Body**:
```json
{
  "studentId": 1,
  "type": "SchoolEvent",
  "title": "School Assembly Tomorrow",
  "message": "Please ensure your child attends the school assembly tomorrow at 9 AM",
  "sentAt": "2024-01-20T10:30:00Z"
}
```

**Response**:
```json
{
  "id": 1,
  "studentId": 1,
  "type": "SchoolEvent",
  "title": "School Assembly Tomorrow",
  "message": "Please ensure your child attends the school assembly tomorrow at 9 AM",
  "sentAt": "2024-01-20T10:30:00Z",
  "isRead": false
}
```

**Status Codes**:
- `201 Created` - Alert created successfully
- `400 Bad Request` - Invalid input or validation error
- `404 Not Found` - Student not found

**Validation Rules**:
- `StudentId` - Required, must be > 0, must reference existing Student
- `Type` - Required, non-empty string
- `Title` - Required, non-empty string
- `Message` - Required, non-empty string

**Relationships**:
- `StudentId` → Students (FK) - Student must exist

---

#### 48. Mark Alert as Read

**Method**: `PATCH`

**URL**: `/api/alerts/{id}/read`

**Description**: Marks an alert as read by setting IsRead flag to true.

**Request Body**:
```json
{
  "isRead": true
}
```

**Response**: Empty body with 204 status

**Status Codes**:
- `204 No Content` - Alert marked as read successfully
- `400 Bad Request` - IsRead must be true

**Validation Rules**:
- `IsRead` - Required, must be true

**Relationships**: None

---

#### 49. Get Student Schedule

**Method**: `GET`

**URL**: `/api/students/{studentId}/schedule`

**Description**: Retrieves the pickup/dropoff schedule for a student. (Placeholder endpoint for future implementation)

**Request Body**: None

**Response**:
```json
{
  "message": "Schedule endpoint for student 1"
}
```

**Status Codes**:
- `200 OK` - Returns placeholder response

**Validation Rules**: None

**Relationships**: None

---

## Error Handling & Validation

### Handler Validation Rules by Module

#### User Module
- **Email**: Required, must be unique, valid email format
- **Password**: Required (on Create), minimum 6 characters
- **Name**: Optional but recommended
- **Status**: Optional
- **Exception**: `InvalidOperationException("Email already in use.")` if email exists

#### Student Module
- **Name**: Required, non-empty string
- **Grade**: Required, non-empty string
- **ParentUserId**: Required, must be > 0, must reference existing User
- **Status**: Required, non-empty string
- **Section**: Optional
- **Exception**: `InvalidOperationException("Parent user not found.")` if parent doesn't exist

#### Guardian Module
- **Name**: Required, non-empty string
- **Relation**: Required, non-empty string
- **UserId**: Required, must be > 0
- **Phone**: Optional
- **Status**: Optional
- **Note**: Optional

#### Driver Module
- **Name**: Required, non-empty string
- **LicenseNo**: Required, non-empty string
- **Status**: Required, non-empty string
- **Phone**: Optional
- **Email**: Optional

#### Route Module
- **Name**: Required, non-empty string
- **Status**: Required, non-empty string (Active/Inactive/Maintenance)
- **DriverId**: Required, must be > 0, must reference existing Driver
- **Description**: Optional
- **Exception**: `InvalidOperationException("Driver not found.")` if driver doesn't exist

#### RouteStop Module
- **StopName**: Required, non-empty string
- **OrderIndex**: Required, must be > 0
- **RouteId**: Required, must be > 0, must reference existing Route
- **ArrivalTime**: Optional, TimeSpan format
- **Exception**: `InvalidOperationException("Route not found.")` if route doesn't exist

#### SubscriptionPlan Module
- **Name**: Required, non-empty string
- **Price**: Required, must be > 0
- **MaxChildren**: Required, must be > 0
- **Features**: Required, non-empty string

#### Subscription Module
- **UserId**: Required, must be > 0, must reference existing User
- **PlanId**: Required, must be > 0, must reference existing SubscriptionPlan
- **Status**: Required, non-empty string
- **PaymentMethod**: Required, non-empty string
- **PlanName**: Required, non-empty string
- **Price**: Required, must be > 0
- **Exceptions**:
  - `InvalidOperationException("User not found.")` if user doesn't exist
  - `InvalidOperationException("Subscription Plan not found.")` if plan doesn't exist

#### Alert Module
- **StudentId**: Required, must be > 0, must reference existing Student
- **Type**: Required, non-empty string
- **Title**: Required, non-empty string
- **Message**: Required, non-empty string
- **Exception**: `InvalidOperationException("Student not found.")` if student doesn't exist

### HTTP Status Codes Used

| Status Code | Description | When Used |
|------------|-------------|-----------|
| 200 OK | Request succeeded | GET requests return data |
| 201 Created | Resource created | POST requests create records |
| 204 No Content | Request succeeded, no body | PUT, PATCH, DELETE operations |
| 400 Bad Request | Invalid request data | Validation errors, malformed JSON |
| 404 Not Found | Resource not found | GET requests with non-existent IDs |

### Common Exception Types

1. **ArgumentException**
   - Thrown for property-level validation failures
   - Example: `throw new ArgumentException("Name is required.", nameof(dto.Name))`

2. **InvalidOperationException**
   - Thrown for business logic violations and FK relationship issues
   - Example: `throw new InvalidOperationException("Parent user not found.")`

3. **BadHttpRequestException**
   - Thrown for malformed HTTP requests
   - Example: Invalid JSON in request body

---

## Testing Guide

### Prerequisites
- **Postman** installed and configured
- **API Base URL**: `https://localhost:5001` (or your deployed URL)
- All endpoints available and database connection active

### Step-by-Step Testing Instructions

#### 1. Test User Module

**Create User**:
1. Open Postman
2. Create new POST request to `https://localhost:5001/api/users`
3. Set Header: `Content-Type: application/json`
4. Set Body (raw JSON):
```json
{
  "name": "Test User",
  "email": "test@example.com",
  "password": "TestPassword123",
  "phone": "123-456-7890",
  "city": "New York",
  "role": "Parent",
  "status": "Active"
}
```
5. Click Send
6. Expected Response: `201 Created` with user object including new ID

**Get All Users**:
1. Create new GET request to `https://localhost:5001/api/users`
2. Click Send
3. Expected Response: `200 OK` with array of user objects

**Get User by ID**:
1. Create new GET request to `https://localhost:5001/api/users/1`
2. Click Send
3. Expected Response: `200 OK` with single user object or `404 Not Found`

**Update User**:
1. Create new PUT request to `https://localhost:5001/api/users/1`
2. Set Header: `Content-Type: application/json`
3. Set Body with updated fields
4. Click Send
5. Expected Response: `204 No Content`

**Delete User**:
1. Create new DELETE request to `https://localhost:5001/api/users/1`
2. Click Send
3. Expected Response: `204 No Content`

#### 2. Test Student Module

Follow similar pattern as User module:
- GET `/api/students` - Get all
- POST `/api/students` - Create (requires valid ParentUserId)
- GET `/api/students/{id}` - Get by ID
- GET `/api/students/parent/{parentId}` - Get by parent
- PUT `/api/students/{id}` - Update
- DELETE `/api/students/{id}` - Delete

#### 3. Test Guardian Module

**Link Guardian to Student**:
1. First create a user and student
2. Create POST request to `/api/students/{studentId}/guardians`
3. Set Body:
```json
{
  "guardianId": 1,
  "isPrimary": true,
  "status": "Active"
}
```
4. Click Send
5. Expected Response: `201 Created`

#### 4. Test Driver Module with Status Update

**Create Driver**:
```json
{
  "name": "Driver Name",
  "phone": "123-456-7890",
  "email": "driver@example.com",
  "licenseNo": "DL123456",
  "status": "Active"
}
```

**Update Status**:
1. PATCH request to `/api/drivers/1/status`
2. Set Body:
```json
{
  "status": "On Leave"
}
```

#### 5. Test Route and RouteStop

**Create Route** (requires DriverId):
```json
{
  "name": "Morning Route",
  "status": "Active",
  "driverId": 1,
  "description": "Morning pickup route"
}
```

**Add Stop to Route**:
1. POST to `/api/routes/1/stops`
2. Set Body:
```json
{
  "stopName": "School",
  "arrivalTime": "08:30:00",
  "orderIndex": 1
}
```

#### 6. Test Subscription Module

**Create Subscription Plan** (test separately first):
```json
{
  "name": "Premium Plan",
  "price": 199.99,
  "maxChildren": 5,
  "features": "All features including live tracking"
}
```

**Create Subscription** (requires UserId and PlanId):
```json
{
  "userId": 1,
  "planId": 1,
  "planName": "Basic Plan",
  "price": 99.99,
  "status": "Active",
  "paymentMethod": "Credit Card",
  "startedAt": "2024-01-15T08:00:00Z"
}
```

#### 7. Test Alert Module

**Create Alert** (requires StudentId):
1. POST to `/api/alerts`
2. Set Body:
```json
{
  "studentId": 1,
  "type": "SchoolEvent",
  "title": "School Event",
  "message": "Important notification",
  "sentAt": "2024-01-20T10:30:00Z"
}
```

**Mark as Read**:
1. PATCH to `/api/alerts/1/read`
2. Set Body:
```json
{
  "isRead": true
}
```

### Testing Validation Errors

**Test Required Field Validation**:
1. Send POST to any endpoint
2. Omit a required field
3. Expected Response: `400 Bad Request` with error message

**Test Foreign Key Validation**:
1. Create Student with invalid ParentUserId: 99999
2. Expected Response: `404 Not Found` or `400 Bad Request` with message "Parent user not found."

**Test Unique Constraint**:
1. Create User with email "test@example.com"
2. Create another User with same email
3. Expected Response: `400 Bad Request` with message "Email already in use."

**Test Numeric Constraint**:
1. Create Subscription with Price: -100
2. Expected Response: `400 Bad Request` with message "Price must be greater than zero."

### Collection Testing (Advanced)

1. **Import Postman Collection**:
   - Export all endpoints as Postman collection
   - Share team members for collaborative testing

2. **Set Up Environment Variables**:
   - `{{base_url}}` = `https://localhost:5001`
   - `{{userId}}` = Saved from create response
   - `{{studentId}}` = Saved from create response

3. **Write Pre-request Scripts**:
   - Use to set headers automatically
   - Validate common values

4. **Run Tests in Collection**:
   - Postman → Collections → Run → Select Environment
   - Monitor pass/fail rates

---

## API Response Patterns

### Success Response Pattern

```json
{
  "id": <number>,
  "property1": "<value>",
  "property2": <number>,
  "createdAt": "2024-01-20T10:30:00Z"
}
```

### Error Response Pattern

```json
{
  "error": "Error message describing what went wrong",
  "details": {
	"field": "error description"
  }
}
```

### List Response Pattern

```json
[
  { /* object 1 */ },
  { /* object 2 */ },
  { /* object 3 */ }
]
```

---

## Future Enhancements

The following features are planned for future versions:

1. **Authentication & Authorization**
   - JWT-based token authentication
   - Role-based access control (RBAC)
   - Secure password hashing

2. **Pagination & Filtering**
   - Implement pagination for large datasets
   - Add filtering and sorting options
   - Support for query parameters

3. **API Documentation**
   - Swagger/OpenAPI integration
   - Interactive API explorer
   - Auto-generated documentation

4. **Caching**
   - Redis caching layer
   - Cache invalidation strategies
   - Performance optimization

5. **Real-time Updates**
   - WebSocket support
   - SignalR for live notifications
   - Push notifications

6. **Logging & Monitoring**
   - Structured logging (Serilog)
   - Application performance monitoring
   - Error tracking and reporting

7. **Advanced Features**
   - Schedule management completion
   - Analytics and reporting
   - Bulk operations
   - Import/Export functionality

---

## Deployment Guidelines

### Prerequisites
- .NET 10 SDK installed
- PostgreSQL 14+ database
- IIS/Kestrel web server

### Environment Configuration
1. Update `appsettings.{Environment}.json` with:
   - Database connection string
   - API endpoint configuration
   - CORS settings

2. Database Migration:
```bash
dotnet ef database update
```

3. Run Application:
```bash
dotnet run
```

### Production Checklist
- [ ] All secret keys configured
- [ ] Database backups enabled
- [ ] HTTPS enabled
- [ ] CORS properly configured
- [ ] API rate limiting enabled
- [ ] Logging configured
- [ ] Error tracking setup
- [ ] Monitoring enabled

---

## Support & Documentation

For additional support or questions:
- Check GitHub Issues for existing problems
- Review inline code documentation
- Consult Entity Framework Core documentation
- Review ASP.NET Core documentation

---

**Document Version**: 1.0  
**Last Updated**: January 2024  
**Project**: VanGo Plus Backend  
**Framework**: ASP.NET Core 10  
**Database**: PostgreSQL
