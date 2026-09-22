# Parent Chatbot

An intent classifier trained on our own dataset (ML.NET, no pre-trained model), plus answer
handlers that read the parent's real data from PostgreSQL.

## Setup

The trained model (`ML/intent_model.zip`) is committed, so the API runs as-is:

```bash
dotnet restore
dotnet run
```

Retrain only after editing the dataset — it overwrites the model file:

```bash
dotnet run -- train
```

Check what the model makes of a message, without the API or the database:

```bash
dotnet run -- classify "van kab aye gi"
# pickup_time  (confidence 84.1%, matched by model)
```

```bash
curl -k -X POST https://localhost:7270/api/chat \
  -H "Content-Type: application/json" \
  -d '{"parentUserId":1,"message":"van kab aye gi"}'
```

## Showing the supervisor how the model was trained

Everything needed is in the repo, and training runs in about 3 seconds, so it can be done live:

1. `ML/vango_intents.csv` — the dataset, written by us: 2,448 rows, 31 intents.
2. `ML/ModelTrainer.cs` — the training code. The pipeline is five readable lines: map the
   intent to a label, turn the text into numbers (word unigrams + character 3-grams, TF-IDF),
   train SdcaMaximumEntropy, map the prediction back to a name.
3. `dotnet run -- train` — trains from the CSV in front of him and prints accuracy, per-intent
   recall and precision, and the confusion matrix. Same numbers every run.
4. `dotnet run -- classify "van kab aye gi"` — shows a single prediction with its confidence.
5. `git log ML/vango_intents.csv` — the dataset's history, showing it was built up by hand.

No pre-trained model is downloaded or used anywhere. The only thing the model ever learned
came from that CSV.

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

## Results

Trained on 2,448 rows, evaluated on a held-out 20%:

| Metric | Value |
|---|---|
| MicroAccuracy | **80.63%** |
| MacroAccuracy | **79.88%** |
| LogLoss | 0.7482 |

Training is **reproducible**: `NumberOfThreads = 1` and a fixed seed mean `dotnet run -- train`
prints these same figures every time, so the numbers in the report can be re-checked live.
It takes about 3 seconds.

Strongest: `greeting` 100% recall, `guardian_info` 96%, `driver_info` 95%.
Weakest: `emergency` 42.9%, `complaint` 50%, `how_to_add_guardian` 54%, `out_of_scope` 57% —
the broadest categories, where a parent can say almost anything.

**That `emergency` number is why the keyword rule exists.** The model alone catches fewer than
half of them, so emergencies are matched on keywords before the model ever runs. It is the one
intent where a miss is not a bad answer but a dangerous one.

### Choices made by measurement

Trainers tried: SdcaMaximumEntropy 76.5%, OVA-LinearSvm 76.1%, OVA-AveragedPerceptron 75.9%,
SdcaNonCalibrated 75.5%, LbfgsMaximumEntropy 53.8%, NaiveBayes 5.5%. (Those were measured with
the default featurisation, before the featurisation below was tuned.)

Featurisation mattered more than the trainer: word unigrams + character 3-grams with TF-IDF
scored **81.0%**, against 77.3% for word bigrams + char 4-grams with plain TF. Bigrams hurt
because Roman Urdu reorders freely — "van kab aye gi" and "kab aye gi van" are the same
question, and a bigram splits evidence that a unigram keeps together.

### Dataset history

| Version | Rows | Intents |
|---|---|---|
| v1 | 1,432 | 35 |
| v2 | 1,758 | 32 |
| current | 2,448 | 31 |

Four merges, each because one query answers both questions: `route_not_assigned`→`route_info`,
`driver_contact`→`driver_info`, `guardian_status`+`list_guardians`→`guardian_info`,
`how_to_subscribe`+`how_to_renew`→`subscribe_or_renew`.
