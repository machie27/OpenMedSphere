namespace OpenMedSphere.Domain.Enums;

/// <summary>
/// Defines the status of a data share between researchers.
/// </summary>
public enum DataShareStatus
{
    /// <summary>
    /// The data share has been created and is awaiting acceptance by the recipient.
    /// </summary>
    Pending = 0,

    /// <summary>
    /// The data share has been accepted and accessed by the recipient.
    /// </summary>
    Accepted = 1,

    /// <summary>
    /// The data share has been revoked by the sender.
    /// </summary>
    Revoked = 2,

    /// <summary>
    /// The data share has expired and is no longer accessible.
    /// </summary>
    Expired = 3
}
