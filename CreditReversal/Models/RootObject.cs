using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CreditReversal.Models
{
    public class Type
    {
        public string dollar { get; set; }
    }

    public class Factor
    {
        public string atabbreviation { get; set; }
        public string atdescription { get; set; }
        public string atsymbol { get; set; }
        public string atrank { get; set; }
    }

    public class CreditScoreFactor
    {
        public string atbureauCode { get; set; }
        public string atFactorType { get; set; }
        public Factor Factor { get; set; }
        public object FactorText { get; set; }
    }

    public class CreditScoreModel
    {
        public string atabbreviation { get; set; }
        public string atdescription { get; set; }
        public string atsymbol { get; set; }
        public string atrank { get; set; }
    }

    public class NoScoreReason
    {
        public string atabbreviation { get; set; }
        public string atdescription { get; set; }
        public string atsymbol { get; set; }
        public string atrank { get; set; }
    }

    public class Bureau
    {
        public string atabbreviation { get; set; }
        public string atdescription { get; set; }
        public string atsymbol { get; set; }
        public string atrank { get; set; }
    }

    public class InquiryDate
    {
        public string dollar { get; set; }
    }

    public class Reference
    {
        public string dollar { get; set; }
    }

    public class Source
    {
        public object BorrowerKey { get; set; }
        public Bureau Bureau { get; set; }
        public InquiryDate InquiryDate { get; set; }
        public Reference Reference { get; set; }
    }

    public class CreditScoreType
    {
        public string atriskScore { get; set; }
        public string atscoreName { get; set; }
        public string atpopulationRank { get; set; }
        public string atinquiriesAffectedScore { get; set; }
        public List<CreditScoreFactor> CreditScoreFactor { get; set; }
        public CreditScoreModel CreditScoreModel { get; set; }
        public NoScoreReason NoScoreReason { get; set; }
        public Source Source { get; set; }
    }

    public class BirthDate
    {
        public string atyear { get; set; }
        public string atmonth { get; set; }
        public string atday { get; set; }
    }

    public class Bureau2
    {
        public string atabbreviation { get; set; }
        public string atdescription { get; set; }
        public string atsymbol { get; set; }
        public string atrank { get; set; }
    }

    public class InquiryDate2
    {
        public string dollar { get; set; }
    }

    public class Reference2
    {
        public string dollar { get; set; }
    }

    public class Source2
    {
        public object BorrowerKey { get; set; }
        public Bureau2 Bureau { get; set; }
        public InquiryDate2 InquiryDate { get; set; }
        public Reference2 Reference { get; set; }
    }

    public class Birth
    {
        public string atage { get; set; }
        public string atdate { get; set; }
        public string atpartitionSet { get; set; }
        public BirthDate BirthDate { get; set; }
        public Source2 Source { get; set; }
    }

    public class CreditAddress
    {
        public string atcity { get; set; }
        public string atdirection { get; set; }
        public string athouseNumber { get; set; }
        public string atpostDirection { get; set; }
        public string atstateCode { get; set; }
        public string atstreetName { get; set; }
        public string atstreetType { get; set; }
        public string atunit { get; set; }
        public string atpostalCode { get; set; }
        public string atunparsedStreet { get; set; }
    }

    public class Dwelling
    {
        public string atabbreviation { get; set; }
        public string atdescription { get; set; }
        public string atsymbol { get; set; }
        public string atrank { get; set; }
    }

    public class Origin
    {
        public string atabbreviation { get; set; }
        public string atdescription { get; set; }
        public string atsymbol { get; set; }
        public string atrank { get; set; }
    }

    public class Ownership
    {
        public string atabbreviation { get; set; }
        public string atdescription { get; set; }
        public string atsymbol { get; set; }
        public string atrank { get; set; }
    }

    public class Bureau3
    {
        public string atabbreviation { get; set; }
        public string atdescription { get; set; }
        public string atsymbol { get; set; }
        public string atrank { get; set; }
    }

    public class InquiryDate3
    {
        public string dollar { get; set; }
    }

    public class Reference3
    {
        public string dollar { get; set; }
    }

    public class Source3
    {
        public object BorrowerKey { get; set; }
        public Bureau3 Bureau { get; set; }
        public InquiryDate3 InquiryDate { get; set; }
        public Reference3 Reference { get; set; }
    }

    public class BorrowerAddress
    {
        public string atdateReported { get; set; }
        public string ataddressOrder { get; set; }
        public string atpartitionSet { get; set; }
        public CreditAddress CreditAddress { get; set; }
        public Dwelling Dwelling { get; set; }
        public Origin Origin { get; set; }
        public Ownership Ownership { get; set; }
        public Source3 Source { get; set; }
        public string atdateUpdated { get; set; }
    }

    public class Name
    {
        public string atfirst { get; set; }
        public string atmiddle { get; set; }
        public string atlast { get; set; }
        public string atsuffix { get; set; }
    }

    public class NameType
    {
        public string atabbreviation { get; set; }
        public string atdescription { get; set; }
        public string atsymbol { get; set; }
        public string atrank { get; set; }
    }

    public class Bureau4
    {
        public string atabbreviation { get; set; }
        public string atdescription { get; set; }
        public string atsymbol { get; set; }
        public string atrank { get; set; }
    }

    public class InquiryDate4
    {
        public string dollar { get; set; }
    }

    public class Reference4
    {
        public string dollar { get; set; }
    }

    public class Source4
    {
        public object BorrowerKey { get; set; }
        public Bureau4 Bureau { get; set; }
        public InquiryDate4 InquiryDate { get; set; }
        public Reference4 Reference { get; set; }
    }

    public class BorrowerName
    {
        public string atpartitionSet { get; set; }
        public Name Name { get; set; }
        public NameType NameType { get; set; }
        public Source4 Source { get; set; }
    }

    public class Factor2
    {
        public string atabbreviation { get; set; }
        public string atdescription { get; set; }
        public string atsymbol { get; set; }
        public string atrank { get; set; }
    }

    public class CreditScoreFactor2
    {
        public string atbureauCode { get; set; }
        public string atFactorType { get; set; }
        public Factor2 Factor { get; set; }
        public object FactorText { get; set; }
    }

    public class CreditScoreModel2
    {
        public string atabbreviation { get; set; }
        public string atdescription { get; set; }
        public string atsymbol { get; set; }
        public string atrank { get; set; }
    }

    public class NoScoreReason2
    {
        public string atabbreviation { get; set; }
        public string atdescription { get; set; }
        public string atsymbol { get; set; }
        public string atrank { get; set; }
    }

    public class Bureau5
    {
        public string atabbreviation { get; set; }
        public string atdescription { get; set; }
        public string atsymbol { get; set; }
        public string atrank { get; set; }
    }

    public class InquiryDate5
    {
        public string dollar { get; set; }
    }

    public class Reference5
    {
        public string dollar { get; set; }
    }

    public class Source5
    {
        public object BorrowerKey { get; set; }
        public Bureau5 Bureau { get; set; }
        public InquiryDate5 InquiryDate { get; set; }
        public Reference5 Reference { get; set; }
    }

    public class CreditScore
    {
        public string atriskScore { get; set; }
        public string atscoreName { get; set; }
        public string atpopulationRank { get; set; }
        public string atinquiriesAffectedScore { get; set; }
        public List<CreditScoreFactor2> CreditScoreFactor { get; set; }
        public CreditScoreModel2 CreditScoreModel { get; set; }
        public NoScoreReason2 NoScoreReason { get; set; }
        public Source5 Source { get; set; }
    }

    public class CreditAddress2
    {
        public string atcity { get; set; }
        public string atstateCode { get; set; }
        public string atunparsedStreet { get; set; }
        public string atpostalCode { get; set; }
    }

    public class Bureau6
    {
        public string atabbreviation { get; set; }
        public string atdescription { get; set; }
        public string atsymbol { get; set; }
        public string atrank { get; set; }
    }

    public class InquiryDate6
    {
        public string dollar { get; set; }
    }

    public class Reference6
    {
        public string dollar { get; set; }
    }

    public class Source6
    {
        public object BorrowerKey { get; set; }
        public Bureau6 Bureau { get; set; }
        public InquiryDate6 InquiryDate { get; set; }
        public Reference6 Reference { get; set; }
    }

    public class Employer
    {
        public string atdateUpdated { get; set; }
        public string atname { get; set; }
        public string atpartitionSet { get; set; }
        public CreditAddress2 CreditAddress { get; set; }
        public Source6 Source { get; set; }
        public string atdateReported { get; set; }
    }

    public class CreditAddress3
    {
        public string atcity { get; set; }
        public string atdirection { get; set; }
        public string athouseNumber { get; set; }
        public string atpostDirection { get; set; }
        public string atstateCode { get; set; }
        public string atstreetName { get; set; }
        public string atstreetType { get; set; }
        public string atunit { get; set; }
        public string atpostalCode { get; set; }
        public string atunparsedStreet { get; set; }
    }

    public class Dwelling2
    {
        public string atabbreviation { get; set; }
        public string atdescription { get; set; }
        public string atsymbol { get; set; }
        public string atrank { get; set; }
    }

    public class Origin2
    {
        public string atabbreviation { get; set; }
        public string atdescription { get; set; }
        public string atsymbol { get; set; }
        public string atrank { get; set; }
    }

    public class Ownership2
    {
        public string atabbreviation { get; set; }
        public string atdescription { get; set; }
        public string atsymbol { get; set; }
        public string atrank { get; set; }
    }

    public class Bureau7
    {
        public string atabbreviation { get; set; }
        public string atdescription { get; set; }
        public string atsymbol { get; set; }
        public string atrank { get; set; }
    }

    public class InquiryDate7
    {
        public string dollar { get; set; }
    }

    public class Reference7
    {
        public string dollar { get; set; }
    }

    public class Source7
    {
        public object BorrowerKey { get; set; }
        public Bureau7 Bureau { get; set; }
        public InquiryDate7 InquiryDate { get; set; }
        public Reference7 Reference { get; set; }
    }

    public class PreviousAddress
    {
        public string atdateReported { get; set; }
        public string ataddressOrder { get; set; }
        public string atpartitionSet { get; set; }
        public CreditAddress3 CreditAddress { get; set; }
        public Dwelling2 Dwelling { get; set; }
        public Origin2 Origin { get; set; }
        public Ownership2 Ownership { get; set; }
        public Source7 Source { get; set; }
        public string atdateUpdated { get; set; }
    }

    public class SocialSecurityNumber
    {
        public string dollar { get; set; }
    }

    public class Bureau8
    {
        public string atabbreviation { get; set; }
        public string atdescription { get; set; }
        public string atsymbol { get; set; }
        public string atrank { get; set; }
    }

    public class InquiryDate8
    {
        public string dollar { get; set; }
    }

    public class Reference8
    {
        public string dollar { get; set; }
    }

    public class Source8
    {
        public object BorrowerKey { get; set; }
        public Bureau8 Bureau { get; set; }
        public InquiryDate8 InquiryDate { get; set; }
        public Reference8 Reference { get; set; }
    }

    public class Social
    {
        public SocialSecurityNumber SocialSecurityNumber { get; set; }
        public Source8 Source { get; set; }
    }

    public class SocialPartition
    {
        public List<Social> Social { get; set; }
    }

    public class Borrower
    {
        public string atSocialSecurityNumber { get; set; }
        public List<Birth> Birth { get; set; }
        public List<BorrowerAddress> BorrowerAddress { get; set; }
        public List<BorrowerName> BorrowerName { get; set; }
        public List<CreditScore> CreditScore { get; set; }
        public List<Employer> Employer { get; set; }
        public List<PreviousAddress> PreviousAddress { get; set; }
        public SocialPartition SocialPartition { get; set; }
    }

    public class IndustryCode
    {
        public string atabbreviation { get; set; }
        public string atdescription { get; set; }
        public string atsymbol { get; set; }
        public string atrank { get; set; }
    }

    public class Bureau9
    {
        public string atabbreviation { get; set; }
        public string atdescription { get; set; }
        public string atsymbol { get; set; }
        public string atrank { get; set; }
    }

    public class InquiryDate9
    {
        public string dollar { get; set; }
    }

    public class Reference9
    {
        public string dollar { get; set; }
    }

    public class Source9
    {
        public object BorrowerKey { get; set; }
        public Bureau9 Bureau { get; set; }
        public InquiryDate9 InquiryDate { get; set; }
        public Reference9 Reference { get; set; }
    }

    public class Inquiry
    {
        public string atinquiryDate { get; set; }
        public string atsubscriberName { get; set; }
        public string atsubscriberNumber { get; set; }
        public string atbureau { get; set; }
        public string atinquiryType { get; set; }
        public IndustryCode IndustryCode { get; set; }
        public Source9 Source { get; set; }
    }

    public class InquiryPartition
    {
        public Inquiry Inquiry { get; set; }
    }

    public class Code
    {
        public string atabbreviation { get; set; }
        public string atdescription { get; set; }
        public string atsymbol { get; set; }
        public string atrank { get; set; }
    }

    public class Type2
    {
        public string atabbreviation { get; set; }
        public string atdescription { get; set; }
        public string atsymbol { get; set; }
        public string atrank { get; set; }
    }

    public class Message
    {
        public string attext { get; set; }
        public Code Code { get; set; }
        public Type2 Type { get; set; }
    }

    public class AccountDesignator
    {
        public string atabbreviation { get; set; }
        public string atdescription { get; set; }
        public string atsymbol { get; set; }
        public string atrank { get; set; }
    }

    public class Bankruptcy
    {
        public string atcourtNumber { get; set; }
        public string atdivision { get; set; }
        public string atassetAmount { get; set; }
        public string atexemptAmount { get; set; }
        public string atliabilityAmount { get; set; }
        public string attrustee { get; set; }
        public string atcompany { get; set; }
        public string atthirdParty { get; set; }
    }

    public class Classification
    {
        public string atabbreviation { get; set; }
        public string atdescription { get; set; }
        public string atsymbol { get; set; }
        public string atrank { get; set; }
    }

    public class IndustryCode2
    {
        public string atabbreviation { get; set; }
        public string atdescription { get; set; }
        public string atsymbol { get; set; }
        public string atrank { get; set; }
    }

    public class Bureau10
    {
        public string atabbreviation { get; set; }
        public string atdescription { get; set; }
        public string atsymbol { get; set; }
        public string atrank { get; set; }
    }

    public class InquiryDate10
    {
        public string dollar { get; set; }
    }

    public class Reference10
    {
        public string dollar { get; set; }
    }

    public class Source10
    {
        public object BorrowerKey { get; set; }
        public Bureau10 Bureau { get; set; }
        public InquiryDate10 InquiryDate { get; set; }
        public Reference10 Reference { get; set; }
    }

    public class Status
    {
        public string atabbreviation { get; set; }
        public string atdescription { get; set; }
        public string atsymbol { get; set; }
        public string atrank { get; set; }
    }

    public class Type3
    {
        public string atabbreviation { get; set; }
        public string atdescription { get; set; }
        public string atsymbol { get; set; }
        public string atrank { get; set; }
    }

    public class PublicRecord
    {
        public string atcourtName { get; set; }
        public string atdateFiled { get; set; }
        public string atreferenceNumber { get; set; }
        public string atsubscriberCode { get; set; }
        public string atbureau { get; set; }
        public AccountDesignator AccountDesignator { get; set; }
        public Bankruptcy Bankruptcy { get; set; }
        public Classification Classification { get; set; }
        public IndustryCode2 IndustryCode { get; set; }
        public Source10 Source { get; set; }
        public Status Status { get; set; }
        public Type3 Type { get; set; }
    }

    public class PulblicRecordPartition
    {
        public List<PublicRecord> PublicRecord { get; set; }
    }

    public class SafetyCheckPassed
    {
        public string dollar { get; set; }
    }

    public class SB168Frozen
    {
        public string atequifax { get; set; }
        public string atexperian { get; set; }
        public string attransunion { get; set; }
    }

    public class Bureau11
    {
        public string atabbreviation { get; set; }
        public string atdescription { get; set; }
        public string atsymbol { get; set; }
        public string atrank { get; set; }
    }

    public class InquiryDate11
    {
        public string dollar { get; set; }
    }

    public class OriginalData
    {
        public string dollar { get; set; }
    }

    public class Source11
    {
        public Bureau11 Bureau { get; set; }
        public InquiryDate11 InquiryDate { get; set; }
        public OriginalData OriginalData { get; set; }
    }

    public class Sources
    {
        public List<Source11> Source { get; set; }
    }

    public class CreditAddress4
    {
        public string atcity { get; set; }
        public string atstateCode { get; set; }
        public string atunparsedStreet { get; set; }
        public string atpostalCode { get; set; }
    }

    public class IndustryCode3
    {
        public string atabbreviation { get; set; }
        public string atdescription { get; set; }
        public string atsymbol { get; set; }
        public string atrank { get; set; }
    }

    public class Bureau12
    {
        public string atabbreviation { get; set; }
        public string atdescription { get; set; }
        public string atsymbol { get; set; }
        public string atrank { get; set; }
    }

    public class InquiryDate12
    {
        public string dollar { get; set; }
    }

    public class Reference11
    {
        public string dollar { get; set; }
    }

    public class Source12
    {
        public object BorrowerKey { get; set; }
        public Bureau12 Bureau { get; set; }
        public InquiryDate12 InquiryDate { get; set; }
        public Reference11 Reference { get; set; }
    }

    public class Subscriber
    {
        public string atname { get; set; }
        public string attelephone { get; set; }
        public string atsubscriberCode { get; set; }
        public CreditAddress4 CreditAddress { get; set; }
        public IndustryCode3 IndustryCode { get; set; }
        public Source12 Source { get; set; }
    }

    public class Equifax
    {
        public string atNumberInLast2Years { get; set; }
    }

    public class Experian
    {
        public string atNumberInLast2Years { get; set; }
    }

    public class Merge
    {
        public string atNumberInLast2Years { get; set; }
    }

    public class TransUnion
    {
        public string atNumberInLast2Years { get; set; }
    }

    public class InquirySummary
    {
        public Equifax Equifax { get; set; }
        public Experian Experian { get; set; }
        public Merge Merge { get; set; }
        public TransUnion TransUnion { get; set; }
    }

    public class Equifax2
    {
        public string atNumberOfRecords { get; set; }
    }

    public class Experian2
    {
        public string atNumberOfRecords { get; set; }
    }

    public class Merge2
    {
        public string atNumberOfRecords { get; set; }
    }

    public class TransUnion2
    {
        public string atNumberOfRecords { get; set; }
    }

    public class PublicRecordSummary
    {
        public Equifax2 Equifax { get; set; }
        public Experian2 Experian { get; set; }
        public Merge2 Merge { get; set; }
        public TransUnion2 TransUnion { get; set; }
    }

    public class Equifax3
    {
        public string atTotalAccounts { get; set; }
        public string atOpenAccounts { get; set; }
        public string atCloseAccounts { get; set; }
        public string atDelinquentAccounts { get; set; }
        public string atDerogatoryAccounts { get; set; }
        public string atTotalBalances { get; set; }
        public string atTotalMonthlyPayments { get; set; }
    }

    public class Experian3
    {
        public string atTotalAccounts { get; set; }
        public string atOpenAccounts { get; set; }
        public string atCloseAccounts { get; set; }
        public string atDelinquentAccounts { get; set; }
        public string atDerogatoryAccounts { get; set; }
        public string atTotalBalances { get; set; }
        public string atTotalMonthlyPayments { get; set; }
    }

    public class Merge3
    {
        public string atTotalAccounts { get; set; }
        public string atOpenAccounts { get; set; }
        public string atCloseAccounts { get; set; }
        public string atDelinquentAccounts { get; set; }
        public string atDerogatoryAccounts { get; set; }
        public string atTotalBalances { get; set; }
        public string atTotalMonthlyPayments { get; set; }
    }

    public class TransUnion3
    {
        public string atTotalAccounts { get; set; }
        public string atOpenAccounts { get; set; }
        public string atCloseAccounts { get; set; }
        public string atDelinquentAccounts { get; set; }
        public string atDerogatoryAccounts { get; set; }
        public string atTotalBalances { get; set; }
        public string atTotalMonthlyPayments { get; set; }
    }

    public class TradelineSummary
    {
        public Equifax3 Equifax { get; set; }
        public Experian3 Experian { get; set; }
        public Merge3 Merge { get; set; }
        public TransUnion3 TransUnion { get; set; }
    }

    public class Summary
    {
        public InquirySummary InquirySummary { get; set; }
        public PublicRecordSummary PublicRecordSummary { get; set; }
        public TradelineSummary TradelineSummary { get; set; }
    }

    public class TradeLinePartition
    {
        public string ataccountTypeDescription { get; set; }
        public string ataccountTypeSymbol { get; set; }
        public string ataccountTypeAbbreviation { get; set; }
        public object Tradeline { get; set; }


       
    }

    public class TrueLinkCreditReportType
    {
        public string atFraudIndicator { get; set; }
        public string atDeceasedIndicator { get; set; }
        public Borrower Borrower { get; set; }
        public List<InquiryPartition> InquiryPartition { get; set; }
        public List<Message> Message { get; set; }
        public PulblicRecordPartition PulblicRecordPartition { get; set; }
        public SafetyCheckPassed SafetyCheckPassed { get; set; }
        public SB168Frozen SB168Frozen { get; set; }
        public Sources Sources { get; set; }
        public List<Subscriber> Subscriber { get; set; }
        public Summary Summary { get; set; }
        public List<TradeLinePartition> TradeLinePartition { get; set; }
    }

    public class BundleComponent
    {
        public Type Type { get; set; }
        public CreditScoreType CreditScoreType { get; set; }
        public TrueLinkCreditReportType TrueLinkCreditReportType { get; set; }
    }

    public class BundleComponents
    {
        public List<BundleComponent> BundleComponent { get; set; }
    }

    public class RootObject
    {
        public BundleComponents BundleComponents { get; set; }
    }



    public class AccountCondition
    {
        public string atabbreviation { get; set; }
        public string atdescription { get; set; }
        public string atsymbol { get; set; }
        public string atrank { get; set; }
    }

    //public class AccountDesignator
    //{
    //    public string atabbreviation { get; set; }
    //    public string atdescription { get; set; }
    //    public string atsymbol { get; set; }
    //    public string atrank { get; set; }
    //}

    public class DisputeFlag
    {
        public string atabbreviation { get; set; }
        public string atdescription { get; set; }
        public string atsymbol { get; set; }
        public string atrank { get; set; }
    }

    public class AccountType
    {
        public string atabbreviation { get; set; }
        public string atdescription { get; set; }
        public string atsymbol { get; set; }
        public string atrank { get; set; }
    }

    public class CreditLimit
    {
        public string dollar { get; set; }
    }

    public class CreditType
    {
        public string atabbreviation { get; set; }
        public string atdescription { get; set; }
        public string atsymbol { get; set; }
        public string atrank { get; set; }
    }

    public class PaymentFrequency
    {
        public string atabbreviation { get; set; }
        public string atdescription { get; set; }
        public string atsymbol { get; set; }
        public string atrank { get; set; }
    }

    public class MonthlyPayStatu
    {
        public string atdate { get; set; }
        public string atstatus { get; set; }
    }

    public class PayStatusHistory
    {
        public string atstatus { get; set; }
        public string atstartDate { get; set; }
        public List<MonthlyPayStatu> MonthlyPayStatus { get; set; }
    }

    public class TermType
    {
        public string atabbreviation { get; set; }
        public string atdescription { get; set; }
        public string atsymbol { get; set; }
        public string atrank { get; set; }
    }

    public class WorstPayStatus
    {
        public string atabbreviation { get; set; }
        public string atdescription { get; set; }
        public string atsymbol { get; set; }
        public string atrank { get; set; }
    }

    public class GrantedTrade
    {
        public string atmonthsReviewed { get; set; }
        public string atmonthlyPayment { get; set; }
        public string atlate90Count { get; set; }
        public string atlate60Count { get; set; }
        public string atlate30Count { get; set; }
        public string atdateLastPayment { get; set; }
        public string attermMonths { get; set; }
        public string atcollateral { get; set; }
        public string atamountPastDue { get; set; }
        public string atworstPatStatusCount { get; set; }
        public AccountType AccountType { get; set; }
        public CreditLimit CreditLimit { get; set; }
        public CreditType CreditType { get; set; }
        public PaymentFrequency PaymentFrequency { get; set; }
        public PayStatusHistory PayStatusHistory { get; set; }
        public TermType TermType { get; set; }
        public WorstPayStatus WorstPayStatus { get; set; }
    }

    //public class IndustryCode
    //{
    //    public string atabbreviation { get; set; }
    //    public string atdescription { get; set; }
    //    public string atsymbol { get; set; }
    //    public string atrank { get; set; }
    //}

    public class OpenClosed
    {
        public string atabbreviation { get; set; }
        public string atdescription { get; set; }
        public string atsymbol { get; set; }
        public string atrank { get; set; }
    }

    public class PayStatus
    {
        public string atabbreviation { get; set; }
        public string atdescription { get; set; }
        public string atsymbol { get; set; }
        public string atrank { get; set; }
    }

    //public class Bureau
    //{
    //    public string atabbreviation { get; set; }
    //    public string atdescription { get; set; }
    //    public string atsymbol { get; set; }
    //    public string atrank { get; set; }
    //}

    //public class InquiryDate
    //{
    //    public string dollar { get; set; }
    //}

    //public class Reference
    //{
    //    public string dollar { get; set; }
    //}

    //public class Source
    //{
    //    public object BorrowerKey { get; set; }
    //    public Bureau Bureau { get; set; }
    //    public InquiryDate InquiryDate { get; set; }
    //    public Reference Reference { get; set; }
    //}

    public class VerificationIndicator
    {
        public string atabbreviation { get; set; }
        public string atdescription { get; set; }
        public string atsymbol { get; set; }
        public string atrank { get; set; }
    }

    public class TradeLine
    {
        public string atsubscriberCode { get; set; }
        public string athighBalance { get; set; }
        public string atdateVerified { get; set; }
        public string atdateReported { get; set; }
        public string atdateOpened { get; set; }
        public string ataccountNumber { get; set; }
        public string atdateAccountStatus { get; set; }
        public string atcurrentBalance { get; set; }
        public string atcreditorName { get; set; }
        public string atposition { get; set; }
        public string atdateClosed { get; set; }
        public string atbureau { get; set; }
        public AccountCondition AccountCondition { get; set; }
        public AccountDesignator AccountDesignator { get; set; }
        public DisputeFlag DisputeFlag { get; set; }
        public GrantedTrade GrantedTrade { get; set; }
        public IndustryCode IndustryCode { get; set; }
        public OpenClosed OpenClosed { get; set; }
        public PayStatus PayStatus { get; set; }
        public object Remark { get; set; }
        public Source Source { get; set; }
        public VerificationIndicator VerificationIndicator { get; set; }
    }

    public class ChildModel
    {
        public List<TradeLine> TradeLine { get; set; }
    }
}