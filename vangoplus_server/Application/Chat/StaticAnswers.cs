namespace vangoplus_server.Application.Chat
{
    /// <summary>
    /// Intents whose answer never depends on the database: how-to steps, policy and small talk.
    /// Anything the system genuinely does not record says so plainly — a wrong answer about a
    /// child's whereabouts is worse than "we don't track that yet".
    /// </summary>
    public static class StaticAnswers
    {
        public const string Fallback =
            "Sorry, I didn't understand that. I can help with pickup and drop-off times, routes, " +
            "drivers, alerts, guardians, subscriptions and fees. Could you rephrase it?";

        public static readonly IReadOnlyDictionary<string, string> Answers =
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["greeting"] =
                    "Assalam o Alaikum! I'm the VanGo Plus assistant. Ask me about pickup times, " +
                    "routes, drivers, alerts or your subscription. How can I help?",

                ["goodbye"] =
                    "Allah Hafiz! Message me any time you need transport information.",

                ["thanks"] =
                    "You're welcome! Anything else I can help with?",

                ["help_capabilities"] =
                    "I'm the VanGo Plus assistant. I can tell you:\n" +
                    "• your child's pickup and drop-off times\n" +
                    "• their route, stops and driver (with phone number)\n" +
                    "• your alerts and notifications\n" +
                    "• your subscription, plan prices and payment records\n" +
                    "• your authorised guardians and their approval status\n\n" +
                    "I can also explain how to add a child or guardian, pay a fee, or report an " +
                    "absence. Ask in English or Roman Urdu — both work.",

                ["out_of_scope"] =
                    "I can only help with school transport — pickup and drop-off times, routes, " +
                    "drivers, alerts, guardians and subscriptions. Try asking something like " +
                    "\"what time is pickup?\" or \"who is my child's driver?\"",

                ["free_trial"] =
                    "Free trials aren't offered in the system at the moment — all plans are paid " +
                    "monthly. For any trial or special arrangement, please contact the school " +
                    "transport office.",

                ["cancel_subscription"] =
                    "To stop van service, open Subscription from the sidebar and contact the school " +
                    "transport office to cancel — cancellations and refunds are handled by the " +
                    "school directly, not in the app. Your current plan stays active until the end " +
                    "of the period you've paid for.",

                ["how_to_pay_fee"] =
                    "To pay:\n" +
                    "1. Open Subscription from the sidebar\n" +
                    "2. Tap \"Choose a Plan\" and select your plan\n" +
                    "3. Pick your payment method (JazzCash, EasyPaisa, bank transfer or card)\n" +
                    "4. Confirm\n\n" +
                    "If a payment went through but isn't showing on your account, contact the " +
                    "office with your transaction ID.",

                ["how_to_add_guardian"] =
                    "To authorise someone else to collect your child:\n" +
                    "1. Open Guardians from the sidebar\n" +
                    "2. Tap \"Add Guardian\"\n" +
                    "3. Enter their name, relation and phone number\n" +
                    "4. Upload a clear photo of their CNIC\n\n" +
                    "The admin reviews it, usually within 24–48 hours. Once approved, they're " +
                    "allowed to collect your child.",

                ["guardian_requirements"] =
                    "A guardian needs:\n" +
                    "• full name and relation to the child\n" +
                    "• a phone number\n" +
                    "• a clear photo of their CNIC (they must be an adult)\n\n" +
                    "The admin verifies the CNIC before approving — this normally takes 24–48 hours. " +
                    "At pickup the guardian should carry the same ID, because the driver checks it.",

                ["how_to_add_child"] =
                    "To add a child:\n" +
                    "1. Open My Children from the sidebar\n" +
                    "2. Tap \"Add Child\"\n" +
                    "3. Enter their name, grade and section\n\n" +
                    "The admin approves the student and then assigns a route — you'll get an alert " +
                    "when the route is set. How many children you can add depends on your plan.",

                ["change_password"] =
                    "Open Settings → Security to change your password; you'll need your current one. " +
                    "If you can't log in, use \"Forgot password\" on the login page and a reset link " +
                    "goes to your registered email. Check your spam folder if it doesn't arrive " +
                    "within a few minutes.",

                ["update_profile"] =
                    "Open Settings → Profile to update your name, phone number or email, then press " +
                    "Save. Keep your phone number current — pickup and drop-off alerts go to that " +
                    "number.",

                ["notification_settings"] =
                    "Open Settings → Notifications to turn SMS and email alerts on or off. If you're " +
                    "not receiving alerts at all, first check that your phone number and email are " +
                    "correct under Settings → Profile.",

                ["contact_support"] =
                    "For anything the app can't do — refunds, route changes, or a complaint about a " +
                    "driver — contact the school transport office; the number is on the Settings " +
                    "page. For something urgent during a trip, call your child's driver directly — " +
                    "their number is on the Schedule page and I can give it to you here too."
            };
    }
}
