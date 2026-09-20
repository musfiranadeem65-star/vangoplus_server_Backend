using vangoplus_server.Application.Chat;
using vangoplus_server.Application.DTOs;
using vangoplus_server.Application.Interfaces;

namespace vangoplus_server.Application.Handlers
{
    public class ChatHandler : IChatHandler
    {
        private readonly IntentClassifier _classifier;
        private readonly IChatContextService _context;
        private readonly IAnswerService _answers;
        private readonly IEscalationService _escalation;

        public ChatHandler(
            IntentClassifier classifier,
            IChatContextService context,
            IAnswerService answers,
            IEscalationService escalation)
        {
            _classifier = classifier;
            _context = context;
            _answers = answers;
            _escalation = escalation;
        }

        public async Task<ChatResponseDto> AskAsync(ChatRequestDto dto)
        {
            if (dto.ParentUserId <= 0)
                throw new ArgumentException("ParentUserId must be greater than zero.", nameof(dto.ParentUserId));
            if (string.IsNullOrWhiteSpace(dto.Message))
                throw new ArgumentException("Message is required.", nameof(dto.Message));
            if (dto.Message.Length > 500)
                throw new ArgumentException("Message is too long.", nameof(dto.Message));

            var result = _classifier.Classify(dto.Message);

            if (result.Intent == "fallback")
                return new ChatResponseDto
                {
                    Intent = "fallback",
                    Confidence = result.Confidence,
                    Answer = StaticAnswers.Fallback,
                    Escalated = false
                };

            var ctx = await _context.LoadAsync(dto.ParentUserId);

            var answer = _escalation.IsEscalation(result.Intent)
                ? await _escalation.HandleAsync(result.Intent, dto.Message, ctx)
                : await _answers.BuildAsync(result.Intent, ctx);

            return new ChatResponseDto
            {
                Intent = result.Intent,
                Confidence = result.Confidence,
                Answer = answer,
                Escalated = _escalation.IsEscalation(result.Intent)
            };
        }
    }
}
