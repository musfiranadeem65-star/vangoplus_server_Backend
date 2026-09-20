# Parent Chatbot

An intent classifier trained on our own dataset (ML.NET, no pre-trained model), plus answer
handlers that read the parent's real data from PostgreSQL.

## Setup

```bash
dotnet restore          # Microsoft.ML 5.0.0 was added to the csproj
dotnet run -- train     # trains, prints accuracy + confusion matrix, writes ML/intent_model.zip
dotnet run              # normal API start
```

Commit `ML/intent_model.zip` once trained — the API loads it at startup and fails without it.

```bash
curl -k -X POST https://localhost:7270/api/chat \
  -H "Content-Type: application/json" \
  -d '{"parentUserId":1,"message":"van kab aye gi"}'
```

## How it answers

```
message → normalise → emergency keywords → short-phrase exact match
        → ML.NET model → confidence < 0.60 ? fallback : answer handler
```

Two rules run before the model, deliberately:

- **Emergency keywords.** In testing, emergency was sometimes classified as `child_status` or
  `van_location`. It is the one intent that must never be missed, so it does not depend on the model.
- **Short phrases.** "good morning" was classified as `pickup_time`, because "morning" appears
  throughout the pickup examples. One or two words carry too little signal.

31 intents in three groups: **data-backed** (11, EF queries), **static** (15, fixed text),
**escalation** (4 — writes an `Alert` row for the admin and returns contact details).

## Files

| File | Job |
|---|---|
| `ML/vango_intents.csv` | dataset — 2,448 rows, 31 intents, English + Roman Urdu |
| `ML/ModelTrainer.cs` | training + evaluation |
| `Application/Chat/IntentClassifier.cs` | model + the two pre-model rules (singleton) |
| `Application/Chat/ChatContextService.cs` | loads the parent's children, routes, driver, subscription |
| `Application/Chat/AnswerService.cs` | builds the reply for each intent |
| `Application/Chat/StaticAnswers.cs` | fixed text answers |
| `Application/Chat/EscalationService.cs` | absence / complaint / route change / emergency |
| `Controllers/ChatController.cs` | `POST /api/chat` |

## Limitations (for the report)

Where the database holds nothing, the bot says so instead of inventing data:

| Gap | What it answers |
|---|---|
| No GPS / trip tracking | scheduled time + driver's number, says tracking is planned |
| No attendance records | latest `Alert` if one exists, otherwise the schedule |
| No absence table | files an `Alert` for the admin + driver's number |
| No transactions table | lists subscriptions, says receipts aren't available |
| No trial concept | says trials aren't offered |
| `Driver` has no vehicle fields | says vehicle numbers aren't recorded |
| `Subscription` has no end date | computes +1 month, labels it "around" |

`parentUserId` is sent in the request body because frontend auth is still mocked. With real
auth it comes from the session and the body field goes away.

## Dataset history

| Version | Rows | Intents | Baseline accuracy |
|---|---|---|---|
| v1 | 1,432 | 35 | 68–74% |
| v2 | 1,758 | 32 | 77.9% |
| v3/v4 (current) | 2,448 | 31 | 78.9% |

Those come from a naive-Bayes baseline used to sanity-check the data. The real number is what
`dotnet run -- train` prints.

Four merges, each because one query answers both questions: `route_not_assigned`→`route_info`,
`driver_contact`→`driver_info`, `guardian_status`+`list_guardians`→`guardian_info`,
`how_to_subscribe`+`how_to_renew`→`subscribe_or_renew`.
