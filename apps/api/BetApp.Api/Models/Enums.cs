namespace BetApp.Api.Models;

// All enums are persisted as their string name via HasConversion<string>().
// The C# enum is the single source of truth for allowed values — invalid input
// is rejected at the API boundary, so a separate DB CHECK constraint is omitted.

public enum EventStatus { Scheduled, Live, Finished, Cancelled }

public enum ParticipantSide { Home, Away }

public enum UserStatus { Active, Suspended, Closed }

public enum TransactionType { Deposit, Withdrawal }

public enum TransactionStatus { Pending, Completed, Failed }

public enum BonusType { FreeBet, DepositMatch, OddsBoost }

public enum CouponStatus { Placed, Won, Lost, Cancelled, CashedOut }

public enum SelectionResult { Open, Won, Lost, Void }
