namespace Joyful.API.Enums;

public enum MessageStatus
{
    /// <summary>
    /// The message has been sent but not yet delivered.
    /// </summary>
    Sent,

    /// <summary>
    /// The message has been delivered to the recipient.
    /// </summary>
    Delivered,

    /// <summary>
    /// The message has been read by the recipient.
    /// </summary>
    Read,

    /// <summary>
    /// The message failed to send or deliver.
    /// </summary>
    Failed
}