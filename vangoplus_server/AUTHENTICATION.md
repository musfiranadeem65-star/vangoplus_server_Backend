# Authentication

Before this change every API endpoint was open. Anyone who knew the address could
request `/api/Student` and read every child's name, school and stop, with no sign-in.
This document describes what replaced that.

## How it works

1. The parent signs in at `POST /api/User/login`.
2. The server checks the password and returns a **JWT**: a short string, signed with a
   secret key, that says who this user is and what role they have.
3. The website stores that token and sends it back on every later request as
   `Authorization: Bearer <token>`.
4. Every controller carries `[Authorize]`, so a request with no token, an expired token
   or an edited token is refused with **401 Unauthorized**.

The signature is what makes this work. The token says "user 7, role Parent", and a
person can read that, but they cannot change it to "user 8" because they do not have
the key needed to sign the result.

## The two endpoints that stay open

| Endpoint | Why |
| --- | --- |
| `POST /api/User/login` | you cannot be signed in before you sign in |
| `POST /api/User` | the public signup form |

Signup always creates a **Parent**, whatever role the request asks for. Without that, a
visitor could register themselves as an administrator.

## Passwords

Passwords were stored as a plain SHA-256 hash with no salt. SHA-256 is designed to be
fast, which is the wrong property here: an attacker with the database can try millions
of guesses a second. They are now stored with **BCrypt**, which is deliberately slow and
salts every password separately, so two people with the same password get different
stored values.

Accounts created before this change still work. `PasswordHasher` recognises the old
format, accepts it once, and immediately replaces it with a BCrypt hash the next time
that person signs in. Nobody has to reset anything, and no new password is ever written
in the old format.

## Changing a password

`POST /api/User/change-password` requires a token, and the email in the request must be
your own. Being signed in is not enough: without that check any parent could change any
other parent's password just by knowing their email.

## The chatbot

`POST /api/chat` takes the parent id from the token, not from the request body. The
request carries only the message. If the id came from the body, any signed-in parent
could read another family's pickup times by changing one number.

## Secrets

Two secrets are needed and **neither is committed**:

| Setting | What it is |
| --- | --- |
| `ConnectionStrings:DefaultConnection` | the database password |
| `Jwt:Key` | the key that signs tokens, at least 32 characters |

Copy `appsettings.Development.json.example` to `appsettings.Development.json` and fill
in your own values. That file is in `.gitignore`.

In development, a built-in signing key is used when `Jwt:Key` is not set, so the project
runs straight after cloning. Outside development the application refuses to start
without one, because a key that everybody can read is the same as no key at all.

> The old database password was committed to this repository earlier, so it is in the
> git history. Change the password on your PostgreSQL server.

## What is still to do

`[Authorize]` means **signed in**. It does not yet mean **allowed to see this
particular row**. A signed-in parent can still request another parent's records by id on
most endpoints.

Three places are already scoped to the caller and are the pattern to copy:

| Endpoint | How it is scoped |
| --- | --- |
| `POST /api/chat` | parent id comes from the token, never the body |
| `GET /api/trip-records/my-children` | same, and the roster is staff only |
| `GET /api/students/{id}/schedule` | a parent may only read their own child |

The next step is per-row checks, and `[Authorize(Roles = "Admin")]` on the management
endpoints. That was left out of this change on purpose, because the parent portal reads
drivers, routes and students too, and locking those by role without checking each screen
would break the parent pages.

## Trying it

```bash
# refused
curl -k https://localhost:7270/api/Student
# 401

# sign in
TOKEN=$(curl -ks -X POST https://localhost:7270/api/User/login \
  -H "Content-Type: application/json" \
  -d '{"email":"parent@example.com","password":"your-password"}' \
  | python3 -c "import sys,json;print(json.load(sys.stdin)['token'])")

# allowed
curl -k https://localhost:7270/api/Student -H "Authorization: Bearer $TOKEN"
# 200
```
