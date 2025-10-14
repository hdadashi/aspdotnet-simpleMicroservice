namespace Payment.Application.EventBus
{
    public interface IMessagePublisher
    {
        void PublishPaymentProcessed(PaymentProcessedEvent ev);
    }
}