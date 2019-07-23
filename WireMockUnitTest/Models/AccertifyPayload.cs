using System;
using System.Xml.Serialization;

namespace WireMockTests.Models
{
    [Serializable]
    [XmlRoot(ElementName = "Transaction")]
    public class AccertifyPayload
    {
        [XmlElement(IsNullable = true)]
        public Order Order { get; set; }

        [XmlIgnore]
        public string OrderNumber { get; set; }
    }

    [Serializable]
    [XmlRoot(ElementName = "Order")]
    public class Order
    {
        [XmlElement]
        public int Number { get; set; }

        [XmlElement]
        public Payment Payment { get; set; }
    }

    [Serializable]
    [XmlRoot(ElementName = "Payment")]
    public class Payment
    {
        [XmlElement]
        public Address Address { get; set; }
    }

    public class Address
    {
        [XmlElement(IsNullable = true)]
        public string LastName { get; set; }
    }

    [XmlRoot("transaction-results")]
    public class AccertifyResponse
    {
        [XmlElement("transaction-id")]
        public string TransactionId { get; set; }

        [XmlElement("cross-reference")]
        public string CrossReference { get; set; }

        [XmlElement("total-score")]
        public decimal TotalScore { get; set; }

        [XmlElement("recommendation-code")]
        public AccertifyResult Result { get; set; }

        [XmlElement("remarks")]
        public string Remarks { get; set; }
    }


    [Serializable]
    public enum AccertifyResult
    {
        Unknown = 0,

        CannotProcessRequest = 0xFF,
        Accept = 0xFFF,
        Review = 0xFFFF,
        Reject = 0xFFFFF,
        SecondaryResolution = 0xFFFFFF
    }
}
