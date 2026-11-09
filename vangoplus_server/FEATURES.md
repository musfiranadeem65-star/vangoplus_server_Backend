# Stops on a map, and pickup and drop records

Two features added on 9 November 2026, plus the fixes they uncovered.

---

## 1. Stops on a map

### Why a map at all

A parent wants to see where the van goes, not only a list of stop names.

This is **not** live GPS tracking. Nothing in the system knows where the van is
right now, because there is no driver app sending its position. What the map
shows is the route: each stop in order, in its real place, joined by a line. Say
that plainly if you are asked. A map of the route is an honest feature; calling
it live tracking would not be.

### How it works

`RouteStops` gained two nullable columns, `Latitude` and `Longitude`. Nullable
matters: every stop that existed before this change has no position, and a stop
with only a name is still a usable stop. The map simply leaves those out.

An administrator types the two numbers when editing a route
(Routes, Edit, under each stop). The easiest way to find them is to right-click
the place in Google Maps and copy the pair it shows.

The parent sees the map on Transport Schedule, under the stop list.

### Why OpenStreetMap and not Google Maps

Google Maps needs an API key attached to a billing account, and a key committed
to a public repository is a key somebody else can spend. **Leaflet** with
OpenStreetMap tiles needs no key and no account, and it is the standard choice
for exactly this reason.

Leaflet writes to the DOM itself rather than through React, so
`components/map/RouteStopsMap.tsx` drives it from an effect, cleans the map up
when it unmounts, and is loaded with `ssr: false` because Leaflet reads `window`
as soon as it is imported.

---

## 2. Pickup and drop records

### What it does

Staff open **Attendance**, pick a route and a day, and mark each child
*Boarded* and *Dropped*. The parent sees "Picked up 07:42, Dropped off 14:10" on
their own Transport Schedule screen.

### Why a table and not two columns

A pair of columns on the route assignment can only ever describe the most recent
day. A child travels every school day, so each event is a row: `TripRecords`
holds one record per child, per day, per kind.

### What stops it going wrong

- **A unique index on (StudentId, OnDate, Kind).** Two taps on one button is the
  normal mistake, and without the index it writes two rows and the parent sees
  two different pickup times for one morning. The second attempt returns the
  record that already exists instead of failing.
- **`OnDate` is stored separately from `OccurredAt`.** Timestamps are UTC and
  Pakistan is UTC+5, so a 07:40 pickup is 02:40 UTC. Deriving the day from the
  timestamp would file a morning pickup against the day before.
- **`RecordedByUserId`** is stamped from the token, so every entry can be traced
  to a person.
- **The route link is `Restrict`, not `Cascade`.** These rows are a record of
  what happened; deleting a route must not quietly erase the history of the
  children who travelled on it.
- **Staff only.** The roster names every child on a route, so a parent opening it
  would see the other families travelling with their child. `GET` and `POST` are
  both `[Authorize(Roles = "Admin")]`. A parent reads their own children through
  `my-children`, which takes the parent id from the token.

### Endpoints

| Method | Path | Who |
| --- | --- | --- |
| `GET` | `/api/trip-records/route/{routeId}?date=` | staff |
| `POST` | `/api/trip-records` | staff |
| `GET` | `/api/trip-records/my-children?days=7` | the signed-in parent, own children only |

---

## 3. Three things found while testing these

Each of these was already broken before this change. They are listed because
they are worth knowing, and because the two features above do not work without
them.

**The parent's schedule endpoint was never implemented.** `GET
/api/students/{id}/schedule` returned `{"message": "Schedule endpoint for
student 2"}` with a comment saying "for future implementation", so the Transport
Schedule page had always shown "No schedule available for this child". It is
implemented now, in `StudentScheduleController`, and a parent may only read their
own child.

**Editing one stop edited every stop.** The API returns a route's stops with no
`id`, so in the edit form every stop had `id` of `undefined`. `updateStop`
matches on `stop.id === id`, and `undefined === undefined` is true for all of
them, so changing one stop name changed all of them and removing one would have
removed the lot. The form numbers its own rows now.

**The driver lookup used the wrong URL.** The client called `/api/drivers/{id}`;
the controller is `/api/driver/{id}`. It had never worked, and nobody saw it
because the page never got far enough to call it.

Two smaller ones: the API address was hard-coded as
`https://localhost:7270` in all eight service files, so the app could only ever
talk to a developer's own machine. It reads `NEXT_PUBLIC_API_URL` now and falls
back to that same address, so nothing changes locally. And the stop normaliser
dropped latitude and longitude, which would have left the map empty.

---

## Still to do

The map shows where the stops are, not where the van is. Live position needs a
driver app sending its location, and a place to store it. That is a separate
piece of work, and it is honest to describe it as future work rather than to
imply the current map does it.
