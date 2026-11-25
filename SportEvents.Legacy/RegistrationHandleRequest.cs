#pragma warning disable
using System.Collections.Generic;

namespace SportEvents.Legacy;

public sealed record RegistrationHandleRequest(
    int DocId,
    Athlete Athlete,
    IReadOnlyList<EventLineItem>? Items,
    string State,
    string Currency,
    bool SendMail,
    string ClientMail,
    bool Urgent,
    decimal ManualBonus);

public sealed record RegistrationHandleResult(
    string Report,
    decimal GrandTotal);